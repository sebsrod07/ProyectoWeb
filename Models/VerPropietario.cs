using System;
using System.Collections.Generic;

namespace Models;

public partial class VerPropietario
{
    public string Curp { get; set; } = null!;

    public string NombreCompleto { get; set; } = null!;

    public string? Rfc { get; set; }

    public string Correo { get; set; } = null!;
}
