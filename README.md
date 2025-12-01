# APIUsuarios

## Visão Geral
APIUsuarios é uma aplicação web projetada para gerenciar entidades de usuários. Ela fornece uma API RESTful para criar, ler, atualizar e deletar informações de usuários.

## Estrutura do Projeto
O projeto está organizado em várias pastas, cada uma servindo a um propósito específico:

- **Domain**: Contém as entidades principais da aplicação.
  - **Entities**: Define a classe `Usuario`, representando um usuário.

- **Application**: Contém a lógica de negócios e os objetos de transferência de dados (DTOs).
  - **DTOs**: Classes para transferir dados de usuários.
    - `UsuarioCreateDto`: Usado para criar novos usuários.
    - `UsuarioReadDto`: Usado para ler dados de usuários.
    - `UsuarioUpdateDto`: Usado para atualizar usuários existentes.
  - **Interfaces**: Define contratos para repositórios e serviços.
    - `IUsuarioRepository`: Interface para métodos de acesso a dados de usuários.
    - `IUsuarioService`: Interface para métodos de lógica de negócios de usuários.
  - **Services**: Implementa a lógica de negócios para gerenciamento de usuários.
    - `UsuarioService`: Contém métodos para operações de usuários.
  - **Validators**: Valida os DTOs para garantir a integridade dos dados.
    - `UsuarioCreateDtoValidator`: Valida os dados de criação de usuários.
    - `UsuarioUpdateDtoValidator`: Valida os dados de atualização de usuários.

- **Infrastructure**: Contém a camada de acesso a dados.
  - **Persistence**: Define o contexto do banco de dados.
    - `AppDbContext`: Gerencia conexões com o banco de dados e entidades de usuários.
  - **Repositories**: Implementa a lógica de acesso a dados.
    - `UsuarioRepository`: Interage com o banco de dados para operações de usuários.

- **Migrations**: Contém arquivos de migração gerados automaticamente para alterações no esquema do banco de dados.

## Instruções de Configuração
1. Clone o repositório para sua máquina local.
2. Navegue até o diretório do projeto.
3. Restaure as dependências do projeto usando o comando:
   ```
   dotnet restore
   ```
4. Atualize o arquivo `appsettings.json` com sua string de conexão do banco de dados.
5. Execute a aplicação usando o comando:
   ```
   dotnet run
   ```

## Uso
Uma vez que a aplicação esteja em execução, você pode acessar os endpoints da API para gerenciar usuários. Os seguintes endpoints estão disponíveis:

- `POST /usuarios`: Criar um novo usuário.
- `GET /usuarios/{id}`: Recuperar um usuário pelo ID.
- `PUT /usuarios/{id}`: Atualizar um usuário existente.
- `DELETE /usuarios/{id}`: Deletar um usuário pelo ID.

## Contribuindo
Contribuições são bem-vindas! Por favor, envie um pull request ou abra uma issue para quaisquer melhorias ou correções de bugs.

## Licença
Este projeto está licenciado sob a Licença MIT. Veja o arquivo LICENSE para mais detalhes.