using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Models;
namespace Contextos;

public partial class DbProyectoWebContext : DbContext
{
    public DbProyectoWebContext()
    {
    }

    public DbProyectoWebContext(DbContextOptions<DbProyectoWebContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Ingreso> Ingresos { get; set; }

    public virtual DbSet<Propietario> Propietarios { get; set; }

    public virtual DbSet<Vehiculo> Vehiculos { get; set; }

    public virtual DbSet<VerIngreso> VerIngresos { get; set; }

    public virtual DbSet<VerPropietario> VerPropietarios { get; set; }

    public virtual DbSet<VerVehiculo> VerVehiculos { get; set; }

    public virtual DbSet<VerVehiculosPropietario> VerVehiculosPropietarios { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ingreso>(entity =>
        {
            entity.HasKey(e => e.IdSalida);

            entity.Property(e => e.IdSalida).HasColumnName("idSalida");
            entity.Property(e => e.FechaSalida)
                .HasComment("Fecha")
                .HasColumnType("datetime")
                .HasColumnName("fechaSalida");
            entity.Property(e => e.HoraSalida)
                .HasComment("hora de salida del vehiculo")
                .HasDefaultValueSql("(CONVERT([time],getdate()))", "DEFAULT_Ingresos_horaSalida")
                .HasColumnName("horaSalida");
            entity.Property(e => e.IdUsuario)
                .HasMaxLength(100)
                .HasComment("Mail del usuario que salio con el vehiculo")
                .HasColumnName("idUsuario");
            entity.Property(e => e.PlacasVehiculo)
                .HasComment("Placas del vehiculo de salida")
                .HasColumnName("placasVehiculo");
        });

        modelBuilder.Entity<Propietario>(entity =>
        {
            entity.HasKey(e => e.CurpPropietario);

            entity.Property(e => e.CurpPropietario)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CURP_Propietario");
            entity.Property(e => e.CorreoPropietario)
                .HasMaxLength(50)
                .HasColumnName("Correo_Propietario");
            entity.Property(e => e.MapellidoPropietario)
                .HasMaxLength(50)
                .HasColumnName("MApellido:Propietario");
            entity.Property(e => e.PapellidoPropietario)
                .HasMaxLength(50)
                .HasColumnName("PApellido_Propietario");
            entity.Property(e => e.PnombrePropietario)
                .HasMaxLength(50)
                .HasColumnName("PNombre_Propietario");
            entity.Property(e => e.RfcPropietario)
                .HasMaxLength(50)
                .HasColumnName("RFC_Propietario");
            entity.Property(e => e.SnombrePropietario)
                .HasMaxLength(50)
                .HasColumnName("SNombre_Propietario");
        });

        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.HasKey(e => e.PlacaVehiculo);

            entity.HasIndex(e => e.NumeroSerieVehiculo, "VehiculoUnique")
                .IsUnique()
                .IsDescending();

            entity.Property(e => e.PlacaVehiculo)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("Placa_Vehiculo");
            entity.Property(e => e.ColorVehiculo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Color_Vehiculo");
            entity.Property(e => e.CurppropietarioVehiculo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CURPPropietario_Vehiculo");
            entity.Property(e => e.FabricanteVehiculo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Fabricante_Vehiculo");
            entity.Property(e => e.ModeloVehiculo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Modelo_Vehiculo");
            entity.Property(e => e.NumeroSerieVehiculo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NumeroSerie_Vehiculo");
            entity.Property(e => e.TipoVehiculo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Tipo_Vehiculo");

            entity.HasOne(d => d.CurppropietarioVehiculoNavigation).WithMany(p => p.Vehiculos)
                .HasForeignKey(d => d.CurppropietarioVehiculo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehiculos_A_CURPPropietarios");
        });

        modelBuilder.Entity<VerIngreso>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("verIngresos");

            entity.Property(e => e.Curp)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CURP");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(122)
                .IsUnicode(false);
            entity.Property(e => e.FechaSalida)
                .HasColumnType("datetime")
                .HasColumnName("fechaSalida");
            entity.Property(e => e.HoraSalida).HasColumnName("horaSalida");
            entity.Property(e => e.NombreCompleto).HasMaxLength(203);
            entity.Property(e => e.NumeroSerie)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Placas)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Tipo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UserName).HasMaxLength(256);
        });

        modelBuilder.Entity<VerPropietario>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("verPropietarios");

            entity.Property(e => e.Correo).HasMaxLength(50);
            entity.Property(e => e.Curp)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CURP");
            entity.Property(e => e.NombreCompleto).HasMaxLength(203);
            entity.Property(e => e.Rfc)
                .HasMaxLength(50)
                .HasColumnName("RFC");
        });

        modelBuilder.Entity<VerVehiculo>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("verVehiculos");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(122)
                .IsUnicode(false);
            entity.Property(e => e.NumeroSerie)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Placas)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Propietario)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Tipo)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VerVehiculosPropietario>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("verVehiculos_Propietarios");

            entity.Property(e => e.Correo).HasMaxLength(50);
            entity.Property(e => e.Curp)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CURP");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(122)
                .IsUnicode(false);
            entity.Property(e => e.NombreCompleto).HasMaxLength(203);
            entity.Property(e => e.NumeroSerie)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Placas)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Rfc)
                .HasMaxLength(50)
                .HasColumnName("RFC");
            entity.Property(e => e.Tipo)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
