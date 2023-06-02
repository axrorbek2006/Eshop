namespace Eshop
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Product> products = new List<Product>()
            {
                new Product(1, "Coca-Cola", 5000),
                new Product(2, "Fanta", 8000),
                new Product(3, "Pepsi", 7000),
                new Product(4, "Sprite", 9000),
                new Product(5, "Chortoq", 15000),
            };

            Cart cart = new Cart(1, 2);


            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n _______________________________Products_____________________________\n");
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var product in products)
            {
                Console.WriteLine($" \n Id : {product.Id} , Name : {product.Name} , Price : {product.Price} \n");
            }
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("\n 1 - Add Product , 2 - Remove Product , 3 - Reduce Quantity , 4 - Display Cart , 5 - Undo Last Action , 6 - Show History\n \n : ");
                int num = Convert.ToInt32(Console.ReadLine());
                switch (num)
                {
                    case 1:
                        cart.AddProduct();
                        break;
                    case 2:
                        cart.RemoveProduct();
                        break;
                    case 3:
                        cart.ReduceQuantity();
                        break;
                    case 4:
                        cart.DisplayCart();
                        break;
                    case 5:
                        cart.UndoLastAction();
                        break;
                    case 6:
                        cart.ShowHistory();
                        break;
                }
            }
        }
    }

    class Product
    {
        public int Id { get; set; }
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        public static List<Product> products = new List<Product>();
        public Product(int id, string name, decimal price)
        {
            Id = id;
            Name = name;
            Price = price;
            products.Add(this);
        }
    }

    class Cart
    {
        public int Id { get; }
        public int CustomerId { get; }

        public Dictionary<Product, int> products = new Dictionary<Product, int>();
        private Stack<Action> actionHistory = new Stack<Action>();
        private List<string> history = new List<string>();

        public Cart(int id, int customerId)
        {
            Id = id;
            CustomerId = customerId;
        }

        public void AddProduct()
        {
            Console.Write("Product ID: ");
            int id = Convert.ToInt32(Console.ReadLine());

            Product selectedProduct = Product.products.FirstOrDefault(p => p.Id == id);

            if (selectedProduct != null)
            {
                Console.Write("Quantity: ");
                int quantity = Convert.ToInt32(Console.ReadLine());

                if (quantity > 0)
                {
                    if (products.ContainsKey(selectedProduct))
                        products[selectedProduct] += quantity;
                    else
                        products[selectedProduct] = quantity;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Product: {selectedProduct.Name} added, Quantity: {quantity}");
                    Console.ForegroundColor = ConsoleColor.Red;

                    Action addAction = () =>
                    {
                        if (products.ContainsKey(selectedProduct))
                        {
                            products[selectedProduct] -= quantity;
                            if (products[selectedProduct] <= 0)
                                products.Remove(selectedProduct);
                        }
                    };

                    actionHistory.Push(addAction);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    history.Add($"Product {selectedProduct.Name} added, Quantity: {quantity}");
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Invalid quantity. Quantity must be greater than 0.");
                    Console.ForegroundColor = ConsoleColor.Red;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Product not found.");
                Console.ForegroundColor = ConsoleColor.Red;
            }
        }

        public void RemoveProduct()
        {
            Console.Write("Product ID: ");
            int id = Convert.ToInt32(Console.ReadLine());

            Product selectedProduct = products.Keys.FirstOrDefault(p => p.Id == id);

            if (selectedProduct != null)
            {
                if (products[selectedProduct] > 0)
                {
                    products[selectedProduct]--;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Product: {selectedProduct.Name} quantity reduced to: {products[selectedProduct]}");
                    Console.ForegroundColor = ConsoleColor.Red;

                    Action removeAction = () =>
                    {
                        if (products.ContainsKey(selectedProduct))
                            products[selectedProduct]++;
                        else
                            products[selectedProduct] = 1;
                    };

                    actionHistory.Push(removeAction);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    history.Add($"Product {selectedProduct.Name} quantity reduced to: {products[selectedProduct]}");
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Product quantity is already 0.");
                    Console.ForegroundColor = ConsoleColor.Red;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Product not found.");
                Console.ForegroundColor = ConsoleColor.Red;
            }
        }

        public void ReduceQuantity()
        {
            Console.Write("Product ID: ");
            int id = Convert.ToInt32(Console.ReadLine());

            Product selectedProduct = products.Keys.FirstOrDefault(p => p.Id == id);

            if (selectedProduct != null)
            {
                Console.Write($"Quantity to reduce: ");
                int quantityToReduce = Convert.ToInt32(Console.ReadLine());

                if (quantityToReduce <= products[selectedProduct])
                {
                    products[selectedProduct] -= quantityToReduce;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Product: {selectedProduct.Name} quantity reduced to: {products[selectedProduct]}");
                    Console.ForegroundColor = ConsoleColor.Red;

                    Action reduceAction = () =>
                    {
                        if (products.ContainsKey(selectedProduct))
                            products[selectedProduct] += quantityToReduce;
                        else
                            products[selectedProduct] = quantityToReduce;
                    };

                    actionHistory.Push(reduceAction);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    history.Add($"Product {selectedProduct.Name} quantity reduced to: {products[selectedProduct]}");
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Invalid quantity. Quantity to reduce exceeds available quantity.");
                    Console.ForegroundColor = ConsoleColor.Red;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Product not found.");
                Console.ForegroundColor = ConsoleColor.Red;
            }
        }

        public decimal CalculateTotalPrice()
        {
            decimal total = 0;

            foreach (var kvp in products)
            {
                total += (kvp.Key.Price * kvp.Value);
            }

            return total;
        }

        public void DisplayCart()
        {
            if (!products.Any())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Cart is empty.");
                Console.ForegroundColor = ConsoleColor.Red;
                return;
            }

            var currentColor = ChangeConsoleColor("green");
            foreach (var kvp in products)
            {
                Console.WriteLine($"{kvp.Key.Name} x{kvp.Value}");
            }

            decimal totalPrice = CalculateTotalPrice();

            Console.WriteLine($"Total due: {totalPrice}");
            Console.WriteLine();
            Console.ForegroundColor = currentColor;
        }

        public void UndoLastAction()
        {
            if (actionHistory.Count > 0)
            {
                Action lastAction = actionHistory.Pop();
                lastAction.Invoke();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Last action undone.");
                Console.ForegroundColor = ConsoleColor.Red;

                if (history.Count > 0)
                    history.RemoveAt(history.Count - 1);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No actions to undo.");
                Console.ForegroundColor = ConsoleColor.Red;
            }
        }

        public void ShowHistory()
        {
            if (history.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No history available.");
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n _______________________________History_____________________________\n");
                Console.ForegroundColor = ConsoleColor.Green;
                foreach (var action in history)
                {
                    Console.WriteLine(action);
                }
                Console.WriteLine();
            }
        }

        private ConsoleColor ChangeConsoleColor(string color)
        {
            var currentColor = Console.ForegroundColor;

            if (color == "green")
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }

            return currentColor;
        }
    }
}