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
using ATMApplication.Filters;

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
            var user = HttpContext.User;
            if (!Guid.TryParse(cardId, out var id))
            {
                return BadRequest();
            }

            var CardRepository = RepositoryFactory.GetRepository<Card>();

            var cardInfo = await(CardRepository.GetAsync(card => card.Id.Equals(cardId)));

            throw new NotImplementedException();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ValidateGuidFormat("userId")]
        [HttpGet("/user/{userId}/cards")]
        public async Task<IActionResult> GetUserCards(string userId,
            [FromServices] IMapper mapper)
        {
            IEnumerable<CardViewModel> cardViews = null;

            try
            {
                var cards = await CardService.GetUserCards(userId);
                cardViews = mapper.Map<ICollection<Card>, IEnumerable<CardViewModel>>(cards);
            }
            catch
            {
                BadRequest("");
            }

            return Ok(cardViews);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCard(string userId, CardType cardType)
        {
            throw new NotImplementedException();
        }
    }
}