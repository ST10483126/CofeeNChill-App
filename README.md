## Menu Management API

The Menu Management API was developed using **Azure Functions** and **Azurite** for local Azure Storage emulation.

### Implemented Features

* Developed HTTP-triggered functions for menu management:

  * `POST /api/menu` – Create a menu item
  * `GET /api/menu` – Retrieve all menu items
  * `GET /api/menu/category/{category}` – Retrieve menu items by category
  * `PUT /api/menu/{category}/{id}` – Update the price and availability of a menu item
  * `DELETE /api/menu/{category}/{id}` – Delete a menu item
* Integrated the functions with **Azurite** for local Azure Table Storage.
* Implemented DTOs to separate API data transfer from the Azure Table entity model.
* Added input validation for required fields and invalid prices.
* Added error handling for:

  * `400 Bad Request`
  * `404 Not Found`
  * `409 Conflict`
  * `500 Internal Server Error`
* Implemented category-based filtering for menu items.
* Tested the Menu API endpoints using **Postman**.

### Contribution

**Cheyanne Chetty – Menu Management API**

Responsible for developing the Menu HTTP Functions, including CRUD operations, category filtering, DTO mapping, input validation, error handling, and integration with Azurite for local Azure Table Storage.

## Setup and Run Instructions

### Prerequisites

Before running the CoffeeNChill project, ensure the following are installed:

* Visual Studio
* .NET SDK
* Docker Desktop
* Azurite
* Postman

### 1. Clone the Repository

Clone the CoffeeNChill repository from GitHub and open the solution in Visual Studio.

```bash
git clone <repository-url>
```

### 2. Start Azurite

Start the Azurite storage emulator using Docker Desktop.

The project uses Azurite to emulate Azure Storage services locally during development.

### 3. Configure Local Settings

Create a `local.settings.json` file in the Functions project if one is not already present.

```json
{
  "IsEncrypted": false,
  "Values": {
    "CoffeeNChillStorageConnection": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
  }
}
```

> **Note:** `local.settings.json` should not be committed to GitHub if it contains local or sensitive configuration.

### 4. Run the Azure Functions Project

Open the CoffeeNChill project in Visual Studio and run the project.

The Azure Functions host will start locally. The Menu API is available at:

```text
http://localhost:7169/api/menu
```

### 5. Test the API Using Postman

Open the CoffeeNChill Menu API Postman collection and test the available endpoints:

```text
POST   /api/menu
GET    /api/menu
GET    /api/menu/category/{category}
PUT    /api/menu/{category}/{id}
DELETE /api/menu/{category}/{id}
```

The API communicates with Azurite to store and retrieve menu data locally.

### 6. Verify the Application

Use Postman to create, retrieve, update, and delete menu items. Successful responses confirm that the Azure Functions application is running and communicating with the local Azurite storage emulator.



# cldv6212-g2-2026-cldv6212-poe-st10483126



\## Docker Build



The Azure Functions application is packaged as a standalone Docker image using the official .NET 10 SDK and Azure Functions isolated runtime.



\### Build the Functions Image



Open a terminal inside the `CoffeeNChill` project folder and run:



```powershell

docker build -t coffeenchill-functions:v1.0 .

## Running the Containers

Part 1 uses standalone Docker containers without Docker Compose. Both containers use the same Docker network.

### Create the Docker Network

```powershell
docker network create coffeenchill-network
```

### Run the Azurite Container

```powershell
docker run -d --name coffeenchill-azurite --network coffeenchill-network -p 10000:10000 -p 10001:10001 -p 10002:10002 coffeenchill-azurite:v1.0
```

### Configure the Local Azurite Connection

```powershell
$azuriteConnection = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://coffeenchill-azurite:10000/devstoreaccount1;QueueEndpoint=http://coffeenchill-azurite:10001/devstoreaccount1;TableEndpoint=http://coffeenchill-azurite:10002/devstoreaccount1;"
```

### Run the Functions Container

```powershell
docker run -d --name coffeenchill-functions --network coffeenchill-network -p 7071:80 -e "AzureWebJobsStorage=$azuriteConnection" -e "CoffeeNChillStorageConnection=$azuriteConnection" coffeenchill-functions:v1.0
```

### Check the Running Containers

```powershell
docker ps
```
## Docker Hub Images

The CoffeeNChill Docker images are publicly available on Docker Hub.

- [CoffeeNChill Functions Image](https://hub.docker.com/r/perihannaoz23/coffeenchill-functions)
- [CoffeeNChill Azurite Image](https://hub.docker.com/r/perihannaoz23/coffeenchill-azurite)

### Pull the Images

```powershell
docker pull perihannaoz23/coffeenchill-functions:v1.0
docker pull perihannaoz23/coffeenchill-azurite:v1.0
```
## Blob Storage Verification

The document functions were tested using the `staff-docs` Blob container running in Azurite.

### Upload a Document

```powershell
curl.exe -X POST -F "file=@..\CoffeeNChill Barista Recipe Handbook.pdf;type=application/pdf" http://localhost:7071/api/documents/upload
```

### List Documents

```powershell
Invoke-RestMethod http://localhost:7071/api/documents | Format-Table
```

### Download a Document

```powershell
Invoke-WebRequest -Uri "http://localhost:7071/api/documents/download/CoffeeNChill%20Barista%20Recipe%20Handbook.pdf" -OutFile "$env:TEMP\CoffeeNChill-download-test.pdf"
```

The upload, list and download endpoints were successfully tested against local Azurite Blob Storage.
