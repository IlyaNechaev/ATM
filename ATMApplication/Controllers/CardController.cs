using ATMApplication.Extensions;
using ATMApplication.Services;
using ATMApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ATMApplication.Data;
using Microsoft.AspNetCore.Identity;
using AutoMapper;

namespace ATMApplication.Controllers
{
    [ApiController]
    [Route("card")]
    public class CardController : Controller
    {
        ICardService CardService { get; set; }
        IUserService UserService { get; set; }
        IRepositoryFactory RepositoryFactory { get; set; }
        public CardController(ICardService cardService,
                              IUserService userService,
                              IRepositoryFactory repositoryFactory)
        {
            CardService = cardService;
            UserService = userService;
            RepositoryFactory = repositoryFactory;
        }

        [HttpGet("{cardId}/info")]
        public async Task<IActionResult> GetCardInfo(string cardId)
        {
            if (!Guid.TryParse(cardId, out var id))
            {
                return BadRequest();
            }

            var CardRepository = RepositoryFactory.GetRepository<Card>();

            var cardInfo = await(CardRepository.GetAsync(card => card.Id.Equals(cardId)));

            throw new NotImplementedException();
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("/user/{userId}/cards")]
        public async Task<IActionResult> GetUserCards(string userId,
            [FromServices] IMapper mapper)
        {
            if (!Guid.TryParse(userId, out var id))
            {
                return BadRequest();
            }

            var user = await UserService.GetUserById(id);

            var cards = new List<Card>
            {
                new Card()
                {
                    CardNumber = 1111_2222_3333_4444,
                    CardType = CardType.DEBIT,
                    Id = Guid.NewGuid(),
                    Owner = user,
                    MonthYear = DateTime.Now,
                    OwnerName = $"{user.FirstName} {user.MiddleName}"
                },
                new Card()
                {
                    CardNumber = 1110_2222_3333_4444,
                    CardType = CardType.CREDIT,
                    Id = Guid.NewGuid(),
                    Owner = user,
                    MonthYear = DateTime.Now,
                    OwnerName = $"{user.FirstName} {user.MiddleName}"
                }
            };

            var cardViews = mapper.Map<List<Card>, IEnumerable<CardViewModel>>(cards);

            return Ok(cardViews);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCard(string userId, CardType cardType)
        {
            throw new NotImplementedException();
        }
    }
}
