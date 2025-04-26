using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Collections.Generic;
using Gestion_Ventas_P.Models; // Aquí está tu clase AccesoDatos
using System;

namespace Gestion_Ventas_P.Controllers
{
    public class ReportesController : Controller
    {
        private readonly AccesoDatos _accesoDatos;

        public ReportesController(AccesoDatos accesoDatos)
        {
            _accesoDatos = accesoDatos;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ExportarPDF()
        {
            var productos = _accesoDatos.MostrarProducto();

            var stream = new MemoryStream(); // fuera del using

            var doc = new Document(PageSize.A4.Rotate());
            var writer = PdfWriter.GetInstance(doc, stream);
            doc.Open();

            doc.Add(new Paragraph("Reporte de Productos"));
            doc.Add(new Paragraph("Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm")));
            doc.Add(new Paragraph(" "));

            var table = new PdfPTable(7);
            table.WidthPercentage = 100;
            table.AddCell("Nombre");
            table.AddCell("Descripción");
            table.AddCell("Precio Unitario");
            table.AddCell("Categoría");
            table.AddCell("Tipo de Pan");
            table.AddCell("Fecha Adición");
            table.AddCell("Adicionado Por");

            foreach (var p in productos)
            {
                table.AddCell(p.Nombre);
                table.AddCell(p.Descripcion);
                table.AddCell(p.PrecioUnitario.ToString("C"));
                table.AddCell(p.CategoriaNombre);
                table.AddCell(p.TipoPanNombre);
                table.AddCell(p.FechaAdicion.ToString("dd/MM/yyyy"));
                table.AddCell(p.AdicionadoPor);
            }

            doc.Add(table);
            doc.Close(); // esto NO cierra el stream

            stream.Position = 0; // esto es importante
            return File(stream, "application/pdf", "Reporte_Productos.pdf");
        }


        public IActionResult ExportarExcel()
        {
            var productos = _accesoDatos.MostrarProducto();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Productos");
                worksheet.Cells[1, 1].Value = "Nombre";
                worksheet.Cells[1, 2].Value = "Descripción";
                worksheet.Cells[1, 3].Value = "Precio Unitario";
                worksheet.Cells[1, 4].Value = "Categoría";
                worksheet.Cells[1, 5].Value = "Tipo de Pan";
                worksheet.Cells[1, 6].Value = "Fecha Adición";
                worksheet.Cells[1, 7].Value = "Adicionado Por";

                for (int i = 0; i < productos.Count; i++)
                {
                    var p = productos[i];
                    worksheet.Cells[i + 2, 1].Value = p.Nombre;
                    worksheet.Cells[i + 2, 2].Value = p.Descripcion;
                    worksheet.Cells[i + 2, 3].Value = p.PrecioUnitario;
                    worksheet.Cells[i + 2, 4].Value = p.CategoriaNombre;
                    worksheet.Cells[i + 2, 5].Value = p.TipoPanNombre;
                    worksheet.Cells[i + 2, 6].Value = p.FechaAdicion.ToString("dd/MM/yyyy");
                    worksheet.Cells[i + 2, 7].Value = p.AdicionadoPor;
                }

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte_Productos.xlsx");
            }
        }


        public IActionResult ExportarInventarioPDF()
        {
            var inventario = _accesoDatos.MostrarInventario();

            var stream = new MemoryStream();

            var doc = new Document(PageSize.A4.Rotate());
            PdfWriter.GetInstance(doc, stream);
            doc.Open();

            doc.Add(new Paragraph("Reporte de Inventario Actual"));
            doc.Add(new Paragraph("Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm")));
            doc.Add(new Paragraph(" "));

            var table = new PdfPTable(7);
            table.WidthPercentage = 100;

            table.AddCell("Producto");
            table.AddCell("Cantidad");
            table.AddCell("Fecha Actualización");
            table.AddCell("Adicionado Por");
            table.AddCell("Fecha Adición");
            table.AddCell("Modificado Por");
            table.AddCell("Fecha Modificación");

            foreach (var inv in inventario)
            {
                table.AddCell(inv.ProductoNombre);
                table.AddCell(inv.Cantidad.ToString());
                table.AddCell(inv.FechaActualizacion.ToString("dd/MM/yyyy"));
                table.AddCell(inv.AdicionadoPor);
                table.AddCell(inv.FechaAdicion.ToString("dd/MM/yyyy"));
                table.AddCell(inv.ModificadoPor ?? "N/A");
                table.AddCell(inv.FechaModificacion?.ToString("dd/MM/yyyy") ?? "N/A");
            }

            doc.Add(table);
            doc.Close();

            stream.Position = 0;
            return File(stream, "application/pdf", "Reporte_Inventario.pdf");
        }

        public IActionResult ExportarInventarioExcel()
        {
            var inventario = _accesoDatos.MostrarInventario();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Inventario");

                worksheet.Cells[1, 1].Value = "Producto";
                worksheet.Cells[1, 2].Value = "Cantidad";
                worksheet.Cells[1, 3].Value = "Fecha Actualización";
                worksheet.Cells[1, 4].Value = "Adicionado Por";
                worksheet.Cells[1, 5].Value = "Fecha Adición";
                worksheet.Cells[1, 6].Value = "Modificado Por";
                worksheet.Cells[1, 7].Value = "Fecha Modificación";

                for (int i = 0; i < inventario.Count; i++)
                {
                    var inv = inventario[i];
                    worksheet.Cells[i + 2, 1].Value = inv.ProductoNombre;
                    worksheet.Cells[i + 2, 2].Value = inv.Cantidad;
                    worksheet.Cells[i + 2, 3].Value = inv.FechaActualizacion.ToString("dd/MM/yyyy");
                    worksheet.Cells[i + 2, 4].Value = inv.AdicionadoPor;
                    worksheet.Cells[i + 2, 5].Value = inv.FechaAdicion.ToString("dd/MM/yyyy");
                    worksheet.Cells[i + 2, 6].Value = inv.ModificadoPor ?? "N/A";
                    worksheet.Cells[i + 2, 7].Value = inv.FechaModificacion?.ToString("dd/MM/yyyy") ?? "N/A";
                }

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte_Inventario.xlsx");
            }
        }

        public IActionResult ExportarDetalleCompraPDF()
        {
            var detalles = _accesoDatos.MostrarDetalleCompra();

            var stream = new MemoryStream();

            var doc = new Document(PageSize.A4.Rotate());
            PdfWriter.GetInstance(doc, stream);
            doc.Open();

            doc.Add(new Paragraph("Reporte de Detalle de Compras"));
            doc.Add(new Paragraph("Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm")));
            doc.Add(new Paragraph(" "));

            var table = new PdfPTable(9);
            table.WidthPercentage = 100;

            table.AddCell("Compra ID");
            table.AddCell("Producto ID");
            table.AddCell("Nombre Producto");
            table.AddCell("Cantidad");
            table.AddCell("Precio Unitario");
            table.AddCell("Fecha Adición");
            table.AddCell("Adicionado Por");
            table.AddCell("Fecha Modificación");
            table.AddCell("Modificado Por");

            foreach (var d in detalles)
            {
                table.AddCell(d.CompraID.ToString());
                table.AddCell(d.ProductoID.ToString());
                table.AddCell(d.NombreProducto);
                table.AddCell(d.Cantidad.ToString());
                table.AddCell(d.PrecioUnitario.ToString("C"));
                table.AddCell(d.FechaAdicion.ToString("dd/MM/yyyy"));
                table.AddCell(d.AdicionadoPor);
                table.AddCell(d.FechaModificacion?.ToString("dd/MM/yyyy") ?? "N/A");
                table.AddCell(d.ModificadoPor ?? "N/A");
            }

            doc.Add(table);
            doc.Close();

            stream.Position = 0;
            return File(stream, "application/pdf", "Reporte_DetalleCompras.pdf");
        }

        public IActionResult ExportarDetalleCompraExcel()
        {
            var detalles = _accesoDatos.MostrarDetalleCompra();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Detalle Compras");

                worksheet.Cells[1, 1].Value = "Compra ID";
                worksheet.Cells[1, 2].Value = "Producto ID";
                worksheet.Cells[1, 3].Value = "Nombre Producto";
                worksheet.Cells[1, 4].Value = "Cantidad";
                worksheet.Cells[1, 5].Value = "Precio Unitario";
                worksheet.Cells[1, 6].Value = "Fecha Adición";
                worksheet.Cells[1, 7].Value = "Adicionado Por";
                worksheet.Cells[1, 8].Value = "Fecha Modificación";
                worksheet.Cells[1, 9].Value = "Modificado Por";

                for (int i = 0; i < detalles.Count; i++)
                {
                    var d = detalles[i];
                    worksheet.Cells[i + 2, 1].Value = d.CompraID;
                    worksheet.Cells[i + 2, 2].Value = d.ProductoID;
                    worksheet.Cells[i + 2, 3].Value = d.NombreProducto;
                    worksheet.Cells[i + 2, 4].Value = d.Cantidad;
                    worksheet.Cells[i + 2, 5].Value = d.PrecioUnitario;
                    worksheet.Cells[i + 2, 6].Value = d.FechaAdicion.ToString("dd/MM/yyyy");
                    worksheet.Cells[i + 2, 7].Value = d.AdicionadoPor;
                    worksheet.Cells[i + 2, 8].Value = d.FechaModificacion?.ToString("dd/MM/yyyy") ?? "N/A";
                    worksheet.Cells[i + 2, 9].Value = d.ModificadoPor ?? "N/A";
                }

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte_DetalleCompras.xlsx");
            }
        }

        public IActionResult ExportarClientesPDF()
        {
            var clientes = _accesoDatos.ObtenerTodosLosClientes(); // Método que deberías tener

            var stream = new MemoryStream();
            var doc = new Document(PageSize.A4.Rotate());
            PdfWriter.GetInstance(doc, stream);
            doc.Open();

            doc.Add(new Paragraph("Reporte de Clientes"));
            doc.Add(new Paragraph("Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm")));
            doc.Add(new Paragraph(" "));

            var table = new PdfPTable(10);
            table.WidthPercentage = 100;

            table.AddCell("Nombre");
            table.AddCell("Apellido 1");
            table.AddCell("Apellido 2");
            table.AddCell("Dirección");
            table.AddCell("Provincia");
            table.AddCell("Cantón");
            table.AddCell("Teléfono");
            table.AddCell("Email");
            table.AddCell("Nacimiento");
            table.AddCell("Nacionalidad");

            foreach (var c in clientes)
            {
                table.AddCell(c.Nombre);
                table.AddCell(c.Apellido);
                table.AddCell(c.Apellido2 ?? "N/A");
                table.AddCell(c.Direccion ?? "N/A");
                table.AddCell(c.Provincia ?? "N/A");
                table.AddCell(c.Canton ?? "N/A");
                table.AddCell(c.Telefono ?? "N/A");
                table.AddCell(c.Email ?? "N/A");
                table.AddCell(c.FechaNacimiento?.ToString("dd/MM/yyyy") ?? "N/A");
                table.AddCell(c.Nacionalidad ?? "N/A");
            }

            doc.Add(table);
            doc.Close();

            stream.Position = 0;
            return File(stream, "application/pdf", "Reporte_Clientes.pdf");
        }

        public IActionResult ExportarClientesExcel()
        {
            var clientes = _accesoDatos.ObtenerTodosLosClientes();

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Clientes");

                ws.Cells[1, 1].Value = "Nombre";
                ws.Cells[1, 2].Value = "Apellido 1";
                ws.Cells[1, 3].Value = "Apellido 2";
                ws.Cells[1, 4].Value = "Dirección";
                ws.Cells[1, 5].Value = "Provincia";
                ws.Cells[1, 6].Value = "Cantón";
                ws.Cells[1, 7].Value = "Teléfono";
                ws.Cells[1, 8].Value = "Email";
                ws.Cells[1, 9].Value = "Nacimiento";
                ws.Cells[1, 10].Value = "Nacionalidad";

                for (int i = 0; i < clientes.Count; i++)
                {
                    var c = clientes[i];
                    ws.Cells[i + 2, 1].Value = c.Nombre;
                    ws.Cells[i + 2, 2].Value = c.Apellido;
                    ws.Cells[i + 2, 3].Value = c.Apellido2 ?? "N/A";
                    ws.Cells[i + 2, 4].Value = c.Direccion ?? "N/A";
                    ws.Cells[i + 2, 5].Value = c.Provincia ?? "N/A";
                    ws.Cells[i + 2, 6].Value = c.Canton ?? "N/A";
                    ws.Cells[i + 2, 7].Value = c.Telefono ?? "N/A";
                    ws.Cells[i + 2, 8].Value = c.Email ?? "N/A";
                    ws.Cells[i + 2, 9].Value = c.FechaNacimiento?.ToString("dd/MM/yyyy") ?? "N/A";
                    ws.Cells[i + 2, 10].Value = c.Nacionalidad ?? "N/A";
                }

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte_Clientes.xlsx");
            }
        }

        public IActionResult ExportarVentasPDF()
        {
            var ventas = _accesoDatos.MostrarVenta(); // Método para listar ventas

            var stream = new MemoryStream();
            var doc = new Document(PageSize.A4.Rotate());
            PdfWriter.GetInstance(doc, stream);
            doc.Open();

            doc.Add(new Paragraph("Reporte de Ventas"));
            doc.Add(new Paragraph("Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm")));
            doc.Add(new Paragraph(" "));

            var table = new PdfPTable(9);
            table.WidthPercentage = 100;

            table.AddCell("Venta ID");
            table.AddCell("Cliente Nombre");
            table.AddCell("Fecha Venta");
            table.AddCell("Total");
            table.AddCell("Estado");
            table.AddCell("Adicionado Por");
            table.AddCell("Fecha Adición");
            table.AddCell("Modificado Por");
            table.AddCell("Fecha Modificación");

            foreach (var v in ventas)
            {
                table.AddCell(v.VentaID.ToString());
                table.AddCell(v.ClienteNombre.ToString());
                table.AddCell(v.FechaVenta.ToString("dd/MM/yyyy HH:mm"));
                table.AddCell(v.Total.ToString("C"));
                table.AddCell(v.Estado);
                table.AddCell(v.AdicionadoPor);
                table.AddCell(v.FechaAdicion.ToString("dd/MM/yyyy"));
                table.AddCell(v.ModificadoPor ?? "N/A");
                table.AddCell(v.FechaModificacion?.ToString("dd/MM/yyyy") ?? "N/A");
            }

            doc.Add(table);
            doc.Close();

            stream.Position = 0;
            return File(stream, "application/pdf", "Reporte_Ventas.pdf");
        }

        public IActionResult ExportarVentasExcel()
        {
            var ventas = _accesoDatos.MostrarVenta();

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Ventas");

                ws.Cells[1, 1].Value = "Venta ID";
                ws.Cells[1, 2].Value = "Cliente Nombre";
                ws.Cells[1, 3].Value = "Fecha Venta";
                ws.Cells[1, 4].Value = "Total";
                ws.Cells[1, 5].Value = "Estado";
                ws.Cells[1, 6].Value = "Adicionado Por";
                ws.Cells[1, 7].Value = "Fecha Adición";
                ws.Cells[1, 8].Value = "Modificado Por";
                ws.Cells[1, 9].Value = "Fecha Modificación";

                for (int i = 0; i < ventas.Count; i++)
                {
                    var v = ventas[i];
                    ws.Cells[i + 2, 1].Value = v.VentaID;
                    ws.Cells[i + 2, 2].Value = v.ClienteNombre;
                    ws.Cells[i + 2, 3].Value = v.FechaVenta.ToString("dd/MM/yyyy HH:mm");
                    ws.Cells[i + 2, 4].Value = v.Total;
                    ws.Cells[i + 2, 5].Value = v.Estado;
                    ws.Cells[i + 2, 6].Value = v.AdicionadoPor;
                    ws.Cells[i + 2, 7].Value = v.FechaAdicion.ToString("dd/MM/yyyy");
                    ws.Cells[i + 2, 8].Value = v.ModificadoPor ?? "N/A";
                    ws.Cells[i + 2, 9].Value = v.FechaModificacion?.ToString("dd/MM/yyyy") ?? "N/A";
                }

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte_Ventas.xlsx");
            }
        }

        public IActionResult ExportarDetalleVentaPDF()
        {
            var detalles = _accesoDatos.MostrarDetalleVenta();

            var stream = new MemoryStream();
            var doc = new Document(PageSize.A4.Rotate());
            PdfWriter.GetInstance(doc, stream);
            doc.Open();

            doc.Add(new Paragraph("Reporte de Detalle de Ventas"));
            doc.Add(new Paragraph("Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm")));
            doc.Add(new Paragraph(" "));

            var table = new PdfPTable(9);
            table.WidthPercentage = 100;

            table.AddCell("Detalle ID");
            table.AddCell("Venta ID");
            table.AddCell("Cliente");
            table.AddCell("Producto ID");
            table.AddCell("Nombre Producto");
            table.AddCell("Cantidad");
            table.AddCell("Precio Unitario");
            table.AddCell("Adicionado Por");
            table.AddCell("Fecha Adición");

            foreach (var d in detalles)
            {
                table.AddCell(d.DetalleVentaID.ToString());
                table.AddCell(d.VentaID.ToString());
                table.AddCell(d.ClienteVenta);
                table.AddCell(d.ProductoID.ToString());
                table.AddCell(d.NombreProducto);
                table.AddCell(d.Cantidad.ToString());
                table.AddCell(d.PrecioUnitario.ToString("C"));
                table.AddCell(d.AdicionadoPor);
                table.AddCell(d.FechaAdicion.ToString("dd/MM/yyyy"));
            }

            doc.Add(table);
            doc.Close();

            stream.Position = 0;
            return File(stream, "application/pdf", "Reporte_DetalleVentas.pdf");
        }


        public IActionResult ExportarDetalleVentaExcel()
        {
            var detalles = _accesoDatos.MostrarDetalleVenta();

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Detalle Ventas");

                ws.Cells[1, 1].Value = "Detalle ID";
                ws.Cells[1, 2].Value = "Venta ID";
                ws.Cells[1, 3].Value = "Cliente";
                ws.Cells[1, 4].Value = "Producto ID";
                ws.Cells[1, 5].Value = "Nombre Producto";
                ws.Cells[1, 6].Value = "Cantidad";
                ws.Cells[1, 7].Value = "Precio Unitario";
                ws.Cells[1, 8].Value = "Adicionado Por";
                ws.Cells[1, 9].Value = "Fecha Adición";

                for (int i = 0; i < detalles.Count; i++)
                {
                    var d = detalles[i];
                    ws.Cells[i + 2, 1].Value = d.DetalleVentaID;
                    ws.Cells[i + 2, 2].Value = d.VentaID;
                    ws.Cells[i + 2, 3].Value = d.ClienteVenta;
                    ws.Cells[i + 2, 4].Value = d.ProductoID;
                    ws.Cells[i + 2, 5].Value = d.NombreProducto;
                    ws.Cells[i + 2, 6].Value = d.Cantidad;
                    ws.Cells[i + 2, 7].Value = d.PrecioUnitario;
                    ws.Cells[i + 2, 8].Value = d.AdicionadoPor;
                    ws.Cells[i + 2, 9].Value = d.FechaAdicion.ToString("dd/MM/yyyy");
                }

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte_DetalleVentas.xlsx");
            }
        }

        public IActionResult ExportarComprasPDF()
        {
            var compras = _accesoDatos.MostrarCompra();

            var stream = new MemoryStream();
            var doc = new Document(PageSize.A4.Rotate());
            PdfWriter.GetInstance(doc, stream);
            doc.Open();

            doc.Add(new Paragraph("Reporte de Compras"));
            doc.Add(new Paragraph("Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm")));
            doc.Add(new Paragraph(" "));

            var table = new PdfPTable(9);
            table.WidthPercentage = 100;

            table.AddCell("Compra ID");
            table.AddCell("Proveedor ID");
            table.AddCell("Proveedor");
            table.AddCell("Fecha Compra");
            table.AddCell("Total");
            table.AddCell("Adicionado Por");
            table.AddCell("Fecha Adición");
            table.AddCell("Modificado Por");
            table.AddCell("Fecha Modificación");

            foreach (var c in compras)
            {
                table.AddCell(c.CompraID.ToString());
                table.AddCell(c.ProveedorID.ToString());
                table.AddCell(c.NombreProveedor);
                table.AddCell(c.FechaCompra.ToString("dd/MM/yyyy"));
                table.AddCell(c.Total.ToString("C"));
                table.AddCell(c.AdicionadoPor);
                table.AddCell(c.FechaAdicion.ToString("dd/MM/yyyy"));
                table.AddCell(c.ModificadoPor ?? "N/A");
                table.AddCell(c.FechaModificacion?.ToString("dd/MM/yyyy") ?? "N/A");
            }

            doc.Add(table);
            doc.Close();

            stream.Position = 0;
            return File(stream, "application/pdf", "Reporte_Compras.pdf");
        }

        public IActionResult ExportarComprasExcel()
        {
            var compras = _accesoDatos.MostrarCompra();

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Compras");

                ws.Cells[1, 1].Value = "Compra ID";
                ws.Cells[1, 2].Value = "Proveedor ID";
                ws.Cells[1, 3].Value = "Proveedor";
                ws.Cells[1, 4].Value = "Fecha Compra";
                ws.Cells[1, 5].Value = "Total";
                ws.Cells[1, 6].Value = "Adicionado Por";
                ws.Cells[1, 7].Value = "Fecha Adición";
                ws.Cells[1, 8].Value = "Modificado Por";
                ws.Cells[1, 9].Value = "Fecha Modificación";

                for (int i = 0; i < compras.Count; i++)
                {
                    var c = compras[i];
                    ws.Cells[i + 2, 1].Value = c.CompraID;
                    ws.Cells[i + 2, 2].Value = c.ProveedorID;
                    ws.Cells[i + 2, 3].Value = c.NombreProveedor;
                    ws.Cells[i + 2, 4].Value = c.FechaCompra.ToString("dd/MM/yyyy");
                    ws.Cells[i + 2, 5].Value = c.Total;
                    ws.Cells[i + 2, 6].Value = c.AdicionadoPor;
                    ws.Cells[i + 2, 7].Value = c.FechaAdicion.ToString("dd/MM/yyyy");
                    ws.Cells[i + 2, 8].Value = c.ModificadoPor ?? "N/A";
                    ws.Cells[i + 2, 9].Value = c.FechaModificacion?.ToString("dd/MM/yyyy") ?? "N/A";
                }

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte_Compras.xlsx");
            }
        }

        public IActionResult ExportarPagosProveedoresPDF()
        {
            var pagos = _accesoDatos.MostrarPagosProveedor();

            var stream = new MemoryStream();
            var doc = new Document(PageSize.A4.Rotate());
            PdfWriter.GetInstance(doc, stream);
            doc.Open();

            doc.Add(new Paragraph("Reporte de Pagos a Proveedores"));
            doc.Add(new Paragraph("Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm")));
            doc.Add(new Paragraph(" "));

            var table = new PdfPTable(10);
            table.WidthPercentage = 100;

            table.AddCell("Pago ID");
            table.AddCell("Compra ID");
            table.AddCell("Monto");
            table.AddCell("Fecha Pago");
            table.AddCell("Método ID");
            table.AddCell("Método");
            table.AddCell("Adicionado Por");
            table.AddCell("Fecha Adición");
            table.AddCell("Modificado Por");
            table.AddCell("Fecha Modificación");

            foreach (var p in pagos)
            {
                table.AddCell(p.PagoID.ToString());
                table.AddCell(p.CompraID.ToString());
                table.AddCell(p.Monto.ToString("C"));
                table.AddCell(p.FechaPago.ToString("dd/MM/yyyy"));
                table.AddCell(p.MetodoPagoID.ToString());
                table.AddCell(p.MetodoPagoNombre);
                table.AddCell(p.AdicionadoPor);
                table.AddCell(p.FechaAdicion.ToString("dd/MM/yyyy"));
                table.AddCell(p.ModificadoPor ?? "N/A");
                table.AddCell(p.FechaModificacion?.ToString("dd/MM/yyyy") ?? "N/A");
            }

            doc.Add(table);
            doc.Close();

            stream.Position = 0;
            return File(stream, "application/pdf", "Reporte_PagosProveedores.pdf");
        }

        public IActionResult ExportarPagosProveedoresExcel()
        {
            var pagos = _accesoDatos.MostrarPagosProveedor();

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Pagos Proveedores");

                ws.Cells[1, 1].Value = "Pago ID";
                ws.Cells[1, 2].Value = "Compra ID";
                ws.Cells[1, 3].Value = "Monto";
                ws.Cells[1, 4].Value = "Fecha Pago";
                ws.Cells[1, 5].Value = "Método ID";
                ws.Cells[1, 6].Value = "Método";
                ws.Cells[1, 7].Value = "Adicionado Por";
                ws.Cells[1, 8].Value = "Fecha Adición";
                ws.Cells[1, 9].Value = "Modificado Por";
                ws.Cells[1, 10].Value = "Fecha Modificación";

                for (int i = 0; i < pagos.Count; i++)
                {
                    var p = pagos[i];
                    ws.Cells[i + 2, 1].Value = p.PagoID;
                    ws.Cells[i + 2, 2].Value = p.CompraID;
                    ws.Cells[i + 2, 3].Value = p.Monto;
                    ws.Cells[i + 2, 4].Value = p.FechaPago.ToString("dd/MM/yyyy");
                    ws.Cells[i + 2, 5].Value = p.MetodoPagoID;
                    ws.Cells[i + 2, 6].Value = p.MetodoPagoNombre;
                    ws.Cells[i + 2, 7].Value = p.AdicionadoPor;
                    ws.Cells[i + 2, 8].Value = p.FechaAdicion.ToString("dd/MM/yyyy");
                    ws.Cells[i + 2, 9].Value = p.ModificadoPor ?? "N/A";
                    ws.Cells[i + 2, 10].Value = p.FechaModificacion?.ToString("dd/MM/yyyy") ?? "N/A";
                }

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte_PagosProveedores.xlsx");
            }
        }

        public IActionResult ExportarPagosClientesPDF()
        {
            var pagos = _accesoDatos.VerPagoCliente();

            var stream = new MemoryStream();
            var doc = new Document(PageSize.A4.Rotate());
            PdfWriter.GetInstance(doc, stream);
            doc.Open();

            doc.Add(new Paragraph("Reporte de Pagos de Clientes"));
            doc.Add(new Paragraph("Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm")));
            doc.Add(new Paragraph(" "));

            var table = new PdfPTable(10);
            table.WidthPercentage = 100;

            table.AddCell("Pago ID");
            table.AddCell("Venta ID");
            table.AddCell("Cliente");
            table.AddCell("Monto");
            table.AddCell("Fecha Pago");
            table.AddCell("Método de Pago");
            table.AddCell("Adicionado Por");
            table.AddCell("Fecha Adición");
            table.AddCell("Modificado Por");
            table.AddCell("Fecha Modificación");

            foreach (var p in pagos)
            {
                table.AddCell(p.PagoClienteID.ToString());
                table.AddCell(p.VentaID.ToString());
                table.AddCell(p.Cliente);
                table.AddCell(p.Monto.ToString("C"));
                table.AddCell(p.FechaPago.ToString("dd/MM/yyyy"));
                table.AddCell(p.MetodoPagoNombre);
                table.AddCell(p.AdicionadoPor);
                table.AddCell(p.FechaAdicion.ToString("dd/MM/yyyy"));
                table.AddCell(p.ModificadoPor ?? "N/A");
                table.AddCell(p.FechaModificacion?.ToString("dd/MM/yyyy") ?? "N/A");
            }

            doc.Add(table);
            doc.Close();

            stream.Position = 0;
            return File(stream, "application/pdf", "Reporte_PagosClientes.pdf");
        }



        public IActionResult ExportarPagosClientesExcel()
        {
            var pagos = _accesoDatos.VerPagoCliente();

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Pagos Clientes");

                ws.Cells[1, 1].Value = "Pago ID";
                ws.Cells[1, 2].Value = "Venta ID";
                ws.Cells[1, 3].Value = "Cliente";
                ws.Cells[1, 4].Value = "Monto";
                ws.Cells[1, 5].Value = "Fecha Pago";
                ws.Cells[1, 6].Value = "Método de Pago";
                ws.Cells[1, 7].Value = "Adicionado Por";
                ws.Cells[1, 8].Value = "Fecha Adición";
                ws.Cells[1, 9].Value = "Modificado Por";
                ws.Cells[1, 10].Value = "Fecha Modificación";

                for (int i = 0; i < pagos.Count; i++)
                {
                    var p = pagos[i];
                    ws.Cells[i + 2, 1].Value = p.PagoClienteID;
                    ws.Cells[i + 2, 2].Value = p.VentaID;
                    ws.Cells[i + 2, 3].Value = p.Cliente;
                    ws.Cells[i + 2, 4].Value = p.Monto;
                    ws.Cells[i + 2, 5].Value = p.FechaPago.ToString("dd/MM/yyyy");
                    ws.Cells[i + 2, 6].Value = p.MetodoPagoNombre;
                    ws.Cells[i + 2, 7].Value = p.AdicionadoPor;
                    ws.Cells[i + 2, 8].Value = p.FechaAdicion.ToString("dd/MM/yyyy");
                    ws.Cells[i + 2, 9].Value = p.ModificadoPor ?? "N/A";
                    ws.Cells[i + 2, 10].Value = p.FechaModificacion?.ToString("dd/MM/yyyy") ?? "N/A";
                }

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte_PagosClientes.xlsx");
            }
        }



    }
}
