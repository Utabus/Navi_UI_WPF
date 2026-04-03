# MVVM & CommunityToolkit Rules

## 1. .NET 4.7.2 Restriction (CRITICAL)
- The project runs on **.NET Framework 4.7.2**.
- **Source Generators DO NOT WORK.** 
- **Rule:** Do NOT use `[ObservableProperty]` or `[RelayCommand]` attributes. They will fail to compile or generate code.

## 2. Base ViewModel
- ALL ViewModels must inherit from `CommunityToolkit.Mvvm.ComponentModel.ObservableObject`.

## 3. Property Pattern
You **MUST** write full properties and use `SetProperty(ref field, value)`.
```csharp
private string _myProperty;
public string MyProperty
{
    get => _myProperty;
    set => SetProperty(ref _myProperty, value);
}
```

## 4. Command Pattern
You **MUST** explicitly instantiate `RelayCommand` or `RelayCommand<T>` in the constructor.

**Parameterless Command:**
```csharp
public IRelayCommand SaveCommand { get; }

// In constructor:
SaveCommand = new RelayCommand(ExecuteSave, CanExecuteSave);
```

**Parameterized Command:**
```csharp
public IRelayCommand<NaviProductDto> EditCommand { get; }

// In constructor:
EditCommand = new RelayCommand<NaviProductDto>(item => DoEdit(item), item => item != null);
```

**Async Command:**
```csharp
public IAsyncRelayCommand<int> LoadCommand { get; }

// In constructor:
LoadCommand = new AsyncRelayCommand<int>(async id => await LoadAsync(id));
```

## 5. View-ViewModel Mapping
Every View (`Views/XxxView.xaml`) must have a matching ViewModel (`ViewModels/XxxViewModel.cs`). The DataContext must be explicitly bound, either in XAML or dynamically mapped in `MainWindow.xaml` via `DataTemplate`.
