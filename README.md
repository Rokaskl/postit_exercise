# PostIt Exercise

## Setup

### **Set up database**
To run this project, you need an MSSQL database running. Create a new database and run the setup scripts located at [MSSQLDatabaseScripts.sql](Scripts/MSSQLDatabaseScripts.sql)

### **Set up configuration**
Update the ConnectionStrings.Default value in the [appsettings.json](PostitExercise/PostitExercise/appsettings.json) file with your MSSQL connection string.

## Starting the application

You can start the application using either the .NET 7 SDK or Visual Studio.

### **Starting the Project with SDK**

To get started with this project, you'll need to have the following installed on your machine:

- .NET 7 SDK: **[Download and install](https://dotnet.microsoft.com/download/dotnet/7.0)**

Once you have the above installed, you can clone this repository and run the following command to start the application:

```bash
dotnet run
```
This will start the web API on port `:5026`

### **Starting the Project from Visual Studio**

To start the project from Visual Studio, follow these steps:

1. Open Visual Studio and select **Open a project or solution** from the start page, or go to **File > Open > Project/Solution** from the menu bar.
2. Navigate to the directory where you cloned the repository and select the **`.csproj`** file.
3. After the project has been loaded, you can run it by clicking the **Run** button or by pressing **F5** on your keyboard.
4. Visual Studio will start the project and launch a browser window displaying the Swagger UI page, where you can test the three endpoints.

## **Endpoints**

This API has three endpoints:

### **`POST /clients/upload`**

This endpoint accepts a JSON file, validates it, and inserts clients into the database.

An example of the valid JSON:

```
[
  {
    "Name": "Local shop nr. 1",
    "Address": "Kauno g., 1B, Kaunas",
    "PostCode": ""
  },
  {
    "Name": "Local shop nr. 2",
    "Address": "Vilniaus g., 1, Vilnius",
    "PostCode": ""
  },
  {
    "Name": "Local shop nr. 3",
    "Address": "Klaipėdos g., 3-1B, Klaipėda",
  },
]
```

The `Address` property is unique. If it matches any other record. Record `Name` property is updated instead

### **`GET /clients?skip=&take=`**

This endpoint returns clients from the database. You can use the `skip` and `take` query parameters to control the pagination of the results. Default `skip` = `0`, `take` = `100`.

### **`POST /clients/{id}/update-post-code`**

This endpoint calls the [PostIt API](https://postit.lt/API/) to update the `PostCode` for the client.

## User Interface
 
API uses Swagger UI. To test the endpoints, click on the endpoint in the Swagger UI and click the **Try it out** button. Enter any required parameters and click **Execute** to see the response. You can also test the endpoints using an API client like Postman. To do this, copy the URL of the endpoint you want to test from the Swagger UI and paste it into Postman
