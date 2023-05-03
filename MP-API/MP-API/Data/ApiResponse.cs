namespace MP_API.Data;

// For sending api response data back to client
public class ApiResponse
{
    public object? DataObject { get; set; } // Data might not always be retrieved or needed
    public bool Success { get; set; }
    public List<string>? Messages { get; set; }
}
