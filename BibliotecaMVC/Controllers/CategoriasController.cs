using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BibliotecaMVC.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly string _connectionstring;

        public CategoriasController (IConfiguration configuration)
        {
            _connectionstring = configuration.GetConnectionString("BibliotecaDB");
        }
        public IActionResult Index()
        {
            var categorias = new List<Categoria>();

            using(var conexion = new SqlConnection(_connectionstring))
            {
                var sql = "SELECT ID, Nombre, Descripcion FROM Categorias";
                using (var comando = new SqlCommand(sql, conexion))
                {
                    conexion.Open();
                    using(var lector = comando.ExecuteReader())
                    {
                        while(lector.Read())
                        {
                            categorias.Add(new Categoria
                            {
                                ID = lector.GetInt32(0),
                                Nombre = lector.GetString(1),
                                Descripcion = lector.IsDBNull(2) ? null : lector.GetString(2),

                            });

                        }
                    }
                }
               
            }
            return View(categorias);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Categoria categoria)
        {
            if (categoria == null || string.IsNullOrWhiteSpace(categoria.Nombre))
            {
                ModelState.AddModelError("Nombre", "El nombre es obligatorio");
                return View(categoria);
            }

            using(var conexion = new SqlConnection(_connectionstring))
            {
                var sql = "INSERT INTO Categorias (Nombre, Descripcion) VALUES (@Nombre, @Descripcion)";
                using (var comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@Nombre", categoria.Nombre);
                    comando.Parameters.AddWithValue("@Descripcion",(object)categoria.Descripcion ?? System.DBNull.Value);

                    conexion.Open();
                    comando.ExecuteNonQuery();
                }


            }

            TempData["SuccessMessage"] = "Categoria guardada correctamente.";

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            Categoria categoria = null;

            using (var conexion = new SqlConnection(_connectionstring))
            {
                var sql = "SELECT ID, Nombre, Descripcion FROM Categorias WHERE ID = @ID";

                using (var comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@ID", id);

                    conexion.Open();

                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            categoria = new Categoria
                            {
                                ID = lector.GetInt32(0),
                                Nombre = lector.GetString(1),
                                Descripcion = lector.IsDBNull(2) ? null : lector.GetString(2)
                            };
                        }
                    }
                }
            }

            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Categoria categoria)
        {
            if (categoria == null || string.IsNullOrWhiteSpace(categoria.Nombre))
            {
                ModelState.AddModelError("Nombre", "El nombre es obligatorio");
                return View(categoria);
            }

            using (var conexion = new SqlConnection(_connectionstring))
            {
                var sql = "UPDATE Categorias SET Nombre = @Nombre, Descripcion = @Descripcion WHERE ID = @ID";

                using (var comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@ID", categoria.ID);
                    comando.Parameters.AddWithValue("@Nombre", categoria.Nombre);
                    comando.Parameters.AddWithValue("@Descripcion",
                        (object)categoria.Descripcion ?? DBNull.Value);

                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }

            TempData["SuccesMessage"] = "Categoría actualizada correctamente.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            using (var conexion = new SqlConnection(_connectionstring))
            {
                var sql = "DELETE FROM Categorias WHERE ID = @ID";

                using (var comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@ID", id);

                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }

            TempData["SuccesMessage"] = "Categoría eliminada correctamente.";

            return RedirectToAction("Index");
        }

    }
}
