namespace Assignment1.Areas.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    
 
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
