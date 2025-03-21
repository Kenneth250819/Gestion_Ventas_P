namespace Gestion_Ventas_P.Models
{
    public class PagoCliente
    {
        public int PagoClienteID { get; set; }  // ID del pago
        public int VentaID { get; set; }  // ID de la venta realizada
        public string Cliente { get; set; }
        public decimal Monto { get; set; }  // Monto pagado por el cliente
        public DateTime FechaPago { get; set; }  // Fecha y hora del pago
        public int MetodoPagoID { get; set; }  // ID del método de pago utilizado
        public string MetodoPagoNombre { get; set; } //Se usa para mostrar el nombre de metodo de pago y no solo la ID
        public DateTime FechaAdicion { get; set; }  // Fecha de adición del registro
        public string AdicionadoPor { get; set; }  // Usuario que agregó el pago
        public DateTime? FechaModificacion { get; set; }  // Fecha de modificación del registro (nullable)
        public string ModificadoPor { get; set; }  // Usuario que modificó el pago (nullable)

        

    }
}
