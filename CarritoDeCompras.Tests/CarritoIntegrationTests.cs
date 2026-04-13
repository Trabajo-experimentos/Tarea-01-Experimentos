using CarritoDeCompras;

namespace CarritoDeCompras.Tests
{
    public class CarritoIntegrationTests
    {
        private string _tempDir = string.Empty;
        private string _rutaCatalogo = string.Empty;
        private string _rutaRecibos = string.Empty;

        [SetUp]
        public void Setup()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "Tarea01ExperimentosTests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDir);

            _rutaCatalogo = Path.Combine(_tempDir, "catalogo_test.txt");
            _rutaRecibos = Path.Combine(_tempDir, "recibos_test.txt");

            File.WriteAllLines(_rutaCatalogo, new[]
            {
                "PROD-A1001|Laptop Pro|2500.00|Tecnologia|Laptop para trabajo pesado.|5",
                "PROD-B2002|Mouse Gamer|150.00|Accesorios|Mouse RGB con 7 botones.|20",
                "PROD-C3003|Silla Ergonomica|800.00|Hogar|Silla ergonomica para oficina.|8"
            });
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_tempDir))
            {
                Directory.Delete(_tempDir, true);
            }
        }

        [Test]
        public void Administrador_BuscarPorNombre_EncuentraCoincidenciasEsperadas()
        {
            var carrito = new Carrito(_rutaCatalogo, _rutaRecibos);

            var resultados = carrito.BuscarCatalogoPorNombre("Laptop");

            Assert.That(resultados.Count, Is.EqualTo(1));
            Assert.That(resultados[0].Code, Is.EqualTo("PROD-A1001"));
        }

        [Test]
        public void Administrador_BuscarPorCategoria_DevuelveProductosFiltrados()
        {
            var carrito = new Carrito(_rutaCatalogo, _rutaRecibos);

            var resultados = carrito.BuscarCatalogoPorCategoria("Accesorios");

            Assert.That(resultados.Count, Is.EqualTo(1));
            Assert.That(resultados[0].Name, Is.EqualTo("Mouse Gamer"));
        }

        [Test]
        public void Cliente_ProcesarCompra_DescuentaStockYGeneraRecibo()
        {
            var carrito = new Carrito(_rutaCatalogo, _rutaRecibos);
            var compra = new Dictionary<string, int>
            {
                { "PROD-A1001", 2 },
                { "PROD-B2002", 3 }
            };

            bool ok = carrito.ProcesarCompra(compra, out string mensaje);

            Assert.That(ok, Is.True);
            Assert.That(mensaje, Does.Contain("Compra realizada con éxito"));

            var laptop = carrito.BuscarProductoPorCodigo("PROD-A1001");
            var mouse = carrito.BuscarProductoPorCodigo("PROD-B2002");

            Assert.That(laptop, Is.Not.Null);
            Assert.That(mouse, Is.Not.Null);
            Assert.That(laptop!.Quantity, Is.EqualTo(3));
            Assert.That(mouse!.Quantity, Is.EqualTo(17));

            string historial = carrito.ObtenerHistorialCompras();
            Assert.That(historial, Does.Contain("Recibo:"));
            Assert.That(historial, Does.Contain("Laptop Pro"));
            Assert.That(historial, Does.Contain("Mouse Gamer"));
        }

        [Test]
        public void Cliente_ProcesarCompra_ConStockInsuficiente_NoModificaCatalogo()
        {
            var carrito = new Carrito(_rutaCatalogo, _rutaRecibos);
            var compra = new Dictionary<string, int>
            {
                { "PROD-A1001", 99 }
            };

            bool ok = carrito.ProcesarCompra(compra, out string mensaje);

            Assert.That(ok, Is.False);
            Assert.That(mensaje, Does.Contain("Stock insuficiente"));

            var laptop = carrito.BuscarProductoPorCodigo("PROD-A1001");
            Assert.That(laptop, Is.Not.Null);
            Assert.That(laptop!.Quantity, Is.EqualTo(5));
        }

        [Test]
        public void Cliente_ProcesarCompra_ConStockExacto_DejaStockEnCero()
        {
            var carrito = new Carrito(_rutaCatalogo, _rutaRecibos);
            var compra = new Dictionary<string, int>
            {
                { "PROD-C3003", 8 }
            };

            bool ok = carrito.ProcesarCompra(compra, out string mensaje);

            Assert.That(ok, Is.True);
            Assert.That(mensaje, Does.Contain("Compra realizada con éxito"));

            var silla = carrito.BuscarProductoPorCodigo("PROD-C3003");
            Assert.That(silla, Is.Not.Null);
            Assert.That(silla!.Quantity, Is.EqualTo(0));
        }

        [Test]
        public void Cliente_ProcesarCompra_ConCodigoInexistente_RetornaErrorYNoGeneraRecibo()
        {
            var carrito = new Carrito(_rutaCatalogo, _rutaRecibos);
            var compra = new Dictionary<string, int>
            {
                { "PROD-Z9999", 1 }
            };

            bool ok = carrito.ProcesarCompra(compra, out string mensaje);

            Assert.That(ok, Is.False);
            Assert.That(mensaje, Does.Contain("No se encontró el producto"));

            string historial = carrito.ObtenerHistorialCompras();
            Assert.That(historial, Is.EqualTo("No hay recibos registrados aún."));
        }

        [Test]
        public void Cliente_ObtenerHistorialCompras_SinCompras_IndicaHistorialVacio()
        {
            var carrito = new Carrito(_rutaCatalogo, _rutaRecibos);

            string historial = carrito.ObtenerHistorialCompras();

            Assert.That(historial, Is.EqualTo("No hay recibos registrados aún."));
        }
    }
}