using System;
using System.Collections.Generic;

namespace Models;

public partial class Ingreso
{
    /// <summary>
    /// Fecha
    /// </summary>
    public DateOnly FechaSalida { get; set; }

    /// <summary>
    /// Mail del usuario que salio con el vehiculo
    /// </summary>
    public string IdUsuario { get; set; } = null!;

    /// <summary>
    /// Placas del vehiculo de salida
    /// </summary>
    public string PlacasVehiculo { get; set; } = null!;

    /// <summary>
    /// hora de salida del vehiculo
    /// </summary>
    public TimeOnly HoraSalida { get; set; }

    public long IdSalida { get; set; }
}
