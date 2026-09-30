# Viktigt
Programmet virker ikke før 

# Formålet med dette projekt

lave en blazor app der kan tilføje data (support hendvendele) til CosmosDB
og hente data der fra og ned fra databasen så data'erne (support hendvendeler) kan ses i blazor

## Hvordan sætter jeg op en database
note nogle a step 

### Forudsætter
antager at du har en Azure konto + en subscription

Login til Azure:

```bash
az login
```

hvis du ikke har en registeret provider så kør følgene kode (du kan teste med coden neden for og tjekke om den sider: Registered)
Register Cosmos DB resource provideren:

```bash
az provider register --namespace Microsoft.DocumentDB
```


Vent og Check registrations status intil den siger siger: Registered

```bash
az provider show \
  --namespace Microsoft.DocumentDB \
  --query registrationState \
  --output tsv
```

## Hvordan sætter jeg op en database (fortsat)

### 1. Definer variables
du kan navn give dem noget andet hvis du vil (husk at tjekke at LOCATION skal match en Localition, som du har ret til på din konto)
hvis du har en resource gruppe og Cosmos DB for NoSQL account erstat værdien for "RESGRP" "DBACCOUNT" med hvad den heder 
og skip step 2 og 3
```bash
export RESGRP="supportwebapp-rg"
export LOCATION="swedencentral"
export DBACCOUNT="Din CosmosDb account"
export DATABASE="IBasSupportWebApp"
export CONTAINER="ibassupport"
```

### 2. Lave en resource group

```bash
az group create \
  --name $RESGRP \
  --location $LOCATION
```

### 3. Lave en Cosmos DB for NoSQL account

```bash
az cosmosdb create \
  --name $DBACCOUNT \
  --resource-group $RESGRP \
  --enable-free-tier true
```

### 4. lave en SQL database

```bash
az cosmosdb sql database create \
  --account-name $DBACCOUNT \
  --resource-group $RESGRP \
  --name $DATABASE
```

### 5. Laver en container (windows)

```bash
az cosmosdb sql container create \
  --account-name $DBACCOUNT \
  --resource-group $RESGRP \
  --database-name $DATABASE \
  --name $CONTAINER \
  --partition-key-path "//category"
```
### 5. Laver en container (mac)

```bash
az cosmosdb sql container create \
  --account-name $DBACCOUNT \
  --resource-group $RESGRP \
  --database-name $DATABASE \
  --name $CONTAINER \
  --partition-key-path "/category"
```

### 6. Henter den (primære nøgle) connection string
hvis du ikke veed hvor din connection string er
```bash
az cosmosdb keys list \
  --name "$DBACCOUNT" \
  --resource-group "$RESGRP" \
  --type connection-strings \
  --query "connectionStrings[0].connectionString" \
  --output tsv
```

### 7 Lokal configuration (nødvendig for at kunne køre programmet)

Initialiser User Secrets

```bash
dotnet user-secrets init
```

gemmer configurationen:

```bash
dotnet user-secrets set "CosmosDb:ConnectionString" "PASTE_THE_CONNECTION_STRING_HERE"
dotnet user-secrets set "CosmosDb:DatabaseName" "$CONTAINER"
dotnet user-secrets set "CosmosDb:ContainerName" "supportmessages"
```

Check at settingen eksister:

```bash
dotnet user-secrets list
```

## Run the application

kør projektet:

```bash
dotnet run
```
## Hvad har jeg nået

lavet 
Main pages:

- `/create-support`: submitting a support Message
- `/support-list`: overview of submitted Messages

## Status

### Completed

- Cosmos DB connection gennem en dependency-injected service (CosmosDBServies.cs)
- Submission af support Messages
- validation af support Messages requirements  
- Database/container der bruger `/category` as partition key
- Visning af submitted messages
- Navigation til de to sider
    * `/create-support`: submitting a support Message
    * `/support-list`: overview of submitted Messages
- Removal of the template Weather and Counter pages
- Updated home page.

