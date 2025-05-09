Here is an installation guide for setting up the DMSMicroservice project, including the ApiGateway and other services:
---
Installation Guide for DMSMicroservice
Prerequisites
1.	.NET 9 SDK: Ensure that the .NET 9 SDK is installed on your machine.
2.	Database: Install and configure a SQL Server instance.
3.	Azure Storage Account: If using Azure Blob Storage, ensure you have an Azure Storage Account set up.
4.	Development Environment: Install Visual Studio 2022 with the following workloads:
•	ASP.NET and web development
•	Azure development (if deploying to Azure)
---
Step 1: Clone the Repository
5.	Clone the repository to your local machine:
 gh repo clone AjitJKTech/DMSMicroservice
 cd DMSMicroservice

Step 2: Configure the appsettings.json Files
Each service has its own appsettings.json file. Update the following configurations:

ApiGateway (DMSMicroservice.ApiGateway/appsettings.json)
·	Update the JWT:Key with a secure key for authentication.
·	Ensure the DownstreamHostAndPorts match the ports of the respective services.
·	Example:
  "JWT": {
    "Key": "YourSecureKeyHere"
  },
  
AuthService (DMSMicroservice.AuthService/appsettings.json)
·	Configure the database connection string:
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AuthDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }





UserManagementService (DMSMicroservice.UserManagementService/appsettings.json)
·	Configure the database connection string:
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=UserManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
  
  DocumentManagementService (DMSMicroservice.DocumentManagementService/appsettings.json)
·	Add Azure Blob Storage configuration:
  "AzureBlobStorage": {
    "ConnectionString": "YourAzureBlobStorageConnectionString",
    "ContainerName": "YourContainerName"
  }
  
Step 3: Apply Migrations
Run the following commands for each service to apply database migrations:
AuthService
cd DMSMicroservice.AuthService
dotnet ef database update

UserManagementService
cd DMSMicroservice.UserManagementService
dotnet ef database update

Step 4: Run the Services
1.	Open the solution in Visual Studio 2022.
2.	Set the solution to start multiple projects:
·	DMSMicroservice.ApiGateway
·	DMSMicroservice.AuthService
·	DMSMicroservice.UserManagementService
·	DMSMicroservice.DocumentManagementService
3.	Press F5 to start the services.

Step 5: Test the API Gateway
Use tools like Postman or Swagger to test the API Gateway endpoints. The base URL will be:
https://localhost:<ApiGatewayPort>

Step 6: Deployment (Optional)
·	Deploy the services to your preferred environment (e.g., Azure, Docker, or on-premises).
·	Update the DownstreamHostAndPorts in the ApiGateway configuration to point to the deployed services.

