using System.Globalization;

namespace CarritoDeCompras
{
    class Program
    {
        static void Main(string[] args)
        {
            string rutaCatalogo = Path.Combine(Directory.GetCurrentDirectory(), "catálogo.txt");
            string rutaRecibos = Path.Combine(Directory.GetCurrentDirectory(), "recibos.txt");
            var sistema = new Carrito(rutaCatalogo, rutaRecibos);
            sistema.MenuPrincipal();
        }
    }

    class Carrito
    {
        private readonly List<Producto> _productos;
        private readonly CatalogoTxtRepository _repositorio;
        private readonly ReciboTxtRepository _recibos;

        public Carrito(string rutaCatalogo, string rutaRecibos)
        {
            _repositorio = new CatalogoTxtRepository(rutaCatalogo);
            _recibos = new ReciboTxtRepository(rutaRecibos);
            _productos = _repositorio.Cargar();
        }

        public void MenuPrincipal()
        {
            bool salir = false;

            while (!salir)
            {
                CambiarPantalla(  "===== MENÚ PRINCIPAL =====");
                Console.WriteLine("| 1) Administrador");
                Console.WriteLine("| 2) Cliente");
                Console.WriteLine("| 0) Salir del sistema");
                Console.WriteLine("==========================");

                int opcion = ReadInt("Seleccione una opción: ", allowZero: true);

                switch (opcion)
                {
                    case 1:
                        var menuAdministrador = new MenuAdministrador(this);
                        menuAdministrador.Mostrar();
                        break;
                    case 2:
                        var menuCliente = new MenuCliente(this);
                        menuCliente.Mostrar();
                        break;
                    case 0:
                        salir = true;
                        break;
                    default:
                        Console.WriteLine("Opción inválida.");
                        Pause();
                        break;
                }
            }
        }

        public void RegistrarProducto()
        {
            CambiarPantalla("=== REGISTRAR PRODUCTO ===");

            string nombre = ReadNonEmptyText("Nombre: ");
            decimal precio = ReadDecimal("Precio: ", allowZero: false);
            string categoria = ReadNonEmptyText("Categoría: ");
            string descripcion = ReadNonEmptyText("Descripción: ");
            int cantidad = ReadInt("Cantidad: ", allowZero: true);
            string codigoUnico = GenerarCodigoProductoUnico();

            var producto = new Producto(nombre, precio, categoria, descripcion, codigoUnico, cantidad);
            _productos.Add(producto);
            GuardarCambios();

            Console.WriteLine("Producto registrado correctamente.");
            Console.WriteLine($"Código generado: {producto.Code}");
            Pause();
        }

        public void VisualizarTodos()
        {
            CambiarPantalla("=== CATÁLOGO COMPLETO ===");

            if (_productos.Count == 0)
            {
                Console.WriteLine("No hay productos registrados.");
            }
            else
            {
                ImprimirListado(_productos);
            }

            Pause();
        }

        public void EditarDesdeListaCompleta()
        {
            CambiarPantalla("=== EDITAR PRODUCTO ===");

            if (_productos.Count == 0)
            {
                Console.WriteLine("No hay productos para editar.");
                Pause();
                return;
            }

            ImprimirListado(_productos);
            int indice = ReadInt("Ingrese el número del producto a editar: ");

            if (!IndiceValido(indice, _productos.Count))
            {
                Console.WriteLine("Número inválido.");
                Pause();
                return;
            }

            EditarProducto(_productos[indice - 1]);
            GuardarCambios();
            Console.WriteLine("Producto editado correctamente.");
            Pause();
        }

        public void BorrarDesdeListaCompleta()
        {
            CambiarPantalla("=== BORRAR PRODUCTO ===");

            if (_productos.Count == 0)
            {
                Console.WriteLine("No hay productos para borrar.");
                Pause();
                return;
            }

            ImprimirListado(_productos);
            int indice = ReadInt("Ingrese el número del producto a borrar: ");

            if (!IndiceValido(indice, _productos.Count))
            {
                Console.WriteLine("Número inválido.");
                Pause();
                return;
            }

            var producto = _productos[indice - 1];
            _productos.RemoveAt(indice - 1);
            GuardarCambios();

            Console.WriteLine($"Producto '{producto.Name}' eliminado correctamente.");
            Pause();
        }

        public void BuscarPorNombre()
        {
            CambiarPantalla("=== BUSCAR POR NOMBRE ===");

            string texto = ReadNonEmptyText("Ingrese parte del nombre: ");
            var resultados = _productos
                .Where(p => p.Name.Contains(texto, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (resultados.Count == 0)
            {
                Console.WriteLine("No se encontraron productos.");
                Pause();
                return;
            }

            ImprimirListado(resultados);
            GestionarResultadoBusqueda(resultados);
        }

        public void BuscarPorCodigo()
        {
            CambiarPantalla("=== BUSCAR POR CÓDIGO ===");

            string codigo = ReadNonEmptyText("Ingrese el código exacto: ");
            var producto = _productos.FirstOrDefault(
                p => p.Code.Equals(codigo, StringComparison.OrdinalIgnoreCase));

            if (producto == null)
            {
                Console.WriteLine("Producto no encontrado.");
                Pause();
                return;
            }

            ImprimirListado(new List<Producto> { producto });
            GestionarResultadoBusqueda(new List<Producto> { producto });
        }

        private void GestionarResultadoBusqueda(List<Producto> resultados)
        {
            Console.WriteLine();
            Console.WriteLine("1) Editar uno de los resultados");
            Console.WriteLine("2) Borrar uno de los resultados");
            Console.WriteLine("0) Volver");

            int opcion = ReadInt("Seleccione una opción: ", allowZero: true);

            if (opcion == 0)
            {
                return;
            }

            int indice = ReadInt("Ingrese el número del producto listado: ");
            if (!IndiceValido(indice, resultados.Count))
            {
                Console.WriteLine("Número inválido.");
                Pause();
                return;
            }

            var seleccionado = resultados[indice - 1];

            switch (opcion)
            {
                case 1:
                    EditarProducto(seleccionado);
                    GuardarCambios();
                    Console.WriteLine("Producto editado correctamente.");
                    break;
                case 2:
                    _productos.Remove(seleccionado);
                    GuardarCambios();
                    Console.WriteLine("Producto eliminado correctamente.");
                    break;
                default:
                    Console.WriteLine("Opción inválida.");
                    break;
            }

            Pause();
        }

        private void EditarProducto(Producto producto)
        {
            bool finalizar = false;

            while (!finalizar)
            {
                CambiarPantalla(  "====== EDITANDO  PRODUCTO ======");
                Console.WriteLine(producto);
                Console.WriteLine();
                Console.WriteLine("| 1) Nombre");
                Console.WriteLine("| 2) Precio");
                Console.WriteLine("| 3) Categoría");
                Console.WriteLine("| 4) Descripción");
                Console.WriteLine("| 5) Cantidad");
                Console.WriteLine("| 0) Finalizar edición");
                Console.WriteLine("================================");

                int opcion = ReadInt("Campo a editar: ", allowZero: true);

                switch (opcion)
                {
                    case 1:
                        producto.Name = ReadNonEmptyText("Nuevo nombre: ");
                        break;
                    case 2:
                        producto.Price = ReadDecimal("Nuevo precio: ", allowZero: false);
                        break;
                    case 3:
                        producto.Category = ReadNonEmptyText("Nueva categoría: ");
                        break;
                    case 4:
                        producto.Description = ReadNonEmptyText("Nueva descripción: ");
                        break;
                    case 5:
                        producto.Quantity = ReadInt("Nueva cantidad: ", allowZero: true);
                        break;
                    case 0:
                        finalizar = true;
                        break;
                    default:
                        Console.WriteLine("Opción inválida.");
                        Pause();
                        break;
                }
            }
        }

        private void GuardarCambios()
        {
            _repositorio.Guardar(_productos);
        }

        private string GenerarCodigoProductoUnico()
        {
            const string alfanumerico = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = Random.Shared;

            while (true)
            {
                char[] bloque = new char[5];
                for (int i = 0; i < bloque.Length; i++)
                {
                    bloque[i] = alfanumerico[random.Next(alfanumerico.Length)];
                }

                string codigo = $"PROD-{new string(bloque)}";
                bool existe = _productos.Any(p => p.Code.Equals(codigo, StringComparison.OrdinalIgnoreCase));
                if (!existe)
                {
                    return codigo;
                }
            }
        }

        public List<Producto> ObtenerCatalogo()
        {
            return _productos
                .OrderBy(p => p.Name)
                .ToList();
        }

        public List<Producto> BuscarCatalogoPorNombre(string texto)
        {
            return _productos
                .Where(p => p.Name.Contains(texto, StringComparison.OrdinalIgnoreCase))
                .OrderBy(p => p.Name)
                .ToList();
        }

        public List<Producto> BuscarCatalogoPorCategoria(string categoria)
        {
            return _productos
                .Where(p => p.Category.Contains(categoria, StringComparison.OrdinalIgnoreCase))
                .OrderBy(p => p.Name)
                .ToList();
        }

        public Producto? BuscarProductoPorCodigo(string codigo)
        {
            return _productos.FirstOrDefault(p => p.Code.Equals(codigo, StringComparison.OrdinalIgnoreCase));
        }

        public int ObtenerStockDisponible(string codigo)
        {
            var producto = BuscarProductoPorCodigo(codigo);
            return producto?.Quantity ?? 0;
        }

        public bool ProcesarCompra(Dictionary<string, int> carritoCliente, out string mensaje)
        {
            mensaje = string.Empty;

            if (carritoCliente.Count == 0)
            {
                mensaje = "Tu carrito está vacío.";
                return false;
            }

            var detallesCompra = new List<(Producto Producto, int Cantidad)>();

            foreach (var item in carritoCliente)
            {
                var producto = BuscarProductoPorCodigo(item.Key);
                if (producto == null)
                {
                    mensaje = $"No se encontró el producto con código {item.Key}.";
                    return false;
                }

                if (item.Value <= 0)
                {
                    mensaje = $"Cantidad inválida para {producto.Name}.";
                    return false;
                }

                if (item.Value > producto.Quantity)
                {
                    mensaje = $"Stock insuficiente para {producto.Name}. Disponible: {producto.Quantity}.";
                    return false;
                }

                detallesCompra.Add((producto, item.Value));
            }

            foreach (var detalle in detallesCompra)
            {
                detalle.Producto.Quantity -= detalle.Cantidad;
            }

            GuardarCambios();

            decimal total = detallesCompra.Sum(x => x.Producto.Price * x.Cantidad);
            string codigoRecibo = _recibos.GuardarRecibo(detallesCompra, total);
            mensaje = $"Compra realizada con éxito. Recibo: {codigoRecibo}. Total: {total:C}.";
            return true;
        }

        public string ObtenerHistorialCompras()
        {
            return _recibos.LeerHistorial();
        }

        private static void ImprimirListado(List<Producto> productos)
        {
            for (int i = 0; i < productos.Count; i++)
            {
                var p = productos[i];
                Console.WriteLine($"{i + 1}) {p.Name} | Código: {p.Code} | Precio: {p.Price:C} | Stock: {p.Quantity}");
            }
        }

        private static bool IndiceValido(int indice, int total)
        {
            return indice >= 1 && indice <= total;
        }

        private static string ReadNonEmptyText(string mensaje)
        {
            string? valor;

            do
            {
                Console.Write(mensaje);
                valor = Console.ReadLine();
            }
            while (string.IsNullOrWhiteSpace(valor));

            return valor.Trim();
        }

        private static decimal ReadDecimal(string mensaje, bool allowZero = true)
        {
            decimal valor;

            while (true)
            {
                Console.Write(mensaje);
                string? entrada = Console.ReadLine();

                if (decimal.TryParse(entrada, NumberStyles.Number, CultureInfo.InvariantCulture, out valor) ||
                    decimal.TryParse(entrada, NumberStyles.Number, CultureInfo.CurrentCulture, out valor))
                {
                    if (allowZero && valor >= 0)
                    {
                        return valor;
                    }

                    if (!allowZero && valor > 0)
                    {
                        return valor;
                    }
                }

                Console.WriteLine(allowZero
                    ? "Valor inválido. Ingrese un número decimal válido (>= 0)."
                    : "Valor inválido. Ingrese un número decimal válido (> 0).");
            }
        }

        internal static int ReadInt(string mensaje, bool allowZero = false)
        {
            int valor;

            while (true)
            {
                Console.Write(mensaje);
                string? entrada = Console.ReadLine();

                if (entrada is null)
                {
                    Console.WriteLine();
                    Console.WriteLine("Entrada finalizada. Cerrando aplicación...");
                    Environment.Exit(0);
                }

                if (int.TryParse(entrada, out valor))
                {
                    if (allowZero && valor >= 0)
                    {
                        return valor;
                    }

                    if (!allowZero && valor > 0)
                    {
                        return valor;
                    }
                }

                Console.WriteLine(allowZero
                    ? "Valor inválido. Ingrese un entero válido (>= 0)."
                    : "Valor inválido. Ingrese un entero válido (> 0).");
            }
        }

        internal static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Presione ENTER para continuar...");
            Console.ReadLine();
        }

        internal static void CambiarPantalla(string titulo)
        {
            Console.Clear();
            Console.WriteLine(titulo);
        }
    }
}