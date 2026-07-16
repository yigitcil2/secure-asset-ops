
# ADR-001: Use a Modular Monolith Architecture

* **Status:** Accepted
* **Date:** 2026-07-16

## Context

Secure Asset & Operations Management Platform will contain multiple business capabilities, including asset management, asset assignments, maintenance operations, user management, authorization, and audit logging.

The system requires clear boundaries between these business areas. However, the project is currently being developed by an individual developer and does not yet have the operational scale, deployment requirements, or organizational structure that would justify a distributed microservices architecture.

The following architectural approaches were considered:

1. Traditional monolith
2. Modular monolith
3. Microservices

A traditional monolith would be simple to deploy, but without explicit module boundaries it could gradually become tightly coupled and difficult to maintain.

A microservices architecture would provide independent deployment and scaling capabilities, but it would also introduce significant operational complexity, including inter-service communication, distributed transactions, service discovery, observability, message delivery, and more complex testing and deployment processes.

## Decision

The system will be implemented as a **Modular Monolith**.

The application will be deployed and operated as a single unit, while its internal structure will be divided into clearly defined business modules.

Initial modules may include:

* Asset Management
* Asset Assignment
* Maintenance
* Identity and Access Management
* Departments
* Audit Logging

Module boundaries will be reflected in the codebase through domain concepts, application use cases, namespaces, and dependency rules.

The solution will also use Clean Architecture principles to keep business rules independent from external technologies such as ASP.NET Core, Entity Framework Core, SQL Server, and authentication providers.

Domain-Driven Design principles will be applied pragmatically where they improve the representation and enforcement of business rules.

## Rationale

A Modular Monolith provides an appropriate balance between simplicity and maintainability for the current project.

It was selected because:

* The application can be developed, tested, debugged, and deployed as a single unit.
* Business areas can still have explicit and enforceable boundaries.
* Operational complexity remains manageable for a small development team.
* Database transactions can initially be handled within a single application and database.
* Clean Architecture and domain-oriented modeling can be applied without introducing distributed-system complexity.
* Modules may be extracted into separate services in the future if independent deployment or scaling becomes necessary.

## Alternatives Considered

### Traditional Monolith

A traditional monolith would have lower initial complexity. However, organizing the application only around technical layers could allow business logic and dependencies to spread across the system. As the project grows, this could result in tightly coupled modules and reduced maintainability.

### Microservices

Microservices would support independent deployment, technology selection, and scaling for individual services. However, these benefits are not currently required.

Starting with microservices would introduce unnecessary complexity in areas such as:

* Network communication
* Distributed transactions
* Eventual consistency
* Service discovery
* API gateway management
* Centralized logging and tracing
* Deployment orchestration
* Integration testing
* Failure handling between services

The operational cost would be disproportionate to the current project requirements and team size.

## Consequences

### Positive Consequences

* The system has a simple deployment model.
* Local development and debugging remain straightforward.
* Business modules can be designed with clear responsibilities.
* Transactions can initially remain within a single database boundary.
* Infrastructure and operational costs remain low.
* The architecture supports incremental development.
* Future service extraction remains possible when supported by concrete requirements.

### Negative Consequences

* Modules cannot initially be deployed or scaled independently.
* All modules share the same application process.
* Poorly enforced module boundaries could cause the architecture to degrade into a tightly coupled monolith.
* A failure affecting the application process may affect the entire system.
* Future extraction of a module may require data and dependency refactoring.

## Constraints and Rules

To preserve modularity:

* Business rules must remain in the Domain layer.
* Application use cases must be organized around business capabilities.
* Modules must not access another module’s internal implementation directly.
* Cross-module communication must use explicitly defined contracts or application-level workflows.
* Infrastructure concerns must not leak into the Domain layer.
* New distributed services must not be introduced without a demonstrated deployment, scaling, ownership, or reliability requirement.

## Review Triggers

This decision should be reviewed when one or more of the following conditions occur:

* A module requires independent deployment.
* A module has substantially different scaling requirements.
* Separate teams require independent ownership and release cycles.
* The shared database becomes a significant constraint.
* Availability requirements demand fault isolation between modules.
* Regulatory or security requirements require physical separation.
