﻿using Dapr;
using Microsoft.AspNetCore.Mvc;

namespace WinnerSubscriberService.Controllers
{
    [ApiController]
    public class WinnerController : Controller
    {
        private readonly ILogger<WinnerController> _logger;

        public WinnerController(ILogger<WinnerController> logger)
        {
            _logger = logger;
        }

        [Topic("pubsub", "newWinner")]
        [HttpPost("winner")]
        public void CreateWinner([FromBody] string winnerName)
        {
            _logger.LogInformation($"Subscriber recieved winner name : {winnerName}");
        }
    }
}
