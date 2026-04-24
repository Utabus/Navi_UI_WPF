# Build & Test Verification Rules

To ensure stability in the **.NET Framework 4.7.2** environment and prevent architectural violations, the following rules apply to any new features or logic changes.

## 1. Mandatory Build Verification (CRITICAL)
- **Rule:** Every time a new class, interface, or service is added, you **MUST** run a build verification check.
- **Command:** `dotnet build Navi_UI_WPF\Navi_UI_WPF.csproj` (or build the solution in Visual Studio).
- **Goal:** Catch "missing file reference" errors caused by the legacy `.csproj` format and verify that libraries (like Serilog or Newtonsoft) are correctly referenced in the project layer.

## 2. Architectural Layer Validation
- **Rule:** Strictly follow the dependency hierarchy.
  - `Navi.Core` -> No dependencies on other projects.
  - `Navi.Application` -> Depends ONLY on `Navi.Core`.
  - `Navi.Infrastructure` -> Depends on `Navi.Core` and potentially `Navi.Application`.
  - `Navi_UI_WPF` -> Depends on `Navi.Application` and `Navi.Core`.
- **Reason:** Prevents circular dependencies and ensures that Core interfaces remain clean of Application/Infrastructure details.

## 3. Functional Build Testing
- **New APIs:** When integrating a new API (e.g., Manufa Assist), verify the full chain:
  1. **Core:** Entity + Interface.
  2. **Infrastructure:** Repository implementation + HttpClient call.
  3. **Application:** Service orchestration.
  4. **ViewModel:** Data binding and async calling.
- **Rule:** If a functional block cannot be unit-tested easily (due to Serial Port or External API dependencies), you must perform a "manual build-and-run" check and verify logic with log outputs or UI state.

## 4. Legacy .csproj Maintenance
- Since this project uses the old `.csproj` format, adding a file to the directory is **NOT enough**.
- **Rule:** You must manually ensure that the `<Compile Include="path\to\file.cs" />` entry is added to the correct `.csproj` file. Failing to do this will result in "Type or namespace not found" errors during build.
