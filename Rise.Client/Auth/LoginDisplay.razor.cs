using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;

namespace Rise.Client.Auth;

public partial class LoginDisplay
{
    public void BeginLogOut()
    {
        var auth0Domain = Configuration["Auth0:Authority"];
        var clientId = Configuration["Auth0:ClientId"];
        var returnTo = Navigation.BaseUri + "authentication/login";

        var logoutUrl = $"{auth0Domain}/v2/logout?client_id={clientId}&returnTo={Uri.EscapeDataString(returnTo)}";

        Navigation.NavigateTo(logoutUrl, forceLoad: true);
    }

}