using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DTOs;
using Contextos;
using Microsoft.EntityFrameworkCore;
[ApiController]
[Route("/[controller]")]
public class registroController:ControllerBase
{
     private readonly UserManager<IdentityUser> userManager;
     private readonly ApplicationDbContext db;
     protected readonly IConfiguration _config;
     public registroController(UserManager<IdentityUser> userManager, IConfiguration config, ApplicationDbContext _db)
    {
        this.userManager=userManager;
        _config=config;
        db=_db;

    }
    [HttpPost("/api/register")]
    public async Task<IActionResult> register([FromBody] RegistroDTO registro)
    {
        try
        {

            var nuevoId = Guid.NewGuid().ToString();
            var usuarioTemp = new IdentityUser { Id = nuevoId, UserName = registro.email, Email = registro.email };
            var passwordHash = userManager.PasswordHasher.HashPassword(usuarioTemp, registro.password);
            await db.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC dbo.crearUsuario 
                    @id = {nuevoId}, 
                    @mail = {registro.email}, 
                    @hashpass = {passwordHash},
                    @curp = {registro.curp},
                    @pNombre = {registro.primerNombre},
                    @sNombre = {registro.segundoNombre},
                    @pApellido = {registro.APaterno},
                    @mApellido = {registro.AMaterno},
                    @rfc = {registro.rfc}
            ");
            return Ok("Usuario Creado Correctamente");
        }
        catch(Exception ex)
        {
            string msj=ex.Message;
            msj=ex.InnerException!=null?ex.InnerException.Message:ex.Message;
            return BadRequest(msj);
            
        }
    }
}