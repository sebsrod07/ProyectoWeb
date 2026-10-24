using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using DTOs;
using Contextos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Models;
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
    public Task<IActionResult> solicitarRegistro([FromBody] Vehiculo V)
    {
        try
        {
            
        }


        
    }
}