# Architecture & Structure Rules

## 1. Project Overview
- **Project Type:** WPF Desktop Application (.NET Framework 4.7.2).
- **Core Objective:** Used in manufacturing assembly lines for step-by-step guidance, product management, and real-time force measurement (via Serial Port).
- **Solution Name:** `Navi_UI_WPF.sln`.

## 2. Directory Structure
Ensure new code is placed in the correct directories:
- `Navi_UI_WPF/`: Main WPF Application.
  - `ViewModels/`: All ViewModels.
  - `Views/`: All XAML Views and their code-behinds.
  - `Styles/`: Shared styling (ResourceDictionaries).
  - `Helpers/` & `Utils/`: Utility classes.
- `Navi.Infrastructure/`: Infrastructure logic (Repositories, Services integrating with APIs or external systems).
- `Navi.Shared/`: Shared DTOs between frontend and backend.
- `USB_IO_Lib/`: External USB I/O libraries.
- `src/Navi.Application/` & `src/Navi.Core/`: Application logic and core domain entities.

## 3. General Principles

### Layer Dependency Rules (STRICT)
| Layer | Description | Allowed Dependencies | Forbidden Dependencies |
| :--- | :--- | :--- | :--- |
| **Navi.Core** | Entities, Enums, Constants, Exceptions | None (Self-contained) | **ANYTHING** internal (App, Infra, UI) |
| **Navi.Application** | DTOs, Service Interfaces, Business Logic | `Navi.Core` | `Navi.Infrastructure`, `Navi_UI_WPF` |
| **Navi.Infrastructure** | Repositories (HTTP Clients for API), Hardware | `Navi.Core`, `Navi.Application` | `Navi_UI_WPF` |
| **Navi_UI_WPF** | User Interface, ViewModels, UI Services | `Navi.Application`, `Navi.Core` | `Navi.Infrastructure` |

### Common Pitfalls to Avoid:
1.  **DTOs in Interfaces in Core**: NEVER place an interface in `Navi.Core` if it returns or accepts a DTO from `Navi.Application`. Move the interface to `Navi.Application/Interfaces` instead.
2.  **Circular References**: Ensure `Core` never references `Application`. If you need a model in Core, create an **Entity** in `Navi.Core.Entities`.
3.  **Logging in Infrastructure**: Avoid direct references to `Serilog` in `Infrastructure` or `Core`. Use abstractions or standard `Exception` throwing for errors.

## 4. Navigation Model
- The `MainViewModel.cs` manages navigation via a `CurrentView` property (`object`).
- A `ContentControl` in `MainWindow.xaml` binds to `CurrentView`, leveraging `DataTemplate` to map ViewModels to Views.
- **Gotcha:** Every time navigation occurs, a NEW instance of the ViewModel is created (data is reset). Do not rely on ViewModel continuous state unless injected as a Singleton.
