namespace MTPP5.Data.Entities;

public class UserRoom
{
    public int Id { get; set; }

    public string UserId { get; set; }

    public User User { get; set; }

    public int RoomId { get; set; }

    public Room Room { get; set; }
}
