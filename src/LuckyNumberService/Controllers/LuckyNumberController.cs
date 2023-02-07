using LuckyNumberService.Repository;
using Microsoft.AspNetCore.Mvc;

namespace LuckyNumberService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LuckyNumberController : ControllerBase
    {
        private IRoundWinnerRepository _roundWinnerRepository;
        private readonly ILogger<LuckyNumberController> _logger;

        public LuckyNumberController(IRoundWinnerRepository roundWinnerRepository, ILogger<LuckyNumberController> logger)
        {
            _roundWinnerRepository = roundWinnerRepository;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetWinnerNumber/{roundId}/{minNumber}/{maxNumber}")]
        public IActionResult GetWinnerNumber(int roundId, int minNumber, int maxNumber)
        {
            _logger.LogInformation($"LuckyNumberController/GetWinnerNumber is called for roundId {roundId}, minNumber {minNumber}, maxNumber {maxNumber}");
            var winnerNumber = _roundWinnerRepository.GetWinnerNumber(roundId, minNumber, maxNumber);
            return new OkObjectResult(winnerNumber);
        }
    }
}
