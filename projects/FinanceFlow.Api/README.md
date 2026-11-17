# FinanceFlow API

API REST para registrar receitas e despesas e acompanhar o saldo mensal. Os valores são armazenados em centavos para evitar erros comuns de precisão em operações monetárias.

## Funcionalidades

- Registrar receitas e despesas.
- Consultar transações por mês e ano.
- Excluir lançamentos.
- Resumo mensal com receitas, despesas, saldo e quantidade de lançamentos.
- Persistência SQLite com Entity Framework Core.

## Tecnologias

C#, .NET 10, ASP.NET Core Minimal APIs, EF Core, SQLite e OpenAPI.

## Executar

```bash
dotnet restore
dotnet run
```

O banco `financeflow.db` é criado automaticamente. Consulte `/openapi/v1.json`.

## Endpoints principais

- `GET /api/transactions?month=11&year=2026`
- `POST /api/transactions`
- `DELETE /api/transactions/{id}`
- `GET /api/summary?month=11&year=2026`

## Referências de estudo

Práticas de APIs e aplicações .NET consultadas em [Microsoft — eShop](https://github.com/dotnet/eShop) (MIT) e padrões de organização em [Ardalis — CleanArchitecture](https://github.com/ardalis/CleanArchitecture) (MIT). Implementação autoral para fins de estudo, não é um fork.
