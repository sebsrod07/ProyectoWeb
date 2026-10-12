using System;
using System.Collections.Generic;

namespace Models;

public partial class VerIngreso
{
    public string? UserName { get; set; }

    public string NombreCompleto { get; set; } = null!;

    public string Curp { get; set; } = null!;

    public string Placas { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public string NumeroSerie { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public DateTime FechaSalida { get; set; }

    public TimeOnly HoraSalida { get; set; }
}
