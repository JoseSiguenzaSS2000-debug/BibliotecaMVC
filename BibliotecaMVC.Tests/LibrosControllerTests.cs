
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BibliotecaMVC.Controllers;
using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using BibliotecaMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;


namespace BibliotecaMVC.Tests;

public class LibrosControllerTests
{
    [Fact]
    // Comprueba que Index devuelva todos los libros guardados.
    public async Task Index_DevuelveLaListaDeLibros()
    {
        // Crea un contexto EF Core con una base de datos en memoria única.
        await using var context = CreateContext();

        // Inserta dos libros de prueba en el contexto.
        context.Libros.AddRange(
            new Libro
            {
                Titulo = "Cien años de soledad",
                Autor = "Gabriel García Márquez",
                Categoria = "Novela",
                Precio = 25.00,
                Disponible = true
            },
            new Libro
            {
                Titulo = "El principito",
                Autor = "Antoine de Saint-Exupéry",
                Categoria = "Ficción",
                Precio = 15.00,
                Disponible = false
            });

        // Persiste los libros en la base de datos en memoria.
        await context.SaveChangesAsync();

        // Crea el controlador usando el contexto y un logger simulado.
        var controller = CreateController(context);

        // Ejecuta la acción Index del controlador.
        var result = await controller.Index();

        // Comprueba que el resultado sea una vista.
        var view = Assert.IsType<ViewResult>(result);

        // Comprueba que el modelo de la vista no sea nulo.
        Assert.NotNull(view.Model);

        // Obtiene el modelo de la vista como una colección de libros.
        var libros = Assert.IsAssignableFrom<IEnumerable<Libro>>(view.Model);

        // Comprueba que la vista contenga exactamente dos libros.
        Assert.Equal(2, libros.Count());

        // Comprueba que el primer libro tenga el título esperado.
        Assert.Equal("Cien años de soledad", libros.First().Titulo);

        // Comprueba que el primer libro esté disponible.
        Assert.True(libros.First().Disponible);

        // Comprueba que el segundo libro tenga el título esperado.
        Assert.Equal("El principito", libros.Last().Titulo);

        // Comprueba que el segundo libro no esté disponible.
        Assert.False(libros.Last().Disponible);
    }

    [Fact]
    // Comprueba que Index devuelva una lista vacía cuando no existen libros.
    public async Task Index_SinLibros_DevuelveListaVacia()
    {
        // Crea un contexto con una base de datos en memoria vacía.
        await using var context = CreateContext();

        // Crea el controlador de prueba.
        var controller = CreateController(context);

        // Ejecuta la acción Index.
        var result = await controller.Index();

        // Comprueba que el resultado sea una vista.
        var view = Assert.IsType<ViewResult>(result);

        // Comprueba que el modelo de la vista no sea nulo.
        Assert.NotNull(view.Model);

        // Obtiene el modelo como una colección de libros.
        var libros = Assert.IsAssignableFrom<IEnumerable<Libro>>(view.Model);

        // Comprueba que la colección no contenga libros.
        Assert.Empty(libros);
    }

    [Fact]
    // Comprueba que Details devuelva un libro existente.
    public async Task Details_ConLibroExistente_DevuelveElLibro()
    {
        // Crea una base de datos en memoria aislada.
        await using var context = CreateContext();

        // Crea un libro de prueba.
        var libro = new Libro
        {
            Titulo = "Don Quijote de la Mancha",
            Autor = "Miguel de Cervantes",
            Categoria = "Clásico",
            Precio = 30.00,
            Disponible = true
        };

        // Añade el libro al contexto.
        context.Libros.Add(libro);

        // Guarda el libro y genera su identificador.
        await context.SaveChangesAsync();

        // Crea el controlador de prueba.
        var controller = CreateController(context);

        // Solicita los detalles usando el identificador generado.
        var result = await controller.Details(libro.Id);

        // Comprueba que la respuesta sea una vista.
        var view = Assert.IsType<ViewResult>(result);

        // Comprueba que el modelo de la vista no sea nulo.
        Assert.NotNull(view.Model);

        // Comprueba que la vista reciba exactamente el libro guardado.
        Assert.Same(libro, view.Model);

        // Comprueba que el título del libro sea el esperado.
        Assert.Equal("Don Quijote de la Mancha", libro.Titulo);
    }

    [Fact]
    // Comprueba la respuesta cuando el libro no existe.
    public async Task Details_ConLibroInexistente_DevuelveNotFound()
    {
        // Crea un contexto vacío.
        await using var context = CreateContext();

        // Crea el controlador de prueba.
        var controller = CreateController(context);

        // Solicita los detalles de un identificador inexistente.
        var result = await controller.Details(999);

        // Comprueba que el controlador responda con NotFound.
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    // Comprueba que Create guarde un libro válido y redirija al listado.
    public async Task Create_ConLibroValido_GuardaElLibroYRedirigeAlIndice()
    {
        // Crea una base de datos en memoria vacía.
        await using var context = CreateContext();

        // Crea el controlador de prueba.
        var controller = CreateController(context);

        // Construye el libro válido que se enviará a la acción.
        var libro = new Libro
        {
            Titulo = "La ciudad y los perros",
            Autor = "Mario Vargas Llosa",
            Categoria = "Novela",
            Precio = 28.00,
            Disponible = true
        };

        // Ejecuta la acción Create con el libro.
        var result = await controller.Create(libro);

        // Comprueba que la acción devuelva una redirección.
        var redirect = Assert.IsType<RedirectToActionResult>(result);

        // Comprueba que la redirección apunte a Index.
        Assert.Equal(nameof(LibrosController.Index), redirect.ActionName);

        // Comprueba que se haya guardado un único libro.
        Assert.Equal(1, await context.Libros.CountAsync());

        // Obtiene el libro guardado.
        var libroGuardado = await context.Libros.SingleAsync();

        // Comprueba que el libro guardado no sea nulo.
        Assert.NotNull(libroGuardado);

        // Comprueba que el título sea el esperado.
        Assert.Equal("La ciudad y los perros", libroGuardado.Titulo);

        // Comprueba que el autor sea el esperado.
        Assert.Equal("Mario Vargas Llosa", libroGuardado.Autor);

        // Comprueba que la categoría sea la esperada.
        Assert.Equal("Novela", libroGuardado.Categoria);

        // Comprueba que el precio sea el esperado.
        Assert.Equal(28.00, libroGuardado.Precio);

        // Comprueba que el libro esté disponible.
        Assert.True(libroGuardado.Disponible);
    }

    [Fact]
    // Comprueba que Create no guarde datos cuando el modelo es inválido.
    public async Task Create_ConModeloInvalido_NoGuardaYDevuelveLaVista()
    {
        // Crea una base de datos en memoria vacía.
        await using var context = CreateContext();

        // Crea el controlador de prueba.
        var controller = CreateController(context);

        // Simula un error de validación del autor.
        controller.ModelState.AddModelError(
            nameof(Libro.Autor),
            "El autor es obligatorio.");

        // Crea el libro que se enviará con el modelo inválido.
        var libro = new Libro
        {
            Titulo = "Libro de prueba",
            Autor = "",
            Categoria = "Novela",
            Precio = 20.00,
            Disponible = true
        };

        // Ejecuta la acción Create.
        var result = await controller.Create(libro);

        // Comprueba que se devuelva nuevamente la vista.
        var view = Assert.IsType<ViewResult>(result);

        // Comprueba que la vista conserve el modelo enviado.
        Assert.Same(libro, view.Model);

        // Comprueba que no se haya guardado ningún libro.
        Assert.Empty(context.Libros);

        // Comprueba que el modelo continúe siendo inválido.
        Assert.False(controller.ModelState.IsValid);
    }

    [Fact]
    // Comprueba que Edit actualice los datos de un libro existente.
    public async Task Edit_ConLibroValido_ActualizaElLibro()
    {
        // Genera un nombre único para la base de datos de esta prueba.
        var databaseName = Guid.NewGuid().ToString();

        // Declara la variable donde se almacenará el identificador del libro.
        int libroId;

        // Crea un contexto separado para preparar los datos iniciales.
        await using (var seedContext = CreateContext(databaseName))
        {
            // Crea el libro original.
            var libro = new Libro
            {
                Titulo = "Título original",
                Autor = "Autor original",
                Categoria = "Novela",
                Precio = 20.00,
                Disponible = true
            };

            // Añade el libro al contexto inicial.
            seedContext.Libros.Add(libro);

            // Guarda el libro en la base de datos en memoria.
            await seedContext.SaveChangesAsync();

            // Conserva el identificador generado.
            libroId = libro.Id;
        }

        // Crea otro contexto para simular una nueva petición HTTP.
        await using var context = CreateContext(databaseName);

        // Crea el controlador con el contexto de la petición.
        var controller = CreateController(context);

        // Crea la versión modificada del libro.
        var libroEditado = new Libro
        {
            // Mantiene el identificador original.
            Id = libroId,

            // Define el nuevo título.
            Titulo = "Título actualizado",

            // Define el nuevo autor.
            Autor = "Autor actualizado",

            // Mantiene la categoría.
            Categoria = "Novela",

            // Define el nuevo precio.
            Precio = 35.00,

            // Mantiene la disponibilidad.
            Disponible = true
        };

        // Ejecuta la acción Edit.
        var result = await controller.Edit(libroId, libroEditado);

        // Comprueba que la acción redirija después de actualizar.
        var redirect = Assert.IsType<RedirectToActionResult>(result);

        // Comprueba que la redirección vaya al listado.
        Assert.Equal(nameof(LibrosController.Index), redirect.ActionName);

        // Obtiene el libro sin seguimiento para comprobar el valor persistido.
        var libroGuardado = await context.Libros
            .AsNoTracking()
            .SingleAsync();

        // Comprueba que el libro actualizado no sea nulo.
        Assert.NotNull(libroGuardado);

        // Comprueba que el título se haya actualizado.
        Assert.Equal("Título actualizado", libroGuardado.Titulo);

        // Comprueba que el autor se haya actualizado.
        Assert.Equal("Autor actualizado", libroGuardado.Autor);

        // Comprueba que el precio se haya actualizado.
        Assert.Equal(35.00, libroGuardado.Precio);

        // Comprueba que la disponibilidad siga siendo verdadera.
        Assert.True(libroGuardado.Disponible);
    }

    [Fact]
    // Comprueba que Edit rechace un identificador distinto al del modelo.
    public async Task Edit_ConIdentificadorDiferente_DevuelveBadRequest()
    {
        // Crea un contexto en memoria.
        await using var context = CreateContext();

        // Crea el controlador de prueba.
        var controller = CreateController(context);

        // Crea un libro cuyo Id no coincide con el parámetro de la acción.
        var libro = new Libro
        {
            Id = 1,
            Titulo = "Libro de prueba",
            Autor = "Autor de prueba"
        };

        // Ejecuta Edit con dos identificadores diferentes.
        var result = await controller.Edit(2, libro);

        // Comprueba que la respuesta sea BadRequest.
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    // Comprueba que Edit devuelva NotFound si el libro no existe.
    public async Task Edit_ConLibroInexistente_DevuelveNotFound()
    {
        // Crea un contexto vacío.
        await using var context = CreateContext();

        // Crea el controlador de prueba.
        var controller = CreateController(context);

        // Construye un libro con un identificador inexistente.
        var libro = new Libro
        {
            Id = 999,
            Titulo = "Libro inexistente",
            Autor = "Autor de prueba"
        };

        // Ejecuta Edit con el identificador inexistente.
        var result = await controller.Edit(999, libro);

        // Comprueba que la respuesta sea NotFound.
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    // Comprueba que Edit devuelva la vista cuando el modelo es inválido.
    public async Task Edit_ConModeloInvalido_DevuelveLaVista()
    {
        // Crea un contexto en memoria.
        await using var context = CreateContext();

        // Crea el controlador de prueba.
        var controller = CreateController(context);

        // Crea un libro con datos inválidos.
        var libro = new Libro
        {
            Id = 1,
            Titulo = "Libro de prueba",
            Autor = ""
        };

        // Agrega un error de validación al modelo.
        controller.ModelState.AddModelError(
            nameof(Libro.Autor),
            "El autor es obligatorio.");

        // Ejecuta Edit con el modelo inválido.
        var result = await controller.Edit(1, libro);

        // Comprueba que el resultado sea una vista.
        var view = Assert.IsType<ViewResult>(result);

        // Comprueba que la vista conserve el libro enviado.
        Assert.Same(libro, view.Model);

        // Comprueba que el modelo no sea válido.
        Assert.False(controller.ModelState.IsValid);
    }

    [Fact]
    // Comprueba que Delete elimine un libro existente y redirija al listado.
    public async Task Delete_ConLibroExistente_EliminaElLibroYRedirigeAlIndice()
    {
        // Crea una base de datos en memoria.
        await using var context = CreateContext();

        // Crea el libro que se eliminará.
        var libro = new Libro
        {
            Titulo = "Libro para eliminar",
            Autor = "Autor de prueba",
            Categoria = "Novela",
            Precio = 20.00,
            Disponible = true
        };

        // Añade el libro al contexto.
        context.Libros.Add(libro);

        // Persiste el libro y genera su identificador.
        await context.SaveChangesAsync();

        // Crea el controlador de prueba.
        var controller = CreateController(context);

        // Ejecuta Delete con el identificador del libro.
        var result = await controller.Delete(libro.Id);

        // Comprueba que la acción redirija después de eliminar.
        var redirect = Assert.IsType<RedirectToActionResult>(result);

        // Comprueba que la redirección apunte a Index.
        Assert.Equal(nameof(LibrosController.Index), redirect.ActionName);

        // Comprueba que ya no queden libros en la base de datos.
        Assert.Empty(context.Libros);

        // Comprueba que el total de libros sea cero.
        Assert.Equal(0, await context.Libros.CountAsync());
    }

    [Fact]
    // Comprueba que Delete devuelva NotFound si el libro no existe.
    public async Task Delete_ConLibroInexistente_DevuelveNotFound()
    {
        // Crea un contexto vacío.
        await using var context = CreateContext();

        // Crea el controlador de prueba.
        var controller = CreateController(context);

        // Ejecuta Delete con un identificador inexistente.
        var result = await controller.Delete(999);

        // Comprueba que la respuesta sea NotFound.
        Assert.IsType<NotFoundResult>(result);

        // Comprueba que la base de datos continúe vacía.
        Assert.Empty(context.Libros);
    }

    // Crea un controlador usando el servicio indicado y un logger simulado.
    private static LibrosController CreateController(BibliotecaContext context)
    {
        // Crea un mock para no depender de un sistema de logs real.
        var logger = new Mock<ILogger<LibrosController>>();

        // Crea el servicio utilizando el contexto de base de datos en memoria.
        var service = new LibroService(context);

        // Devuelve una instancia del controlador preparada para la prueba.
        return new LibrosController(service, logger.Object);
    }

    // Crea un contexto conectado a una base de datos EF Core en memoria.
    private static BibliotecaContext CreateContext(string? databaseName = null)
    {
        // Configura las opciones del contexto.
        var options = new DbContextOptionsBuilder<BibliotecaContext>()
            // Usa el nombre recibido o genera uno nuevo para aislar la prueba.
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            // Finaliza la construcción de opciones.
            .Options;

        // Devuelve el contexto configurado.
        return new BibliotecaContext(options);
    }
}
