# Queries

The Application Model provides comprehensive support for implementing queries in your backend application.
Queries are used for retrieving data and are a key component of CQRS (Command Query Responsibility Segregation) architecture, offering powerful features like observability, validation, and flexible parameter handling.

## Topics

| Topic | Description |
| ------- | ----------- |
| [Controller based](./controller-based.md) | How to implement queries using controller-based approach. |
| [Model Bound](./model-bound.md) | How to work with model-bound queries for simplified parameter handling. |
| [Observable Queries](./observable-queries.md) | How to implement reactive, observable queries that update in real-time. |
| [Query Pipeline](./query-pipeline.md) | Understanding the query pipeline and how queries are processed. |
| [Validation](./validation.md) | How to implement validation for query parameters. |

> **💡 Frontend Integration**: Automatically generate TypeScript proxies for your queries with the [Proxy Generation](../proxy-generation.md) feature.

## Overview

Queries in the Application Model are designed to be flexible and powerful, supporting both traditional request-response patterns and reactive, observable queries that can provide real-time updates.
The framework handles parameter validation, binding, and processing through a comprehensive pipeline that ensures consistent behavior across your application.
Query arguments and parameter binding are covered within the controller-based and model-bound topics.
