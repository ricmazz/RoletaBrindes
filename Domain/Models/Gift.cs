namespace RoletaBrindes.Domain.Models;

public class Gift
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Stock { get; set; }
    public int Weight { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime Created_At { get; set; }
    public DateTime Updated_At { get; set; }
}