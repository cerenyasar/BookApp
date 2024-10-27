# BOOK API

## Installation and Running the Project

Follow these steps to set up and run the Book API project:

1. **Download or Clone the Repository**  
   Download the repository as a ZIP file or clone it using Git:
   ```bash
   git clone <repository-url>
   ```

2. **Create a SQL Server Database**  
   Create a new SQL Server database. You can name it anything you prefer, for example, `BookDB`.

3. **Open the Project in Visual Studio**  
   Open the project in Visual Studio. Ensure that you have .NET 8 installed on your machine. You can download it from the [.NET official website](https://dotnet.microsoft.com/download).

4. **Rename the Configuration File**  
   Locate the `appsettings.Example.json` file in the project directory and rename it to `appsettings.json`.

5. **Update the Connection String**  
   Open the `appsettings.json` file and update the connection string to match your database name and credentials. It should look something like this:
   ```json
   "ConnectionStrings": {
       "BookDB": "Server=your_server;Database=your_database;User Id=your_username;Password=your_password;TrustServerCertificate=True;MultipleActiveResultSets=True"
   }
   ```

6. **Run Migrations**  
   To apply database migrations, open the Package Manager Console in Visual Studio. You can do this by navigating to `Tools > NuGet Package Manager > Package Manager Console`.

7. **Execute the Migration Command**  
   In the Package Manager Console, run the following command to update the database:
   ```bash
   Update-Database
   ```

8. **Build and Run the API**  
   After the migrations are applied successfully, you can build the project and run the API. Use the following options:
   - To build: Press `Ctrl + Shift + B`.
   - To run: Press `F5` or click the "Start" button in Visual Studio.

## API Documentation

You can access the API documentation using Swagger UI by navigating to `https://localhost:5001/swagger` in your web browser (replace the port number as necessary).

## Note

Ensure that your SQL Server instance is running and that the connection string is correct to avoid connection issues. 
