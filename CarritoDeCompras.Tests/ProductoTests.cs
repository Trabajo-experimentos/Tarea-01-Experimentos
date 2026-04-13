using System.IO;
using CarritoDeCompras;

namespace CarritoDeCompras.Tests
{
    public class ProductoTests
    {
        [TestCase(0)]
        [TestCase(-1)]
        public void Constructor_ArrojaCuandoElPrecioNoEsPositivo(decimal precio)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Producto("Teclado", precio, "Accesorios", "Teclado mecanico", "PROD-12345", 1));
        }

        [TestCase(-1)]
        [TestCase(-20)]
        public void Constructor_ArrojaCuandoLaCantidadEsNegativa(int cantidad)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Producto("Teclado", 120m, "Accesorios", "Teclado mecanico", "PROD-12345", cantidad));
        }

        [Test]
        public void UpdatePrice_RepiteHastaRecibirUnValorPositivo()
        {
            var originalIn = Console.In;
            var originalOut = Console.Out;

            try
            {
                Console.SetIn(new StringReader("0\n-5\n19.99\n"));
                Console.SetOut(new StringWriter());

                var producto = new Producto("Teclado", 120m, "Accesorios", "Teclado mecanico", "PROD-12345", 1);

                producto.updatePrice();

                Assert.That(producto.Price, Is.EqualTo(19.99m));
            }
            finally
            {
                Console.SetIn(originalIn);
                Console.SetOut(originalOut);
            }
        }

        [Test]
        public void UpdateQuantity_RepiteHastaRecibirUnValorNoNegativo()
        {
            var originalIn = Console.In;
            var originalOut = Console.Out;

            try
            {
                Console.SetIn(new StringReader("-3\n0\n"));
                Console.SetOut(new StringWriter());

                var producto = new Producto("Teclado", 120m, "Accesorios", "Teclado mecanico", "PROD-12345", 1);

                producto.updateQuantity();

                Assert.That(producto.Quantity, Is.EqualTo(0));
            }
            finally
            {
                Console.SetIn(originalIn);
                Console.SetOut(originalOut);
            }
        }

        [Test]
        public void Constructor_SinCodigo_GeneraUnCodigoConFormatoEsperado()
        {
            var producto = new Producto("Teclado", 120m, "Accesorios", "Teclado mecanico", 4);

            Assert.That(producto.Code, Does.Match(@"^PROD-[A-Z0-9]{5}$"));
        }

        [Test]
        public void Carrito_IgnoraProductosInvalidosAlCargarCatalogo()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "Tarea01ExperimentosTests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);

            string rutaCatalogo = Path.Combine(tempDir, "catalogo_test.txt");
            string rutaRecibos = Path.Combine(tempDir, "recibos_test.txt");

            try
            {
                File.WriteAllLines(rutaCatalogo, new[]
                {
                    "PROD-A1001|Laptop Pro|2500.00|Tecnologia|Laptop para trabajo pesado.|5",
                    "PROD-B2002|Mouse Gamer|0|Accesorios|Mouse RGB con 7 botones.|20",
                    "PROD-C3003|Silla Ergonomica|800.00|Hogar|Silla ergonomica para oficina.|-1"
                });

                var carrito = new Carrito(rutaCatalogo, rutaRecibos);

                var catalogo = carrito.ObtenerCatalogo();

                Assert.That(catalogo.Count, Is.EqualTo(1));
                Assert.That(catalogo[0].Code, Is.EqualTo("PROD-A1001"));
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