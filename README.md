# NiyoTaskManager

This is a .NET 8 application that leverages SQL Server DB to store and retrieve Tasks created by users. Below, you will find the necessary configurations and dependencies.

## Technologies

- **.NET 8**: Leverages newer features such as implicit usings, a deprecated startup class, and robust reflection properties.
- **Clean Architecture**: Maintains core ideas of clean architecture principles while considering project size.
- **SQL Server DB**: Provides a SQL database.
- **SignalR**: Provides real time update when a new task is created
- **xUnit**: Used for unit tests (and integration tests if needed).

## Prerequisites

- .NET 8 SDK
- SQL Server DB Account or Azure Cosmos DB Emulator

## Installation and Setup

1. **Clone the repository to your local machine:**

    ```bash
    git clone <repository-url>
    ```

2. **Open the solution in your IDE.**

3. **Configure SQL Server DB:**


    - Add these details to the `appsettings.json` file under the `ConnectionStrings` and `JWTSettings` sections:

    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Add your Connection Strings here",
      }
    `JWTSettings`: {
        `Key`: `Y0UR-JWT-K3Y`,
        `Issuer`: `YOUR|ISSUER\URL`,
        `Audience`: `YOUR|USERS`
    }
    ```

## Running Tests

Unit tests for this project are written using xUnit. To run the tests, use the following command in the terminal:

```bash
dotnet test

