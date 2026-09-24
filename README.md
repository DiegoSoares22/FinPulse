# FinPulse

Secure financial management REST API built with C# and ASP.NET Core 8.

`C#` `ASP.NET Core 8` `Entity Framework Core` `SQL Server` `JWT` `xUnit`

## Highlights

- **Authentication** - ASP.NET Core Identity with JWT and refresh tokens, with ClockSkew set to zero so tokens expire exactly when configured.
- **Abuse protection** - two-level rate limiting: a global per-IP limit and a stricter policy on the login endpoint.
- **API versioning** - by URL segment and query string, with a separate Swagger document per version.
- **Data access** - Repository + Unit of Work, with interfaces declared in the domain layer.
- **Reliability** - global exception middleware, controller-wide logging filter, in-memory caching and AutoMapper.

## Project structure

```
FinPulse/                REST API - Program.cs, controllers, configuration
FinPulse.Domain/         Entities, enums, validation, pagination, repository interfaces
FinPulse.Application/    Application layer
FinPulse.Infrastructure/ Data access
FinPulse.MVC/            MVC client consuming the API
FinPulse.Tests/          Automated tests
finpulse-web/            React + TypeScript frontend
```

## Running locally

Requirements: .NET 8 SDK and SQL Server LocalDB.

```bash
git clone https://github.com/DiegoSoares22/FinPulse.git
cd FinPulse/FinPulse

dotnet user-secrets init
dotnet user-secrets set "JWT:SecretKey" "your-key-with-at-least-32-characters"

dotnet ef database update
dotnet run
```

Swagger runs at `https://localhost:{port}/swagger`. Secrets are never committed: User Secrets in development, environment variables in production.

## Roadmap

- [ ] Finish the React + TypeScript frontend
- [ ] Containerize with Docker
- [ ] Deploy to production
- [ ] Expand test coverage

## Sobre o projeto (PT-BR)

API REST de gestao financeira construida em C# com ASP.NET Core 8.

- **Autenticacao** - ASP.NET Core Identity com JWT e refresh tokens, com ClockSkew zerado para o token expirar no tempo exato configurado.
- **Protecao contra abuso** - rate limiting em dois niveis: limite global por IP e politica mais restritiva no endpoint de login.
- **Versionamento de API** - por segmento de URL e query string, com Swagger separado por versao.
- **Acesso a dados** - Repository com Unit of Work e interfaces declaradas na camada de dominio.
- **Confiabilidade** - middleware global de excecoes, filtro de logging nos controllers, cache em memoria e AutoMapper.

Requisitos para rodar: .NET 8 SDK e SQL Server LocalDB. Os segredos nao sao versionados: User Secrets no desenvolvimento e variaveis de ambiente em producao.

---

**Diego Soares** - https://www.linkedin.com/in/diego-soaresdev/ - https://diegosoares.vercel.app
