using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MTPP5.Data;

namespace BlazorAuth.Components.Bases;

public class DbContextComponentBase : BaseComponentBase
{
    [Inject]
    public IDbContextFactory<ApplicationDbContext> ApplicationDbContextFactory { get; set; }
}
