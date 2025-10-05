# Commands

The Application Model provides comprehensive support for implementing commands in your backend application. Commands represent actions or operations that modify the state of your system and are a fundamental part of CQRS (Command Query Responsibility Segregation) architecture.

## Topics

| Topic | Description |
| ------- | ----------- |
| [Controller based](./controller-based.md) | How to implement commands using controller-based approach. |
| [Model Bound](./model-bound.md) | How to work with model-bound commands for simplified parameter handling. |
| [Command Context](./command-context.md) | Understanding CommandContext and how to extend it with custom values for the non-controller-based pipeline. |
| [Command Filters](./command-filters.md) | How to implement command filters for cross-cutting concerns in the non-controller-based pipeline. |
| [Response Value Handlers](./response-value-handlers.md) | How to customize command response handling with value handlers. |
| [Validation](./validation.md) | How to implement validation for commands. |

> **💡 Frontend Integration**: Automatically generate TypeScript proxies for your commands with the [Proxy Generation](../proxy-generation.md) feature.

## Overview

Commands in the Application Model are designed to be simple to implement while providing powerful features like automatic validation, response handling, and integration with the overall application architecture. Whether you prefer controller-based approaches or model-bound commands, the framework provides the flexibility to work with your preferred style while maintaining consistency and best practices.
