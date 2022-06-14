using Microsoft.AspNetCore.Identity;

namespace MTPP5.Data.Entities;

public class Message
{
    public int Id { get; set; }

    public string Content { get; set; }

    public DateTime DateTime { get; set; }

    public int RoomId { get; set; }

    public Room Room { get; set; }

    public string UserId { get; set; }

    public User User { get; set; }
}
