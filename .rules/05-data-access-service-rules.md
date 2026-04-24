# Data Access & Service Logic Rules

## 1. HTTP Service Pattern (Application Layer)
All services communicating with the backend API must follow this pattern:

### Standard Base Method
Use a `SendAsync<T>` helper to handle request/response logic consistently.

```csharp
private async Task<T> SendAsync<T>(HttpRequestMessage request)
{
    var response = await _httpClient.SendAsync(request);
    var content  = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode)
        throw new ApiException((int)response.StatusCode, response.ReasonPhrase, content);

    // Standard wrapper defined in Navi.Core.Common
    var wrapper = JsonConvert.DeserializeObject<Core.Common.ApiResponse<T>>(content);
    
    if (wrapper == null || !wrapper.Success)
        throw new ApiException((int)response.StatusCode, wrapper?.Message ?? "Unknown Error");

    return wrapper.Data;
}
```

## 2. DTO Usage
- **Input:** Use `CreateXxxDto` or `UpdateXxxDto` for POST/PUT requests.
- **Output:** Services should return `XxxDto` or `List<XxxDto>`.
- **Mapping:** Mapping from Entity to DTO happens in the Backend. The Client Services only handle DTOs.

## 3. Error Handling
- Do not return null on failure unless null is a valid business result.
- Throw custom exceptions (e.g., `ApiException`) so the UI layer can catch them and show user-friendly messages.
- Log critical failures using a logging abstraction (if available) or standard UI notifications.

## 4. Parameter Handling
- Use `Uri.EscapeDataString(term)` for query parameters.
- Use `string.Format(ApiEndpoints.XxxById, id)` for path parameters.

## 5. Transaction Handling
- Operations involving multiple entities (e.g., `CreateWithItems`) should be called via a specialized API endpoint that handles the transaction on the server side.
- Do not attempt to manage database transactions from the WPF Client.
