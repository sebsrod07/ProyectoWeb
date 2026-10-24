using System;
using System.Collections.Generic;

namespace Models;

public partial class VerVehiculosPropietario
{
    public string Descripcion { get; set; } = null!;

    public string Placas { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public string NumeroSerie { get; set; } = null!;

    public string Curp { get; set; } = null!;

    public string NombreCompleto { get; set; } = null!;

    public string? Rfc { get; set; }

    public string Correo { get; set; } = null!;

    public string Estatus { get; set; } = null!;
}
