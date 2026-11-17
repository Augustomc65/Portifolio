# TicketFlow API

API REST para organizar chamados de suporte técnico, com fluxo de atendimento, prioridade, status e visão resumida da fila.

## Funcionalidades

- Criar e consultar chamados.
- Filtrar chamados por status.
- Alterar status do atendimento.
- Registrar solicitante, prioridade e datas.
- Calcular prazo de SLA automaticamente conforme a prioridade (4h, 8h, 24h ou 48h).
- Consultar resumo da fila, incluindo chamados com SLA vencido.
- Persistir dados localmente com SQLite e Entity Framework Core.

## Tecnologias

C#, .NET 10, ASP.NET Core Minimal APIs, EF Core, SQLite e OpenAPI.

## Executar

```bash
dotnet restore
dotnet run
```

O banco `ticketflow.db` é criado automaticamente. Consulte `/openapi/v1.json`.

## Endpoints principais

- `GET /api/tickets`
- `GET /api/tickets/{id}`
- `POST /api/tickets`
- `PATCH /api/tickets/{id}/status`
- `GET /api/tickets/summary`
- `GET /health`

## Referências de estudo

Arquitetura e organização inspiradas por boas práticas observadas em [Jason Taylor — CleanArchitecture](https://github.com/jasontaylordev/CleanArchitecture) (MIT) e [Ardalis — CleanArchitecture](https://github.com/ardalis/CleanArchitecture) (MIT). Implementação deste projeto escrita para este portfólio; não é um fork.
