# FinPulse API

API REST de gestão financeira pessoal construída com C# e ASP.NET Core 8, com autenticação via ASP.NET Core Identity + JWT, versionamento de API e proteção contra abuso por rate limiting.

> Projeto pessoal de estudo, desenvolvido para aplicar na prática padrões de arquitetura e segurança usados em APIs de produção.

## Stack

| Camada | Tecnologias |
|---|---|
| Backend | C#, ASP.NET Core 8, Entity Framework Core, SQL Server |
| Autenticação | ASP.NET Core Identity, JWT Bearer, Refresh Tokens |
| Documentação | Swagger / OpenAPI (v1 e v2) |
| Testes | xUnit |
| Frontend | React + TypeScript (`finpulse-web`), MVC (`FinPulse.MVC`) |

## O que está implementado

- **Autenticação e autorização** com Identity, emissão de JWT e refresh tokens.
- **Rate limiting** em dois níveis: limite global por IP (20 requisições a cada 10 segundos) e política específica de login (5 tentativas por minuto), como mitigação de força bruta.
- **Versionamento de API** por segmento de URL e query string, com Swagger separado por versão.
- **Repository + Unit of Work**, com as interfaces declaradas na camada de domínio.
- **Middleware global de exceções**, para respostas de erro padronizadas.
- **Filtro de logging** aplicado a todos os controllers.
- **AutoMapper** para conversão entre entidades e DTOs.
- **MemoryCache** para reduzir consultas repetidas ao banco.
- **CORS** configurado para o frontend local.

## Estrutura do projeto

```
FinPulse/              API REST (Program.cs, controllers, configuração)
FinPulse.Domain/       Entidades, enums, validações, paginação e interfaces de repositório
FinPulse.Application/  Camada de aplicação
FinPulse.Infrastructure/ Acesso a dados
FinPulse.MVC/          Aplicação MVC que consome a API
FinPulse.Tests/        Testes automatizados
finpulse-web/          Frontend em React + TypeScript
```

## Como rodar

Pré-requisitos: .NET 8 SDK e SQL Server LocalDB.

```bash
git clone https://github.com/DiegoSoares22/FinPulse.git
cd FinPulse/FinPulse
```

Configure a chave JWT via User Secrets (ela não é versionada):

```bash
dotnet user-secrets init
dotnet user-secrets set "JWT:SecretKey" "sua-chave-com-no-minimo-32-caracteres"
```

Aplique as migrations e suba a API:

```bash
dotnet ef database update
dotnet run
```

A documentação fica disponível em `https://localhost:<porta>/swagger`.

## Decisões técnicas

- **Segredos fora do repositório:** a chave JWT e as connection strings ficam em User Secrets no desenvolvimento e em variáveis de ambiente em produção.
- **`ClockSkew = TimeSpan.Zero`:** o padrão do .NET tolera 5 minutos de diferença na expiração do token. Zerar esse valor faz o token expirar no tempo exato configurado.
- **Rate limit separado para login:** o endpoint de autenticação é o alvo mais comum de força bruta, por isso tem um limite mais restritivo que o global.
- **Versionamento desde o início:** permite evoluir contratos da API sem quebrar clientes já existentes.

## Próximos passos

- [ ] Concluir o frontend em React + TypeScript
- [ ] Containerizar com Docker
- [ ] Publicar em ambiente de produção
- [ ] Ampliar a cobertura de testes

---

Desenvolvido por [Diego Soares](https://www.linkedin.com/in/diego-soaresdev/)
