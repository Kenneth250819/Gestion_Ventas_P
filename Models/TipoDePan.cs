namespace Gestion_Ventas_P.Models
{
    public class TipoDePan
    {

        public int TipoPanID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaAdicion { get; set; }
        public string AdicionadoPor { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string ModificadoPor { get; set; }


    }
}
