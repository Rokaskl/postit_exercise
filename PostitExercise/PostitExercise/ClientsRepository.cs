using Microsoft.EntityFrameworkCore;

namespace PostitExercise;

public interface IClientsRepository
{
    Task<IList<Client>> GetAsync(int skip, int take, CancellationToken cancellationToken = default);
    Task<Client> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Client client, CancellationToken cancellationToken = default);
    Task AddOrUpdateBatchAsync(IList<Client> clients, CancellationToken cancellationToken = default);
}

public class ClientsRepository : IClientsRepository
{
    private ApiContext _context;
    public ClientsRepository(ApiContext context)
    {
        _context = context;
    }

    public async Task AddOrUpdateBatchAsync(IList<Client> clients, CancellationToken cancellationToken = default)
    {
        if (clients == null || clients.Count == 0)
        {
            return;
        }

        foreach (Client client in clients)
        {
            var existingClient = await _context.Clients.FirstOrDefaultAsync(x => x.Address.Equals(client.Address), cancellationToken)
                ?? _context.Clients.Local.FirstOrDefault(x => x.Address.Equals(client.Address));

            if (existingClient != null)
            {
                existingClient.Name = client.Name;
            }

            else
            {
                await _context.Clients.AddAsync(client, cancellationToken);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IList<Client>> GetAsync(int skip, int take, CancellationToken cancellationToken = default)
    {
        return await _context.Clients.Skip(skip).Take(take).ToListAsync(cancellationToken);
    }

    public async Task<Client> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Clients.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(Client client, CancellationToken cancellationToken = default)
    {
        _context.Clients.Update(client);
        await _context.SaveChangesAsync(cancellationToken);
    }
}