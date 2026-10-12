using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Models;
using Contextos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
namespace Controllers;
using Models;
public class pruebasController:ControllerBase
{
    private readonly ApplicationDbContext db;
    private readonly DbProyectoWebContext dbD;
    protected readonly IConfiguration _config;

    public pruebasController(IConfiguration config,ApplicationDbContext _db, DbProyectoWebContext _DBD)
    {
        _config = config;
        db=_db;
        dbD=_DBD;
    }
    [HttpGet]
    [Route("/prueba1")]
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme, Roles ="usuario")]
    public async Task<IActionResult> prueba1()
    {
        try
        {
            var correo =User.Identity.Name;
            if (string.IsNullOrEmpty(correo))
                return Unauthorized("El token no contiene la información del usuario.");
            var CURP=await dbD.VerPropietarios.Where(
                p=>p.Correo==correo
            ).Select(
                a=>a.Curp
            ).FirstOrDefaultAsync();
            var misVehiculos=dbD.VerVehiculosPropietarios.Where(V=>V.Curp==CURP).ToList();
            return Ok(misVehiculos);
        }
        catch (Exception ex)
        {
            string msj=ex.Message;
            msj=ex.InnerException!=null?ex.InnerException.Message:ex.Message;
            return BadRequest(msj);
            
        }
    }
}