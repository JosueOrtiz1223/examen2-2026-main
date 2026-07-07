using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
// using ExamTwo.Repositories;
// using ExamTwo.Services;
//using ExamTwo.Models;

namespace ExamTwo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoffeeMachineController : ControllerBase
    {
        private readonly ICoffeeMachineRepository _repo;
        private readonly ICoffeeMachineService _service;

        public CoffeeMachineController(ICoffeeMachineRepository repo, ICoffeeMachineService service)
        {
            _repo = repo;
            _service = service;
        }

        [HttpGet("Coffees")]
        public ActionResult<Dictionary<string, int>> GetCoffee()
        {
            return Ok(_repo.GetCoffeeTypes());
        }

        [HttpGet("CoffeePrices")]
        public ActionResult<Dictionary<string, int>> GetCoffeePrices()
        {
            return Ok(_repo.GetCoffeePrices());
        }

        [HttpGet("Quantity")]
        public ActionResult<Dictionary<int, int>> GetQuantity()
        {
            return Ok(_repo.GetCoinInventory());
        }

        [HttpPost("buyCoffee")]
        public ActionResult<string> BuyCoffee([FromBody] OrderRequest request)
        {
            var result = _service.BuyCoffee(request);
            if (result.Success)
                return Ok(result.Message);

            return StatusCode(result.StatusCode, result.Message);
        }
    }
}
