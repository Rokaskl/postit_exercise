using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PostitExercise;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//DI 
builder.Services.AddHttpClient();
builder.Services.AddTransient<IPostItClient, PostItClient>();
builder.Services.AddTransient<IClientsRepository, ClientsRepository>();
builder.Services.AddDbContext<ApiContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

//Setup Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PostIt Excercise API", Version = "v1" });
});

//Setup Text.Json serializer
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

//Setup Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = "";
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PostIt Excercise API V1");
    });
}

app.MapPost("/clients/upload",
    async (HttpRequest request, IClientsRepository clientsRepository, CancellationToken cancellationToken) =>
    {
        using var reader = new StreamReader(request.Body, System.Text.Encoding.UTF8);

        // Read the raw file as a `string`.
        string fileContent = await reader.ReadToEndAsync(cancellationToken);

        List<Client> clients;

        //Try parse json
        try
        {
            clients = JsonSerializer.Deserialize<List<Client>>(fileContent);
        }
        catch (JsonException)
        {
            return Results.BadRequest("Invalid Json");
        }

        await clientsRepository.AddOrUpdateBatchAsync(clients, cancellationToken);

        return Results.Ok("File Was Processed Sucessfully!");
    }).Accepts<IFormFile>("application/json");

app.MapGet("/clients", async (int? skip, int? take, IClientsRepository clientsRepository, CancellationToken cancellationToken) =>
    {
        return await clientsRepository.GetAsync(skip ?? 0, take ?? 100, cancellationToken);
    });

app.MapPost("/clients/{id}/update-post-code", async (int id, IClientsRepository clientsRepository, IPostItClient postItClient,
    CancellationToken cancellationToken) =>
{
    var client = await clientsRepository.GetByIdAsync(id, cancellationToken);
    if (client == null) return Results.BadRequest("Not found");

    client.PostCode = await postItClient.GetZipCodeFromAdderessAsync(client.Address);
    await clientsRepository.UpdateAsync(client, cancellationToken);

    return Results.Ok(client);
});

app.Run();