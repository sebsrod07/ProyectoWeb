using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DTOs;
[ApiController]
[Route("/api/[controller]")]
public class RoleController:ControllerBase
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly SignInManager<IdentityUser> signInManager;
    private readonly RoleManager<IdentityRole> roleManager;
    public RoleController(UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager,RoleManager<IdentityRole> roleManager)
    {
        this.userManager=userManager;
        this.signInManager=signInManager;
        this.roleManager=roleManager;
    }
    [HttpPost("/createRole")]
    public async Task<IActionResult> createRole(string nombreRol)
    {
        if (string.IsNullOrWhiteSpace(nombreRol))
                return BadRequest("El nombre del rol es obligatorio.");

            // Verificamos si el rol ya existe
            var rolExiste = await roleManager.RoleExistsAsync(nombreRol);
            if (rolExiste)
                return BadRequest($"El rol '{nombreRol}' ya existe.");

            // Creamos el rol
            var resultado = await roleManager.CreateAsync(new IdentityRole(nombreRol));
            
            if (resultado.Succeeded)
                return Ok(new { mensaje = $"Rol '{nombreRol}' creado exitosamente." });
                
            return BadRequest(resultado.Errors);
        
    }
    [HttpPost("/addToRole")]
    public async Task<IActionResult> addToRole(RoleAccountDTO account)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(account.Email);
            if(user is null)
                return NotFound("user not Found");
            if(!await roleManager.RoleExistsAsync(account.Role))
                return BadRequest("Role doesn't exists");
            var response=await userManager.AddToRoleAsync(user,account.Role);
            return response.Succeeded?Ok("Role added correctly"):BadRequest("Somthing went wrong");
            
        }
        catch(Exception ex)
        {
            string msj=ex.InnerException is null?ex.Message:ex.InnerException.Message;
            return BadRequest(msj);
        }   
    }
    
}