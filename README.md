# CoffeeNChill API – Part 1

CoffeeNChill is an Azure Functions project built with C# and .NET. It uses Azurite to run Azure Table Storage and Blob Storage locally.

## Group Members

- Cheyanne Chetty
- Matthias Kalonji
- Uminathi Mthembu
- Perihan Ozcanan

## Repository

[CoffeeNChill GitHub Repository](https://github.com/EMKNDW/cldv6212-g2-2026-cldv6212-poe-st10483126)

## Video Demonstration

[Watch the project demonstration on YouTube]( https://youtu.be/p3FRKoh4G00 )



## Project Features

The project includes:

- Menu management using Azure Table Storage
- Staff document management using Azure Blob Storage
- Local Azure Storage using Azurite
- Postman API testing
- Standalone Docker containers
- Public Docker Hub images

## API Endpoints

### Menu Management

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/menu` | Create a menu item |
| `GET` | `/api/menu` | Get all menu items |
| `GET` | `/api/menu/category/{category}` | Get items by category |
| `PUT` | `/api/menu/{category}/{id}` | Update a menu item |
| `DELETE` | `/api/menu/{category}/{id}` | Delete a menu item |

The Menu API includes category filtering, DTO mapping, input validation and error handling.

### Staff Documents

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/documents/upload` | Upload a staff document |
| `GET` | `/api/documents` | List uploaded documents |
| `GET` | `/api/documents/download/{fileName}` | Download a document |

The documents are stored in the `staff-docs` Blob container in Azurite.

## Requirements

Install the following before running the project:

- Visual Studio
- .NET SDK
- Docker Desktop
- Postman
- Git

## Running the Project Locally

### 1. Clone the Repository

```powershell
git clone https://github.com/EMKNDW/cldv6212-g2-2026-cldv6212-poe-st10483126.git
```

### 2. Create the Local Settings File

Create a `local.settings.json` file inside the `CoffeeNChill` project folder:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "CoffeeNChillStorageConnection": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
  }
}
```

The `local.settings.json` file should not be uploaded to GitHub.

### 3. Run the Project

Open the solution in Visual Studio and run the `CoffeeNChill` project.

The local Functions URL used during testing was:

```text
http://localhost:7169
```

The port may be different depending on the Visual Studio settings. Check the terminal after the Functions host starts.

## Docker Setup

The multi-stage Dockerfile is located at:

```text
CoffeeNChill/Dockerfile
```

Open PowerShell inside the `CoffeeNChill` folder and build the Functions image:

```powershell
docker build -t perihannaoz23/coffeenchill-functions:v1.0 .
```

### Pull the Docker Hub Images

```powershell
docker pull perihannaoz23/coffeenchill-functions:v1.0
docker pull perihannaoz23/coffeenchill-azurite:v1.0
```

### Create the Docker Network

```powershell
docker network create coffeenchill-network
```

### Run the Azurite Container

```powershell
docker run -d --name coffeenchill-azurite --network coffeenchill-network -p 10000:10000 -p 10001:10001 -p 10002:10002 perihannaoz23/coffeenchill-azurite:v1.0
```

### Set the Azurite Connection

```powershell
$azuriteConnection = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://coffeenchill-azurite:10000/devstoreaccount1;QueueEndpoint=http://coffeenchill-azurite:10001/devstoreaccount1;TableEndpoint=http://coffeenchill-azurite:10002/devstoreaccount1;"
```

### Run the Functions Container

```powershell
docker run -d --name coffeenchill-functions --network coffeenchill-network -p 7071:80 -e "AzureWebJobsStorage=$azuriteConnection" -e "CoffeeNChillStorageConnection=$azuriteConnection" perihannaoz23/coffeenchill-functions:v1.0
```

Check the containers:

```powershell
docker ps
```

When running through Docker, the API base URL is:

```text
http://localhost:7071
```

## Docker Hub Images

- [CoffeeNChill Functions](https://hub.docker.com/r/perihannaoz23/coffeenchill-functions)
- [CoffeeNChill Azurite](https://hub.docker.com/r/perihannaoz23/coffeenchill-azurite)

Both images use the `v1.0` tag.

## Postman Testing

The complete Postman tests are stored in the `postman` folder.

Before running the tests, select the `CoffeeENChill Local` environment.

Use the correct base URL:

- Visual Studio: `http://localhost:7169`
- Docker: `http://localhost:7071`

The collection uses `partitionKey` and `rowKey` variables for the update and delete requests.

To run the tests in VS Code:

1. Open the Postman extension.
2. Select the `CoffeeENChill Local` environment.
3. Right-click the collection and select **Run collection**.
4. Start the collection run.
5. Check the test results.

## Contributions

## Contributions

- **Matthias Kalonji:** Project setup, initial development and Blob Storage integration
- **Cheyanne Chetty:** Menu Management API and menu functions
- **Uminathi Mthembu:** Postman collection and API testing
- **Perihan Ozcanan:** Docker setup, Docker networking and Docker Hub images
  
- ## References
- Microsoft (2026). *Azure Functions documentation*. Available at: https://learn.microsoft.com/azure/azure-functions/ (Accessed: 14 September 2026).
- Microsoft (2026). *Use Azurite for local Azure Storage development*. Available at: https://learn.microsoft.com/azure/storage/common/storage-use-azurite (Accessed: 14 September 2026).
- Docker (2026). *Multi-stage builds*. Available at: https://docs.docker.com/build/building/multi-stage/ (Accessed: 14 September 2026).
- Postman (2026). *Writing tests*. Available at: https://learning.postman.com/docs/tests-and-scripts/write-scripts/test-scripts/ (Accessed: 14 September 2026).
