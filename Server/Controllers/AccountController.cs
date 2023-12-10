namespace MicrosoftEntraIdMudBlazor.Server.Controllers;

// orig src https://github.com/berhir/BlazorWebAssemblyCookieAuth
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private static readonly char[] trimChars = ['"'];

    [HttpGet("Login")]
    public ActionResult Login(string? returnUrl, string? claimsChallenge)
    {
        // var claims = "{\"access_token\":{\"acrs\":{\"essential\":true,\"value\":\"c1\"}}}";
        // var claims = "{\"id_token\":{\"acrs\":{\"essential\":true,\"value\":\"c1\"}}}";
        var redirectUri = !string.IsNullOrEmpty(returnUrl) ? returnUrl : "/";

        var properties = new AuthenticationProperties { RedirectUri = redirectUri };

        if(claimsChallenge != null)
        {
            string jsonString = claimsChallenge.Replace("\\", "")
                .Trim(trimChars);

            properties.Items["claims"] = jsonString;
        }

        return Challenge(properties);
    }

    // [ValidateAntiForgeryToken] // not needed explicitly due the the Auto global definition.
    [Authorize]
    [HttpPost("Logout")]
    public IActionResult Logout()
    {
        return SignOut(
            new AuthenticationProperties { RedirectUri = "/" },
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme);
    }
}
