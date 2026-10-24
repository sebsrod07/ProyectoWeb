using System;
using System.Collections.Generic;

namespace Models;

public partial class VerVehiculo
{
    public string Descripcion { get; set; } = null!;

    public string Placas { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public string NumeroSerie { get; set; } = null!;

    public string Propietario { get; set; } = null!;

    public string Estatus { get; set; } = null!;
}
