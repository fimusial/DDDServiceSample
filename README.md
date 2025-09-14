# DDD Service Sample
Project showcasing a simple microservice architecture oriented toward Domain Driven Design, built using .NET, RabbitMQ and PostgreSQL. It can also be used as a template for new projects.

## Running the sample locally
---
#### Prerequisites:
- .NET >= 8.0
- Docker (optionally, with Kubernetes enabled)
- PowerShell

#### Building the images
Build the images using the PowerShell script: `./build-images.ps1`

#### Option 1 - Running as Docker containers
Run `docker-compose up`
The API should shortly become available at `localhost:7171`

#### Option 2 - Running in Kubernetes
Run `./helm-deploy.ps1`
The API should shortly become available at `localhost:31111`

## API endpoints
---
#### `HTTP POST /memo`
Creates a new Memo. Accepts the following query string parameter:
- `content` - the text content of the memo
``
```
> Invoke-WebRequest -Method POST -URI http://localhost:7171/memo?content=hello

StatusCode        : 200
StatusDescription : OK
Content           : {}
```

#### `HTTP GET /memo/search`
 Searches the Memos using a term and returns their IDs. Accepts the following query string parameter:
- `term` - the term to compare against Memo content

```
> Invoke-WebRequest -Method GET -URI http://localhost:7171/memo/search?term=ell

StatusCode        : 200
StatusDescription : OK
Content           : [2,1]
```

#### `HTTP GET /memo/{id}`
Fetches the content of the memo with the id specified in the path.

```
> Invoke-WebRequest -Method GET -URI http://localhost:7171/memo/1

StatusCode        : 200
StatusDescription : OK
Content           : "hello"
```

## Integration events
---
Creating a Memo produces an integration event, which is first inserted into an outbox inside PostgreSQL and later published to RabbitMQ as a message. The service is also subscribed to integration events and will process them, which might result in the creation of new Memos, or if the processing fails, the message is put on a dead-letter queue.