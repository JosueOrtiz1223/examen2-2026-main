using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExamTwo.Controllers
{
    public class CoffeeMachineController : Controller
    {

        private readonly Database _db;

        public CoffeeMachineController(Database db)
        {
            _db = db;
        }

        [HttpGet("getCoffees")]
        public ActionResult<Dictionary<string, int>> GetCoffeePrices()
        {
            return Ok(_db.keyValues);
        }

        [HttpGet("getCoffeePricesInCents")]
        public ActionResult<Dictionary<string, int>> GetCoffeePricesInCents()
        {
            return Ok(_db.keyValues2);
        }

        [HttpGet("getQuantity")]
        public ActionResult<Dictionary<string, int>> GetQuantity()
        {
            return Ok(_db.keyValues3);
        }

        [HttpPost("buyCoffee")]
        public ActionResult<string> BuyCoffee([FromBody] OrderRequest request)
        {
            if (request.Order == null || request.Order.Count == 0)
                return BadRequest("Ordem vacia.");

            if (request.Payment.TotalAmount <= 0)
                return BadRequest("Dinero insuficiente ");

            try
            {
                var costoTotal = request.Order.Sum(o => _db.keyValues2.First(c => c.Key == o.Key).Value * o.Value);

                if (request.Payment.TotalAmount < costoTotal)
                { 
                    return BadRequest("Dinero insuficiente ");
                }


                foreach (var cafe in request.Order)
                {
                    var selected = _db.keyValues.First(c => c.Key == cafe.Key).Key;
                    if (cafe.Value > _db.keyValues[selected])
                    {
                        return $"No hay suficientes {selected} en la máquina.";
                    }
                    _db.keyValues[selected] -= cafe.Value;
                }

                var change = request.Payment.TotalAmount - costoTotal;
                String result = $"Su vuelto es de: {change} colones. Desglose:";

                foreach (var coin in _db.keyValues3.Keys.OrderByDescending(c => c))
                {
                    var count = Math.Min(change / coin, _db.keyValues3[coin]);
                    if (count > 0)
                    {
                        result +=  $" {count} moneda de {coin},  ";              
                        change -= coin * count;
                    }
                }


                if (change > 0)
                {
                    return StatusCode(500, "No hay suficiente cambio en la máquina.");
                }

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

    public class OrderRequest
    {
        public Dictionary<string, int> Order { get; set; }
        public Payment Payment { get; set; }
    }

    public class Payment
    {
        public int TotalAmount { get; set; }
        public List<int> Coins { get; set; }
        public List<int> Bills { get; set; }
    }
}
