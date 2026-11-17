# TaskBoard API

API REST para gestão de tarefas com prioridade, prazo, filtros e acompanhamento do fluxo de trabalho.

## Funcionalidades

- Criar, consultar, atualizar e excluir tarefas.
- Filtrar por status e prioridade.
- Alterar status do fluxo: Todo, InProgress, Blocked e Done.
- Resumo com tarefas por situação e tarefas atrasadas.
- Persistência local com SQLite e Entity Framework Core.

## Tecnologias

C#, .NET 10, ASP.NET Core Minimal APIs, EF Core, SQLite e OpenAPI.

## Executar

```bash
dotnet restore
dotnet run
```

O banco `taskboard.db` é criado automaticamente. Consulte `/openapi/v1.json`.

## Endpoints principais

- `GET /api/tasks`
- `GET /api/tasks/{id}`
- `POST /api/tasks`
- `PUT /api/tasks/{id}`
- `PATCH /api/tasks/{id}/status`
- `DELETE /api/tasks/{id}`
- `GET /api/tasks/summary`

## Referências de estudo

Organização e princípios de arquitetura consultados em [Ardalis — CleanArchitecture](https://github.com/ardalis/CleanArchitecture) (MIT) e [Kamil Grzybek — Modular Monolith with DDD](https://github.com/kgrzybek/modular-monolith-with-ddd) (MIT). Implementação autoral para fins de estudo, não é um fork.
