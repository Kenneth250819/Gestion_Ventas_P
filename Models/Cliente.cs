namespace Gestion_Ventas_P.Models
{
    public class Cliente
    {
        // Propiedades que corresponden a las columnas de la tabla Clientes
        public int ClienteID { get; set; } 
        public string Nombre { get; set; } 
        public string Apellido { get; set; } 
        public string Apellido2 { get; set; } 
        public string Direccion { get; set; } 
        public string Provincia { get; set; } 
        public string Canton { get; set; } 
        public string Telefono { get; set; }
        public string Email { get; set; } 
        public DateTime? FechaNacimiento { get; set; } 
        public string Nacionalidad { get; set; } 
        public DateTime FechaAdicion { get; set; } 
        public string AdicionadoPor { get; set; } 
        public DateTime? FechaModificacion { get; set; } 
        public string ModificadoPor { get; set; }                         

    }
}
