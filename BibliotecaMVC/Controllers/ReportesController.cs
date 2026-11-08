
using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;
using BibliotecaMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ClosedXML.Excel;
using System.IO;

namespace BibliotecaMVC.Controllers
{
    [Authorize]
    public class ReportesController : Controller
    {
        private readonly ILibroService _libroService;
        private readonly ILogger<ReportesController> _logger;

        public ReportesController(
            ILibroService libroService,
            ILogger<ReportesController> logger)
        {
            _libroService = libroService;
            _logger = logger;
        }

        // Reporte de libros en PDF
        public async Task<IActionResult> Reporte()
        {
            _logger.LogInformation("Generando el reporte de libros");

            try
            {
                var libros = await _libroService.ObtenerTodosAsync();

                return File(
                    GenerarReporte(libros),
                    "application/pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Se produjo un error al generar el reporte de libros");

                throw;
            }
        }

        // Descargar reporte de libros en PDF
        public async Task<IActionResult> DescargarReporte()
        {
            _logger.LogInformation("Descargando el reporte de libros");

            try
            {
                var libros = await _libroService.ObtenerTodosAsync();

                return File(
                    GenerarReporte(libros),
                    "application/pdf",
                    "reporte-libros.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Se produjo un error al descargar el reporte de libros");

                throw;
            }
        }

        // Descargar reporte de libros en Excel
        [Authorize(Roles = "Admin, Bibliotecario")]
        public async Task<IActionResult> DescargarExcel()
        {
            _logger.LogInformation("Descargando el reporte de libros en Excel");

            try
            {
                var libros = await _libroService.ObtenerTodosAsync();

                var archivo = GenerarReporteExcel(libros);

                return File(
                    archivo,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "reporte-libros.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Se produjo un error al generar el reporte Excel de libros");

                throw;
            }
        }

        // Generar reporte de libros en Excel
        private static byte[] GenerarReporteExcel(
            IReadOnlyCollection<Libro> libros)
        {
            using var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Libros");

            worksheet.ShowGridLines = false;

            // Encabezado principal
            worksheet.Range("A1:B2").Merge();
            worksheet.Range("C1:D2").Merge();
            worksheet.Range("E1:F2").Merge();

            worksheet.Cell("A1").Value = "BIBLIOTECA UNIVERSITARIA";
            worksheet.Cell("C1").Value = "REPORTE DE LIBROS";
            worksheet.Cell("E1").Value =
                $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";

            var encabezadoPrincipal = worksheet.Range("A1:F2");

            encabezadoPrincipal.Style.Fill.BackgroundColor =
                XLColor.FromHtml("#17365D");

            encabezadoPrincipal.Style.Font.FontColor = XLColor.White;
            encabezadoPrincipal.Style.Font.Bold = true;

            encabezadoPrincipal.Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            encabezadoPrincipal.Style.Alignment.Vertical =
                XLAlignmentVerticalValues.Center;

            encabezadoPrincipal.Style.Alignment.WrapText = true;

            worksheet.Cell("A1").Style.Font.FontSize = 13;
            worksheet.Cell("C1").Style.Font.FontSize = 13;
            worksheet.Cell("E1").Style.Font.FontSize = 10;

            worksheet.Row(1).Height = 30;
            worksheet.Row(2).Height = 20;

            // Subtítulo
            worksheet.Range("A3:F3").Merge();

            worksheet.Cell("A3").Value =
                "Listado de libros registrados";

            worksheet.Cell("A3").Style.Font.Italic = true;
            worksheet.Cell("A3").Style.Font.FontSize = 11;
            worksheet.Cell("A3").Style.Font.FontColor = XLColor.DarkGray;

            worksheet.Cell("A3").Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            worksheet.Row(3).Height = 25;
            worksheet.Row(4).Height = 10;

            // Encabezados de la tabla
            worksheet.Cell("A5").Value = "ID";
            worksheet.Cell("B5").Value = "TÍTULO";
            worksheet.Cell("C5").Value = "AUTOR";
            worksheet.Cell("D5").Value = "CATEGORÍA";
            worksheet.Cell("E5").Value = "PRECIO";
            worksheet.Cell("F5").Value = "DISPONIBLE";

            var encabezadoTabla = worksheet.Range("A5:F5");

            encabezadoTabla.Style.Font.Bold = true;
            encabezadoTabla.Style.Font.FontColor = XLColor.White;

            encabezadoTabla.Style.Fill.BackgroundColor =
                XLColor.FromHtml("#4472C4");

            encabezadoTabla.Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            encabezadoTabla.Style.Alignment.Vertical =
                XLAlignmentVerticalValues.Center;

            encabezadoTabla.Style.Border.OutsideBorder =
                XLBorderStyleValues.Thin;

            worksheet.Row(5).Height = 25;

            // Registrar información de los libros
            int fila = 6;

            foreach (var libro in libros)
            {
                worksheet.Cell(fila, 1).Value = libro.Id;
                worksheet.Cell(fila, 2).Value = libro.Titulo;
                worksheet.Cell(fila, 3).Value = libro.Autor;
                worksheet.Cell(fila, 4).Value = libro.Categoria;
                worksheet.Cell(fila, 5).Value = libro.Precio;
                worksheet.Cell(fila, 6).Value =
                    libro.Disponible ? "Sí" : "No";

                worksheet.Cell(fila, 5).Style.NumberFormat.Format =
                    "$#,##0.00";

                // Filas alternadas
                if ((fila - 6) % 2 == 1)
                {
                    worksheet.Range(fila, 1, fila, 6)
                        .Style.Fill.BackgroundColor =
                        XLColor.FromHtml("#E7E6E6");
                }

                fila++;
            }

            // Bordes de la tabla
            int ultimaFila = Math.Max(fila - 1, 5);

            var tabla = worksheet.Range(5, 1, ultimaFila, 6);

            tabla.Style.Border.OutsideBorder =
                XLBorderStyleValues.Thin;

            tabla.Style.Border.InsideBorder =
                XLBorderStyleValues.Thin;

            tabla.Style.Border.OutsideBorderColor = XLColor.LightGray;
            tabla.Style.Border.InsideBorderColor = XLColor.LightGray;

            // Alineación de columnas
            worksheet.Column(1).Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            worksheet.Column(5).Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            worksheet.Column(6).Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            // Ancho de columnas
            worksheet.Column(1).Width = 12;
            worksheet.Column(2).Width = 34;
            worksheet.Column(3).Width = 28;
            worksheet.Column(4).Width = 22;
            worksheet.Column(5).Width = 15;
            worksheet.Column(6).Width = 18;

            // Configuración de la hoja
            worksheet.SheetView.FreezeRows(5);

            worksheet.Range(5, 1, ultimaFila, 6).SetAutoFilter();

            // Configuración de impresión
            worksheet.PageSetup.PageOrientation =
                XLPageOrientation.Landscape;

            worksheet.PageSetup.PaperSize =
                XLPaperSize.LetterPaper;

            worksheet.PageSetup.PagesWide = 1;
            worksheet.PageSetup.PagesTall = 0;

            // Guardar archivo en memoria
            using var stream = new MemoryStream();

            workbook.SaveAs(stream);

            return stream.ToArray();
        }

        // Generar documento PDF
        private static byte[] GenerarReporte(
            IReadOnlyCollection<Libro> libros)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(document => document.Page(page =>
            {
                page.Size(612, 792, Unit.Point);
                page.Margin(42);
                page.PageColor(Colors.Grey.Lighten5);

                page.Header().Column(column =>
                {
                    column.Item().Text("BIBLIOTECA UNIVERSITARIA")
                        .FontSize(11)
                        .Bold()
                        .FontColor(Colors.Blue.Darken2);

                    column.Item().Text("Reporte de libros")
                        .FontSize(26)
                        .Bold()
                        .FontColor(Colors.Grey.Darken3);

                    column.Item().Text(
                        $"Listado actualizado el {DateTime.Now:dd/MM/yyyy HH:mm}")
                        .FontSize(10)
                        .FontColor(Colors.Grey.Darken1);

                    column.Item().PaddingTop(14)
                        .LineHorizontal(2)
                        .LineColor(Colors.Blue.Medium);
                });

                page.Content().PaddingVertical(22).Column(column =>
                {
                    column.Item()
                        .Background(Colors.Blue.Darken2)
                        .Padding(14)
                        .Row(row =>
                        {
                            row.RelativeItem().Text("Libros registrados")
                                .FontSize(11)
                                .Bold()
                                .FontColor(Colors.White);

                            row.ConstantItem(45).AlignRight()
                                .Text(libros.Count.ToString())
                                .FontSize(18)
                                .Bold()
                                .FontColor(Colors.White);
                        });

                    column.Item().PaddingTop(18).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(35);
                            columns.RelativeColumn(2.5f);
                            columns.RelativeColumn(1.8f);
                            columns.RelativeColumn(1.5f);
                            columns.ConstantColumn(55);
                            columns.ConstantColumn(55);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderCellStyle).Text("ID");
                            header.Cell().Element(HeaderCellStyle).Text("TÍTULO");
                            header.Cell().Element(HeaderCellStyle).Text("AUTOR");
                            header.Cell().Element(HeaderCellStyle).Text("CATEGORÍA");
                            header.Cell().Element(HeaderCellStyle).Text("PRECIO");
                            header.Cell().Element(HeaderCellStyle).Text("DISP.");
                        });

                        foreach (var libro in libros)
                        {
                            table.Cell().Element(BodyCellStyle)
                                .Text(libro.Id.ToString());

                            table.Cell().Element(BodyCellStyle)
                                .Text(libro.Titulo ?? "");

                            table.Cell().Element(BodyCellStyle)
                                .Text(libro.Autor ?? "");

                            table.Cell().Element(BodyCellStyle)
                                .Text(libro.Categoria ?? "");

                            table.Cell().Element(BodyCellStyle)
                                .Text($"${libro.Precio:N2}");

                            table.Cell().Element(BodyCellStyle)
                                .Text(libro.Disponible ? "Sí" : "No");
                        }
                    });
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Biblioteca Universitaria - Página ");
                    text.CurrentPageNumber();
                    text.Span(" de ");
                    text.TotalPages();
                });

            })).GeneratePdf();

            static IContainer HeaderCellStyle(IContainer container) =>
                container
                    .Background(Colors.Blue.Darken2)
                    .PaddingVertical(9)
                    .PaddingHorizontal(8)
                    .DefaultTextStyle(text => text.FontSize(9)
                        .Bold()
                        .FontColor(Colors.White));

            static IContainer BodyCellStyle(IContainer container) =>
                container
                    .Background(Colors.White)
                    .BorderBottom(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .PaddingVertical(9)
                    .PaddingHorizontal(8)
                    .DefaultTextStyle(text => text.FontSize(8)
                        .FontColor(Colors.Grey.Darken3));
        }
    }
}
