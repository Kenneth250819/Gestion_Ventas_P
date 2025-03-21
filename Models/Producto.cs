namespace Gestion_Ventas_P.Models
{
    public class Producto
    {
        public int ProductoID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int CategoriaID { get; set; }
        public string CategoriaNombre { get; set; }
        public int TipoPanID { get; set; }
        public string TipoPanNombre { get; set; }
        public DateTime FechaAdicion { get; set; } = DateTime.Now;
        public string AdicionadoPor { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string ModificadoPor { get; set; }

     
    }

}
