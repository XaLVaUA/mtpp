using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MTPP5.Data.Entities;

namespace MTPP5.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Room> Rooms { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<UserRoom> UsersRooms { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var roomEntity = builder.Entity<Room>();

            roomEntity.HasKey(x => x.Id);

            var messageEntity = builder.Entity<Message>();

            messageEntity.HasKey(x => x.Id);
            messageEntity.HasOne(x => x.User).WithMany(x => x.Messages).HasForeignKey(x => x.UserId);
            messageEntity.HasOne(x => x.Room).WithMany(x => x.Messages).HasForeignKey(x => x.RoomId);

            var userEntity = builder.Entity<User>();

            var userRoomEntity = builder.Entity<UserRoom>();

            userRoomEntity.HasKey(x => x.Id);
            userRoomEntity.HasOne(x => x.User).WithMany(x => x.UsersRooms).HasForeignKey(x => x.UserId);
            userRoomEntity.HasOne(x => x.Room).WithMany(x => x.UsersRooms).HasForeignKey(x => x.RoomId);

            base.OnModelCreating(builder);
        }
    }
}