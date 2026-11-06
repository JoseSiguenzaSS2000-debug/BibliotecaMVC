

using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using BibliotecaMVC.Models;


namespace BibliotecaMVC.Tests;


public class LibroValidationTests
{
    [Fact]
    // Comprueba que el autor del libro sea obligatorio.
    public void AutorEsObligatorio()
    {
        // Crea un libro con el autor vacío para provocar la validación requerida.
        var libro = new Libro
        {
            Titulo = "Libro de prueba",
            Autor = string.Empty
        };

        // Ejecuta las validaciones definidas mediante atributos en Libro.
        var errores = Validate(libro);

        // Comprueba que exista al menos un error de validación.
        Assert.NotEmpty(errores);

        // Comprueba que exista un error asociado a la propiedad Autor.
        Assert.Contains(errores, error =>
            error.MemberNames.Contains(nameof(Libro.Autor)));
    }

    [Fact]
    // Comprueba que el autor no supere los 100 caracteres.
    public void AutorNoPuedeSuperarLos100Caracteres()
    {
        // Crea un autor con 101 caracteres para comprobar el límite máximo.
        var libro = new Libro
        {
            Titulo = "Libro de prueba",
            Autor = new string('A', 101)
        };

        // Ejecuta las validaciones del modelo.
        var errores = Validate(libro);

        // Comprueba que exista un error de validación.
        Assert.NotEmpty(errores);

        // Comprueba que el error corresponda a la propiedad Autor.
        Assert.Contains(errores, error =>
            error.MemberNames.Contains(nameof(Libro.Autor)));
    }

    [Fact]
    // Comprueba que el límite exacto de 100 caracteres sea válido.
    public void AutorCon100CaracteresEsValido()
    {
        // Crea un autor con exactamente 100 caracteres.
        var libro = new Libro
        {
            Titulo = "Libro de prueba",
            Autor = new string('A', 100)
        };

        // Ejecuta las validaciones del modelo.
        var errores = Validate(libro);

        // Comprueba que no haya errores para la propiedad Autor.
        Assert.DoesNotContain(errores, error =>
            error.MemberNames.Contains(nameof(Libro.Autor)));

        // Comprueba que no existan otros errores de validación.
        Assert.Empty(errores);
    }

    [Fact]
    // Comprueba que el autor acepte caracteres especiales y tildes.
    public void AutorConCaracteresEspecialesEsValido()
    {
        // Crea un libro cuyo autor contiene tildes y varios nombres.
        var libro = new Libro
        {
            Titulo = "Libro de prueba",
            Autor = "José María de la Cruz"
        };

        // Ejecuta las validaciones del modelo.
        var errores = Validate(libro);

        // Comprueba que el autor no tenga ningún error de validación.
        Assert.Empty(errores);
    }

    // Método auxiliar que concentra la ejecución de las validaciones del modelo.
    private static IReadOnlyCollection<ValidationResult> Validate(Libro libro)
    {
        // Crea el contexto de validación para la instancia recibida.
        var context = new ValidationContext(libro);

        // Prepara la colección donde se almacenarán los errores encontrados.
        var errores = new List<ValidationResult>();

        // Ejecuta todas las validaciones definidas mediante atributos.
        Validator.TryValidateObject(
            libro,
            context,
            errores,
            validateAllProperties: true);

        // Devuelve los errores para que cada prueba pueda analizarlos.
        return errores;
    }
}
