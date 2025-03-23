namespace Gestion_Ventas_P.Models
{
    public class DetalleCompra
    {
        public int DetalleCompraID { get; set; }  // ID de la fila, clave primaria
        public int CompraID { get; set; }         // ID de la compra, clave foránea
        public int ProductoID { get; set; }       // ID del producto, clave foránea
        public string NombreProducto { get; set; } 
        public int Cantidad { get; set; }         // Cantidad de productos comprados
        public decimal PrecioUnitario { get; set; } // Precio unitario del producto al momento de la compra
        public DateTime FechaAdicion { get; set; } // Fecha de adición, con valor por defecto GETDATE() en la base de datos
        public string AdicionadoPor { get; set; }  // Quién agregó el registro
        public DateTime? FechaModificacion { get; set; } // Fecha de modificación (puede ser nula)
        public string ModificadoPor { get; set; } // Quién modificó el registro (puede ser nulo)
    }
}
