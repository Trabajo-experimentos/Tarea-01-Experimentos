using System.Globalization;
using System.Text;

namespace CarritoDeCompras
{
    class ReciboTxtRepository
    {
        private readonly string _rutaRecibos;

        public ReciboTxtRepository(string rutaRecibos)
        {
            _rutaRecibos = rutaRecibos;
        }

        public string GuardarRecibo(List<(Producto Producto, int Cantidad)> detallesCompra, decimal total)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_rutaRecibos) ?? AppContext.BaseDirectory);
            if (!File.Exists(_rutaRecibos))
            {
                File.WriteAllText(_rutaRecibos, string.Empty, Encoding.UTF8);
            }

            string codigoRecibo = $"RCB-{DateTime.Now:yyyyMMdd-HHmmss}";
            var sb = new StringBuilder();
            sb.AppendLine("========================================");
            sb.AppendLine($"Recibo: {codigoRecibo}");
            sb.AppendLine($"Fecha: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine("Detalles:");

            foreach (var detalle in detallesCompra)
            {
                decimal subtotal = detalle.Producto.Price * detalle.Cantidad;
                sb.AppendLine($"- {detalle.Producto.Name} ({detalle.Producto.Code}) | Cantidad: {detalle.Cantidad} | Precio: {detalle.Producto.Price.ToString("0.00", CultureInfo.InvariantCulture)} | Subtotal: {subtotal.ToString("0.00", CultureInfo.InvariantCulture)}");
            }

            sb.AppendLine($"TOTAL: {total.ToString("0.00", CultureInfo.InvariantCulture)}");
            sb.AppendLine("========================================");
            sb.AppendLine();

            File.AppendAllText(_rutaRecibos, sb.ToString(), Encoding.UTF8);
            return codigoRecibo;
        }

        public string LeerHistorial()
        {
            if (!File.Exists(_rutaRecibos))
            {
                return "No hay recibos registrados aún.";
            }

            string contenido = File.ReadAllText(_rutaRecibos, Encoding.UTF8);
            return string.IsNullOrWhiteSpace(contenido)
                ? "No hay recibos registrados aún."
                : contenido;
        }
    }
}
