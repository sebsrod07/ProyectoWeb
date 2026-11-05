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
    
}