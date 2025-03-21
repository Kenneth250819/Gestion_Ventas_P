namespace Gestion_Ventas_P.Models
{
    public class DetalleVenta
    {
        public int DetalleVentaID { get; set; }
        public int VentaID { get; set; }
        public string ClienteVenta { get; set; }
        public int ProductoID { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public DateTime FechaAdicion { get; set; } 
        public string AdicionadoPor { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string ModificadoPor { get; set; }



    }
}
