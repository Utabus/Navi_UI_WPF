# UI, Threading & Third-Party Library Rules

## 1. UI Threading (Dispatcher)
- **Background Tasks:** Any data updates triggered from background threads (e.g., Serial Port data received, Task.Run, asynchronous events) **MUST** be marshaled to the UI thread.
- **Implementation:** Use `Application.Current.Dispatcher.BeginInvoke(...)` or `Invoke(...)`.
```csharp
Application.Current.Dispatcher.BeginInvoke(new Action(() =>
{
    // Update UI-bound properties here
    ChartSeries[0].Values.Add(newPoint);
}));
```

## 2. LiveCharts Real-Time Data
- **Library used:** `LiveCharts` & `LiveCharts.Wpf` v0.9.7.
- **Thread Safety:** `ChartValues<T>` must be modified on the UI thread.
- **Clear Logic:** When initializing or restarting data collection (e.g., opening a COM port), you MUST `.Clear()` the lists/arrays holding chart values to prevent memory leaks and graph distortion.

## 3. HandyControls Usage
- **Library used:** `HandyControls` v3.5.3.
- When using new controls from HandyControls, ensure that the appropriate `ResourceDictionary` is included in `App.xaml` if styling issues occur.

## 4. Serial Port (Hardware integration)
- E.g., `ForceGaugeViewModel.cs`.
- Configuration: `COM3` (can be dynamic later), `9600 baud`, `Parity.None`, `8 data bits`, `1 stop bit`.
- Protocol parsing requires exact byte counting and ASCII hex decoding. Carefully catch exceptions around COM port opening (`UnauthorizedAccessException`, `IOException`) and show appropriate error messages to user instead of crashing the app.

## 5. Sample Data Fallback
- For ViewModels fetching data from API (e.g., `ProductAssemblyViewModel`, `NaviProductViewModel`), maintain a `LoadSampleData()` fallback method.
- This ensures the UI can still be developed, tested, and previewed when the backend API is unreachable.
