namespace RoletaBrindes.Application.DTOs;

public record SpinRequest(string Name, string Phone);
public class SpinResponse
{
    public bool Won { get; set; }
    public string? GiftName { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Segments { get; set; } = new();
    public int TargetIndex { get; set; }
}