namespace RoletaBrindes.Domain.Models;

public class Spin
{
    public int Id { get; set; }
    public int Participant_Id { get; set; }
    public string? Participant_Name { get; set; }
    public string? Participant_Phone { get; set; }
    public int? Gift_Id { get; set; }
    public string? Gift_Name { get; set; }
    public bool Won { get; set; }
    public DateTime Created_At { get; set; }
}