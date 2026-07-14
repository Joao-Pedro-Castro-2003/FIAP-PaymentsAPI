# FIAP Cloud Games - PaymentsAPI

Microsservico responsavel por simular o processamento de pagamento das compras realizadas na plataforma Cloud Games.

Este servico faz parte do Tech Challenge Fase 2 da FIAP e atua de forma assincrona, consumindo eventos de pedido criado e publicando eventos com o resultado do pagamento.

## Responsabilidades

- Consumir eventos de compra criada.
- Simular aprovacao ou rejeicao de pagamento.
- Registrar o resultado no console/log.
- Publicar evento de pagamento processado.

## Tecnologias

- .NET 8
- ASP.NET Core Web API
- RabbitMQ
- MassTransit
- Swagger
- Docker

## Funcionamento

A PaymentsAPI nao expoe fluxo principal via endpoint de negocio. Sua funcao principal e atuar como consumidor de mensageria.

Quando a CatalogAPI publica um `OrderPlacedEvent`, a PaymentsAPI consome esse evento, simula o pagamento e publica um `PaymentProcessedEvent`.

O resultado pode ser pagamento aprovado ou pagamento rejeitado. A simulacao usa uma taxa de aprovacao configuravel por variavel de ambiente.

## Eventos

Evento consumido:

```text
OrderPlacedEvent
```

Evento publicado:

```text
PaymentProcessedEvent
```

## Variaveis de ambiente

| Variavel | Descricao | Exemplo |
| --- | --- | --- |
| `ASPNETCORE_ENVIRONMENT` | Ambiente da aplicacao | `Development` |
| `RabbitMq__Host` | Host do RabbitMQ | `rabbitmq` |
| `RabbitMq__Username` | Usuario do RabbitMQ | `guest` |
| `RabbitMq__Password` | Senha do RabbitMQ | `guest` |
| `Payment__ApprovalRate` | Percentual de aprovacao simulada | `90` |

## Exemplo de comportamento

Com `Payment__ApprovalRate=90`, cada compra tem 90% de chance de ser aprovada e 10% de chance de ser rejeitada.

Cada pedido e processado de forma independente. A API nao aprova automaticamente apos uma quantidade de tentativas.

## Executando localmente

```bash
dotnet restore
dotnet run --project src/PaymentsAPI/PaymentsAPI.csproj
```

Swagger:

```text
http://localhost:5003/swagger
```

## Executando com Docker

```bash
docker build -t fiap-payments-api:latest .
docker run -p 5003:8080 fiap-payments-api:latest
```

No projeto completo, a execucao recomendada e pelo repositorio de orquestracao, usando Docker Compose ou Kubernetes.

