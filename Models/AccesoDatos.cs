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

        public void AgregarTipoDePan(TipoDePan TipoNuevo)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_Insertar_TipoPan  @Nombre, @Descripcion,  @adicionado_por";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {   
                        cmd.Parameters.AddWithValue("@Nombre", TipoNuevo.Nombre);
                        cmd.Parameters.AddWithValue("@Descripcion", TipoNuevo.Descripcion);
                        cmd.Parameters.AddWithValue("@adicionado_por", TipoNuevo.AdicionadoPor);
                      

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al registrar el Tipo de pan: " + ex.Message);
                }
            }
        }

        public List<TipoDePan> MostrarTipoDePans()
        {
            List<TipoDePan> listaTipoDePan = new List<TipoDePan>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_Mostrar_TiposPan";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TipoDePan pan = new TipoDePan
                                {
                                    TipoPanID = Convert.ToInt32(reader["TipoPanID"]),                                 
                                    Nombre = reader["Nombre"].ToString(),
                                    Descripcion = reader["Descripcion"].ToString(),
                                    FechaAdicion = Convert.ToDateTime(reader["fecha_adicion"]),
                                    AdicionadoPor = reader["adicionado_por"].ToString(),
                                   
                                };
                                listaTipoDePan.Add(pan);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener la lista de Panes: " + ex.Message);
                }
            }

            return listaTipoDePan;
        }


        public TipoDePan ObtenerTipoDePanPorID(int TipoPanID)
        {
            TipoDePan TipoPan = null;
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_Mostrar_TipoPan_Por_Id", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TipoPanID", TipoPanID);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    TipoPan = new TipoDePan
                    {
                        TipoPanID = Convert.ToInt32(reader["TipoPanID"]),
                        Nombre = reader["Nombre"].ToString(),
                        Descripcion = reader["Descripcion"].ToString(),
                        ModificadoPor = reader["modificado_por"].ToString()
                    };
                }
            }
            return TipoPan;
        }



        public void ActualizarTipoDePan(TipoDePan PanActualizado)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_Actualizar_TipoPan", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@TipoPanID", PanActualizado.TipoPanID);
                cmd.Parameters.AddWithValue("@Nombre", PanActualizado.Nombre);
                cmd.Parameters.AddWithValue("@Descripcion", PanActualizado.Descripcion);           
                cmd.Parameters.AddWithValue("@modificado_por", "Admin");

                cmd.ExecuteNonQuery();
            }
        }

        public void EliminarTipoDePan(int TipoPanID)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "EXEC sp_Eliminar_TipoPan @TipoPanID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@TipoPanID", TipoPanID);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al eliminar Tipo de pan: " + ex.Message);
                }
            }
        }

        public void AgregarCategoria(Categoria CategoriaNueva)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_Insertar_Categoria  @Nombre, @Descripcion,  @adicionado_por";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", CategoriaNueva.Nombre);
                        cmd.Parameters.AddWithValue("@Descripcion", CategoriaNueva.Descripcion);
                        cmd.Parameters.AddWithValue("@adicionado_por", CategoriaNueva.AdicionadoPor);


                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al registrar la categoria: " + ex.Message);
                }
            }
        }

        public List<Categoria> MostrarCategoria()
        {
            List<Categoria> listaCategoria = new List<Categoria>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_Mostrar_Categoria";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Categoria cat = new Categoria
                                {
                                    CategoriaID = Convert.ToInt32(reader["CategoriaID"]),
                                    Nombre = reader["Nombre"].ToString(),
                                    Descripcion = reader["Descripcion"].ToString(),
                                    FechaAdicion = Convert.ToDateTime(reader["fecha_adicion"]),
                                    AdicionadoPor = reader["adicionado_por"].ToString(),

                                };
                                listaCategoria.Add(cat);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener la lista de Categorias: " + ex.Message);
                }
            }

            return listaCategoria;
        }

        public Categoria ObtenerCategoriaPorID(int CategoriaID)
        {
            Categoria categoria = null;
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_Mostrar_Categoria_Por_Id", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CategoriaID", CategoriaID);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    categoria = new Categoria
                    {
                        CategoriaID = Convert.ToInt32(reader["CategoriaID"]),
                        Nombre = reader["Nombre"].ToString(),
                        Descripcion = reader["Descripcion"].ToString(),
                        ModificadoPor = reader["modificado_por"].ToString()
                    };
                }
            }
            return categoria;
        }


        public void ActualizarCategoria(Categoria CategoriaActualizado)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_Actualizar_Categoria", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@CategoriaID", CategoriaActualizado.CategoriaID);
                cmd.Parameters.AddWithValue("@Nombre", CategoriaActualizado.Nombre);
                cmd.Parameters.AddWithValue("@Descripcion", CategoriaActualizado.Descripcion);
                cmd.Parameters.AddWithValue("@modificado_por", "Admin");

                cmd.ExecuteNonQuery();
            }
        }

        public void EliminarCategoria(int CategoriaID)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "EXEC sp_Eliminar_Categoria @CategoriaID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@CategoriaID", CategoriaID);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al eliminar Categoria: " + ex.Message);
                }
            }
        }

        public void AgregarProductos(Producto ProductoNuevo)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_Insertar_Producto  @Nombre, @Descripcion,  @PrecioUnitario, @CategoriaID, @TipoPanID, @adicionado_por";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", ProductoNuevo.Nombre);
                        cmd.Parameters.AddWithValue("@Descripcion", ProductoNuevo.Descripcion);
                        cmd.Parameters.AddWithValue("@PrecioUnitario", ProductoNuevo.PrecioUnitario);
                        cmd.Parameters.AddWithValue("@CategoriaID", ProductoNuevo.CategoriaID);
                        cmd.Parameters.AddWithValue("@TipoPanID", ProductoNuevo.TipoPanID);
                        cmd.Parameters.AddWithValue("@adicionado_por", ProductoNuevo.AdicionadoPor);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al registrar el Producto: " + ex.Message);
                }
            }
        }


        public List<Categoria> ObtenerCategorias()
        {
            List<Categoria> lista = new List<Categoria>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_lista_Categorias", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new Categoria
                    {
                        CategoriaID = Convert.ToInt32(reader["CategoriaID"]),
                        Nombre = reader["Nombre"].ToString()
                    });
                }
            }
            return lista;
        }

        public List<TipoDePan> ObtenerTiposDePan()
        {
            List<TipoDePan> lista = new List<TipoDePan>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_lista_TiposDePan", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new TipoDePan
                    {
                        TipoPanID = Convert.ToInt32(reader["TipoPanID"]),
                        Nombre = reader["Nombre"].ToString()
                    });
                }
            }
            return lista;
        }


        public List<Producto> MostrarProducto()
        {
            List<Producto> listaProducto = new List<Producto>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_Mostrar_Productos";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Producto pro = new Producto
                                {
                                    ProductoID = Convert.ToInt32(reader["ProductoID"]),
                                    Nombre = reader["Nombre"].ToString(),
                                    Descripcion = reader["Descripcion"].ToString(),
                                    PrecioUnitario = Convert.ToDecimal(reader["PrecioUnitario"]),
                                    CategoriaNombre = reader["Categoria"].ToString(),
                                    TipoPanNombre = reader["TipoDePan"].ToString(),
                                    FechaAdicion = Convert.ToDateTime(reader["fecha_adicion"]),
                                    AdicionadoPor = reader["adicionado_por"].ToString(),


                                };
                                listaProducto.Add(pro);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener la lista de Categorias: " + ex.Message);
                }
            }

            return listaProducto;
        }



        public Producto ObtenerMostrarProductoPorID(int ProductoID)
        {
            Producto Producto = null;
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_Mostrar_Producto_Por_Id", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProductoID", ProductoID);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Producto = new Producto
                    {
                        ProductoID = Convert.ToInt32(reader["ProductoID"]),
                        Nombre = reader["Nombre"].ToString(),
                        Descripcion = reader["Descripcion"].ToString(),
                        PrecioUnitario = Convert.ToDecimal(reader["PrecioUnitario"]),
                        CategoriaNombre = reader["Categoria"]?.ToString(), // Leer el alias "Categoria"
                        TipoPanNombre = reader["TipoDePan"]?.ToString(),   // Leer el alias "TipoDePan"                                           
                        ModificadoPor = reader["modificado_por"]?.ToString()
                    };
                }
            }
            return Producto;
        }


        public void ActualizarProducto(Producto ProductoActualizado)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_Actualizar_Producto", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ProductoID", ProductoActualizado.ProductoID);
                cmd.Parameters.AddWithValue("@Nombre", ProductoActualizado.Nombre);
                cmd.Parameters.AddWithValue("@Descripcion", ProductoActualizado.Descripcion);
                cmd.Parameters.AddWithValue("@PrecioUnitario", ProductoActualizado.PrecioUnitario);
                cmd.Parameters.AddWithValue("@CategoriaID", ProductoActualizado.CategoriaID);
                cmd.Parameters.AddWithValue("@TipoPanID", ProductoActualizado.TipoPanID);
                cmd.Parameters.AddWithValue("@modificado_por", "Admin");

                cmd.ExecuteNonQuery();
            }
        }


        public void EliminarProducto(int ProductoID)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "EXEC sp_Eliminar_Producto @ProductoID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ProductoID", ProductoID);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al eliminar Producto: " + ex.Message);
                }
            }
        }



        public void AgregarVenta(Venta VentaNueva)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_Insertar_Venta  @ClienteID, @FechaVenta, @Total, @Estado, @adicionado_por";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ClienteID", VentaNueva.ClienteID);
                        cmd.Parameters.AddWithValue("@FechaVenta", VentaNueva.FechaVenta);
                        cmd.Parameters.AddWithValue("@Total", VentaNueva.Total);
                        cmd.Parameters.AddWithValue("@Estado", VentaNueva.Estado);
                        cmd.Parameters.AddWithValue("@adicionado_por", VentaNueva.AdicionadoPor);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al registrar la Venta: " + ex.Message);
                }
            }
        }

        public List<Cliente> ObtenerClientes()
        {
            List<Cliente> lista = new List<Cliente>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_lista_Clientes", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new Cliente
                    {
                        ClienteID = Convert.ToInt32(reader["ClienteID"]),
                        Nombre = reader["Nombre"].ToString()
                    });
                }
            }
            return lista;
        }

        public List<Venta> MostrarVenta()
        {
            List<Venta> listaVenta = new List<Venta>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_Mostrar_Venta";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Venta ven = new Venta
                                {
                                    VentaID = Convert.ToInt32(reader["VentaID"]),
                                    ClienteNombre = reader["Cliente"].ToString(),
                                    FechaVenta = Convert.ToDateTime(reader["FechaVenta"]),
                                    Total = Convert.ToInt32(reader["Total"]),
                                    Estado = reader["Estado"].ToString(),
                                    FechaAdicion = Convert.ToDateTime(reader["fecha_adicion"]),
                                    AdicionadoPor = reader["adicionado_por"].ToString(),


                                };
                                listaVenta.Add(ven);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener la lista de Ventas: " + ex.Message);
                }
            }

            return listaVenta;
        }

        public Venta ObtenerVentaPorID(int VentaID)
        {
            Venta venta = null;
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_Mostrar_Venta_Por_Id", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@VentaID", VentaID);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    venta = new Venta
                    {
                        VentaID = Convert.ToInt32(reader["VentaID"]),                    
                        ClienteNombre = reader["Cliente"].ToString(), // Si el SP devuelve el nombre del cliente
                        FechaVenta = Convert.ToDateTime(reader["FechaVenta"]),
                        Total = Convert.ToDecimal(reader["Total"]),
                        Estado = reader["Estado"].ToString(),
                        FechaAdicion = Convert.ToDateTime(reader["fecha_adicion"]),
                        AdicionadoPor = reader["adicionado_por"].ToString(),
                        FechaModificacion = reader["fecha_modificacion"] != DBNull.Value ? Convert.ToDateTime(reader["fecha_modificacion"]) : (DateTime?)null,
                        ModificadoPor = reader["modificado_por"]?.ToString()
                    };
                }
            }
            return venta;
        }


        public void ActualizarVenta(Venta VentaActualizada)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_Actualizar_Venta", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@VentaID", VentaActualizada.VentaID);
                cmd.Parameters.AddWithValue("@ClienteID", VentaActualizada.ClienteID);
                cmd.Parameters.AddWithValue("@FechaVenta", VentaActualizada.FechaVenta);
                cmd.Parameters.AddWithValue("@Total", VentaActualizada.Total);
                cmd.Parameters.AddWithValue("@Estado", VentaActualizada.Estado);
                cmd.Parameters.AddWithValue("@modificado_por", "Admin"); // Debes asignar el usuario real que modifica la venta

                cmd.ExecuteNonQuery();
            }
        }


        public void EliminarVenta(int VentaID)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "EXEC sp_Eliminar_Venta @VentaID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@VentaID", VentaID);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al eliminar Producto: " + ex.Message);
                }
            }
        }












    }
}
