using System;
using System.Text;

namespace CarritoDeCompras
{
    public class Producto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public int Quantity { get; set; }

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
            decimal newPrice;
            while (!decimal.TryParse(Console.ReadLine(), out newPrice))
            {
                Console.WriteLine("Por favor, ingrese un precio válido:");
            }
            Price = newPrice;
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
            while (!int.TryParse(Console.ReadLine(), out newQuantity))
            {
                Console.WriteLine("Por favor, ingrese una cantidad válida:");
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