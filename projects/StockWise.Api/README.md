# StockWise API

API REST de controle de estoque com cadastro de produtos, histórico de movimentações e identificação de itens abaixo do estoque mínimo.

## Funcionalidades

- Cadastro de produtos com SKU único.
- Entradas e saídas de estoque.
- Bloqueio de saída quando não há saldo suficiente.
- Histórico de movimentações por produto.
- Consulta de produtos em nível de estoque mínimo.
- Persistência SQLite e índice único para SKU.

## Tecnologias

C#, .NET 10, ASP.NET Core Minimal APIs, EF Core, SQLite e OpenAPI.

## Executar

```bash
dotnet restore
dotnet run
```

O banco `stockwise.db` é criado automaticamente. Consulte `/openapi/v1.json`.

## Endpoints principais

- `GET /api/products`
- `GET /api/products/low-stock`
- `POST /api/products`
- `POST /api/products/{id}/movements`
- `GET /api/products/{id}/movements`

## Referências de estudo

Conceitos de domínio e organização consultados em [Kamil Grzybek — Modular Monolith with DDD](https://github.com/kgrzybek/modular-monolith-with-ddd) (MIT) e [Jason Taylor — CleanArchitecture](https://github.com/jasontaylordev/CleanArchitecture) (MIT). Implementação autoral para fins de estudo, não é um fork.
