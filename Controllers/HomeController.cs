using System.Data.SqlClient;
using System.Diagnostics;
using Gestion_Ventas_P.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gestion_Ventas_P.Controllers
{
    public class HomeController : Controller
    {
        private readonly AccesoDatos _accesoDatos;

        public HomeController(AccesoDatos accesoDatos)
        {
            _accesoDatos = accesoDatos;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Usuario oUsuario)
        {
            // Validamos el usuario a través del acceso a la base de datos
            Usuario usuarioValido = _accesoDatos.ValidarUsuario(oUsuario.NombreUsuario, oUsuario.Contrasenia);

            if (usuarioValido != null)
            {
                // Si el usuario es válido, se guarda en la sesión
                HttpContext.Session.SetString("usuario", usuarioValido.NombreUsuario); // Guardamos el nombre de usuario

                // Si deseas almacenar todo el objeto Usuario, puedes hacerlo como JSON
                // HttpContext.Session.SetString("usuario", JsonConvert.SerializeObject(usuarioValido));

                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Si el usuario no es válido, se muestra un mensaje de error
                ViewData["Mensaje"] = "Usuario o Contraseña no validos";
                return View();
            }
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult TipoDePan()
        {
            List<TipoDePan> listaTipoDePan = _accesoDatos.MostrarTipoDePans();
            return View(listaTipoDePan);
        }

        public IActionResult Categoria()
        {
            List<Categoria> listaCategoria = _accesoDatos.MostrarCategoria();
            return View(listaCategoria);
        }

        public IActionResult Producto()
        {
            try
            {
                // Obtener Categorías desde la base de datos
                ViewBag.Categorias = new SelectList(_accesoDatos.ObtenerCategorias(), "CategoriaID", "Nombre");

                // Obtener Tipos de Pan desde la base de datos
                ViewBag.TiposDePan = new SelectList(_accesoDatos.ObtenerTiposDePan(), "TipoPanID", "Nombre");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar los datos: " + ex.Message;
            }
            
            return View();
        }
        
        public IActionResult Venta()
        {
            try
            {
                // Obtener la lista de clientes para el dropdown
                ViewBag.Clientes = new SelectList(_accesoDatos.ObtenerClientes(), "ClienteID", "Nombre");
                
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar los datos: " + ex.Message;
                
            }
            return View();
        }

        public IActionResult DetalleVenta()
        {
            ViewBag.Ventas = new SelectList(_accesoDatos.MostrarVenta(), "VentaID", "ClienteNombre");
            ViewBag.Productos = new SelectList(_accesoDatos.MostrarProducto(), "ProductoID", "Nombre");
            return View();
        }

        public IActionResult PagoCliente() 
        {
            ViewBag.Ventas = new SelectList(_accesoDatos.MostrarVenta(), "VentaID", "ClienteNombre");
            ViewBag.MetodosPago = new SelectList(_accesoDatos.MostrarMetodoPago(), "MetodoPagoID", "Nombre");
            return View();
        }

        public IActionResult Inventario() 
        {
            ViewBag.Productos = new SelectList(_accesoDatos.MostrarProducto(), "ProductoID", "Nombre");
            return  View();
        }

        public IActionResult Proveedor()
        {
            return View();
        }

        public IActionResult Compra()
        {
            ViewBag.Proveedor = new SelectList(_accesoDatos.MostrarProveedor(), "ProveedorID", "Nombre");
            return View();
        }
        public IActionResult DetalleCompra()
        {
            ViewBag.Productos = new SelectList(_accesoDatos.MostrarProducto(), "ProductoID", "Nombre");
            return View();
        }

        public IActionResult VerProductos()
        {
            List<Producto> listaProducto = _accesoDatos.MostrarProducto();
            return View(listaProducto);
        }

        public IActionResult VerVentas()
        {
            List<Venta> listaVenta = _accesoDatos.MostrarVenta();
            return View(listaVenta);
        }
       
        public IActionResult VerMetodoPago()
        {
            List<MetodoPago> listaMetodoPago = _accesoDatos.MostrarMetodoPago();
            return View(listaMetodoPago);
        }

        public IActionResult VerPagoCliente()
        {
            List<PagoCliente> listaPagoCliente = _accesoDatos.VerPagoCliente();
            return View(listaPagoCliente);
        }
   
        public IActionResult VerDetalleVenta()
        {
            List<DetalleVenta> listaDetalleVenta = _accesoDatos.MostrarDetalleVenta();
            return View(listaDetalleVenta);
        }
        public IActionResult VerInventario()
        {
            List<Inventario> listaInventario = _accesoDatos.MostrarInventario();
            return View(listaInventario);
        }

        public IActionResult VerProveedor() 
        {
            List<Proveedor> listaProveedor = _accesoDatos.MostrarProveedor();
            return View(listaProveedor);
        }

        public IActionResult VerCompra()
        {
            List<Compra> listaCompra = _accesoDatos.MostrarCompra();
            return View(listaCompra);
        }

        List<Compra> listaCompra = new List<Compra>();
        public IActionResult ActualizarTipoDePan(int id)
        {
            TipoDePan TipoPan = _accesoDatos.ObtenerTipoDePanPorID(id);

            if (TipoPan == null)
            {
                return NotFound();
            }

            return View(TipoPan);
        }

        public IActionResult MetodoPago()
        {
            ViewBag.MetodosPago = new SelectList(_accesoDatos.MostrarMetodoPago(), "MetodoPagoID", "Nombre");
            return View();
        }

        public IActionResult CrearCliente()
        {
            List<Cliente> clientes = _accesoDatos.ObtenerTodosLosClientes();
            return View(clientes);
        }

        [HttpPost]
        public ActionResult CrearCliente(string nombre, string apellido, string apellido2, string direccion,string provincia, string canton, string telefono, string email,DateTime? fechaNacimiento, string nacionalidad, string adicionadoPor)
        {
            string mensaje;
            _accesoDatos.InsertarCliente(nombre, apellido, apellido2, direccion, provincia, canton, telefono,
                                         email, fechaNacimiento, nacionalidad, adicionadoPor, out mensaje);
            try
            {

                TempData["SuccessMessage"] = "Usuario se guardó con éxito";
                return RedirectToAction("CrearCliente");
            }
            catch (Exception ex)
            {
                TempData["SuccessMessage"] = "El usuario no se guardó: " + ex.Message;

                {

                };
                ViewData["Mensaje"] = mensaje;
                return View();
            }
        }


        public IActionResult ActualizarCliente(int id)
        {
            Cliente cliente = _accesoDatos.ObtenerClientePorID(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        [HttpPost]
        public IActionResult ActualizarCliente(Cliente cliente)
        {
            try
            {

                _accesoDatos.ActualizarCliente(cliente);

                TempData["SuccessMessage"] = "Cliente actualizado correctamente.";
                return RedirectToAction("CrearCliente");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al actualizar el Cliente: " + ex.Message;
                return View(cliente);
            }
            
        }

        [HttpPost]
        public IActionResult EliminarCliente(int ClienteID)
        {
            try
            {
                _accesoDatos.EliminarCliente(ClienteID);
                TempData["Mensaje"] = "Cliente eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("CrearCliente");
        }

        [HttpPost]
        public IActionResult AgregarTipoDePan(TipoDePan tipoNuevo)
        {

            try
                {
                    _accesoDatos.AgregarTipoDePan(tipoNuevo);
                    TempData["SuccessMessage"] = "Pan Agregado correctamente.";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error al agregar el pan: " + ex.Message;
                }
            List<TipoDePan> listaTipoDePan = _accesoDatos.MostrarTipoDePans();
            return View("TipoDePan", listaTipoDePan);
        }

        [HttpPost]
        public IActionResult ActualizarTipoDePan(TipoDePan panActualizado)
        {   
                try
                {
                    _accesoDatos.ActualizarTipoDePan(panActualizado);
                    TempData["SuccessMessage"] = "Tipo de pan actualizado correctamente.";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error al actualizar: " + ex.Message;
                }
           
            return RedirectToAction("TipoDePan");
        }


        [HttpPost]
        public IActionResult EliminarTipoDePan(int TipoDePanID)
        {
            try
            {
                _accesoDatos.EliminarTipoDePan(TipoDePanID);
                TempData["Mensaje"] = "Pan eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("TipoDePan");
        }

        [HttpPost]
        public IActionResult AgregarCategoria(Categoria CategoriaNueva)
        {

            try
            {
                _accesoDatos.AgregarCategoria(CategoriaNueva);
                TempData["SuccessMessage"] = "Categoria Agregada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al agregar la Categoria: " + ex.Message;
            }
            List<Categoria> listaCategoria = _accesoDatos.MostrarCategoria();
            return View("Categoria", listaCategoria);
        }

        public IActionResult ActualizarCategoria(int id)
        {
            Categoria categoria = _accesoDatos.ObtenerCategoriaPorID(id);
            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }

        [HttpPost]
        public IActionResult ActualizarCategoria(Categoria CategoriaActualizado)
        {
            try
            {
                _accesoDatos.ActualizarCategoria(CategoriaActualizado);
                TempData["SuccessMessage"] = "Categoria actualizada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al actualizar: " + ex.Message;
            }

            return RedirectToAction("Categoria");
        }

        [HttpPost]
        public IActionResult EliminarCategoria(int CategoriaID)
        {
            try
            {
                _accesoDatos.EliminarCategoria(CategoriaID);
                TempData["Mensaje"] = "Categoria eliminada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Categoria");
        }



        [HttpPost]
        public IActionResult AgregarProducto(Producto ProductoNuevo)
        {

            try
            {
                _accesoDatos.AgregarProductos(ProductoNuevo);
                TempData["SuccessMessage"] = "Producto Agregado correctamente.";
                return RedirectToAction("Producto");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al agregar la Producto: " + ex.Message;
                // Recargar las listas en caso de error
                ViewBag.Categorias = new SelectList(_accesoDatos.ObtenerCategorias(), "CategoriaID", "Nombre");
                ViewBag.TiposDePan = new SelectList(_accesoDatos.ObtenerTiposDePan(), "TipoPanID", "Nombre");
            }
            
            return View("Producto");

        }


        public IActionResult ActualizarProducto(int id)
        {
            Producto Producto = _accesoDatos.ObtenerMostrarProductoPorID(id);
            if (Producto == null)
            {
                return NotFound();
            }
            // Obtener las listas de categorías y tipos de pan para los dropdowns
            ViewBag.Categorias = new SelectList(_accesoDatos.ObtenerCategorias(), "CategoriaID", "Nombre");
            ViewBag.TiposDePan = new SelectList(_accesoDatos.ObtenerTiposDePan(), "TipoPanID", "Nombre");
            return View(Producto);
        }

        [HttpPost]
        public IActionResult ActualizarProducto(Producto ProductoActualizado)
        {
            try
            {
                _accesoDatos.ActualizarProducto(ProductoActualizado);
                TempData["SuccessMessage"] = "Producto actualizada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al actualizar: " + ex.Message;
            }

            return RedirectToAction("VerProductos");
        }


        [HttpPost]
        public IActionResult EliminarProducto(int ProductoID)
        {
            try
            {
                _accesoDatos.EliminarProducto(ProductoID);
                TempData["Mensaje"] = "Producto eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("VerProductos");
        }


        [HttpPost]
        public IActionResult AgregarVenta(Venta VentaNueva)
        {
            try
            {

                // Si hay errores de validación, recargar la vista con los datos ingresados
                ViewBag.Clientes = new SelectList(_accesoDatos.ObtenerClientes(), "ClienteID", "Nombre");

                // Llamar al método para agregar la venta
                _accesoDatos.AgregarVenta(VentaNueva);

                TempData["SuccessMessage"] = "Venta registrada correctamente.";
                return RedirectToAction("Venta"); // Redirigir a la lista de ventas
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al registrar la venta: " + ex.Message;

                // Recargar la vista con los datos ingresados en caso de error
                ViewBag.Clientes = new SelectList(_accesoDatos.ObtenerClientes(), "ClienteID", "Nombre");
                return View("Venta");
            }
        }

        public IActionResult ActualizarVenta(int id)
        {
            Venta Venta = _accesoDatos.ObtenerVentaPorID(id);
            if (Venta == null)
            {
                return NotFound();
            }
            // Obtener las listas de categorías y tipos de pan para los dropdowns
            ViewBag.Clientes = new SelectList(_accesoDatos.ObtenerClientes(), "ClienteID", "Nombre");
            return View(Venta);
        }

        [HttpPost]
        public IActionResult ActualizarVenta(Venta VentaActualizada)
        {
            try
            {
                _accesoDatos.ActualizarVenta(VentaActualizada);
                TempData["SuccessMessage"] = "Venta actualizada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al actualizar: " + ex.Message;
            }

            return RedirectToAction("VerVentas");
        }



        [HttpPost]
        public IActionResult EliminarVenta(int VentaID)
        {
            try
            {
                _accesoDatos.EliminarVenta(VentaID);
                TempData["Mensaje"] = "Venta eliminada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("VerVentas");
        }


        [HttpPost]
        public IActionResult AgregarMetodoPago(MetodoPago MetodoPagoNuevo)
        {
            try
            {

                // Llamar al método para agregar la venta
                _accesoDatos.AgregarMetodoPago(MetodoPagoNuevo);

                TempData["SuccessMessage"] = "MetodoPago registrado correctamente.";
                return RedirectToAction("MetodoPago"); // Redirigir a la lista de ventas
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al registrar el MetodoPago: " + ex.Message;
                return View("MetodoPago");
            }
        }

        public IActionResult ActualizarMetodoPago(int id)
        {
            MetodoPago MetodoPago = _accesoDatos.ObtenerMetodoPagoPorID(id);
            if (MetodoPago == null)
            {
                return NotFound();
            }
            return View(MetodoPago);
        }

        [HttpPost]
        public IActionResult ActualizarMetodoPago(MetodoPago MetodoPagoActualizado)
        {
            try
            {
                _accesoDatos.ActualizarMetodoPago(MetodoPagoActualizado);
                TempData["SuccessMessage"] = "Metodo de Pago actualizado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al actualizar: " + ex.Message;
            }

            return RedirectToAction("VerMetodoPago");
        }

        [HttpPost]
        public IActionResult EliminarMetodoPago(int MetodoPagoID)
        {
            try
            {
                _accesoDatos.EliminarMetodoPago(MetodoPagoID);
                TempData["Mensaje"] = "Metodo de Pago eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("VerMetodoPago");
        }


        [HttpPost]
        public IActionResult AgregarPagoCliente(PagoCliente PagoClienteNuevo)
        {
            try
            {
                ViewBag.Ventas = new SelectList(_accesoDatos.MostrarVenta(), "VentaID", "ClienteNombre");
                ViewBag.MetodosPago = new SelectList(_accesoDatos.MostrarMetodoPago(), "MetodoPagoID", "Nombre");
                // Llamar al método para agregar la venta
                _accesoDatos.AgregarPagoCliente(PagoClienteNuevo);

                TempData["SuccessMessage"] = "Pago de Cliente registrado correctamente.";
                return RedirectToAction("PagoCliente"); // Redirigir a la lista de ventas
            }
            catch (Exception ex)
            {
                ViewBag.Ventas = new SelectList(_accesoDatos.MostrarVenta(), "VentaID", "ClienteNombre");
                ViewBag.MetodosPago = new SelectList(_accesoDatos.MostrarMetodoPago(), "MetodoPagoID", "Nombre");
                TempData["ErrorMessage"] = "Error al registrar el MetodoPago: " + ex.Message;
                return View("PagoCliente");
            }
        }

        public IActionResult ActualizarPagoCliente(int id)
        {
            PagoCliente PagoCliente = _accesoDatos.ObtenerPagoClientePorId(id);
            if (PagoCliente == null)
            {
                return NotFound();
            }
            ViewBag.Ventas = new SelectList(_accesoDatos.MostrarVenta(), "VentaID", "ClienteNombre");
            ViewBag.MetodosPago = new SelectList(_accesoDatos.MostrarMetodoPago(), "MetodoPagoID", "Nombre");
            // Asegurarse de que las listas no sean nulas
            var ventas = _accesoDatos.MostrarVenta() ?? new List<Venta>();
            var metodosPago = _accesoDatos.MostrarMetodoPago() ?? new List<MetodoPago>();

            // Verificar si las listas están vacías
            if (!ventas.Any() || !metodosPago.Any())
            {
                TempData["ErrorMessage"] = "No hay ventas o métodos de pago disponibles.";
                return RedirectToAction("VerPagoCliente");
            }

            // Pasar los datos a la vista
            ViewBag.Ventas = new SelectList(_accesoDatos.MostrarVenta(), "VentaID", "ClienteNombre");
            ViewBag.MetodosPago = new SelectList(_accesoDatos.MostrarMetodoPago(), "MetodoPagoID", "Nombre");

            return View(PagoCliente);
        }

        [HttpPost]
        public IActionResult ActualizarPagoCliente(PagoCliente PagoClienteActualizado)
        {
            try
            {              
                _accesoDatos.ActualizarPagoCliente(PagoClienteActualizado);
                TempData["SuccessMessage"] = "Pago actualizado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al actualizar: " + ex.Message;
            }

            return RedirectToAction("VerPagoCliente");
        }

        [HttpPost]
        public IActionResult EliminarPagoCliente(int PagoClienteID)
        {
            try
            {
                _accesoDatos.EliminarPagoCliente(PagoClienteID);
                TempData["Mensaje"] = "Pago de Cliente eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("VerPagoCliente");
        }

        [HttpPost]
        public IActionResult AgregarDetalleVenta(DetalleVenta DetalleVentaNueva)
        {
            try
            {
                ViewBag.Ventas = new SelectList(_accesoDatos.MostrarVenta(), "VentaID", "ClienteNombre");
                ViewBag.Productos = new SelectList(_accesoDatos.MostrarProducto(), "ProductoID", "Nombre");
                // Llamar al método para agregar la venta
                _accesoDatos.AgregarDetalleVenta(DetalleVentaNueva);

                TempData["SuccessMessage"] = "Detalle de Venta registrado correctamente.";
                return RedirectToAction("DetalleVenta"); // Redirigir a la lista de ventas
            }
            catch (Exception ex)
            {
                ViewBag.Ventas = new SelectList(_accesoDatos.MostrarVenta(), "VentaID", "ClienteNombre");
                ViewBag.Productos = new SelectList(_accesoDatos.MostrarProducto(), "ProductoID", "Nombre");
                TempData["ErrorMessage"] = "Error al registrar el Detalle de Venta: " + ex.Message;
                return View("DetalleVenta");
            }
        }

        public IActionResult ActualizarDetalleVenta(int id)
        {
            DetalleVenta DetalleVenta = _accesoDatos.ObtenerDetalleVentaPorId(id);
            if (DetalleVenta == null)
            {
                return NotFound();
            }
            ViewBag.Ventas = new SelectList(_accesoDatos.MostrarVenta(), "VentaID", "ClienteNombre");
            ViewBag.Productos = new SelectList(_accesoDatos.MostrarProducto(), "ProductoID", "Nombre");
            // Asegurarse de que las listas no sean nulas
            var ventas = _accesoDatos.MostrarVenta() ?? new List<Venta>();
            var productos = _accesoDatos.MostrarProducto() ?? new List<Producto>();

            // Verificar si las listas están vacías
            if (!ventas.Any() || !productos.Any())
            {
                TempData["ErrorMessage"] = "No hay ventas o productos disponibles.";
                return RedirectToAction("VerDetalleVenta");
            }

            // Pasar los datos a la vista
            ViewBag.Ventas = new SelectList(_accesoDatos.MostrarVenta(), "VentaID", "ClienteNombre");
            ViewBag.Productos = new SelectList(_accesoDatos.MostrarProducto(), "ProductoID", "Nombre");

            return View(DetalleVenta);
        }

        [HttpPost]
        public IActionResult ActualizarDetalleVenta(DetalleVenta DetalleVentaActualizado)
        {
            try
            {
                _accesoDatos.ActualizarDetalleVenta(DetalleVentaActualizado);
                TempData["SuccessMessage"] = "Detalle de Venta actualizado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al actualizar: " + ex.Message;
            }

            return RedirectToAction("VerDetalleVenta");
        }

        [HttpPost]
        public IActionResult EliminarDetalleVenta(int DetalleVentaID)
        {
            try
            {
                _accesoDatos.EliminarDetalleVenta(DetalleVentaID);
                TempData["Mensaje"] = "Detalle de Venta eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("VerDetalleVenta");
        }

        [HttpPost]
        public IActionResult AgregarInventario(Inventario InventarioNuevo)
        {
            try
            {
                ViewBag.Productos = new SelectList(_accesoDatos.MostrarProducto(), "ProductoID", "Nombre");
                // Llamar al método para agregar la venta
                _accesoDatos.AgregarInventario(InventarioNuevo);

                TempData["SuccessMessage"] = "Producto registrado en Inventario correctamente.";
                return RedirectToAction("Inventario"); // Redirigir a la lista de ventas
            }
            catch (Exception ex)
            {
                ViewBag.Productos = new SelectList(_accesoDatos.MostrarProducto(), "ProductoID", "Nombre");
                TempData["ErrorMessage"] = "Error al registrar el en Inventario: " + ex.Message;
                return View("Inventario");
            }
        }

        public IActionResult ActualizarInventario(int id)
        {
            Inventario Inventario = _accesoDatos.ObtenerInventarioPorId(id);
            if (Inventario == null)
            {
                return NotFound();
            }
            ViewBag.Productos = new SelectList(_accesoDatos.MostrarProducto(), "ProductoID", "Nombre");
            // Asegurarse de que las listas no sean nulas
            var productos = _accesoDatos.MostrarProducto() ?? new List<Producto>();

            // Verificar si las listas están vacías
            if (!productos.Any())
            {
                TempData["ErrorMessage"] = "No hay productos disponibles.";
                return RedirectToAction("VerInventario");
            }

            // Pasar los datos a la vista
            ViewBag.Productos = new SelectList(_accesoDatos.MostrarProducto(), "ProductoID", "Nombre");

            return View(Inventario);
        }

        [HttpPost]
        public IActionResult ActualizarInventario(Inventario InventarioActualizado)
        {
            try
            {
                _accesoDatos.ActualizarInventario(InventarioActualizado);
                TempData["SuccessMessage"] = "Inventario actualizado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al actualizar: " + ex.Message;
            }

            return RedirectToAction("VerInventario");
        }

        [HttpPost]
        public IActionResult EliminarInventario(int InventarioID)
        {
            try
            {
                _accesoDatos.EliminarInventario(InventarioID);
                TempData["Mensaje"] = "Inventario eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("VerInventario");
        }

        [HttpPost]
        public IActionResult AgregarProveedor(Proveedor ProveedorNuevo)
        {
            try
            {
                _accesoDatos.AgregarProveedor(ProveedorNuevo);

                TempData["SuccessMessage"] = "Proveedor registrado Proveedor correctamente.";
                return RedirectToAction("Proveedor"); // Redirigir a la lista de ventas
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al registrar Proveedor: " + ex.Message;
                return View("Proveedor");
            }
        }

        public IActionResult ActualizarProveedor(int id)
        {
            Proveedor Proveedor = _accesoDatos.ObtenerProveedorPorId(id);
            if (Proveedor == null)
            {
                return NotFound();
            }        
         
            return View(Proveedor);
        }

        [HttpPost]
        public IActionResult ActualizarProveedor(Proveedor ProveedorActualizado)
        {
            try
            {
                _accesoDatos.ActualizarProveedor(ProveedorActualizado);
                TempData["SuccessMessage"] = "Proveedor actualizado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al actualizar: " + ex.Message;
            }

            return RedirectToAction("VerProveedor");
        }

        [HttpPost]
        public IActionResult EliminarProveedor(int ProveedorID)
        {
            try
            {
                _accesoDatos.EliminarProveedor(ProveedorID);
                TempData["Mensaje"] = "Proveedor eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("VerProveedor");
        }

        [HttpPost]
        public IActionResult AgregarCompra(Compra CompraNueva)
        {
            try
            {
                ViewBag.Proveedor = new SelectList(_accesoDatos.MostrarProveedor(), "ProveedorID", "Nombre");
                _accesoDatos.AgregarCompra(CompraNueva);
                TempData["SuccessMessage"] = "Compra registrada correctamente.";
                return RedirectToAction("Compra"); // Redirigir a la lista de ventas
            }
            catch (Exception ex)
            {
                ViewBag.Proveedor = new SelectList(_accesoDatos.MostrarProveedor(), "ProveedorID", "Nombre");
                TempData["ErrorMessage"] = "Error al registrar Compra: " + ex.Message;
                return View("Compra");
            }
        }

        public IActionResult ActualizarCompra(int id)
        {
            Compra Compra = _accesoDatos.ObtenerCompraPorId(id);
            if (Compra == null)
            {
                return NotFound();
            }
            ViewBag.Proveedor = new SelectList(_accesoDatos.MostrarProveedor(), "ProveedorID", "Nombre");
            // Asegurarse de que las listas no sean nulas
            var proveedores = _accesoDatos.MostrarProveedor() ?? new List<Proveedor>();

            // Verificar si las listas están vacías
            if (!proveedores.Any())
            {
                TempData["ErrorMessage"] = "No hay Proveedores disponibles.";
                return RedirectToAction("VerCompra");
            }

            // Pasar los datos a la vista
            ViewBag.Proveedor = new SelectList(_accesoDatos.MostrarProveedor(), "ProveedorID", "Nombre");

            return View(Compra);
        }

        [HttpPost]
        public IActionResult ActualizarCompra(Compra CompraActualizada)
        {
            try
            {
                _accesoDatos.ActualizarCompra(CompraActualizada);
                TempData["SuccessMessage"] = "Compra actualizada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al actualizar: " + ex.Message;
            }

            return RedirectToAction("VerCompra");
        }

        [HttpPost]
        public IActionResult EliminarCompra(int CompraID)
        {
            try
            {
                _accesoDatos.EliminarCompra(CompraID);
                TempData["Mensaje"] = "Compra eliminada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("VerCompra");
        }


        [HttpPost]
        public IActionResult AgregarDetalleCompra(DetalleCompra DetalleCompraNueva)
        {
            try
            {
                ViewBag.Productos = new SelectList(_accesoDatos.MostrarProducto(), "ProductoID", "Nombre");
                _accesoDatos.AgregarDetalleCompra(DetalleCompraNueva);
                TempData["SuccessMessage"] = "Detalla de Compra registrado correctamente.";
                return RedirectToAction("DetalleCompra"); // Redirigir a la lista de ventas
            }
            catch (Exception ex)
            {
                ViewBag.Productos = new SelectList(_accesoDatos.MostrarProducto(), "ProductoID", "Nombre");
                TempData["ErrorMessage"] = "Error al registrar Detalle de Compra: " + ex.Message;
                return View("DetalleCompra");
            }
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
