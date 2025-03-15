using System.Data.SqlClient;
using System.Diagnostics;
using Gestion_Ventas_P.Models;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult ActualizarTipoDePan(int id)
        {
            TipoDePan TipoPan = _accesoDatos.ObtenerTipoDePanPorID(id);

            if (TipoPan == null)
            {
                return NotFound();
            }

            return View(TipoPan);
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




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
