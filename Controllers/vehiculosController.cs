using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using DTOs;
using Contextos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Models;
using Microsoft.EntityFrameworkCore;
[ApiController]
[Route("/api/[controller]")]
public class vehiculosController:ControllerBase
{
      private readonly ApplicationDbContext db;
    private readonly DbProyectoWebContext dbD;
    protected readonly IConfiguration config;

    public vehiculosController(ApplicationDbContext _db, DbProyectoWebContext _dbd, IConfiguration _config )
    {
        db=_db;
        dbD=_dbd;
        config=_config;
        
    }
    [HttpPost("/solicitarRegistro")]
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme, Roles ="usuario")]
    public async Task<IActionResult> solicitarRegistro([FromBody] VehiculoDTO V)
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
            if(string.IsNullOrEmpty(CURP))
                throw new Exception("EL CURP NO EXISTE");
            var VS = new Vehiculo
                {
                CurppropietarioVehiculo=CURP,
                PlacaVehiculo=V.PlacaVehiculo,
                ColorVehiculo=V.ColorVehiculo,
                ModeloVehiculo=V.ModeloVehiculo,
                FabricanteVehiculo=V.FabricanteVehiculo,
                TipoVehiculo=V.TipoVehiculo,
                NumeroSerieVehiculo=V.NumeroSerieVehiculo
            };
            dbD.Vehiculos.Add(VS);
            await dbD.SaveChangesAsync();
            return Ok();
        }
        catch(Exception ex)
        {
            string msj=ex.Message;
            msj=ex.InnerException!=null?ex.InnerException.Message:ex.Message;
            return BadRequest(msj);   
        }


        
    }
}