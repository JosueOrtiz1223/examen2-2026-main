using System;
using System.Collections.Generic;
using System.Linq;
using ExamTwo.Repositories;
using ExamTwo.Models;

namespace ExamTwo.Services
{
    public class PurchaseResult
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }

    public interface ICoffeeMachineService
    {
        Dictionary<string, int> GetCoffeePrices();
        Dictionary<string, int> GetCoffeePricesInCents();
        Dictionary<int, int> GetCoinInventory();
        PurchaseResult BuyCoffee(OrderRequest request);
    }

    public class CoffeeMachineService : ICoffeeMachineService
    {
        private readonly ICoffeeMachineRepository _repo;

       public CoffeeMachineService(ICoffeeMachineRepository repo)
        {
            _repo = repo;
        }

    public Dictionary<string, int> GetCoffeePrices() => _repo.GetCoffeePrices();

    public Dictionary<string, int> GetCoffeePricesInCents() => _repo.GetCoffeePrices();

        public Dictionary<int, int> GetCoinInventory() => _repo.GetCoinInventory();
        public PurchaseResult BuyCoffee(OrderRequest request)
        {
            if (request?.Order == null || request.Order.Count == 0)
                return new PurchaseResult { Success = false, StatusCode = 400, Message = "Orden vacia." };

           if (request.Payment == null || request.Payment.TotalAmount <= 0)
               return new PurchaseResult { Success = false, StatusCode = 400, Message = "Dinero insuficiente" };

            try
            {
                var pricesInCents = _repo.GetCoffeePrices();
                var coinInventory = _repo.GetCoinInventory();

                var costoTotal = request.Order.Sum(o =>
                {
                    if (!pricesInCents.ContainsKey(o.Key))
                        throw new ArgumentException($"Producto {o.Key} no existe.");
                    return pricesInCents[o.Key] * o.Value;
                });

               if (request.Payment.TotalAmount < costoTotal)
                    return new PurchaseResult { Success = false, StatusCode = 400, Message = "Dinero insuficiente" };

                
                foreach (var cafe in request.Order)
                {
                    _repo.DecreaseCoffeeQuantity(cafe.Key, cafe.Value);
                }

                var change = request.Payment.TotalAmount - costoTotal;
                string result = $"Su vuelto es de: {change} colones. Desglose:";

               
                foreach (var coin in coinInventory.Keys.OrderByDescending(c => c))
                {
                    var available = coinInventory[coin];
                    var count = Math.Min(change / coin, available);
                    if (count > 0)
                    {
                        result += $" {count} moneda de {coin},";
                        change -= coin * count;
                        _repo.DecreaseCoinCount(coin, count);
                    }
                }

                if (change > 0)
                {
                    return new PurchaseResult { Success = false, StatusCode = 500, Message = "No hay suficiente cambio en la máquina." };
                }

                return new PurchaseResult { Success = true, StatusCode = 200, Message = result.TrimEnd(',') };
            }
            catch (ArgumentException ex)
            {
                return new PurchaseResult { Success = false, StatusCode = 400, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new PurchaseResult { Success = false, StatusCode = 500, Message = ex.Message };
            }
        }
    }
}
