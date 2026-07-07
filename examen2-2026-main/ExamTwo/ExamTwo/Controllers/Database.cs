namespace ExamTwo.Controllers
{
    public class Database
    {
        public Dictionary<string, int> coffeeTypes = new Dictionary<string, int>
        {
            { "Americano", 10 },
            { "Cappuccino", 8 },
            { "Lates", 10 },
            { "Mocaccino", 15}
        };

        public Dictionary<string, int> coffeePrices = new Dictionary<string, int>
        {
            { "Americano", 950 },
            { "Cappuccino", 1200 },
            { "Lates", 1350 },
            { "Mocaccino", 1500}
        };

        public Dictionary<int, int> coinInventory = new Dictionary<int, int>
        {
            { 500, 20 },
            { 100, 30 },
            { 50, 50 },
            { 25, 25}
        };

    }
}
