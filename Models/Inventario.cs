using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Gestion_Ventas_P.Models
{
    public class Inventario
    {
        
        public int InventarioID { get; set; }

        
        public int ProductoID { get; set; }
        public string ProductoNombre { get; set; }

        public int Cantidad { get; set; }

        public DateTime FechaActualizacion { get; set; }

        public DateTime FechaAdicion { get; set; } = DateTime.Now;

        
        public string AdicionadoPor { get; set; }

        
        public DateTime? FechaModificacion { get; set; }

        
        public string? ModificadoPor { get; set; }

        
    }
}
