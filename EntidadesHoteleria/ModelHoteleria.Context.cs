﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EntidadesHoteleria
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class hoteleria_erp_dbEntities : DbContext
    {
        public hoteleria_erp_dbEntities()
            : base("name=hoteleria_erp_dbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<actividades_economicas> actividades_economicas { get; set; }
        public virtual DbSet<areas> areas { get; set; }
        public virtual DbSet<bancos> bancos { get; set; }
        public virtual DbSet<bancos_cuentas> bancos_cuentas { get; set; }
        public virtual DbSet<bancos_tipos_cuentas> bancos_tipos_cuentas { get; set; }
        public virtual DbSet<barrios> barrios { get; set; }
        public virtual DbSet<cargos> cargos { get; set; }
        public virtual DbSet<categorias> categorias { get; set; }
        public virtual DbSet<ciudades> ciudades { get; set; }
        public virtual DbSet<departamentos> departamentos { get; set; }
        public virtual DbSet<funcionarios> funcionarios { get; set; }
        public virtual DbSet<modulos> modulos { get; set; }
        public virtual DbSet<modulos_operaciones> modulos_operaciones { get; set; }
        public virtual DbSet<monedas> monedas { get; set; }
        public virtual DbSet<paises> paises { get; set; }
        public virtual DbSet<parametros> parametros { get; set; }
        public virtual DbSet<perfiles> perfiles { get; set; }
        public virtual DbSet<permisos> permisos { get; set; }
        public virtual DbSet<profesiones> profesiones { get; set; }
        public virtual DbSet<sucursales> sucursales { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<tipos_documentos> tipos_documentos { get; set; }
        public virtual DbSet<usuarios> usuarios { get; set; }
        public virtual DbSet<habitaciones> habitaciones { get; set; }
        public virtual DbSet<habitaciones_estados> habitaciones_estados { get; set; }
        public virtual DbSet<habitaciones_tipos> habitaciones_tipos { get; set; }
        public virtual DbSet<proveedores> proveedores { get; set; }
        public virtual DbSet<proveedores_contactos> proveedores_contactos { get; set; }
        public virtual DbSet<tipos_contactos> tipos_contactos { get; set; }
    }
}
