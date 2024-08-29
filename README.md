
# Minimal API com JWT Authentication

## Descrição

Esta é uma aplicação minimalista ASP.NET Core com autenticação baseada em JWT (JSON Web Token). A aplicação permite gerenciar administradores e veículos, protegendo os endpoints com autenticação e autorização baseadas em roles.

## Tecnologias Utilizadas

- ASP.NET Core
- Entity Framework Core
- JWT (JSON Web Token) para autenticação
- Swagger para documentação da API
- SQL Server

## Estrutura do Projeto

- **Dominio**: Contém as classes de domínio, DTOs e enums usados na aplicação.
- **Infraestrutura**: Contém a configuração do banco de dados e contextos de dados.
- **Servicos**: Contém a lógica de negócios e serviços que são usados pela aplicação.

## Pré-requisitos

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads)
- Visual Studio ou Visual Studio Code

## Configuração do Ambiente

1. **Clone o repositório:**

   ```bash
   git clone https://github.com/seu-usuario/seu-repositorio.git
   ```

2. **Navegue até a pasta do projeto:**

   ```bash
   cd seu-repositorio
   ```

3. **Configure a string de conexão com o SQL Server no arquivo `appsettings.json`:**

   ```json
   {
     "ConnectionStrings": {
       "SqlString": "Server=seu_servidor;Database=sua_base_de_dados;Trusted_Connection=True;"
     },
     "Jwt": {
       "Key": "sua_chave_secreta"
     }
   }
   ```

4. **Instale as dependências necessárias:**

   ```bash
   dotnet restore
   ```

5. **Aplique as migrações ao banco de dados:**

   ```bash
   dotnet ef database update
   ```

## Executando a Aplicação

1. **Inicie a aplicação:**

   ```bash
   dotnet run
   ```

2. **Acesse o Swagger UI para explorar a API:**
   Abra o navegador e vá para: `http://localhost:5000/swagger`

## Endpoints Principais

### Home

- `GET /`  
  Retorna informações básicas sobre a aplicação.  
  **Acesso:** Público

### Administradores

- `POST /Administradores/login`  
  Realiza o login de um administrador e retorna um token JWT.  
  **Acesso:** Público

- `POST /Administradores`  
  Cria um novo administrador.  
  **Acesso:** Restrito a usuários com role "Adm".

- `GET /Administradores`  
  Retorna uma lista de administradores.  
  **Acesso:** Restrito a usuários com role "Adm".

- `GET /Administradores/{id}`  
  Retorna os detalhes de um administrador específico.  
  **Acesso:** Restrito a usuários com role "Adm".

### Veículos

- `POST /Veiculos`  
  Adiciona um novo veículo.  
  **Acesso:** Restrito a usuários com roles "Adm" ou "Editor".

- `GET /Veiculos`  
  Retorna uma lista de veículos.  
  **Acesso:** Restrito a usuários com roles "Adm" ou "Editor".

- `GET /Veiculos/{id}`  
  Retorna os detalhes de um veículo específico.  
  **Acesso:** Restrito a usuários com roles "Adm" ou "Editor".

- `PUT /Veiculos/{id}`  
  Atualiza um veículo existente.  
  **Acesso:** Restrito a usuários com role "Adm".

- `DELETE /Veiculos/{id}`  
  Remove um veículo.  
  **Acesso:** Restrito a usuários com role "Adm".

## Autenticação e Autorização

A aplicação utiliza JWT para autenticação. Para acessar endpoints protegidos, é necessário obter um token através do endpoint de login e incluí-lo no cabeçalho de autorização das requisições subsequentes.

**Exemplo de cabeçalho de autorização:**

```
Authorization: Bearer seu_token_jwt_aqui
```

## Documentação da API

A documentação da API está disponível no Swagger. Ela pode ser acessada em `http://localhost:5000/swagger` após iniciar a aplicação.

## Contribuição

1. Fork este repositório.
2. Crie uma branch para sua feature: `git checkout -b minha-feature`
3. Commit suas mudanças: `git commit -m 'Adicionar minha feature'`
4. Push para a branch: `git push origin minha-feature`
5. Abra um Pull Request.

## Licença

Este projeto está licenciado sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.
