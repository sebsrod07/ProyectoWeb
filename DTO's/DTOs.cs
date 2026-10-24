using System.ComponentModel.DataAnnotations;

namespace DTOs;
public class AccountDTO
{
    [Required] 
    public string Email {get;set;} = null!;
    [Required]
    public string Password {get;set;} = null!;
}
public class RoleAccountDTO
{
    [Required] 
    public string Email {get;set;} = null!;
    [Required]
    public string Role{get;set;} = null!;
}

public class VehiculoDTO
{
    public string PlacaVehiculo { get; set; } = null!;
    public string ColorVehiculo { get; set; } = null!;
    public string ModeloVehiculo { get; set; } = null!;
    public string FabricanteVehiculo { get; set; } = null!;
    public string TipoVehiculo { get; set; } = null!;
    public string NumeroSerieVehiculo { get; set; } = null!;

}
