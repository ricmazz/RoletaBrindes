namespace RoletaBrindes.Domain.Models;

public class Participant
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime Created_At { get; set; }
}