# Sistema de controle de fluxo de caixa

Criado um sistema de microserviços para controle de fluxo de caixa diário desenvolvido em .NET 8, permitindo que um comerciante controle seus lançamentos (débitos e créditos) e gere relatórios consolidados diários.

## Descrição

Um comerciante precisa controlar o seu fluxo de caixa diário com os lançamentos (débitos e créditos), também precisa de um relatório que disponibilize o saldo diário consolidado (xlsx e csv).

## Arquitetura

O sistema é composto por dois microserviços independentes que se comunicam via mensageria:

```
┌─────────────────┐    RabbitMQ    ┌─────────────────┐
│ Lançamentos API │ ──────────────▶ │ Consolidado API │
│     (5001)      │                │     (5002)      │
└─────────────────┘                └─────────────────┘
        │                                    │
        ▼                                    ▼
┌─────────────────┐                ┌─────────────────┐
│   PostgreSQL    │                │   PostgreSQL    │
│  (lancamentos)  │                │  (consolidado)  │
└─────────────────┘                └─────────────────┘
```

### Serviços

#### 1. Lançamentos API (Porta 5001)
- **Responsabilidade**: Controle de lançamentos financeiros
- **Funcionalidades**:
  - CRUD de lançamentos (débitos e créditos)
  - Validação de regras de negócio
  - Publicação de eventos via RabbitMQ

#### 2. Consolidado API (Porta 5002)  
- **Responsabilidade**: Consolidação diária e relatórios
- **Funcionalidades**:
  - Consolidação automática via eventos
  - Geração de relatórios Excel (.xlsx)
  - Geração de relatórios CSV (.csv)
  - Consulta de saldos consolidados

## Tecnologias Utilizadas

- **.NET 8** - Framework principal
- **PostgreSQL** - Banco de dados
- **RabbitMQ** - Message broker para comunicação assíncrona
- **Entity Framework Core** - ORM
- **Docker & Docker Compose** - Containerização
- **EPPlus** - Geração de relatórios Excel
- **CsvHelper** - Geração de relatórios CSV
- **MediatR** - Padrão CQRS
- **Serilog** - Logging estruturado
- **xUnit** - Framework de testes

## Como Executar Localmente

### Pré-requisitos
- Docker Desktop instalado
- Postman ou curl para testar as APIs

### Passo a Passo

1. **Execute o sistema completo**
   ```bash
   make run
   ```

2. **Importe a collection Postman** (Opcional)
   - Arquivo: `collection/FluxoCaixa_Postman_Collection.json`
   - Contém todos os testes organizados e automatizados

3. **Execute a sequência de testes completa via curl**

### Serviços Disponíveis

| Serviço | URL | Descrição |
|---------|-----|-----------|
| Lançamentos API | http://localhost:5001 | CRUD de lançamentos |
| Consolidado API | http://localhost:5002 | Consolidados e relatórios |
| RabbitMQ Management | http://localhost:15672 | Interface RabbitMQ (guest/guest) |
| PostgreSQL | localhost:5432 | Banco de dados (postgres/postgres) |

## Testando o Sistema Completo

> **IMPORTANTE**: Execute os passos na sequência apresentada. Cada comando depende do anterior para funcionar corretamente.

Siga esta sequência de comandos para validar que todo o fluxo está funcionando corretamente:

### Passo 1: Verificar se os serviços estão rodando
```bash
curl -X GET "http://localhost:5001/health"
```
**Resposta esperada:** `HTTP 200` com status "Healthy"
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.0123456"
}
```

### Passo 2: Criar primeiro lançamento (Crédito)
```bash
curl -X POST "http://localhost:5001/api/v1/lancamentos" \
  -H "Content-Type: application/json" \
  -d '{
    "data": "2024-09-24T09:00:00",
    "valor": 3000.00,
    "tipo": 1,
    "descricao": "Recebimento inicial - Venda A"
  }'
```
**Resposta esperada:** `HTTP 201` com o lançamento criado
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "data": "2024-09-24T09:00:00",
  "valor": 3000.00,
  "tipo": 1,
  "descricao": "Recebimento inicial - Venda A",
  "dataCriacao": "2024-09-24T09:00:01.234Z"
}
```

### Passo 3: Criar um débito
```bash
curl -X POST "http://localhost:5001/api/v1/lancamentos" \
  -H "Content-Type: application/json" \
  -d '{
    "data": "2024-09-24T11:00:00",
    "valor": 800.00,
    "tipo": 0,
    "descricao": "Pagamento de fornecedor"
  }'
```
**Resposta esperada:** `HTTP 201` com o débito criado
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440001",
  "data": "2024-09-24T11:00:00",
  "valor": 800.00,
  "tipo": 0,
  "descricao": "Pagamento de fornecedor",
  "dataCriacao": "2024-09-24T11:00:01.567Z"
}
```

### Passo 4: Criar segundo crédito
```bash
curl -X POST "http://localhost:5001/api/v1/lancamentos" \
  -H "Content-Type: application/json" \
  -d '{
    "data": "2024-09-24T15:00:00",
    "valor": 1200.00,
    "tipo": 1,
    "descricao": "Prestação de serviço"
  }'
```
**Resposta esperada:** `HTTP 201` com o segundo crédito
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440002",
  "data": "2024-09-24T15:00:00",
  "valor": 1200.00,
  "tipo": 1,
  "descricao": "Prestação de serviço",
  "dataCriacao": "2024-09-24T15:00:01.890Z"
}
```

### Passo 5: Listar todos os lançamentos criados
```bash
curl -X GET "http://localhost:5001/api/v1/lancamentos"
```
**Resposta esperada:** `HTTP 200` com array dos 3 lançamentos
```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "data": "2024-09-24T09:00:00",
    "valor": 3000.00,
    "tipo": 1,
    "descricao": "Recebimento inicial - Venda A"
  },
  {
    "id": "550e8400-e29b-41d4-a716-446655440001",
    "data": "2024-09-24T11:00:00",
    "valor": 800.00,
    "tipo": 0,
    "descricao": "Pagamento de fornecedor"
  },
  {
    "id": "550e8400-e29b-41d4-a716-446655440002",
    "data": "2024-09-24T15:00:00",
    "valor": 1200.00,
    "tipo": 1,
    "descricao": "Prestação de serviço"
  }
]
```

### Passo 6: Verificar o consolidado automático
```bash
curl -X GET "http://localhost:5002/api/v1/consolidado/2024-09-24"
```
**⏱️ Aguarde:** Este passo pode levar alguns segundos, pois o consolidado é processado de forma assíncrona via RabbitMQ após cada lançamento.

**Resposta esperada:** `HTTP 200` com consolidado calculado automaticamente
```json
{
  "data": "2024-09-24",
  "saldoInicial": 0.00,
  "totalCreditos": 4200.00,
  "totalDebitos": 800.00,
  "saldoFinal": 3400.00,
  "quantidadeTransacoes": 3,
  "ultimaAtualizacao": "2024-09-24T15:00:02.123Z"
}
```

**Cálculo:** 
- Créditos: R$ 3.000,00 + R$ 1.200,00 = R$ 4.200,00
- Débitos: R$ 800,00
- Saldo Final: R$ 4.200,00 - R$ 800,00 = R$ 3.400,00

### Passo 7: Baixar relatório Excel
```bash
curl -X GET "http://localhost:5002/api/v1/consolidado/relatorio/2024-09-24/excel" \
  --output consolidado_2024-09-24.xlsx
```
**Resposta esperada:** 
- `HTTP 200` 
- Arquivo Excel baixado na pasta atual
- **Localização do arquivo no servidor:** `./output/consolidado_2024-09-24.xlsx`

### Passo 8: Baixar relatório CSV
```bash
curl -X GET "http://localhost:5002/api/v1/consolidado/relatorio/2024-09-24/csv" \
  --output consolidado_2024-09-24.csv
```
**Resposta esperada:** 
- `HTTP 200` 
- Arquivo CSV baixado na pasta atual
- **Localização do arquivo no servidor:** `./output/consolidado_2024-09-24.csv`

### Validação Final

Após executar todos os passos, você deve ter:

1. **3 lançamentos** cadastrados no sistema
2. **1 consolidado** gerado automaticamente via RabbitMQ
3. **2 relatórios** salvos na pasta `output/`:
   - `consolidado_2024-09-24.xlsx`
   - `consolidado_2024-09-24.csv`

## Outros Exemplos de Uso

### Consultas Avançadas

#### Buscar lançamento específico por ID:
```bash
curl -X GET "http://localhost:5001/api/v1/lancamentos/{ID_DO_LANCAMENTO}"
```

#### Listar lançamentos de um período:
```bash
curl -X GET "http://localhost:5001/api/v1/lancamentos?dataInicio=2024-09-01&dataFim=2024-09-30"
```

#### Consolidados de múltiplos dias:
```bash
curl -X GET "http://localhost:5002/api/v1/consolidado?dataInicio=2024-09-01&dataFim=2024-09-30"
```

### Operações CRUD Completas

#### Atualizar um lançamento:
```bash
curl -X PUT "http://localhost:5001/api/v1/lancamentos/{ID_DO_LANCAMENTO}" \
  -H "Content-Type: application/json" \
  -d '{
    "data": "2024-09-24T16:00:00",
    "valor": 1500.00,
    "tipo": 1,
    "descricao": "Valor atualizado"
  }'
```

#### Deletar um lançamento:
```bash
curl -X DELETE "http://localhost:5001/api/v1/lancamentos/{ID_DO_LANCAMENTO}"
```

### Tipos de Lançamento

- **Tipo 0**: Débito (saída de dinheiro) - Ex: Pagamentos, Despesas
- **Tipo 1**: Crédito (entrada de dinheiro) - Ex: Recebimentos, Vendas

## Estrutura do Projeto

```
src/
├── Lancamentos/              # Microserviço de Lançamentos
│   ├── Lancamentos.API/      # Camada de apresentação
│   ├── Lancamentos.Application/ # Camada de aplicação
│   ├── Lancamentos.Domain/   # Camada de domínio
│   └── Lancamentos.Infrastructure/ # Camada de infraestrutura
├── Consolidado/              # Microserviço de Consolidado  
│   ├── Consolidado.API/      # Camada de apresentação
│   ├── Consolidado.Application/ # Camada de aplicação
│   ├── Consolidado.Domain/   # Camada de domínio
│   └── Consolidado.Infrastructure/ # Camada de infraestrutura
└── Shared/                   # Componentes compartilhados
    └── FluxoCaixa.Shared/    # Events, DTOs compartilhados

tests/                        # Testes unitários
├── Lancamentos.Tests/
└── Consolidado.Tests/

output/                       # Relatórios gerados automaticamente
```

## Comandos Disponíveis

| Comando | Descrição |
|---------|-----------|
| `make run` | Executa todo o sistema no Docker |
| `make test` | Executa os testes no Docker |
| `make stop` | Para todos os serviços |
| `make clean` | Para serviços e limpa volumes |

## Padrões e Práticas Implementadas

- **Clean Architecture** - Separação de responsabilidades em camadas
- **CQRS** - Separação de comandos e consultas
- **Domain-Driven Design** - Modelagem rica do domínio
- **Event-Driven Architecture** - Comunicação assíncrona via eventos
- **Repository Pattern** - Abstração de acesso a dados
- **Dependency Injection** - Inversão de dependências
- **SOLID Principles** - Código limpo e manutenível

## Características de Alta Disponibilidade

- **Desacoplamento**: Serviços independentes via RabbitMQ
- **Resiliência**: Sistema de retry com backoff exponencial
- **Concorrência**: Tratamento de conflitos de concorrência no banco
- **Monitoramento**: Health checks em todos os serviços
- **Logs Estruturados**: Rastreabilidade completa via Serilog

## Testes

Execute os testes com:
```bash
make test-docker
```

Os testes cobrem:
- Regras de negócio das entidades
- Validações de entrada
- Comportamento dos repositórios
- Handlers de comandos e queries

## Relatórios

Os relatórios são gerados automaticamente e salvos na pasta `output/` com os seguintes formatos:
- **Excel (.xlsx)**: Formatação profissional com cores e estilos
- **CSV (.csv)**: Formato universal para importação

**Estrutura dos relatórios:**
- Data
- Saldo Inicial  
- Total Débitos
- Total Créditos
- Saldo Final
- Quantidade de Transações
- Última Atualização


