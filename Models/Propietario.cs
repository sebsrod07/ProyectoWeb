using System;
using System.Collections.Generic;

namespace Models;

public partial class Propietario
{
    public string CurpPropietario { get; set; } = null!;

    public string PnombrePropietario { get; set; } = null!;

    public string? SnombrePropietario { get; set; }

    public string PapellidoPropietario { get; set; } = null!;

    public string MapellidoPropietario { get; set; } = null!;

    public string? RfcPropietario { get; set; }

    public string CorreoPropietario { get; set; } = null!;

    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}
