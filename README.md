# .NET Microservices Sample

A minimal microservices architecture with API Gateway, two services, and inter-service communication.

## Architecture

```
                    ┌─────────────────┐
                    │   API Gateway   │
                    │   (YARP Proxy)  │
                    │    Port 5000    │
                    └────────┬────────┘
                             │
              ┌──────────────┴──────────────┐
              │                             │
    ┌─────────▼─────────┐         ┌─────────▼─────────┐
    │  ProductService   │◄────────│   OrderService    │
    │    Port 5001      │  HTTP   │    Port 5002      │
    └───────────────────┘         └───────────────────┘
```

## Services

### API Gateway (Port 5000)
- Routes `/api/products/*` → ProductService
- Routes `/api/orders/*` → OrderService
- Uses YARP (Yet Another Reverse Proxy)

### ProductService (Port 5001)
- `GET /` - List all products
- `GET /{id}` - Get product by ID
- `POST /` - Create product
- `PUT /stock/{id}?quantity={n}` - Update stock

### OrderService (Port 5002)
- `GET /` - List all orders
- `GET /{id}` - Get order by ID
- `POST /` - Create order (calls ProductService to validate and update stock)

## Running with Docker Compose

```bash
docker-compose up --build
```

## API Examples

### Through API Gateway (recommended)

```bash
# Get all products
curl http://localhost:5000/api/products/

# Get product by ID
curl http://localhost:5000/api/products/1

# Create an order (triggers inter-service communication)
curl -X POST http://localhost:5000/api/orders/ \
  -H "Content-Type: application/json" \
  -d '{"productId": 1, "quantity": 2}'

# Get all orders
curl http://localhost:5000/api/orders/
```

### Direct service access

```bash
# ProductService directly
curl http://localhost:5001/

# OrderService directly
curl http://localhost:5002/
```

## Inter-Service Communication

When creating an order, OrderService:
1. Calls ProductService to fetch product details
2. Validates stock availability
3. Calls ProductService to deduct stock
4. Creates and returns the order

This demonstrates synchronous HTTP-based inter-service communication using `HttpClient`.
