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
