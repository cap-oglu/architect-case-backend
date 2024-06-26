using FinancialManagementMVC.Models;
using FinancialManagementMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManagementMVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeRatesController : ControllerBase
    {
        private readonly IExchangeRateService _exchangeRateService;

        public ExchangeRatesController(IExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
        }

        [HttpGet("rate")]
        public ActionResult<ExchangeRate> Get(string baseCurrency, string targetCurrency)
        {
            var rate = _exchangeRateService.GetExchangeRate(baseCurrency, targetCurrency);
            return Ok(rate);
        }

        [HttpGet("all")]
        public ActionResult<ExchangeRate> GetAll()
        {
            
            return Ok();
        }

    }
}
