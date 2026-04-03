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
- **Clean Architecture:** Keep UI logic purely in `Navi_UI_WPF`. Business logic and data access must reside in `Navi.Infrastructure`, `Navi.Application`, or `Navi.Core`.
- **API Integration:** The backend uses standard REST APIs (`/api/{controller}`). All API calls should be encapsulated in Repositories or Services within `Navi.Infrastructure`.
- **Soft Delete:** Entities use `IsDelete = true`. Do not perform hard physical deletes when writing data access logic.

## 4. Navigation Model
- The `MainViewModel.cs` manages navigation via a `CurrentView` property (`object`).
- A `ContentControl` in `MainWindow.xaml` binds to `CurrentView`, leveraging `DataTemplate` to map ViewModels to Views.
- **Gotcha:** Every time navigation occurs, a NEW instance of the ViewModel is created (data is reset). Do not rely on ViewModel continuous state unless injected as a Singleton.
