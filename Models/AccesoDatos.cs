using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Gestion_Ventas_P.Models
{
    public class AccesoDatos
    {
        private readonly string _conexion;

        public AccesoDatos(IConfiguration configuracion)
        {

            _conexion = configuracion.GetConnectionString("Conexion");
            if (string.IsNullOrEmpty(_conexion))
            {
                throw new Exception("Error: La cadena de conexión no se cargó correctamente.");
            }
        }

        // Método para validar al usuario
        public Usuario ValidarUsuario(string nombreUsuario, string contrasenia)
        {
            using (SqlConnection cn = new SqlConnection(_conexion))
            {
                SqlCommand cmd = new SqlCommand("sp_ValidarUsuario", cn);
                cmd.Parameters.AddWithValue("NombreUsuario", nombreUsuario);
                cmd.Parameters.AddWithValue("Contrasenia", contrasenia); // Contraseña sin cifrar si no estás usando SHA256
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    // Acceder a las columnas de la consulta
                    return new Usuario
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        NombreUsuario = reader["NombreUsuario"].ToString(), // Ahora accedes a 'NombreUsuario'
                        Contrasenia = reader["Contrasenia"].ToString(),
                        Rol = reader["Rol"].ToString()
                    };
                }
                else
                {
                    return null; // Si no se encuentra ningún registro
                }
            }
        }

        public void InsertarCliente(string nombre, string apellido, string apellido2, string direccion,
                                 string provincia, string canton, string telefono, string email,
                                 DateTime? fechaNacimiento, string nacionalidad, string adicionadoPor, out string mensaje)
        {
            using (SqlConnection cn = new SqlConnection(_conexion))
            {
                SqlCommand cmd = new SqlCommand("sp_Insertar_Cliente", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Agregar los parámetros
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@Apellido", apellido);
                cmd.Parameters.AddWithValue("@Apellido2", apellido2 ?? (object)DBNull.Value); // Usar DBNull si el valor es null
                cmd.Parameters.AddWithValue("@Direccion", direccion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Provincia", provincia ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Canton", canton ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Telefono", telefono ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaNacimiento", fechaNacimiento ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Nacionalidad", nacionalidad ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@adicionado_por", adicionadoPor);

                // Parámetro de salida
                SqlParameter parametroMensaje = new SqlParameter("@mensaje", SqlDbType.VarChar, 255)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(parametroMensaje);

                // Ejecutar el comando
                cn.Open();
                cmd.ExecuteNonQuery();

                // Obtener el mensaje de salida
                mensaje = parametroMensaje.Value.ToString();
            }
        }

        public List<Cliente> ObtenerTodosLosClientes()
        {
            List<Cliente> clientes = new List<Cliente>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_Mostrar_Clientes", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    clientes.Add(new Cliente
                    {
                        ClienteID = Convert.ToInt32(reader["ClienteID"]),
                        Nombre = reader["Nombre"].ToString(),
                        Apellido = reader["Apellido"].ToString(),
                        Apellido2 = reader["Apellido2"] != DBNull.Value ? reader["Apellido2"].ToString() : null,
                        Direccion = reader["Direccion"] != DBNull.Value ? reader["Direccion"].ToString() : null,
                        Provincia = reader["Provincia"] != DBNull.Value ? reader["Provincia"].ToString() : null,
                        Canton = reader["Canton"] != DBNull.Value ? reader["Canton"].ToString() : null,
                        Telefono = reader["Telefono"] != DBNull.Value ? reader["Telefono"].ToString() : null,
                        Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : null,
                        FechaNacimiento = reader["FechaNacimiento"] != DBNull.Value ? (DateTime?)reader["FechaNacimiento"] : null,
                        Nacionalidad = reader["Nacionalidad"] != DBNull.Value ? reader["Nacionalidad"].ToString() : null,
                        AdicionadoPor = reader["adicionado_por"].ToString() 
                    });
                }
            }

            return clientes;
        }

        public Cliente ObtenerClientePorID(int clienteID)
        {
            Cliente cliente = null;
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_Obtener_Cliente_Por_ID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ClienteID", clienteID);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    cliente = new Cliente
                    {
                        ClienteID = Convert.ToInt32(reader["ClienteID"]),
                        Nombre = reader["Nombre"].ToString(),
                        Apellido = reader["Apellido"].ToString(),
                        Apellido2 = reader["Apellido2"].ToString(),
                        Direccion = reader["Direccion"].ToString(),
                        Provincia = reader["Provincia"].ToString(),
                        Canton = reader["Canton"].ToString(),
                        Telefono = reader["Telefono"].ToString(),
                        Email = reader["Email"].ToString(),
                        FechaNacimiento = reader["FechaNacimiento"] != DBNull.Value ? (DateTime?)reader["FechaNacimiento"] : null,
                        Nacionalidad = reader["Nacionalidad"].ToString()
                    };
                }
            }
            return cliente;
        }

        public void ActualizarCliente(Cliente cliente)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_Actualizar_Cliente", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ClienteID", cliente.ClienteID);
                cmd.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", cliente.Apellido);
                cmd.Parameters.AddWithValue("@Apellido2", cliente.Apellido2);
                cmd.Parameters.AddWithValue("@Direccion", cliente.Direccion);
                cmd.Parameters.AddWithValue("@Provincia", cliente.Provincia);
                cmd.Parameters.AddWithValue("@Canton", cliente.Canton);
                cmd.Parameters.AddWithValue("@Telefono", cliente.Telefono);
                cmd.Parameters.AddWithValue("@Email", cliente.Email);
                cmd.Parameters.AddWithValue("@FechaNacimiento", (object)cliente.FechaNacimiento ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Nacionalidad", cliente.Nacionalidad);
                cmd.Parameters.AddWithValue("@modificado_por", "Admin"); 

                cmd.ExecuteNonQuery();
            }
        }


        public void EliminarCliente(int ClienteID)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "EXEC sp_Eliminar_Cliente @ClienteID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ClienteID", ClienteID);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al eliminar Cliente: " + ex.Message);
                }
            }
        }





    }
}
