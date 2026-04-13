using System.Globalization;
using System.Text;

namespace CarritoDeCompras
{
    class CatalogoTxtRepository
    {
        private readonly string _rutaCatalogo;

        public CatalogoTxtRepository(string rutaCatalogo)
        {
            _rutaCatalogo = rutaCatalogo;
        }

        public List<Producto> Cargar()
        {
            var productos = new List<Producto>();

            Directory.CreateDirectory(Path.GetDirectoryName(_rutaCatalogo) ?? AppContext.BaseDirectory);

            if (!File.Exists(_rutaCatalogo))
            {
                File.WriteAllText(_rutaCatalogo, string.Empty, Encoding.UTF8);
                return productos;
            }

            string[] lineas = File.ReadAllLines(_rutaCatalogo, Encoding.UTF8);

            foreach (string linea in lineas)
            {
                if (string.IsNullOrWhiteSpace(linea))
                {
                    continue;
                }

                if (TryParseProducto(linea, out Producto? producto) && producto is not null)
                {
                    productos.Add(producto);
                }
            }

            return productos;
        }

        public void Guardar(IEnumerable<Producto> productos)
        {
            var lineas = productos.Select(SerializarProducto).ToArray();
            File.WriteAllLines(_rutaCatalogo, lineas, Encoding.UTF8);
        }

        private static string SerializarProducto(Producto producto)
        {
            return string.Join("|", new[]
            {
                Escape(producto.Code),
                Escape(producto.Name),
                producto.Price.ToString(CultureInfo.InvariantCulture),
                Escape(producto.Category),
                Escape(producto.Description),
                producto.Quantity.ToString(CultureInfo.InvariantCulture)
            });
        }

        private static bool TryParseProducto(string linea, out Producto? producto)
        {
            producto = null;
            var campos = SplitCampos(linea);

            if (campos.Count != 6)
            {
                return false;
            }

            if (!decimal.TryParse(campos[2], NumberStyles.Number, CultureInfo.InvariantCulture, out decimal precio))
            {
                return false;
            }

            if (!int.TryParse(campos[5], NumberStyles.Integer, CultureInfo.InvariantCulture, out int cantidad))
            {
                return false;
            }

            producto = new Producto(
                Unescape(campos[1]),
                precio,
                Unescape(campos[3]),
                Unescape(campos[4]),
                Unescape(campos[0]),
                cantidad);

            return true;
        }

        private static List<string> SplitCampos(string linea)
        {
            var campos = new List<string>();
            var actual = new StringBuilder();
            bool escapando = false;

            foreach (char c in linea)
            {
                if (escapando)
                {
                    actual.Append(c);
                    escapando = false;
                    continue;
                }

                if (c == '\\')
                {
                    escapando = true;
                    continue;
                }

                if (c == '|')
                {
                    campos.Add(actual.ToString());
                    actual.Clear();
                    continue;
                }

                actual.Append(c);
            }

            campos.Add(actual.ToString());
            return campos;
        }

        private static string Escape(string valor)
        {
            return valor
                .Replace("\\", "\\\\")
                .Replace("|", "\\|")
                .Replace("\r", "")
                .Replace("\n", "\\n");
        }

        private static string Unescape(string valor)
        {
            return valor.Replace("\\n", "\n");
        }
    }
}
