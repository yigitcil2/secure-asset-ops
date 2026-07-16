# Secure Asset & Operations Management Platform

## Overview

Secure Asset & Operations Management Platform is an enterprise-oriented web application for managing
the inventory and lifecycle of an organization's physical and digital assets.

The platform centralizes information about assets such as laptops, servers, network devices,
security equipment, vehicles, software licenses, and other organizational resources.

In addition to storing asset records, the system manages the operational workflows performed throughout an asset's
lifecycle, including registration, assignment, transfer, return, maintenance, incident reporting, retirement, and disposal.

The project is being developed as a portfolio application with an emphasis on software engineering practices,
maintainability, security, traceability, and explicit architectural decisions.

## Problem Statement

Organizations may manage their assets through spreadsheets, disconnected applications,
or manually maintained records. These approaches can make it difficult to determine:

**Which assets belong to the organization
**Where an asset is currently located
**Which employee or department is responsible for an asset
**Whether an asset is available, assigned, under maintenance, lost, or retired
**When an asset was assigned, returned, transferred, or repaired
**Whether an asset can safely be assigned to another employee
**Who performed a critical operation and when it occurred

For example, once a laptop has been assigned to an employee, it should not be available for another assignment.
When the laptop is returned, its physical condition should be evaluated before it becomes available again.

Without controlled workflows and historical records, organizations may experience duplicate assignments,
lost equipment, incomplete maintenance records, weak accountability, and unreliable inventory information.

## Project Goals

The primary goals of the project are to:

**Maintain a centralized inventory of organizational assets
**Manage the complete lifecycle of each asset
**Model asset operations as explicit business workflows
**Prevent invalid operations through domain business rules
**Preserve assignment, transfer, return, and maintenance history
**Provide role-based and permission-based access control
**Record critical user actions through audit logs
**Improve asset visibility, accountability, and operational traceability
**Build a maintainable system that can evolve as business requirements grow

## Architecture

The system is designed as a Modular Monolith using Clean Architecture principles.

The application will be deployed as a single unit while maintaining explicit boundariesbetween its business
capabilities.

The project follows a pragmatic Domain-Driven Design approach:

**Business rules are placed at the center of the system.
**The Domain layer remains independent from frameworks and infrastructure.
**Rich domain models protect asset lifecycle invariants.
**Aggregate roots control valid state transitions.
**Application use cases are organized around business workflows.
**Infrastructure concerns are implemented outside the Domain layer.

The Application layer will use a feature-first organization, while the Domain layer will be organized around
domain concepts such as Assets, Assignments, Maintenance, Users, and Departments.

## Planned Modules

Asset Management

Manages asset registration, identification, categorization, location, status, and lifecycle information.

Asset Assignment

Manages the assignment of assets to users or departments, including returns, transfers, and assignment history.

Maintenance

Manages planned and unplanned maintenance operations, maintenance status, cost information, and maintenance history.

Identity and Access Management

Manages application users, roles, permissions, authentication, and authorization.

Department Management

Manages organizational departments and their relationships with users and assets.

Audit Logging

Records critical system operations, including the acting user, operation type, affected resource, timestamp,
and relevant changes.

Dashboard and Reporting

Provides summarized information about asset inventory, assignments, maintenance activities, asset status distribution,
and recent operations.

## Technology Stack

**C#
**.NET 8
**ASP.NET Core Web API
**Modular Monolith
**Clean Architecture
**Domain-Driven Design principles
**Rich Domain Model
**Feature-First Application organization
**Architecture Decision Records
**Git and Conventional Commits

## Solution Structure

## Architecture Decision Records

## Current Status

The project is currently in the Foundation stage.

## Roadmap
