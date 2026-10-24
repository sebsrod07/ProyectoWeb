using System;
using System.Collections.Generic;

namespace Models;

public partial class Vehiculo
{
    public string PlacaVehiculo { get; set; } = null!;

    public string ColorVehiculo { get; set; } = null!;

    public string ModeloVehiculo { get; set; } = null!;

    public string FabricanteVehiculo { get; set; } = null!;

    public string CurppropietarioVehiculo { get; set; } = null!;

    public string TipoVehiculo { get; set; } = null!;

    public string NumeroSerieVehiculo { get; set; } = null!;

    public string EstatusVehiculo { get; set; }

    public virtual Propietario CurppropietarioVehiculoNavigation { get; set; } = null!;
}
