# Projetos Backend — Augusto Mota

Quatro projetos autorais de estudo, com foco em C#, ASP.NET Core, APIs REST, persistência com SQLite/Entity Framework Core, validação e regras de negócio.

## Projetos

| Projeto | O que demonstra |
|---|---|
| [TicketFlow API](./TicketFlow.Api/README.md) | Help desk, fluxo de chamados, prioridade e SLA |
| [FinanceFlow API](./FinanceFlow.Api/README.md) | Receitas, despesas, categorias e resumo mensal |
| [StockWise API](./StockWise.Api/README.md) | Produtos, movimentação de estoque e alerta de estoque mínimo |
| [TaskBoard API](./TaskBoard.Api/README.md) | Gestão de tarefas, filtros, prioridades e status |

## Tecnologias

- C# e .NET 10
- ASP.NET Core Minimal APIs
- Entity Framework Core
- SQLite
- OpenAPI
- REST, DTOs, validação e regras de negócio

## Como executar

Entre na pasta de um projeto e execute:

```bash
dotnet restore
dotnet run
```

A aplicação cria seu banco SQLite local automaticamente. O documento OpenAPI fica disponível em `/openapi/v1.json` durante a execução.

## Transparência e referências

Os quatro projetos foram desenvolvidos como implementações autorais para estudo e portfólio. Foram consultadas referências públicas de arquitetura e práticas .NET, sem apresentar o código de terceiros como trabalho próprio:

- [Jason Taylor — CleanArchitecture (MIT)](https://github.com/jasontaylordev/CleanArchitecture)
- [Ardalis — CleanArchitecture (MIT)](https://github.com/ardalis/CleanArchitecture)
- [Microsoft — eShop (MIT)](https://github.com/dotnet/eShop)
- [Kamil Grzybek — Modular Monolith with DDD (MIT)](https://github.com/kgrzybek/modular-monolith-with-ddd)

Cada repositório de referência mantém sua autoria e licença originais. Estes projetos não são forks nem cópias desses repositórios.
