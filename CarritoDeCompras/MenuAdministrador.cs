namespace CarritoDeCompras
{
    class MenuAdministrador
    {
        private readonly Carrito _carrito;

        public MenuAdministrador(Carrito carrito)
        {
            _carrito = carrito;
        }

        public void Mostrar()
        {
            bool volver = false;

            while (!volver)
            {
                Console.Clear();
                Carrito.CambiarPantalla("========== PANEL DE ADMINISTRADOR ==========");
                Console.WriteLine(      "| 1) Registrar producto");
                Console.WriteLine(      "| 2) Editar producto (lista completa)");
                Console.WriteLine(      "| 3) Borrar producto (lista completa)");
                Console.WriteLine(      "| 4) Visualizar todos");
                Console.WriteLine(      "| 5) Buscar por nombre (editar/borrar)");
                Console.WriteLine(      "| 6) Buscar por código (editar/borrar)");
                Console.WriteLine(      "| 7) Ver historial de compras");
                Console.WriteLine(      "| 0) Cerrar sesión");
                Console.WriteLine(      "============================================");

                int opcion = Carrito.ReadInt("Seleccione una opción: ", allowZero: true);

                switch (opcion)
                {
                    case 1:
                        _carrito.RegistrarProducto();
                        break;
                    case 2:
                        _carrito.EditarDesdeListaCompleta();
                        break;
                    case 3:
                        _carrito.BorrarDesdeListaCompleta();
                        break;
                    case 4:
                        _carrito.VisualizarTodos();
                        break;
                    case 5:
                        _carrito.BuscarPorNombre();
                        break;
                    case 6:
                        _carrito.BuscarPorCodigo();
                        break;
                    case 7:
                        MostrarHistorialCompras();
                        break;
                    case 0:
                        volver = true;
                        break;
                    default:
                        Console.WriteLine("Opción inválida.");
                        Carrito.Pause();
                        break;
                }
            }
        }

        private void MostrarHistorialCompras()
        {
            Console.Clear();
            Carrito.CambiarPantalla("===== HISTORIAL DE COMPRAS =====");
            Console.WriteLine(_carrito.ObtenerHistorialCompras());
            Carrito.Pause();
            Console.Clear();
        }
    }
}
