namespace Gestion_Ventas_P.Models
{
    public class PagosProveedor
    {
        public int PagoID { get; set; }

        public int CompraID { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public int MetodoPagoID { get; set; }
        public string MetodoPagoNombre { get; set; }
        public DateTime FechaAdicion { get; set; } // Fecha de adición, con valor por defecto GETDATE() en la base de datos
        public string AdicionadoPor { get; set; }  // Quién agregó el registro
        public DateTime? FechaModificacion { get; set; } // Fecha de modificación (puede ser nula)
        public string ModificadoPor { get; set; } // Quién modificó el registro (puede ser nulo)

    }
}
