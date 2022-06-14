using Microsoft.AspNetCore.Identity;

namespace MTPP5.Data.Entities;

public class User : IdentityUser
{
    public List<Message> Messages { get; set; }

    public List<UserRoom> UsersRooms { get; set; }
}
