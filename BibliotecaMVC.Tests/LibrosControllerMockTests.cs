
using System.Collections.Generic;
using System.Threading.Tasks;
using BibliotecaMVC.Controllers;
using BibliotecaMVC.Models;
using BibliotecaMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;


namespace BibliotecaMVC.Tests;

// Agrupa las pruebas del controlador de libros utilizando mocks.
public class LibrosControllerMockTests
{
    // Crea un mock del servicio de libros.
    private readonly Mock<ILibroService> _mockService;

    // Crea un mock del logger del controlador.
    private readonly Mock<ILogger<LibrosController>> _mockLogger;

    // Constructor que inicializa los objetos simulados.
    public LibrosControllerMockTests()
    {
        // Inicializa el mock de ILibroService.
        _mockService = new Mock<ILibroService>();

        // Inicializa el mock del logger.
        _mockLogger = new Mock<ILogger<LibrosController>>();
    }

    // Crea una instancia del controlador utilizando las dependencias simuladas.
    private LibrosController CreateController()
    {
        // Devuelve el controlador con los mocks configurados.
        return new LibrosController(
            _mockService.Object,
            _mockLogger.Object);
    }

    [Fact]
    // Comprueba que Index devuelva los libros proporcionados por el servicio.
    public async Task Index_DevuelveListaDeLibros()
    {
        // Crea una lista de libros simulados.
        var libros = new List<Libro>
        {
            new Libro
            {
                Id = 1,
                Titulo = "Cien años de soledad",
                Autor = "Gabriel García Márquez",
                Categoria = "Novela",
                Precio = 25.00,
                Disponible = true
            },
            new Libro
            {
                Id = 2,
                Titulo = "El principito",
                Autor = "Antoine de Saint-Exupéry",
                Categoria = "Ficción",
                Precio = 15.00,
                Disponible = false
            }
        };

        // Configura el mock para devolver la lista de libros.
        _mockService
            .Setup(s => s.ObtenerTodosAsync())
            .ReturnsAsync(libros);

        // Crea el controlador de prueba.
        var controller = CreateController();

        // Ejecuta la acción Index.
        var result = await controller.Index();

        // Comprueba que el resultado sea una vista.
        var view = Assert.IsType<ViewResult>(result);

        // Comprueba que el modelo sea una colección de libros.
        var modelo = Assert.IsAssignableFrom<IEnumerable<Libro>>(view.Model);

        // Comprueba que se devuelvan dos libros.
        Assert.Equal(2, System.Linq.Enumerable.Count(modelo));

        // Verifica que el servicio haya sido llamado una vez.
        _mockService.Verify(
            s => s.ObtenerTodosAsync(),
            Times.Once);
    }

    [Fact]
    // Comprueba que Index devuelva una lista vacía cuando el servicio no encuentra libros.
    public async Task Index_SinLibros_DevuelveListaVacia()
    {
        // Configura el servicio para devolver una lista vacía.
        _mockService
            .Setup(s => s.ObtenerTodosAsync())
            .ReturnsAsync(new List<Libro>());

        // Crea el controlador de prueba.
        var controller = CreateController();

        // Ejecuta la acción Index.
        var result = await controller.Index();

        // Comprueba que el resultado sea una vista.
        var view = Assert.IsType<ViewResult>(result);

        // Comprueba que la lista esté vacía.
        var libros = Assert.IsAssignableFrom<IEnumerable<Libro>>(view.Model);
        Assert.Empty(libros);

        // Verifica que el servicio haya sido llamado una vez.
        _mockService.Verify(
            s => s.ObtenerTodosAsync(),
            Times.Once);
    }

    [Fact]
    // Comprueba que Details devuelva un libro cuando existe.
    public async Task Details_ConLibroExistente_DevuelveElLibro()
    {
        // Crea un libro simulado.
        var libro = new Libro
        {
            Id = 1,
            Titulo = "Don Quijote de la Mancha",
            Autor = "Miguel de Cervantes",
            Categoria = "Clásico",
            Precio = 30.00,
            Disponible = true
        };

        // Configura el servicio para devolver el libro cuando se solicite su ID.
        _mockService
            .Setup(s => s.ObtenerPorIdAsync(1))
            .ReturnsAsync(libro);

        // Crea el controlador.
        var controller = CreateController();

        // Ejecuta Details con el identificador del libro.
        var result = await controller.Details(1);

        // Comprueba que el resultado sea una vista.
        var view = Assert.IsType<ViewResult>(result);

        // Comprueba que la vista reciba el libro esperado.
        Assert.Same(libro, view.Model);

        // Verifica que el servicio haya sido llamado con el ID correcto.
        _mockService.Verify(
            s => s.ObtenerPorIdAsync(1),
            Times.Once);
    }

    [Fact]
    // Comprueba que Details devuelva NotFound cuando el libro no existe.
    public async Task Details_ConLibroInexistente_DevuelveNotFound()
    {
        // Configura el servicio para devolver null.
        _mockService
            .Setup(s => s.ObtenerPorIdAsync(999))
            .ReturnsAsync((Libro?)null);

        // Crea el controlador.
        var controller = CreateController();

        // Ejecuta Details con un ID inexistente.
        var result = await controller.Details(999);

        // Comprueba que el resultado sea NotFound.
        Assert.IsType<NotFoundResult>(result);

        // Verifica que el servicio haya sido llamado una vez.
        _mockService.Verify(
            s => s.ObtenerPorIdAsync(999),
            Times.Once);
    }

    [Fact]
    // Comprueba que Create registre un libro válido utilizando el servicio.
    public async Task Create_ConLibroValido_RegistraElLibro()
    {
        // Crea el libro que se enviará al controlador.
        var libro = new Libro
        {
            Id = 1,
            Titulo = "La ciudad y los perros",
            Autor = "Mario Vargas Llosa",
            Categoria = "Novela",
            Precio = 28.00,
            Disponible = true
        };

        // Configura el servicio para completar el registro correctamente.
        _mockService
            .Setup(s => s.RegistrarAsync(It.IsAny<Libro>()))
            .Returns(Task.CompletedTask);

        // Crea el controlador.
        var controller = CreateController();

        // Ejecuta Create con el libro.
        var result = await controller.Create(libro);

        // Comprueba que el resultado sea una redirección.
        var redirect = Assert.IsType<RedirectToActionResult>(result);

        // Comprueba que la redirección apunte a Index.
        Assert.Equal(nameof(LibrosController.Index), redirect.ActionName);

        // Verifica que el servicio haya recibido el libro.
        _mockService.Verify(
            s => s.RegistrarAsync(It.Is<Libro>(
                l => l.Titulo == "La ciudad y los perros"
                && l.Autor == "Mario Vargas Llosa")),
            Times.Once);
    }

    [Fact]
    // Comprueba que Edit actualice un libro existente.
    public async Task Edit_ConLibroExistente_ActualizaElLibro()
    {
        // Crea el libro que se desea actualizar.
        var libro = new Libro
        {
            Id = 1,
            Titulo = "Título actualizado",
            Autor = "Autor actualizado",
            Categoria = "Novela",
            Precio = 35.00,
            Disponible = true
        };

        // Configura el servicio para indicar que la actualización fue exitosa.
        _mockService
            .Setup(s => s.ActualizarAsync(It.IsAny<Libro>()))
            .ReturnsAsync(true);

        // Crea el controlador.
        var controller = CreateController();

        // Ejecuta Edit con el identificador y el libro.
        var result = await controller.Edit(1, libro);

        // Comprueba que el resultado sea una redirección.
        var redirect = Assert.IsType<RedirectToActionResult>(result);

        // Comprueba que la redirección apunte a Index.
        Assert.Equal(nameof(LibrosController.Index), redirect.ActionName);

        // Verifica que el servicio haya recibido el libro con el ID correcto.
        _mockService.Verify(
            s => s.ActualizarAsync(It.Is<Libro>(
                l => l.Id == 1
                && l.Titulo == "Título actualizado")),
            Times.Once);
    }

    [Fact]
    // Comprueba que Edit devuelva NotFound cuando el servicio no encuentra el libro.
    public async Task Edit_ConLibroInexistente_DevuelveNotFound()
    {
        // Crea el libro que se intentará actualizar.
        var libro = new Libro
        {
            Id = 999,
            Titulo = "Libro inexistente",
            Autor = "Autor de prueba"
        };

        // Configura el servicio para indicar que no se pudo actualizar.
        _mockService
            .Setup(s => s.ActualizarAsync(It.IsAny<Libro>()))
            .ReturnsAsync(false);

        // Crea el controlador.
        var controller = CreateController();

        // Ejecuta Edit con un identificador inexistente.
        var result = await controller.Edit(999, libro);

        // Comprueba que el resultado sea NotFound.
        Assert.IsType<NotFoundResult>(result);

        // Verifica que el servicio haya sido llamado una vez.
        _mockService.Verify(
            s => s.ActualizarAsync(It.IsAny<Libro>()),
            Times.Once);
    }

    [Fact]
    // Comprueba que Delete elimine un libro existente.
    public async Task Delete_ConLibroExistente_EliminaElLibro()
    {
        // Configura el servicio para indicar que la eliminación fue exitosa.
        _mockService
            .Setup(s => s.EliminarAsync(1))
            .ReturnsAsync(true);

        // Crea el controlador.
        var controller = CreateController();

        // Ejecuta Delete con el ID del libro.
        var result = await controller.Delete(1);

        // Comprueba que el resultado sea una redirección.
        var redirect = Assert.IsType<RedirectToActionResult>(result);

        // Comprueba que la redirección apunte a Index.
        Assert.Equal(nameof(LibrosController.Index), redirect.ActionName);

        // Verifica que el servicio haya recibido el ID correcto.
        _mockService.Verify(
            s => s.EliminarAsync(1),
            Times.Once);
    }

    [Fact]
    // Comprueba que Delete devuelva NotFound cuando el libro no existe.
    public async Task Delete_ConLibroInexistente_DevuelveNotFound()
    {
        // Configura el servicio para indicar que no existe el libro.
        _mockService
            .Setup(s => s.EliminarAsync(999))
            .ReturnsAsync(false);

        // Crea el controlador.
        var controller = CreateController();

        // Ejecuta Delete con un identificador inexistente.
        var result = await controller.Delete(999);

        // Comprueba que el resultado sea NotFound.
        Assert.IsType<NotFoundResult>(result);

        // Verifica que el servicio haya sido llamado una vez.
        _mockService.Verify(
            s => s.EliminarAsync(999),
            Times.Once);
    }
}
