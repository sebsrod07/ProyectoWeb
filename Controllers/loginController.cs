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
            Console.WriteLine(account.Email);
            var user=await userManager.FindByEmailAsync(account.Email);
            if(user is null)
                return Unauthorized("Invalid Credentials");
            
            var respone=await signInManager.CheckPasswordSignInAsync(user, account.Password,lockoutOnFailure: false);
            if(!respone.Succeeded)
                return Unauthorized("Contraseña incorrecta");
            var token=await jwtService.createJwt(user);
            return Ok(token);
        }
        catch(Exception ex)
        {
            var msj=ex.Message;
            return BadRequest(msj);
        }

    }
}