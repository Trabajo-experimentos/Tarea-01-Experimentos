using CarritoDeCompras;

namespace CarritoDeCompras.Tests
{
    public class CarritoDeComprasTests
    {
        [Test]
        public void ObtenerCatalogo_CuandoArchivoExiste_CargaProductos()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "Tarea01ExperimentosTests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);

            string rutaCatalogo = Path.Combine(tempDir, "catalogo_test.txt");
            string rutaRecibos = Path.Combine(tempDir, "recibos_test.txt");

            try
            {
                File.WriteAllLines(rutaCatalogo, new[]
                {
                    "PROD-T1111|Teclado|120.00|Accesorios|Teclado mecanico.|4"
                });

                var carrito = new Carrito(rutaCatalogo, rutaRecibos);

                var catalogo = carrito.ObtenerCatalogo();
                Assert.That(catalogo.Count, Is.EqualTo(1));
                Assert.That(catalogo[0].Code, Is.EqualTo("PROD-T1111"));
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }
    }
}