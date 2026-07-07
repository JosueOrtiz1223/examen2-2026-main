using System;
using System.Collections.Generic;
using System.Linq;
using ExamTwo.Controllers;

namespace ExamTwo.Repositories
{
    public interface ICoffeeMachineRepository
    {
        Dictionary<string, int> GetCoffeeTypes();
        Dictionary<string, int> GetCoffeePrices();
        Dictionary<int, int> GetCoinInventory();
        int GetCoffeeQuantity(string name);
        void DecreaseCoffeeQuantity(string name, int amount);
        void DecreaseCoinCount(int coinValue, int count);
    }
    

    public class CoffeeMachineRepository : ICoffeeMachineRepository
    {
        private readonly Database _db;

        public CoffeeMachineRepository(Database db)
        {
            _db = db;
        }

        public Dictionary<string, int> GetCoffeeTypes() => _db.coffeeTypes;

        public Dictionary<string, int> GetCoffeePrices() => _db.coffeePrices;

        public Dictionary<int, int> GetCoinInventory() => _db.coinInventory;

        public int GetCoffeeQuantity(string name)
        {
            if (!_db.coffeeTypes.ContainsKey(name))
                throw new ArgumentException($"Producto {name} no existe.");
            return _db.coffeeTypes[name];
        }

        public void DecreaseCoffeeQuantity(string name, int amount)
        {
            if (!_db.coffeeTypes.ContainsKey(name))
                throw new ArgumentException($"Producto {name} no existe.");

            if (_db.coffeeTypes[name] < amount)
                throw new ArgumentException($"No hay suficientes {name} en la máquina.");

            _db.coffeeTypes[name] -= amount;
        }

       public void DecreaseCoinCount(int coinValue, int count)
        {
            if (!_db.coinInventory.ContainsKey(coinValue))
                throw new ArgumentException($"Moneda {coinValue} no existe en inventario.");

            if (_db.coinInventory[coinValue] < count)
                throw new ArgumentException($"No hay suficientes monedas de {coinValue}.");

            _db.coinInventory[coinValue] -= count;
        }
    }
}
