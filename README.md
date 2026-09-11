# cldv6212-g2-2026-cldv6212-poe-st10483126



\## Docker Build



The Azure Functions application is packaged as a standalone Docker image using the official .NET 10 SDK and Azure Functions isolated runtime.



\### Build the Functions Image



Open a terminal inside the `CoffeeNChill` project folder and run:



```powershell

docker build -t coffeenchill-functions:v1.0 .

