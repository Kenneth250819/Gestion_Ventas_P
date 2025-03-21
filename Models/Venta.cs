namespace Gestion_Ventas_P.Models
{
    public class Venta
    {
        public int VentaID { get; set; } // Clave primaria (autoincremental)
        public int ClienteID { get; set; } // Clave foránea que referencia a Clientes
        public string ClienteNombre { get; set; }
        public DateTime FechaVenta { get; set; } // Fecha de la venta
        public decimal Total { get; set; } // Total de la venta
        public string Estado { get; set; } // Estado de la venta (ej: "Pendiente", "Completada")
        public DateTime FechaAdicion { get; set; } // Fecha de creación del registro
        public string AdicionadoPor { get; set; } // Usuario que creó el registro
        public DateTime? FechaModificacion { get; set; } // Fecha de modificación (nullable)
        public string ModificadoPor { get; set; } // Usuario que modificó el registro (nullable)

    }
}
