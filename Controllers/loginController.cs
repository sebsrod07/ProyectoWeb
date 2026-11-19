using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DTOs;
[ApiController]
[Route("/[controller]")]
public class LoginController:ControllerBase
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly SignInManager<IdentityUser> signInManager;
    private readonly JwtService jwtService;

    public LoginController(UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager, JwtService jwtService)
    {
        this.userManager=userManager;
        this.signInManager=signInManager;
        this.jwtService=jwtService;
    }
        [HttpPost("/api/login")]
    public async Task<IActionResult> loginJwt([FromBody] AccountDTO account)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(account.Email);
            if (user is null)
                return Unauthorized("Credenciales inválidas.");
            
            var response = await signInManager.CheckPasswordSignInAsync(user, account.Password, lockoutOnFailure: false);
            if (!response.Succeeded)
                return Unauthorized("Contraseña incorrecta.");

            var token = await jwtService.createJwt(user);
            var cookieOption = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1)
            };
            Response.Cookies.Append("jwtToken", token, cookieOption);

            // 1. Obtenemos los roles del usuario desde ASP.NET Identity
            var roles = await userManager.GetRolesAsync(user);

            // 2. Evaluamos por jerarquía qué vista le corresponde
            string rutaDestino = "/Pages/HTML/Home.html"; // Usuario estándar por defecto

            if (roles.Contains("Admin"))
            {
                rutaDestino = "/Pages/HTML/HomeAdmin.html";
            }
            else if (roles.Contains("Gestion"))
            {
                rutaDestino = "/Pages/HTML/HomeGestion.html";
            }
            else if (roles.Contains("Seguridad"))
            {
                rutaDestino = "/Pages/HTML/check.html";
            }

            // 3. Devolvemos la ruta en formato JSON
            return Ok(new { ruta = rutaDestino });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}