# Navi Project Architecture: Senior Architect Manifesto

This document serves as the ground truth for all code generation and architectural decisions in the Navi solution.

## 1. Architecture Style
- **Pattern:** Distributed Clean Architecture.
- **Client:** WPF .NET Framework 4.7.2 application.
- **Server:** ASP.NET Core Web API (referenced in `API.md`).
- **Layers:**
  - **Core:** Entities, Enums, Constants, Exceptions, and Interface definitions (Shared by all).
  - **Application:** DTOs, Business Logic Services, and Service Interfaces.
  - **Infrastructure:** Repositories and external system integrations (API Clients, Hardware libs).
  - **UI/WPF:** ViewModels, Views, and UI-specific services.

## 2. Dependency Flow (Strict)
1. `UI` → `Application` → `Core`
2. `Infrastructure` → `Application` → `Core`
3. `Core` has **NO** dependencies.
4. `Application` has **NO** dependency on `Infrastructure` or `UI`.

## 3. Programming Patterns
- **MVVM:** Using `CommunityToolkit.Mvvm`, but restricted by .NET 4.7.2. **No Source Generators allowed.**
- **DI:** Constructor injection using `Microsoft.Extensions.DependencyInjection`.
- **Navigation:** View-First navigation managed by `MainViewModel` and `DataTemplate` mapping.
- **Data Access:** Repositories in Infrastructure use `HttpClient` to communicate with the Backend API.
- **API Response:** Standardized `ApiResponse<T>` wrapper for consistency and error handling.

## 4. Coding Standards
- **Naming:**
  - Private fields: `_camelCase` (e.g., `_httpClient`).
  - Public properties/methods: `PascalCase` (e.g., `GetAllAsync`).
  - ViewModels: `[Feature]ViewModel`.
  - Views: `[Feature]View`.
  - DTOs: `[Feature]Dto` or `[Feature]For[Action]Dto`.
- **Async/Await:** All I/O and network operations **MUST** be `async Task`.
- **Error Handling:** Use `try-catch` in Services to wrap `HttpRequestException` into custom `ApiException`.

## 5. UI & UX Rules
- **HandyControls:** Primary UI component library.
- **LiveCharts:** Real-time data visualization.
- **Threading:** Always use the `Dispatcher` when updating UI-bound properties from background threads.
- **Styles:** Centralized in `Navi_UI_WPF/Styles/`.
