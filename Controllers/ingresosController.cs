using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Contextos;
using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
[ApiController]
[Route("/api/[controller]")]
public class ingresosController:ControllerBase
{
    
    private readonly DbProyectoWebContext dbD;
    private readonly IDataProtector _protector;
    protected readonly IConfiguration config;

    public ingresosController( DbProyectoWebContext _dbd, IConfiguration _config, IDataProtectionProvider DP)
    {
        dbD=_dbd;
        config=_config;   
        _protector=DP.CreateProtector("QRAccesoVehicular");
    }
    [HttpPost("/ingresos/registrarIngreso")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles ="Admin")]
    public async Task<IActionResult> registrarIngreso(string placa, string mail)
    {
        try
        {
            await dbD.Database.ExecuteSqlInterpolatedAsync(@$"
            EXEC crearIngreso @placa = {placa},
            @mailConductor={mail}");
            await dbD.SaveChangesAsync();
            return Ok("Ingreso creado");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.InnerException is null? ex.Message: ex.InnerException.Message);
        }
    }
    [HttpGet("/generar-acceso")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles ="usuario")] // Cualquier usuario autenticado puede pedir su propio pase
    public async Task<IActionResult> GenerarCodigoAcceso(string placa)
    {
        var correo = User.Identity.Name;

        var vehiculo = await dbD.VerVehiculosPropietarios.FirstOrDefaultAsync(v=>v.Placas==placa &&v.Correo==correo);
        if (vehiculo == null) return Unauthorized();
        string infoVital = $"{placa}|{correo}|{DateTime.UtcNow:O}";
        string codigoEncriptado = _protector.Protect(infoVital);
        return Ok(new { codigoSeguro = codigoEncriptado });
    }
    [HttpPost("/validar-acceso")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles ="usuario")] // ESTO ES VITAL: Solo el rol de seguridad tiene acceso
    public IActionResult ValidarAcceso([FromBody] string codigoEscaneado)
    {
        try
        {
            string infoDesencriptada = _protector.Unprotect(codigoEscaneado);
            var partes = infoDesencriptada.Split('|');
            var placa = partes[0];
            var propietario = partes[1];
            var fechaGeneracion = DateTime.Parse(partes[2]).ToUniversalTime();
            if ((DateTime.UtcNow - fechaGeneracion).TotalMinutes > 15)
            {
                return BadRequest("El código de acceso ha expirado. Genere uno nuevo.");
            }            
            return Ok("Access Granted");
        }   
        catch (System.Security.Cryptography.CryptographicException)
        {
            return BadRequest("Código de acceso inválido o corrupto.");
        }
        catch(Exception ex)
        {
            return BadRequest(ex.InnerException!=null? ex.InnerException.Message:ex.Message);
        }
    }
}