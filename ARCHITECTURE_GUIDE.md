# Guia Definitivo de Arquitetura

## Objetivo

Manter a solucao coesa, escalavel e previsivel, preservando:

- arquitetura em camadas
- CQRS
- contrato HTTP atual
- comportamento funcional atual

## Principios

- Cada responsabilidade no seu lugar.
- Application organiza casos de uso por feature.
- API adapta HTTP, sem regra de negocio.
- Infrastructure implementa integracoes tecnicas.
- Domain concentra modelos centrais.

## Estrutura alvo

Application/

- Interfaces/
- Messaging/
- Deployments/
  - GetAllDeployments/
  - GetIisStatus/
  - GetSvnCommits/
  - RunDeployment/
  - UpdateSvnRevision/

Api/

- Controllers/
- Dtos/Deployments/
  - Requests/
  - Responses/

Infrastructure/

- Configuration/
- FileSystem/
- Iis/
- Svn/

Domain/

- modelos de dominio

## Regras de Application

- Interfaces de dependencia externa ficam em Application/Interfaces.
- Base CQRS fica em Application/Messaging.
- Cada feature usa pastas de tipo:
  - Commands ou Queries
  - Handlers
  - Responses
  - DTOs/Requests
  - DTOs/Responses
  - Mappers
  - Validators
- Nao mover interfaces para dentro das features.

## Regras de API

- Receber request HTTP.
- Mapear para Commands/Queries.
- Retornar envelope padrao.
- Nao colocar regra de negocio em controllers.

Envelope mantido:
{
"sucesso": true,
"mensagem": "OK",
"codHttp": 200,
"detail": null,
"resultado": "...",
"traceId": "..."
}

## Regras de Infrastructure

- Organizar por contexto tecnico.
- Evitar pasta generica helper.
- Implementacoes concretas ficam isoladas da Application.

## Itens para revisao posterior

- Application/DeploymentConfigurationValidator.cs
- Application/DeploymentOrchestratorService.cs
- Application/SiteStatus.cs

## Nao fazer

- Nao alterar contrato HTTP.
- Nao mudar regra de negocio durante refatoracao estrutural.
- Nao misturar regra de dominio com API.
