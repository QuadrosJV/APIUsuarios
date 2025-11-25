# APIUsuarios

## Overview
APIUsuarios is a web application designed to manage user entities. It provides a RESTful API for creating, reading, updating, and deleting user information.

## Project Structure
The project is organized into several folders, each serving a specific purpose:

- **Domain**: Contains the core entities of the application.
  - **Entities**: Defines the `Usuario` class representing a user.

- **Application**: Contains the business logic and data transfer objects (DTOs).
  - **DTOs**: Classes for transferring user data.
    - `UsuarioCreateDto`: Used for creating new users.
    - `UsuarioReadDto`: Used for reading user data.
    - `UsuarioUpdateDto`: Used for updating existing users.
  - **Interfaces**: Defines contracts for repositories and services.
    - `IUsuarioRepository`: Interface for user data access methods.
    - `IUsuarioService`: Interface for user business logic methods.
  - **Services**: Implements the business logic for user management.
    - `UsuarioService`: Contains methods for user operations.
  - **Validators**: Validates DTOs to ensure data integrity.
    - `UsuarioCreateDtoValidator`: Validates user creation data.
    - `UsuarioUpdateDtoValidator`: Validates user update data.

- **Infrastructure**: Contains the data access layer.
  - **Persistence**: Defines the database context.
    - `AppDbContext`: Manages database connections and user entities.
  - **Repositories**: Implements data access logic.
    - `UsuarioRepository`: Interacts with the database for user operations.

- **Migrations**: Contains automatically generated migration files for database schema changes.

## Setup Instructions
1. Clone the repository to your local machine.
2. Navigate to the project directory.
3. Restore the project dependencies using the command:
   ```
   dotnet restore
   ```
4. Update the `appsettings.json` file with your database connection string.
5. Run the application using the command:
   ```
   dotnet run
   ```

## Usage
Once the application is running, you can access the API endpoints to manage users. The following endpoints are available:

- `POST /usuarios`: Create a new user.
- `GET /usuarios/{id}`: Retrieve a user by ID.
- `PUT /usuarios/{id}`: Update an existing user.
- `DELETE /usuarios/{id}`: Delete a user by ID.

## Contributing
Contributions are welcome! Please submit a pull request or open an issue for any enhancements or bug fixes.

## License
This project is licensed under the MIT License. See the LICENSE file for more details.