using CapWebAPI.Services;
using System.Security.Claims;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerProducer _customerProducer;

        public CustomerController(CustomerProducer customerProducer)
        {
            _customerProducer = customerProducer;
        }

        [HttpPost("produce/{count:int}")]
        [Authorize(Policy = "isAdmin")]
        public async Task<IActionResult> ProduceCustomers(int count)
        {
            await _customerProducer.ProduceCustomersAsync(count);
            return Ok($"{count} müşteri kuyruğa eklendi.");
        }

        [HttpPost("hi")]
        [Authorize(Policy ="isEmployee")]
        public async Task <ActionResult> HiMessage(string name) {
            await _customerProducer.HiMessageAsync(name);
            return Ok($"Merhaba {name}");
        }
    }
}

