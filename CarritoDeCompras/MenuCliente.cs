namespace CarritoDeCompras
{
    class MenuCliente
    {
        private readonly Carrito _carrito;
        private readonly Dictionary<string, int> _carritoCliente;

        public MenuCliente(Carrito carrito)
        {
            _carrito = carrito;
            _carritoCliente = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }

        public void Mostrar()
        {
            bool cerrarSesion = false;

            while (!cerrarSesion)
            {
                Carrito.CambiarPantalla("================== PANEL CLIENTE ==================");
                Console.WriteLine(      "| 1) Ver catálogo completo");
                Console.WriteLine(      "| 2) Buscar por nombre");
                Console.WriteLine(      "| 3) Buscar por categoría");
                Console.WriteLine(      "| 4) Ver mi carrito");
                Console.WriteLine(      "| 5) Agregar producto al carrito");
                Console.WriteLine(      "| 6) Quitar producto del carrito");
                Console.WriteLine(      "| 7) Cambiar cantidad de un producto en carrito");
                Console.WriteLine(      "| 8) Comprar");
                Console.WriteLine(      "| 9) Ver historial de compras");
                Console.WriteLine(      "| 0) Cerrar sesión");
                Console.WriteLine(      "===================================================");

                int opcion = Carrito.ReadInt("Seleccione una opción: ", allowZero: true);

                switch (opcion)
                {
                    case 1:
                        VerCatalogoCompleto();
                        break;
                    case 2:
                        BuscarPorNombre();
                        break;
                    case 3:
                        BuscarPorCategoria();
                        break;
                    case 4:
                        VerCarrito();
                        break;
                    case 5:
                        AgregarAlCarrito();
                        break;
                    case 6:
                        QuitarDelCarrito();
                        break;
                    case 7:
                        CambiarCantidadEnCarrito();
                        break;
                    case 8:
                        Comprar();
                        break;
                    case 9:
                        MostrarHistorialCompras();
                        break;
                    case 0:
                        cerrarSesion = true;
                        break;
                    default:
                        Console.WriteLine("Opción inválida.");
                        Carrito.Pause();
                        break;
                }
            }
        }

        private void VerCatalogoCompleto()
        {
            Carrito.CambiarPantalla("====== CATÁLOGO =====");

            var catalogo = _carrito.ObtenerCatalogo();
            ImprimirProductos(catalogo);
            Carrito.Pause();
        }

        private void BuscarPorNombre()
        {
            Carrito.CambiarPantalla("===== BUSCAR POR NOMBRE =====");
            Console.Write("Texto a buscar: ");
            string texto = Console.ReadLine()?.Trim() ?? string.Empty;

            var resultados = _carrito.BuscarCatalogoPorNombre(texto);
            ImprimirProductos(resultados);
            Carrito.Pause();
        }

        private void BuscarPorCategoria()
        {
            Carrito.CambiarPantalla("===== BUSCAR POR CATEGORÍA =====");
            Console.Write("Categoría a buscar: ");
            string categoria = Console.ReadLine()?.Trim() ?? string.Empty;

            var resultados = _carrito.BuscarCatalogoPorCategoria(categoria);
            ImprimirProductos(resultados);
            Carrito.Pause();
        }

        private void VerCarrito()
        {
            Carrito.CambiarPantalla("====== MI CARRITO ======");

            if (_carritoCliente.Count == 0)
            {
                Console.WriteLine("Tu carrito está vacío.");
                Carrito.Pause();
                return;
            }

            var itemsCarrito = ObtenerItemsCarritoValidos();
            if (itemsCarrito.Count == 0)
            {
                Console.WriteLine("Tu carrito está vacío.");
                Carrito.Pause();
                return;
            }

            decimal total = 0;
            for (int i = 0; i < itemsCarrito.Count; i++)
            {
                var item = itemsCarrito[i];
                decimal subtotal = item.Producto.Price * item.Cantidad;
                total += subtotal;
                Console.WriteLine($"{i + 1}) {item.Producto.Name} | Código: {item.Producto.Code} | Cantidad: {item.Cantidad} | Precio unitario: {item.Producto.Price:C} | Subtotal: {subtotal:C}");
            }

            Console.WriteLine();
            Console.WriteLine($"TOTAL: {total:C}");
            Carrito.Pause();
        }

        private void AgregarAlCarrito()
        {
            Carrito.CambiarPantalla("===== AGREGAR PRODUCTO AL CARRITO =====");

            var catalogo = _carrito.ObtenerCatalogo();
            ImprimirProductos(catalogo);
            if (catalogo.Count == 0)
            {
                Carrito.Pause();
                return;
            }

            int numeroProducto = Carrito.ReadInt("Ingrese el número del producto de la lista: ");
            if (numeroProducto < 1 || numeroProducto > catalogo.Count)
            {
                Console.WriteLine("Número de producto inválido.");
                Carrito.Pause();
                return;
            }

            var producto = catalogo[numeroProducto - 1];

            int cantidad = Carrito.ReadInt("Cantidad a agregar: ");
            int cantidadEnCarrito = _carritoCliente.TryGetValue(producto.Code, out int actual) ? actual : 0;

            if (cantidadEnCarrito + cantidad > producto.Quantity)
            {
                Console.WriteLine($"No puedes agregar esa cantidad. Stock disponible: {producto.Quantity}. Ya tienes {cantidadEnCarrito} en carrito.");
                Carrito.Pause();
                return;
            }

            _carritoCliente[producto.Code] = cantidadEnCarrito + cantidad;
            Console.WriteLine("Producto agregado al carrito.");
            Carrito.Pause();
        }

        private void QuitarDelCarrito()
        {
            Carrito.CambiarPantalla("===== QUITAR PRODUCTO DEL CARRITO =====");

            if (_carritoCliente.Count == 0)
            {
                Console.WriteLine("Tu carrito está vacío.");
                Carrito.Pause();
                return;
            }

            var itemsCarrito = ObtenerItemsCarritoValidos();
            MostrarCarritoSimple(itemsCarrito);

            int numeroProducto = Carrito.ReadInt("Ingrese el número del producto a quitar: ");
            if (numeroProducto < 1 || numeroProducto > itemsCarrito.Count)
            {
                Console.WriteLine("Número de producto inválido.");
                Carrito.Pause();
                return;
            }

            string codigo = itemsCarrito[numeroProducto - 1].Code;
            if (_carritoCliente.Remove(codigo))
            {
                Console.WriteLine("Producto eliminado del carrito.");
            }
            else
            {
                Console.WriteLine("Ese código no está en tu carrito.");
            }

            Carrito.Pause();
        }

        private void CambiarCantidadEnCarrito()
        {
            Carrito.CambiarPantalla("===== CAMBIAR CANTIDAD EN CARRITO =====");

            if (_carritoCliente.Count == 0)
            {
                Console.WriteLine("Tu carrito está vacío.");
                Carrito.Pause();
                return;
            }

            var itemsCarrito = ObtenerItemsCarritoValidos();
            MostrarCarritoSimple(itemsCarrito);

            int numeroProducto = Carrito.ReadInt("Ingrese el número del producto: ");
            if (numeroProducto < 1 || numeroProducto > itemsCarrito.Count)
            {
                Console.WriteLine("Número de producto inválido.");
                Carrito.Pause();
                return;
            }

            string codigo = itemsCarrito[numeroProducto - 1].Code;

            var producto = _carrito.BuscarProductoPorCodigo(codigo);
            if (producto == null)
            {
                Console.WriteLine("Producto no encontrado en catálogo.");
                Carrito.Pause();
                return;
            }

            int nuevaCantidad = Carrito.ReadInt("Nueva cantidad (0 para quitar): ", allowZero: true);
            if (nuevaCantidad == 0)
            {
                _carritoCliente.Remove(codigo);
                Console.WriteLine("Producto removido del carrito.");
                Carrito.Pause();
                return;
            }

            if (nuevaCantidad > producto.Quantity)
            {
                Console.WriteLine($"Cantidad excede el stock disponible ({producto.Quantity}).");
                Carrito.Pause();
                return;
            }

            _carritoCliente[codigo] = nuevaCantidad;
            Console.WriteLine("Cantidad actualizada.");
            Carrito.Pause();
        }

        private void Comprar()
        {
            Carrito.CambiarPantalla("===== CONFIRMAR COMPRA =====");

            if (_carritoCliente.Count == 0)
            {
                Console.WriteLine("Tu carrito está vacío.");
                Carrito.Pause();
                return;
            }

            VerCarritoSinPause();
            Console.Write("¿Deseas confirmar la compra? (s/n): ");
            string confirmacion = (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();

            if (confirmacion != "s")
            {
                Console.WriteLine("Compra cancelada.");
                Carrito.Pause();
                return;
            }

            if (_carrito.ProcesarCompra(_carritoCliente, out string mensaje))
            {
                _carritoCliente.Clear();
            }

            Console.WriteLine(mensaje);
            Carrito.Pause();
        }

        private void MostrarHistorialCompras()
        {
            Carrito.CambiarPantalla("===== HISTORIAL DE COMPRAS =====");
            Console.WriteLine(_carrito.ObtenerHistorialCompras());
            Carrito.Pause();
        }

        private void VerCarritoSinPause()
        {
            var itemsCarrito = ObtenerItemsCarritoValidos();
            if (itemsCarrito.Count == 0)
            {
                Console.WriteLine("Tu carrito está vacío.");
                return;
            }

            decimal total = 0;
            foreach (var item in itemsCarrito)
            {
                decimal subtotal = item.Producto.Price * item.Cantidad;
                total += subtotal;
                Console.WriteLine($"- {item.Producto.Name} | Código: {item.Producto.Code} | Cantidad: {item.Cantidad} | Subtotal: {subtotal:C}");
            }

            Console.WriteLine($"TOTAL: {total:C}");
        }

        private List<(string Code, Producto Producto, int Cantidad)> ObtenerItemsCarritoValidos()
        {
            var items = new List<(string Code, Producto Producto, int Cantidad)>();

            foreach (var item in _carritoCliente)
            {
                var producto = _carrito.BuscarProductoPorCodigo(item.Key);
                if (producto != null)
                {
                    items.Add((item.Key, producto, item.Value));
                }
            }

            return items;
        }

        private void MostrarCarritoSimple(List<(string Code, Producto Producto, int Cantidad)> itemsCarrito)
        {
            for (int i = 0; i < itemsCarrito.Count; i++)
            {
                var item = itemsCarrito[i];
                Console.WriteLine($"{i + 1}) Producto: {item.Producto.Name} | Cantidad en carrito: {item.Cantidad} | Stock: {item.Producto.Quantity} | Código: {item.Producto.Code}");
            }
        }

        private static void ImprimirProductos(List<Producto> productos)
        {
            if (productos.Count == 0)
            {
                Console.WriteLine("No hay productos para mostrar.");
                return;
            }

            for (int i = 0; i < productos.Count; i++)
            {
                Producto p = productos[i];
                Console.WriteLine($"{i + 1}) {p.Name} | Código: {p.Code} | Categoría: {p.Category} | Precio: {p.Price:C} | Stock: {p.Quantity}");
            }
        }
    }
}
