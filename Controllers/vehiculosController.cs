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
public class vehiculosController : ControllerBase
{
    private readonly DbProyectoWebContext dbD;
    protected readonly IConfiguration config;

    public vehiculosController(DbProyectoWebContext _dbd, IConfiguration _config)
    {
        dbD = _dbd;
        config = _config;
    }

    [HttpPost("/solicitarRegistro")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "usuario")]
    public async Task<IActionResult> solicitarRegistro([FromBody] VehiculoDTO V)
    {
        try
        {
            var correo = User.Identity?.Name;
            if (string.IsNullOrEmpty(correo))
                return Unauthorized("El token no contiene la información del usuario.");
            var CURP = await dbD.VerPropietarios.Where(
                p => p.Correo == correo
            ).Select(
                a => a.Curp
            ).FirstOrDefaultAsync();
            if (string.IsNullOrEmpty(CURP))
                throw new Exception("EL CURP NO EXISTE");
            await dbD.Database.ExecuteSqlInterpolatedAsync(
                @$"EXEC ingresarSolicitudVehiculo
                    {V.PlacaVehiculo},
                    {V.ColorVehiculo},
                    {V.ModeloVehiculo},
                    {V.FabricanteVehiculo},
                    {CURP},
                    {V.TipoVehiculo},
                    {V.NumeroSerieVehiculo};"
            );
            await dbD.SaveChangesAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            string msj = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            return BadRequest(msj);
        }
    }

    [HttpPost("/altaVehiculo")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Gestion,Admin")]
    public async Task<IActionResult> altaVehiculo([FromQuery] string? placa)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(placa))
                return BadRequest("Debe proporcionar la placa del vehículo.");

            var placaTarget = placa.Trim().ToUpper();
            var vehiculo = await dbD.Vehiculos.FirstOrDefaultAsync(v => v.PlacaVehiculo == placaTarget || v.PlacaVehiculo.Trim() == placaTarget);
            if (vehiculo is null)
                return NotFound($"Vehículo con placa '{placaTarget}' no está registrado.");

            vehiculo.EstatusVehiculo = "ALTA";
            await dbD.SaveChangesAsync();
            return Ok(new { mensaje = $"Vehículo con placa {placaTarget} dado de alta exitosamente." });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.InnerException is null ? ex.Message : ex.InnerException.Message);
        }
    }

    [HttpPost("/bajaVehiculo")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Gestion,Admin")]
    public async Task<IActionResult> bajaVehiculo([FromQuery] string? placa)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(placa))
                return BadRequest("Debe proporcionar la placa del vehículo.");

            var placaTarget = placa.Trim().ToUpper();
            var vehiculo = await dbD.Vehiculos.FirstOrDefaultAsync(v => v.PlacaVehiculo == placaTarget || v.PlacaVehiculo.Trim() == placaTarget);
            if (vehiculo is null)
                return NotFound($"Vehículo con placa '{placaTarget}' no está registrado.");

            vehiculo.EstatusVehiculo = "BAJA";
            await dbD.SaveChangesAsync();
            return Ok(new { mensaje = $"Vehículo con placa {placaTarget} dado de baja exitosamente." });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.InnerException is null ? ex.Message : ex.InnerException.Message);
        }
    }

    [HttpGet]
    [Route("/getVehiculos")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "usuario,Gestion,Admin")]
    public async Task<IActionResult> getVehiculos(
        [FromQuery] string? estatus = null,
        [FromQuery] string? placa = null,
        [FromQuery] string? curp = null,
        [FromQuery] string? tipo = null)
    {
        try
        {
            var correo = User.Identity?.Name;
            if (string.IsNullOrEmpty(correo))
                return Unauthorized("El token no contiene la información del usuario.");

            IQueryable<VerVehiculosPropietario> query = dbD.VerVehiculosPropietarios;

            bool isGestionOrAdmin = User.IsInRole("Gestion") || User.IsInRole("Admin");

            if (!isGestionOrAdmin)
            {
                var CURP = await dbD.VerPropietarios
                    .Where(p => p.Correo == correo)
                    .Select(a => a.Curp)
                    .FirstOrDefaultAsync();

                if (string.IsNullOrEmpty(CURP))
                    return Ok(new List<VerVehiculosPropietario>());

                query = query.Where(v => v.Curp == CURP);

                if (string.IsNullOrWhiteSpace(estatus))
                {
                    query = query.Where(v => v.Estatus == "ALTA" || v.Estatus.Trim() == "ALTA");
                }
            }

            if (!string.IsNullOrWhiteSpace(estatus))
            {
                var estatusUpper = estatus.Trim().ToUpper();
                query = query.Where(v => v.Estatus == estatusUpper || v.Estatus.Trim() == estatusUpper);
            }

            if (!string.IsNullOrWhiteSpace(placa))
            {
                var placaUpper = placa.Trim().ToUpper();
                query = query.Where(v => v.Placas.Contains(placaUpper));
            }

            if (!string.IsNullOrWhiteSpace(curp))
            {
                var curpUpper = curp.Trim().ToUpper();
                query = query.Where(v => v.Curp == curpUpper);
            }

            if (!string.IsNullOrWhiteSpace(tipo))
            {
                var tipoUpper = tipo.Trim().ToUpper();
                query = query.Where(v => v.Tipo.ToUpper() == tipoUpper);
            }

            var vehiculos = await query.ToListAsync();
            return Ok(vehiculos);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
        }
    }
}
