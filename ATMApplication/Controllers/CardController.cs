using ATMApplication.Extensions;
using ATMApplication.Services;
using System.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Mvc = Microsoft.AspNetCore.Mvc;
using ATMApplication.Models;

namespace ATMApplication.Controllers
{
    [ApiController]
    [Mvc.Route("card")]
    public class CardController : ControllerBase
    {
        ICardService CardService { get; set; }
        IUserService UserService { get; set; }
        public CardController(ICardService cardService,
                              IUserService userService)
        {
            CardService = cardService;
            UserService = userService;
        }

        [Mvc.HttpGet("info")]
        public async Task<HttpResponseMessage> GetCard(Guid cardId)
        {
            throw new NotImplementedException();
        }

        [Mvc.HttpGet("/user/cards")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Card>>> GetUserCards()
        {
            var u = HttpContext.User;
            var f = HttpContext.User.GetClaim(ClaimKey.FirstName);

            if (string.IsNullOrEmpty(f))
            {
                return BadRequest();
            }

            var userId = new Guid(HttpContext.User.GetClaim(ClaimKey.Id));

            var user = await UserService.GetUserById(userId);

            var cards = new List<Card>
            {
                new Card()
                {
                    CardNumber = 1111_2222_3333_4444,
                    CardType = CardType.DEBIT,
                    Id = Guid.NewGuid(),
                    Owner = user,
                    MonthYear = DateTime.Now,
                    OwnerName = $"{user.FirstName} {user.MiddleName}",
                    Hash = "1"
                },
                new Card()
                {
                    CardNumber = 1110_2222_3333_4444,
                    CardType = CardType.CREDIT,
                    Id = Guid.NewGuid(),
                    Owner = user,
                    MonthYear = DateTime.Now,
                    OwnerName = $"{user.FirstName} {user.MiddleName}",
                    Hash = "2"
                }
            };

            return cards;
        }

        [Mvc.HttpPost("create")]
        public async Task<HttpResponseMessage> CreateCard()
        {
            throw new NotImplementedException();
        }
    }
}
