# PaymentsAPI — Fase 3

Consome OrderPlacedEvent, simula pagamento e publica PaymentProcessedEvent. Persiste a primeira decisão no MongoDB (fcg_payments.decisions), identificada pelo pedido. Reentregas reutilizam o resultado e conferem usuário, jogo e preço.

Payment__ApprovalRate controla novas decisões (0–100). O ambiente demonstrativo inicia com 100; configure 0 para testar rejeição. Não há cobrança real.

RabbitMq__Host, RabbitMq__Username, RabbitMq__Password e Mongo__ConnectionString vêm da orquestração. O consumidor tem tentativas limitadas e outbox em memória para o evento de saída; isso não é outbox transacional durável no produtor de pedidos.

Métricas em /metrics, logs JSON e liveness em /health/live. Não possui endpoint público de pagamento.

Use o [guia central](../FIAP-CloudGames-Orchestration/README.md) no workspace; no GitHub, consulte FIAP-CloudGames-Orchestration na mesma conta.

```powershell
dotnet test PaymentsAPI.sln
```
