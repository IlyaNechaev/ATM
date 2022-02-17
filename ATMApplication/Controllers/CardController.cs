using Microsoft.AspNetCore.Mvc;
using ATMApplication.Data;
using AutoMapper;
using ATMApplication.Filters;
using ATMApplication.Initial.Filters;

namespace ATMApplication.Controllers
{
    [ApiController]
    [Route("card")]
    public class CardController : Controller
    {
        ICardService CardService { get; set; }
        IUserService UserService { get; set; }
        IRepositoryFactory RepositoryFactory { get; set; }
        IMapper Mapper { get; set; }
        public CardController(ICardService cardService,
                              IUserService userService,
                              IRepositoryFactory repositoryFactory,
                              IMapper mapper)
        {
            CardService = cardService;
            UserService = userService;
            RepositoryFactory = repositoryFactory;
            Mapper = mapper;
        }

        [JwtAuthorize]
        [ValidateGuidFormat("cardId")]
        [HttpGet("{cardId}/info")]
        public async Task<IActionResult> GetCardInfo(string cardId)
        {
            var id = Guid.Parse(cardId);

            var card = await CardService.GetCardById(id);

            var cardInfo = Mapper.Map<Card, CardViewModel>(card);

            return Ok(cardInfo);
        }

        [JwtAuthorize]
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

        [ValidateGuidFormat("senderCardId", "recieverCardId")]
        [HttpPost("transfer")]
        public async Task<IActionResult> TransferMoney([FromBody] string senderCardId,
                                                       [FromBody] string recieverCardId,
                                                       [FromBody] decimal sum)
        {

        }
    }
}