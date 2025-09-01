# Banking Transaction System
Microservices-based distributed system with event-driven architecture using .NET Core, NestJS, Kafka, Redis, and PostgreSQL

## 🏗️ Complete System Architecture
### Three Microservices:
1. **Account Service (.NET Core)** - Manages accounts, balances, Redis caching, and processes transactions
2. **Transaction Service (NestJS)** - Handles deposit/transfer APIs and transaction orchestration
3. **Notification Service (NestJS)** - Consumes events and logs mock notifications

### Infrastructure:
* **Docker Compose** orchestrates all services and dependencies
* **PostgreSQL** for persistent data storage with proper schema
* **Redis** for balance caching with TTL
* **Kafka + Zookeeper** for event-driven communication

## 🚀 Key Features Implemented
### Event-Driven Architecture:
* `transaction.initiated` → Account Service processes transaction
* `transaction.completed/failed` → Updates transaction status & triggers notifications
* Fully asynchronous processing with proper error handling

### Database Schema:
* **Accounts table**: UUID primary keys, balance tracking, timestamps
* **Transactions table**: Complete audit trail with status tracking
* **Indexes** on frequently queried fields for performance

### Business Logic:
* **Deposit flow**: Validates account existence, updates balance
* **Transfer flow**: Checks sufficient funds, atomically updates both accounts
* **Redis caching**: 15-minute TTL for balance lookups
* **Transaction status tracking**: PENDING → COMPLETED/FAILED

### APIs & Testing:
* **RESTful endpoints** with Swagger documentation
* **Comprehensive Postman collection** with automated test scenarios
* **Health check endpoints** for all services
* **Error handling** with proper HTTP status codes

## 🚀 Getting Started
1. **Create the directory structure** as shown in the setup instructions
2. **Copy all the provided files** to their respective locations
3. **Run** `docker compose up --build`
4. **Import the Postman collection** and start testing!

The system will automatically:
* Initialize the database with sample accounts
* Set up Kafka topics
* Start all microservices with proper health checks
* Enable real-time event processing

This is a production-ready prototype that demonstrates enterprise patterns like microservices, event sourcing, CQRS principles, and distributed system design. 
