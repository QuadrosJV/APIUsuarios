# API de Gerenciamento de Usuários

## Visão Geral

APIUsuarios é uma aplicação web projetada para gerenciar entidades de usuários. Ela fornece uma **API RESTful** para criar, ler, atualizar e deletar informações de usuários, seguindo os princípios da **Clean Architecture** (Arquitetura em Camadas).

---

## Tecnologias Utilizadas

Este projeto foi desenvolvido utilizando as seguintes tecnologias e *frameworks*:

* **Plataforma:** .NET 8.0 (ou superior, como o .NET 9.0)
* **API:** Minimal APIs (ASP.NET Core)
* **Persistência:** Entity Framework Core (EF Core 8.0+)
* **Banco de Dados:** SQLite
* **Validação:** FluentValidation (para validação de DTOs e regras de negócio)

---

## Padrões de Projeto Implementados

A arquitetura do projeto foi estruturada para separar responsabilidades e garantir a manutenibilidade, utilizando os seguintes padrões:

* **Repository Pattern:** A camada `Infrastructure/Repositories` isola o acesso a dados, permitindo que a aplicação não dependa da tecnologia de banco de dados específica.
* **Service Pattern:** A camada `Application/Services` contém a lógica de negócios da aplicação (ex: validação de email único, cálculo de idade mínima e *Soft Delete*).
* **DTO Pattern:** (Data Transfer Objects) Classes específicas (`UsuarioCreateDto`, `UsuarioReadDto`) são usadas para transferir dados de forma segura entre as camadas, evitando expor a entidade de domínio.
* **Dependency Injection (DI):** Utilizado para gerenciar as dependências (`IUsuarioRepository` e `IUsuarioService`), seguindo o princípio de Inversão de Controle (IoC).

---

## Estrutura do Projeto

O projeto está organizado em várias pastas, seguindo o padrão de Arquitetura em Camadas:

-   **Domain**: Contém as entidades principais da aplicação.
    -   **Entities**: Define a classe `Usuario`, representando um usuário.

-   **Application**: Contém a lógica de negócios e os objetos de transferência de dados (DTOs).
    -   **DTOs**: Classes para transferir dados de usuários.
    -   **Interfaces**: Define contratos para repositórios (`IUsuarioRepository`) e serviços (`IUsuarioService`).
    -   **Services**: Implementa a lógica de negócios para gerenciamento de usuários.
    -   **Validators**: Valida os DTOs para garantir a integridade dos dados e o retorno do status **400 Bad Request**.

-   **Infrastructure**: Contém a camada de acesso a dados.
    -   **Persistence**: Define o contexto do banco de dados (`AppDbContext`).
    -   **Repositories**: Implementa a lógica de acesso a dados (`UsuarioRepository`).

-   **Migrations**: Contém arquivos de migração gerados automaticamente para alterações no esquema do banco de dados (crucial para o Code First).

---

## Instruções de Configuração

Para executar este projeto localmente e criar o banco de dados usando o **Code First** (via Migrations):

1.  Clone o repositório para sua máquina local.
2.  Navegue até o diretório do projeto:
    ```bash
    cd APIUsuarios
    ```
3.  Restaure as dependências do projeto:
    ```bash
    dotnet restore
    ```
4.  **Aplique as Migrations** para criar o banco de dados SQLite (`APIUsuarios.db`) na raiz do projeto:
    ```bash
    dotnet ef database update
    ```
5.  Execute a aplicação (A API estará disponível em `http://localhost:5214`):
    ```bash
    dotnet run
    ```

---

## Uso e Exemplos de Requisições

A API é acessível através da interface **Swagger/OpenAPI** em ambiente de desenvolvimento. Abaixo estão exemplos dos principais *endpoints* utilizando JSON.

| Endpoint | Método | Descrição | Status Codes |
| :--- | :--- | :--- | :--- |
| `/usuarios` | **POST** | Cria um novo usuário. | 201 Created, **400 Bad Request** (Validação), **409 Conflict** (Email Duplicado) |
| `/usuarios/{id}` | **GET** | Recupera um usuário pelo ID. | 200 OK, 404 Not Found |
| `/usuarios/{id}` | **PUT** | Atualiza um usuário. | 200 OK, **409 Conflict** (Email Duplicado), 404 Not Found |
| `/usuarios/{id}` | **DELETE** | Remove logicamente o usuário (Soft Delete). | **204 No Content**, 404 Not Found |

### Exemplo de Requisição POST (Criação)

```json
POST /usuarios HTTP/1.1
Host: localhost:5214
Content-Type: application/json

{
  "nome": "João da Silva",
  "email": "joao.silva@exemplo.com",
  "dataNascimento": "1990-05-15T00:00:00",
  "telefone": "(11) 99876-5432"
}

//LINK DO VIDEO:
// https://drive.google.com/file/d/1drhLpdNkNwFg97ahpYTtvvfaCA4wiqXl/view?usp=sharing