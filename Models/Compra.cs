namespace Gestion_Ventas_P.Models
{
    public class Compra
    {
        public int CompraID { get; set; }
        public int ProveedorID { get; set; }
        public string NombreProveedor { get; set; }
        public DateTime FechaCompra { get; set; }
        public decimal Total { get; set; }
        public DateTime FechaAdicion { get; set; } = DateTime.Now;
        public string AdicionadoPor { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string? ModificadoPor { get; set; }

    }
}
