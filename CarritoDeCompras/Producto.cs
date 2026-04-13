using System;
using System.Globalization;
using System.Text;

namespace CarritoDeCompras
{
    public class Producto
    {
        private string _name = string.Empty;
        private decimal _price;
        private string _category = string.Empty;
        private string _description = string.Empty;
        private string _code = string.Empty;
        private int _quantity;

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public decimal Price
        {
            get => _price;
            set => _price = ValidatePrice(value);
        }

        public string Category
        {
            get => _category;
            set => _category = value;
        }

        public string Description
        {
            get => _description;
            set => _description = value;
        }

        public string Code
        {
            get => _code;
            set => _code = value;
        }

        public int Quantity
        {
            get => _quantity;
            set => _quantity = ValidateQuantity(value);
        }

        public Producto(string name, decimal price, string category, string description, string code, int quantity)
        {
            Name = name;
            Price = price;
            Category = category;
            Description = description;
            Code = code;
            Quantity = quantity;
        }
        public Producto(string name, decimal price, string category, string description, int quantity)
        {
            Name = name;
            Price = price;
            Category = category;
            Description = description;
            Quantity = quantity;
            Code = GenerarCodigoProducto();
        }

        private static decimal ValidatePrice(decimal price)
        {
            if (price <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(price), "El precio debe ser mayor que cero.");
            }

            return price;
        }

        private static int ValidateQuantity(int quantity)
        {
            if (quantity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "La cantidad no puede ser negativa.");
            }

            return quantity;
        }

        private static string GenerarCodigoProducto()
        {
            const string alfanumerico = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = Random.Shared;
            var codigo = new StringBuilder(5);

            for (int i = 0; i < 5; i++)
            {
                int indice = random.Next(alfanumerico.Length);
                codigo.Append(alfanumerico[indice]);
            }

            return $"PROD-{codigo}";
        }

        public override string ToString()
        {
            return $"{Name} - ${Price} - {Category} - {Code} \n {Description} - Available: {Quantity}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is Producto producto)
            {
                return Name == producto.Name && Price == producto.Price && Category == producto.Category && Description == producto.Description && Code == producto.Code;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Price, Category, Description, Code, Quantity);
        }

        public void updateName()
        {
            Console.WriteLine("Ingrese el new nombre del producto:");
            string? newName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newName))
            {
                Name = newName;
            }
        }

        public void updatePrice()
        {
            Console.WriteLine("Ingrese el new precio del producto:");
            while (true)
            {
                string? entrada = Console.ReadLine();
                if (TryReadPrice(entrada, out decimal newPrice) && newPrice > 0)
                {
                    Price = newPrice;
                    return;
                }

                Console.WriteLine("Por favor, ingrese un precio válido mayor que cero:");
            }
        }

        private static bool TryReadPrice(string? input, out decimal price)
        {
            return decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out price) ||
                   decimal.TryParse(input, NumberStyles.Number, CultureInfo.CurrentCulture, out price);
        }
        public void updateCategory()
        {
            Console.WriteLine("Ingrese la nueva categoría del producto:");
            string? newCategory = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newCategory))
            {
                Category = newCategory;
            }
        }
        public void updateDescription()
        {
            Console.WriteLine("Ingrese la nueva descripción del producto:");
            string? newDescription = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newDescription))
            {
                Description = newDescription;
            }
        }
        public void updateQuantity()
        {
            Console.WriteLine("Ingrese la nueva cantidad del producto:");
            int newQuantity;
            while (!int.TryParse(Console.ReadLine(), out newQuantity) || newQuantity < 0)
            {
                Console.WriteLine("Por favor, ingrese una cantidad válida mayor o igual a cero:");
            }
            Quantity = newQuantity;
        }

        public void findByCode(string codigo)
        {
            if (Code == codigo)
            {
                Console.WriteLine(this);
            }
            else
            {
                Console.WriteLine("Producto no encontrado.");
            }
        }
    }
}