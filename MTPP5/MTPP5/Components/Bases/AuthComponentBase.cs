using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MTPP5.Data.Entities;

namespace BlazorAuth.Components.Bases
{
    public class AuthComponentBase : DbContextComponentBase
    {
        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        protected AuthenticationState AuthenticationState { get; set; }

        protected User User { get; set; }

        protected override async Task OnComponentInitializedAsync()
        {
            await base.OnComponentInitializedAsync();

            AuthenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

            using var applicationDbContext = await ApplicationDbContextFactory.CreateDbContextAsync();

            User = applicationDbContext.Users.First(x => x.UserName == AuthenticationState.User.Identity!.Name);
        }
    }
}
