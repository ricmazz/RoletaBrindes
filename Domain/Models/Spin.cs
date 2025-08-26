namespace RoletaBrindes.Domain.Models;

public class Spin
{
    public int Id { get; set; }
    public int Participant_Id { get; set; }
    public int? Gift_Id { get; set; }
    public bool Won { get; set; }
    public DateTime Created_At { get; set; }
}