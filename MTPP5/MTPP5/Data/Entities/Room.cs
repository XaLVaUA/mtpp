namespace MTPP5.Data.Entities;

public class Room
{
    public int Id { get; set; }

    public string Name { get; set; }

    public bool IsClosed { get; set; }

    public List<Message> Messages { get; set; }

    public List<UserRoom> UsersRooms { get; set; }
}
