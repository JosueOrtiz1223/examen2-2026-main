using System.Collections.Generic;
using ExamTwo.Models;
using ExamTwo.Repositories;
using ExamTwo.Services;
using System.Linq;


namespace Tests
{
    public class UnitTestService
    {
        private class MockCoffeeMachineRepository : ICoffeeMachineRepository
        {
            public Dictionary<string, int> CoffeeTypes { get; } = new()
            {
                { "Americano", 10 },
                { "Cappuccino", 8 }
            };

            public Dictionary<string, int> CoffeePrices { get; } = new()
            {
                { "Americano", 950 },
                { "Cappuccino", 1200 }
            };

            public Dictionary<int, int> CoinInventory { get; } = new()
            {
                { 500, 1 },
                { 100, 2 }
            };

            public Dictionary<string, int> ProductInventory { get; } = new()
            {
                { "Americano", 5 },
                { "Cappuccino", 3 }
            };

            public Dictionary<string, int> GetCoffeeTypes() => new(CoffeeTypes);
            public Dictionary<string, int> GetCoffeePrices() => new(CoffeePrices);
            public Dictionary<int, int> GetCoinInventory() => CoinInventory;
            public int GetCoffeeQuantity(string name) => ProductInventory[name];
            public void DecreaseCoffeeQuantity(string name, int amount) => ProductInventory[name] -= amount;
            public void DecreaseCoinCount(int coinValue, int count) => CoinInventory[coinValue] -= count;
        }

        private readonly MockCoffeeMachineRepository _repo = new();

        [Fact]
        public void BuyCoffee_ShouldReturnBadRequest_WhenOrderIsEmpty()
        {
            var service = new CoffeeMachineService(_repo);
            var result = service.BuyCoffee(new OrderRequest { Order = new Dictionary<string, int>(), Payment = new Payment { TotalAmount = 1000 } });

            Assert.False(result.Success);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Orden vacia.", result.Message);
        }

        [Fact]
        public void BuyCoffee_ShouldReturnBadRequest_WhenPaymentIsInsufficient()
        {
            var service = new CoffeeMachineService(_repo);
            var result = service.BuyCoffee(new OrderRequest
            {
                Order = new Dictionary<string, int> { { "Americano", 1 } },
                Payment = new Payment { TotalAmount = 500 }
            });

            Assert.False(result.Success);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Dinero insuficiente", result.Message);
        }

        [Fact]
        public void BuyCoffee_ShouldReturnSuccess_WhenEnoughMoneyAndChangeAvailable()
        {
            var service = new CoffeeMachineService(_repo);
            var result = service.BuyCoffee(new OrderRequest
            {
                Order = new Dictionary<string, int> { { "Americano", 1 } },
                Payment = new Payment { TotalAmount = 1500 }
            });

            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            Assert.Contains("Su vuelto es de", result.Message);
            Assert.Equal(4, _repo.ProductInventory["Americano"]);
            Assert.Equal(1, _repo.CoinInventory[100]);
        }

        [Fact]
        public void BuyCoffee_ShouldReturnError_WhenCannotMakeChange()
        {
            _repo.CoinInventory[100] = 0;
            var service = new CoffeeMachineService(_repo);
            var result = service.BuyCoffee(new OrderRequest
            {
                Order = new Dictionary<string, int> { { "Americano", 1 } },
                Payment = new Payment { TotalAmount = 1500 }
            });

            Assert.False(result.Success);
            Assert.Equal(500, result.StatusCode);
            Assert.Equal("No hay suficiente cambio en la máquina.", result.Message);
        }
    }
}

