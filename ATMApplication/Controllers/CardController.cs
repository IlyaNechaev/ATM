using Microsoft.AspNetCore.Mvc;
using ATMApplication.Data;
using AutoMapper;
using ATMApplication.Filters;
using ATMApplication.Initial.Filters;

namespace ATMApplication.Controllers
{
    [ApiController]
    [JwtAuthorize]
    [Route("card")]
    public class CardController : Controller
    {
        ICardService CardService { get; set; }
        IUserService UserService { get; set; }
        IRepositoryFactory RepositoryFactory { get; set; }
        IMapper Mapper { get; set; }
        IBankService BankService { get; set; }
        public CardController(ICardService cardService,
                              IUserService userService,
                              IRepositoryFactory repositoryFactory,
                              IBankService bankService,
                              IMapper mapper)
        {
            CardService = cardService;
            UserService = userService;
            RepositoryFactory = repositoryFactory;
            Mapper = mapper;
            BankService = bankService;
        }

        [ValidateGuidFormat("cardId")]
        [HttpGet("{cardId}/info")]
        public async Task<IActionResult> GetCardInfo(string cardId)
        {
            var id = Guid.Parse(cardId);

            var card = await CardService.GetCardById(id);

            var cardInfo = Mapper.Map<Card, CardViewModel>(card);

            return Ok(cardInfo);
        }

        [ValidateGuidFormat("userId")]
        [HttpGet("/user/{userId}/cards")]
        public async Task<IActionResult> GetUserCards(string userId,
            [FromServices] IMapper mapper)
        {
            IEnumerable<CardViewModel> cardViews = null;

            try
            {
                var cards = await CardService.GetUserCards(userId);
                cardViews = mapper.Map<IEnumerable<Card>, IEnumerable<CardViewModel>>(cards);
            }
            catch (Exception ex)
            {
                Ok(ex.Message);
            }

            return Ok(cardViews);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCard([FromBody] CreateCardRequest request)
        {
            string userId = request.UserId;
            BankAccountType accountType = request.BankAccountType;

            var UserRepository = RepositoryFactory.GetRepository<User>();
            var user = await UserRepository.GetSingleAsync(user => Guid.Parse(userId) == user.Id);
            var card = new CardEditModel();

            try
            {
                card = await CardService.CreateCardForUser(user, accountType);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

            return Ok(card);
        }

        [ValidateGuidFormat("senderCardId", "recieverCardId")]
        [HttpPost("transfer")]
        public async Task<IActionResult> TransferMoney(string senderCardId,
                                                       string recieverCardId,
                                                       decimal sum)
        {
            var senderAccountTask = CardService.GetCardBankAccount(senderCardId);
            var receiverAccountTask = CardService.GetCardBankAccount(recieverCardId);

            try
            {
                await BankService.TransferMoney(await senderAccountTask, await receiverAccountTask, sum);
            }
            catch(Exception ex)
            {
                return Ok(ex.Message);
            }

            return Ok();
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> DepositMoney([FromBody] DepositWithdrawCash request)
        {
            try
            {
                await CardService.DepositWithdrawCash(request.CardId, request.Sum, true);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

            var card = await RepositoryFactory.GetRepository<Card>().GetSingleAsync(card => card.Id == Guid.Parse(request.CardId));
            var accountBalance = (await RepositoryFactory.GetRepository<BankAccount>().GetSingleAsync(account => account.Id == card.AccountId)).Balance;
            var response = new
            {
                SourceCard = Mapper.Map<Card, CardViewModel>(card),
                Sum = request.Sum,
                Balance = accountBalance,
                DepositTime = DateTime.Now
            };

            return Ok(response);
        }
        
        [HttpPost("withdraw")]
        public async Task<IActionResult> WithdrawMoney([FromBody] DepositWithdrawCash request)
        {
            try
            {
                await CardService.DepositWithdrawCash(request.CardId, request.Sum, false);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

            var card = await RepositoryFactory.GetRepository<Card>().GetSingleAsync(card => card.Id == Guid.Parse(request.CardId));
            var accountBalance = (await RepositoryFactory.GetRepository<BankAccount>().GetSingleAsync(account => account.Id == card.AccountId)).Balance;
            var response = new
            {
                SourceCard = Mapper.Map<Card, CardViewModel>(card),
                Sum = request.Sum,
                Balance = accountBalance,
                WithdrawTime = DateTime.Now
            };

            return Ok(response);
        }
    }
}