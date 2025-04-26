 using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Gestion_Ventas_P.Models
{
    public class AccesoDatos
    {
        private readonly string _conexion;

        public AccesoDatos(IConfiguration configuracion)
        {

            _conexion = configuracion["ConnectionStrings:Conexion"];
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


        public void AgregarMetodoPago(MetodoPago MetodoPagoNuevo)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_Insertar_MetodoPago  @Nombre";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", MetodoPagoNuevo.Nombre);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al registrar el Metodo de Pago: " + ex.Message);
                }
            }
        }


        public List<MetodoPago> MostrarMetodoPago()
        {
            List<MetodoPago> listaMetodoPago = new List<MetodoPago>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_Mostrar_MetodoPago";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                MetodoPago men = new MetodoPago
                                {
                                    MetodoPagoID = Convert.ToInt32(reader["MetodoPagoID"]),
                                    Nombre = reader["Nombre"].ToString(),

                                };
                                listaMetodoPago.Add(men);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener la lista de Metodo de Pago: " + ex.Message);
                }
            }

            return listaMetodoPago;
        }


        public MetodoPago ObtenerMetodoPagoPorID(int MetodoPagoID)
        {
            MetodoPago MetodoPago = null;
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_Mostrar_MetodoPago_Por_Id", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MetodoPagoID", MetodoPagoID);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    MetodoPago = new MetodoPago
                    {
                        MetodoPagoID = Convert.ToInt32(reader["MetodoPagoID"]),
                        Nombre = reader["Nombre"].ToString(), // Si el SP devuelve el nombre del cliente

                    };
                }
            }
            return MetodoPago;
        }

        public void ActualizarMetodoPago(MetodoPago MetodoPagoActualizado)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_Actualizar_MetodoPago", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@MetodoPagoID", MetodoPagoActualizado.MetodoPagoID);
                cmd.Parameters.AddWithValue("@Nombre", MetodoPagoActualizado.Nombre);

                cmd.ExecuteNonQuery();
            }
        }


        public void EliminarMetodoPago(int MetodoPagoID)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "EXEC sp_Eliminar_MetodoPago @MetodoPagoID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@MetodoPagoID", MetodoPagoID);
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

        public void AgregarPagoCliente(PagoCliente PagoClienteNuevo)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_Insertar_PagoCliente  @VentaID, @Monto, @FechaPago, @MetodoPagoID, @adicionado_por";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@VentaID", PagoClienteNuevo.VentaID);
                        cmd.Parameters.AddWithValue("@Monto", PagoClienteNuevo.Monto);
                        cmd.Parameters.AddWithValue("@FechaPago", PagoClienteNuevo.FechaPago);
                        cmd.Parameters.AddWithValue("@MetodoPagoID", PagoClienteNuevo.MetodoPagoID);
                        cmd.Parameters.AddWithValue("@adicionado_por", PagoClienteNuevo.AdicionadoPor);

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

        public List<PagoCliente> VerPagoCliente()
        {
            List<PagoCliente> listaPagoCliente = new List<PagoCliente>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_Mostrar_PagoCliente";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PagoCliente pag = new PagoCliente
                                {
                                    PagoClienteID = Convert.ToInt32(reader["PagoClienteID"]),
                                    VentaID = Convert.ToInt32(reader["VentaID"]),
                                    Cliente = reader["Cliente"].ToString(),
                                    Monto = Convert.ToDecimal(reader["Monto"]),
                                    FechaPago = Convert.ToDateTime(reader["FechaPago"]),
                                    MetodoPagoNombre = reader["MetodoPago"].ToString(),
                                    FechaAdicion = Convert.ToDateTime(reader["fecha_adicion"]),
                                    AdicionadoPor = reader["adicionado_por"].ToString(),
                                    FechaModificacion = reader["fecha_modificacion"] != DBNull.Value ? Convert.ToDateTime(reader["fecha_modificacion"]) : (DateTime?)null,
                                    ModificadoPor = reader["modificado_por"]?.ToString()

                                };
                                listaPagoCliente.Add(pag);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener la lista de Metodo de Pago: " + ex.Message);
                }
            }

            return listaPagoCliente;
        }



        public PagoCliente ObtenerPagoClientePorId(int PagoClienteID)
        {
            PagoCliente pagoCliente = null;

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "sp_Mostrar_PagoCliente_Por_Id"; // Procedimiento almacenado

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PagoClienteID", PagoClienteID);

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                pagoCliente = new PagoCliente
                                {
                                    PagoClienteID = Convert.ToInt32(reader["PagoClienteID"]),
                                    VentaID = Convert.ToInt32(reader["VentaID"]),
                                    Cliente = reader["Cliente"].ToString(),
                                    Monto = Convert.ToDecimal(reader["Monto"]),
                                    FechaPago = Convert.ToDateTime(reader["FechaPago"]),
                                    MetodoPagoNombre = reader["MetodoPago"].ToString(),
                                    FechaAdicion = Convert.ToDateTime(reader["fecha_adicion"]),
                                    AdicionadoPor = reader["adicionado_por"].ToString(),
                                    FechaModificacion = reader["fecha_modificacion"] != DBNull.Value ? Convert.ToDateTime(reader["fecha_modificacion"]) : (DateTime?)null,
                                    ModificadoPor = reader["modificado_por"].ToString()
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener el pago del cliente: " + ex.Message);
                }
            }

            return pagoCliente;
        }


        public void ActualizarPagoCliente(PagoCliente PagoClienteActualizado)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_Actualizar_PagoCliente", con);
                cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@PagoClienteID", PagoClienteActualizado.PagoClienteID);
                        cmd.Parameters.AddWithValue("@VentaID", PagoClienteActualizado.VentaID);
                        cmd.Parameters.AddWithValue("@Monto", PagoClienteActualizado.Monto);
                        cmd.Parameters.AddWithValue("@FechaPago", PagoClienteActualizado.FechaPago);
                        cmd.Parameters.AddWithValue("@MetodoPagoID", PagoClienteActualizado.MetodoPagoID);
                        cmd.Parameters.AddWithValue("@modificado_por", "Admin");

                cmd.ExecuteNonQuery();
            }
        }

        public void EliminarPagoCliente(int PagoClienteID)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "EXEC sp_Eliminar_PagoCliente @PagoClienteID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@PagoClienteID", PagoClienteID);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al eliminar Pago de Cliente: " + ex.Message);
                }
            }
        }


        public void AgregarDetalleVenta(DetalleVenta DetalleVentaNueva)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_Insertar_DetalleVenta  @VentaID, @ProductoID, @Cantidad, @PrecioUnitario, @adicionado_por";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@VentaID", DetalleVentaNueva.VentaID);
                        cmd.Parameters.AddWithValue("@ProductoID", DetalleVentaNueva.ProductoID);
                        cmd.Parameters.AddWithValue("@Cantidad", DetalleVentaNueva.Cantidad);
                        cmd.Parameters.AddWithValue("@PrecioUnitario", DetalleVentaNueva.PrecioUnitario);
                        cmd.Parameters.AddWithValue("@adicionado_por", DetalleVentaNueva.AdicionadoPor);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al registrar el detalle de Venta: " + ex.Message);
                }
            }
        }


        public List<DetalleVenta> MostrarDetalleVenta()
        {
            List<DetalleVenta> listaDetalleVenta = new List<DetalleVenta>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_Mostrar_DetallesVenta";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DetalleVenta det = new DetalleVenta
                                {
                                    DetalleVentaID = Convert.ToInt32(reader["DetalleVentaID"]),
                                    VentaID = Convert.ToInt32(reader["VentaID"]),  // Ahora es solo el ID de la venta
                                    ClienteVenta = reader["Cliente"].ToString(),// 
                                    ProductoID = Convert.ToInt32(reader["ProductoID"]),  // ID del producto
                                    NombreProducto = reader["Producto"].ToString(),  // Nombre del producto
                                    Cantidad = Convert.ToInt32(reader["Cantidad"]),
                                    PrecioUnitario = Convert.ToDecimal(reader["PrecioUnitario"]),
                                    AdicionadoPor = reader["adicionado_por"].ToString(),
                                    FechaAdicion = Convert.ToDateTime(reader["fecha_adicion"])

                                };
                                listaDetalleVenta.Add(det);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener la lista de Metodo de Pago: " + ex.Message);
                }
            }

            return listaDetalleVenta;
        }

        public DetalleVenta ObtenerDetalleVentaPorId(int DetalleVentaID)
        {
            DetalleVenta DetalleVenta = null;

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "sp_Mostrar_DetalleVenta_Por_Id"; // Procedimiento almacenado

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@DetalleVentaID", DetalleVentaID);

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                DetalleVenta = new DetalleVenta
                                {
                                    DetalleVentaID = Convert.ToInt32(reader["DetalleVentaID"]),
                                    VentaID = Convert.ToInt32(reader["VentaID"]),
                                    ClienteVenta = reader["Cliente"].ToString(),
                                    ProductoID = Convert.ToInt32(reader["ProductoID"]),
                                    NombreProducto = reader["Producto"].ToString(),
                                    Cantidad = Convert.ToInt32(reader["Cantidad"]),
                                    PrecioUnitario = Convert.ToDecimal(reader["PrecioUnitario"]),
                                    AdicionadoPor = reader["adicionado_por"].ToString(),
                                    FechaModificacion = reader["fecha_modificacion"] != DBNull.Value ? Convert.ToDateTime(reader["fecha_modificacion"]) : (DateTime?)null,
                                    ModificadoPor = reader["modificado_por"].ToString()
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener el Detalle de Venta del cliente: " + ex.Message);
                }
            }

            return DetalleVenta;
        }


        public void ActualizarDetalleVenta(DetalleVenta DetalleVentaActualizado)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_Actualizar_DetalleVenta", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@DetalleVentaID", DetalleVentaActualizado.DetalleVentaID);
                cmd.Parameters.AddWithValue("@VentaID", DetalleVentaActualizado.VentaID);
                cmd.Parameters.AddWithValue("@ProductoID", DetalleVentaActualizado.ProductoID);
                cmd.Parameters.AddWithValue("@Cantidad", DetalleVentaActualizado.Cantidad);
                cmd.Parameters.AddWithValue("@PrecioUnitario", DetalleVentaActualizado.PrecioUnitario);
                cmd.Parameters.AddWithValue("@modificado_por", "Admin");

                cmd.ExecuteNonQuery();
            }
        }


        public void EliminarDetalleVenta(int DetalleVentaID)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "EXEC sp_Eliminar_DetalleVenta @DetalleVentaID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@DetalleVentaID", DetalleVentaID);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al eliminar Detalle de Venta: " + ex.Message);
                }
            }
        }


        public void AgregarInventario(Inventario InventarioNuevo)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_Insertar_Inventario  @ProductoID, @Cantidad, @FechaActualizacion, @adicionado_por";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ProductoID", InventarioNuevo.ProductoID);
                        cmd.Parameters.AddWithValue("@Cantidad", InventarioNuevo.Cantidad);
                        cmd.Parameters.AddWithValue("@FechaActualizacion", InventarioNuevo.FechaActualizacion);
                        cmd.Parameters.AddWithValue("@adicionado_por", InventarioNuevo.AdicionadoPor);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al registrar en el Inventario: " + ex.Message);
                }
            }
        }

        public List<Inventario> MostrarInventario()
        {
            List<Inventario> listaInventario = new List<Inventario>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_Mostrar_Inventario"; // Asegúrate de que este sea el nombre correcto del procedimiento almacenado
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Inventario inv = new Inventario
                                {
                                    InventarioID = Convert.ToInt32(reader["InventarioID"]),
                                    ProductoID = Convert.ToInt32(reader["ProductoID"]),
                                    ProductoNombre = reader["Producto"].ToString(), // Nombre del producto desde la tabla Productos
                                    Cantidad = Convert.ToInt32(reader["Cantidad"]),
                                    FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"]),
                                    AdicionadoPor = reader["adicionado_por"].ToString(),
                                    FechaAdicion = Convert.ToDateTime(reader["fecha_adicion"]),
                                    FechaModificacion = reader["fecha_modificacion"] != DBNull.Value ? Convert.ToDateTime(reader["fecha_modificacion"]) : (DateTime?)null,
                                    ModificadoPor = reader["modificado_por"].ToString()
                                };
                                listaInventario.Add(inv);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener la lista de inventario: " + ex.Message);
                }
            }

            return listaInventario;
        }

        public Inventario ObtenerInventarioPorId(int InventarioID)
        {
            Inventario Inventario = null;

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "sp_Mostrar_Inventario_Por_Id"; // Procedimiento almacenado

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@InventarioID", InventarioID);

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Inventario = new Inventario
                                {
                                    InventarioID = Convert.ToInt32(reader["InventarioID"]),
                                    ProductoID = Convert.ToInt32(reader["ProductoID"]),
                                    ProductoNombre = reader["Producto"].ToString(),
                                    Cantidad = Convert.ToInt32(reader["Cantidad"]),
                                    FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"]),
                                    AdicionadoPor = reader["adicionado_por"].ToString(),
                                    FechaAdicion = Convert.ToDateTime(reader["fecha_adicion"]),
                                    FechaModificacion = reader["fecha_modificacion"] != DBNull.Value ? Convert.ToDateTime(reader["fecha_modificacion"]) : (DateTime?)null,
                                    ModificadoPor = reader["modificado_por"].ToString()
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener el Inventario: " + ex.Message);
                }
            }

            return Inventario;
        }


        public void ActualizarInventario(Inventario InventarioActualizado)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_Actualizar_Inventario", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@InventarioID", InventarioActualizado.InventarioID);
                cmd.Parameters.AddWithValue("@ProductoID", InventarioActualizado.ProductoID);
                cmd.Parameters.AddWithValue("@Cantidad", InventarioActualizado.Cantidad);
                cmd.Parameters.AddWithValue("@FechaActualizacion", InventarioActualizado.FechaActualizacion);
                cmd.Parameters.AddWithValue("@modificado_por", "Admin");

                cmd.ExecuteNonQuery();
            }
        }


        public void EliminarInventario(int InventarioID)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "EXEC sp_Eliminar_Inventario @InventarioID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@InventarioID", InventarioID);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al eliminar Inventario: " + ex.Message);
                }
            }
        }


        public void AgregarProveedor(Proveedor ProveedorNuevo)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_Insertar_Proveedor  @Nombre, @Contacto, @Telefono, @Email, @Direccion, @adicionado_por";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", ProveedorNuevo.Nombre);
                        cmd.Parameters.AddWithValue("@Contacto", ProveedorNuevo.Contacto);
                        cmd.Parameters.AddWithValue("@Telefono", ProveedorNuevo.Telefono);
                        cmd.Parameters.AddWithValue("@Email", ProveedorNuevo.Email);
                        cmd.Parameters.AddWithValue("@Direccion", ProveedorNuevo.Direccion);
                        cmd.Parameters.AddWithValue("@adicionado_por", ProveedorNuevo.AdicionadoPor);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al registrar  el Proveedor: " + ex.Message);
                }
            }
        }

        public List<Proveedor> MostrarProveedor()
        {
            List<Proveedor> listaProveedor = new List<Proveedor>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_Mostrar_Proveedores"; // Asegúrate de que este sea el nombre correcto del procedimiento almacenado
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Proveedor pro = new Proveedor
                                {
                                    ProveedorID = Convert.ToInt32(reader["ProveedorID"]),
                                    Nombre = reader["Nombre"].ToString(),
                                    Contacto = reader["Contacto"].ToString(), // Nombre del producto desde la tabla Productos
                                    Telefono = reader["Telefono"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    FechaAdicion = Convert.ToDateTime(reader["fecha_adicion"]),
                                    AdicionadoPor = reader["adicionado_por"].ToString(),
                                    FechaModificacion = reader["fecha_modificacion"] != DBNull.Value ? Convert.ToDateTime(reader["fecha_modificacion"]) : (DateTime?)null,
                                    ModificadoPor = reader["modificado_por"].ToString()
                                };
                                listaProveedor.Add(pro);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener la lista de inventario: " + ex.Message);
                }
            }

            return listaProveedor;
        }

        public Proveedor ObtenerProveedorPorId(int ProveedorID)
        {
            Proveedor Proveedor = null;

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "sp_Mostrar_Proveedor_Por_Id"; // Procedimiento almacenado

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ProveedorID", ProveedorID);

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Proveedor = new Proveedor
                                {
                                    ProveedorID = Convert.ToInt32(reader["ProveedorID"]),
                                    Nombre = reader["Nombre"].ToString(),
                                    Contacto = reader["Contacto"].ToString(),
                                    Telefono = reader["Telefono"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Direccion = reader["Direccion"].ToString(),
                                    AdicionadoPor = reader["adicionado_por"].ToString(),
                                    FechaAdicion = Convert.ToDateTime(reader["fecha_adicion"]),
                                    FechaModificacion = reader["fecha_modificacion"] != DBNull.Value ? Convert.ToDateTime(reader["fecha_modificacion"]) : (DateTime?)null,
                                    ModificadoPor = reader["modificado_por"].ToString()
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener el Inventario: " + ex.Message);
                }
            }

            return Proveedor;
        }

        public void ActualizarProveedor(Proveedor ProveedorActualizado)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_Actualizar_Proveedor", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ProveedorID", ProveedorActualizado.ProveedorID);
                cmd.Parameters.AddWithValue("@Nombre", ProveedorActualizado.Nombre);
                cmd.Parameters.AddWithValue("@Contacto", ProveedorActualizado.Contacto);
                cmd.Parameters.AddWithValue("@Telefono", ProveedorActualizado.Telefono);
                cmd.Parameters.AddWithValue("@Email", ProveedorActualizado.Email);
                cmd.Parameters.AddWithValue("@Direccion", ProveedorActualizado.Direccion);
                cmd.Parameters.AddWithValue("@modificado_por", "Admin");

                cmd.ExecuteNonQuery();
            }
        }

        public void EliminarProveedor(int ProveedorID)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "EXEC sp_Eliminar_Proveedor @ProveedorID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ProveedorID", ProveedorID);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al eliminar Proveedor: " + ex.Message);
                }
            }
        }

        public void AgregarCompra(Compra CompraNuevo)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_Insertar_Compra  @ProveedorID, @FechaCompra, @Total, @adicionado_por";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ProveedorID", CompraNuevo.ProveedorID);
                        cmd.Parameters.AddWithValue("@FechaCompra", CompraNuevo.FechaCompra);
                        cmd.Parameters.AddWithValue("@Total", CompraNuevo.Total);
                        cmd.Parameters.AddWithValue("@adicionado_por", CompraNuevo.AdicionadoPor);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al registrar  el Compra: " + ex.Message);
                }
            }
        }

        public List<Compra> MostrarCompra()
        {
            List<Compra> listaCompra = new List<Compra>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_Mostrar_Compras"; // Asegúrate de que este sea el nombre correcto del procedimiento almacenado
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Compra com = new Compra
                                {
                                    CompraID = Convert.ToInt32(reader["CompraID"]),
                                    ProveedorID = Convert.ToInt32(reader["ProveedorID"]),
                                    NombreProveedor = reader["ProveedorNombre"].ToString(),
                                    FechaCompra = Convert.ToDateTime(reader["FechaCompra"]), // Nombre del producto desde la tabla Productos
                                    Total = Convert.ToInt32(reader["Total"]),
                                    FechaAdicion = Convert.ToDateTime(reader["fecha_adicion"]),
                                    AdicionadoPor = reader["adicionado_por"].ToString(),
                                    FechaModificacion = reader["fecha_modificacion"] != DBNull.Value ? Convert.ToDateTime(reader["fecha_modificacion"]) : (DateTime?)null,
                                    ModificadoPor = reader["modificado_por"].ToString()
                                };
                                listaCompra.Add(com);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener la lista de inventario: " + ex.Message);
                }
            }

            return listaCompra;
        }

        public Compra ObtenerCompraPorId(int CompraID)
        {
            Compra Compra = null;

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "sp_Mostrar_Compra_Por_Id"; // Procedimiento almacenado

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CompraID", CompraID);

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Compra = new Compra
                                {
                                    CompraID = Convert.ToInt32(reader["CompraID"]),
                                    ProveedorID = Convert.ToInt32(reader["ProveedorID"]),
                                    NombreProveedor = reader["ProveedorNombre"].ToString(),
                                    FechaCompra = Convert.ToDateTime(reader["FechaCompra"]),
                                    Total = Convert.ToInt32(reader["Total"]),
                                    AdicionadoPor = reader["adicionado_por"].ToString(),
                                    FechaAdicion = Convert.ToDateTime(reader["fecha_adicion"]),
                                    FechaModificacion = reader["fecha_modificacion"] != DBNull.Value ? Convert.ToDateTime(reader["fecha_modificacion"]) : (DateTime?)null,
                                    ModificadoPor = reader["modificado_por"].ToString()
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener el Inventario: " + ex.Message);
                }
            }

            return Compra;
        }

        public void ActualizarCompra(Compra CompraActualizada)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_Actualizar_Compra", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@CompraID", CompraActualizada.CompraID);
                cmd.Parameters.AddWithValue("@ProveedorID", CompraActualizada.ProveedorID);
                cmd.Parameters.AddWithValue("@FechaCompra", CompraActualizada.FechaCompra);
                cmd.Parameters.AddWithValue("@Total", CompraActualizada.Total);
                cmd.Parameters.AddWithValue("@fecha_modificacion", CompraActualizada.FechaModificacion);
                cmd.Parameters.AddWithValue("@modificado_por", "Admin");

                cmd.ExecuteNonQuery();
            }
        }
        public void EliminarCompra(int CompraID)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "EXEC sp_Eliminar_Compra @CompraID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@CompraID", CompraID);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al eliminar Compra: " + ex.Message);
                }
            }
        }
        public void AgregarDetalleCompra(DetalleCompra DetalleCompraNuevo)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_Insertar_DetalleCompra  @CompraID, @ProductoID, @Cantidad, @PrecioUnitario, @adicionado_por";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@CompraID", DetalleCompraNuevo.CompraID);
                        cmd.Parameters.AddWithValue("@ProductoID", DetalleCompraNuevo.ProductoID);
                        cmd.Parameters.AddWithValue("@Cantidad", DetalleCompraNuevo.Cantidad);
                        cmd.Parameters.AddWithValue("@PrecioUnitario", DetalleCompraNuevo.PrecioUnitario);
                        cmd.Parameters.AddWithValue("@adicionado_por", DetalleCompraNuevo.AdicionadoPor);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al registrar el Detalle de Compra: " + ex.Message);
                }
            }
        }

        public List<DetalleCompra> MostrarDetalleCompra()
        {
            List<DetalleCompra> listaDetalleCompra = new List<DetalleCompra>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_Mostrar_DetalleCompra"; // Asegúrate de que este sea el nombre correcto del procedimiento almacenado
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DetalleCompra dec = new DetalleCompra
                                {
                                    DetalleCompraID = Convert.ToInt32(reader["DetalleCompraID"]),
                                    CompraID = Convert.ToInt32(reader["CompraID"]),
                                    ProductoID = Convert.ToInt32(reader["ProductoID"]),
                                    NombreProducto = reader["ProductoNombre"].ToString(),
                                    Cantidad = Convert.ToInt32(reader["Cantidad"]), // Nombre del producto desde la tabla Productos
                                    PrecioUnitario = Convert.ToInt32(reader["PrecioUnitario"]),
                                    FechaAdicion = Convert.ToDateTime(reader["fecha_adicion"]),
                                    AdicionadoPor = reader["adicionado_por"].ToString(),
                                    FechaModificacion = reader["fecha_modificacion"] != DBNull.Value ? Convert.ToDateTime(reader["fecha_modificacion"]) : (DateTime?)null,
                                    ModificadoPor = reader["modificado_por"].ToString()
                                };
                                listaDetalleCompra.Add(dec);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener la lista de inventario: " + ex.Message);
                }
            }

            return listaDetalleCompra;
        }

        public DetalleCompra ObtenerDetalleCompraPorId(int DetalleCompraID)
        {
            DetalleCompra DetalleCompra = null;

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "sp_Mostrar_DetalleCompra_Por_Id"; // Procedimiento almacenado

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@DetalleCompraID", DetalleCompraID);

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                DetalleCompra = new DetalleCompra
                                {
                                    DetalleCompraID = Convert.ToInt32(reader["DetalleCompraID"]),
                                    CompraID = Convert.ToInt32(reader["CompraID"]),
                                    ProductoID = Convert.ToInt32(reader["ProductoID"]),
                                    NombreProducto = reader["ProductoNombre"].ToString(),
                                    Cantidad = Convert.ToInt32(reader["Cantidad"]), // Nombre del producto desde la tabla Productos
                                    PrecioUnitario = Convert.ToInt32(reader["PrecioUnitario"]),
                                    FechaAdicion = Convert.ToDateTime(reader["fecha_adicion"]),
                                    AdicionadoPor = reader["adicionado_por"].ToString(),
                                    FechaModificacion = reader["fecha_modificacion"] != DBNull.Value ? Convert.ToDateTime(reader["fecha_modificacion"]) : (DateTime?)null,
                                    ModificadoPor = reader["modificado_por"].ToString()
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener el Inventario: " + ex.Message);
                }
            }

            return DetalleCompra;
        }

        public void ActualizarDetalleCompra(DetalleCompra DetalleCompraActualizada)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_Actualizar_DetalleCompra", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@DetalleCompraID", DetalleCompraActualizada.DetalleCompraID);
                cmd.Parameters.AddWithValue("@CompraID", DetalleCompraActualizada.CompraID);
                cmd.Parameters.AddWithValue("@ProductoID", DetalleCompraActualizada.ProductoID);
                cmd.Parameters.AddWithValue("@Cantidad", DetalleCompraActualizada.Cantidad);
                cmd.Parameters.AddWithValue("@PrecioUnitario", DetalleCompraActualizada.PrecioUnitario);
                cmd.Parameters.AddWithValue("@modificado_por", "Admin");

                cmd.ExecuteNonQuery();
            }
        }

        public void EliminarDetalleCompra(int DetalleCompraID)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "EXEC sp_Eliminar_DetalleCompra @DetalleCompraID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@DetalleCompraID", DetalleCompraID);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al eliminar Detalle de Compra: " + ex.Message);
                }
            }
        }

        public void AgregarPagosProveedor(PagosProveedor PagosProveedorNuevo)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_Insertar_PagoProveedor @CompraID, @Monto, @FechaPago, @MetodoPagoID, @fecha_adicion, @adicionado_por";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@CompraID", PagosProveedorNuevo.CompraID);
                        cmd.Parameters.AddWithValue("@Monto", PagosProveedorNuevo.Monto);
                        cmd.Parameters.AddWithValue("@FechaPago", PagosProveedorNuevo.FechaPago);
                        cmd.Parameters.AddWithValue("@MetodoPagoID", PagosProveedorNuevo.MetodoPagoID);
                        cmd.Parameters.AddWithValue("@fecha_adicion", PagosProveedorNuevo.FechaAdicion);
                        cmd.Parameters.AddWithValue("@adicionado_por", PagosProveedorNuevo.AdicionadoPor);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al registrar el Pago a Proveedor: " + ex.Message);
                }
            }
        }

        public List<PagosProveedor> MostrarPagosProveedor()
        {
            List<PagosProveedor> listaPagosProveedor = new List<PagosProveedor>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_Mostrar_PagoProveedor"; // Asegúrate de que este sea el nombre correcto del procedimiento almacenado
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PagosProveedor pap = new PagosProveedor
                                {
                                    PagoID = Convert.ToInt32(reader["PagoID"]),
                                    CompraID = Convert.ToInt32(reader["CompraID"]),
                                    Monto = Convert.ToInt32(reader["Monto"]),
                                    FechaPago = Convert.ToDateTime(reader["FechaPago"]),
                                    MetodoPagoID = Convert.ToInt32(reader["MetodoPagoID"]), // Nombre del producto desde la tabla Productos
                                    MetodoPagoNombre = reader["MetodoPago"].ToString(),
                                    FechaAdicion = Convert.ToDateTime(reader["fecha_adicion"]),
                                    AdicionadoPor = reader["adicionado_por"].ToString(),
                                    FechaModificacion = reader["fecha_modificacion"] != DBNull.Value ? Convert.ToDateTime(reader["fecha_modificacion"]) : (DateTime?)null,
                                    ModificadoPor = reader["modificado_por"].ToString()
                                };
                                listaPagosProveedor.Add(pap);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener la lista de inventario: " + ex.Message);
                }
            }

            return listaPagosProveedor;
        }

        public PagosProveedor ObtenerPagosProveedorPorId(int PagoID)
        {
            PagosProveedor PagosProveedor = null;

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "sp_Mostrar_PagoProveedor_Por_Id"; // Procedimiento almacenado

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PagoID", PagoID);

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                PagosProveedor = new PagosProveedor
                                {
                                    PagoID = Convert.ToInt32(reader["PagoID"]),
                                    CompraID = Convert.ToInt32(reader["CompraID"]),
                                    Monto = Convert.ToInt32(reader["Monto"]),
                                    FechaPago = Convert.ToDateTime(reader["FechaPago"]),
                                    MetodoPagoID = Convert.ToInt32(reader["MetodoPagoID"]), // Nombre del producto desde la tabla Productos
                                    MetodoPagoNombre = reader["MetodoPago"].ToString(),
                                    FechaAdicion = Convert.ToDateTime(reader["fecha_adicion"]),
                                    AdicionadoPor = reader["adicionado_por"].ToString(),
                                    FechaModificacion = reader["fecha_modificacion"] != DBNull.Value ? Convert.ToDateTime(reader["fecha_modificacion"]) : (DateTime?)null,
                                    ModificadoPor = reader["modificado_por"].ToString()
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener el Inventario: " + ex.Message);
                }
            }

            return PagosProveedor;
        }


        public void ActualizarPagosProveedor(PagosProveedor PagosProveedorActualizada)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_Actualizar_PagoProveedor", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@PagoID", PagosProveedorActualizada.PagoID);
                cmd.Parameters.AddWithValue("@CompraID", PagosProveedorActualizada.CompraID);
                cmd.Parameters.AddWithValue("@Monto", PagosProveedorActualizada.Monto);
                cmd.Parameters.AddWithValue("@FechaPago", PagosProveedorActualizada.FechaPago);
                cmd.Parameters.AddWithValue("@MetodoPagoID", PagosProveedorActualizada.MetodoPagoID);
                cmd.Parameters.AddWithValue("@modificado_por", "Admin");

                cmd.ExecuteNonQuery();
            }
        }

        public void EliminarPagosProveedor(int PagoID)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "EXEC sp_Eliminar_PagoProveedor @PagoID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@PagoID", PagoID);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al eliminar Pago de Proveedores: " + ex.Message);
                }
            }
        }

















    }
}
    

