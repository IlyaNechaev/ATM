using Microsoft.AspNetCore.Mvc;
using ATMApplication.Data;
using AutoMapper;
using ATMApplication.Filters;
using ATMApplication.Initial.Filters;
using ATMApplication.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace ATMApplication.Controllers
{
    [ApiController]
    [Authorize]
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
        [HttpGet("info/{cardId}")]
        public async Task<IActionResult> GetCardInfo(string cardId)
        {
            var id = Guid.Parse(cardId);
            Card card = null;
            try
            {
                card = await CardService.GetCard(id);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

            var cardInfo = Mapper.Map<Card, CardViewModel>(card);

            return Ok(cardInfo);
        }

        [HttpGet("/user/cards")]
        public async Task<IActionResult> GetUserCards([FromServices] IMapper mapper)
        {
            var userId = GetUserId();
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
            string userId = GetUserId();
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

        [HttpPost("transfer")]
        public async Task<IActionResult> TransferMoney([FromBody] TransactionRequest request)
        {
            var userId = GetUserId();
            // Если карта, с которой должен быть осуществлен перевод не принадлежит текущему пользователю
            if (!(await CardService.GetUserCards(userId)).Any(card => card.CardNumber == request.SourceCardNumber))
            {
                Ok("Не удалось совершить перевод");
            }
            var sourceAccountTask = CardService.GetCardBankAccount(request.SourceCardNumber);
            var targetAccountTask = CardService.GetCardBankAccount(request.TargetCardNumber);

            try
            {
                await BankService.TransferMoney(await sourceAccountTask, await targetAccountTask, request.Sum);
            }
            catch(Exception ex)
            {
                return Ok(ex.Message);
            }

            return Ok();
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> DepositCash([FromBody] DepositWithdrawCashRequest request)
        {
            var response = new object();
            try
            {
                response = await DepositWithdrawCash(request, true);
            }
            catch
            {
                return Ok("Не удалось внести наличные");
            }

            return Ok(response);
        }
        
        [HttpPost("withdraw")]
        public async Task<IActionResult> WithdrawCash([FromBody] DepositWithdrawCashRequest request)
        {
            var response = new object();
            try
            {
                response = await DepositWithdrawCash(request, false);
            }
            catch
            {
                return Ok("Не удалось снять наличные");
            }

            return Ok(response);
        }

        [HttpPost("block")]
        public async Task<IActionResult> BlockCard([FromBody] BlockCardRequest request)
        {
            try
            {
                var card = await CardService.GetCard(request.CardNumber);
                await CardService.BlockCard(card);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

            return Ok();
        }

        private string GetUserId()
        {
            return User.GetClaim(ClaimKey.Id);
        }

        private async Task<object> DepositWithdrawCash(DepositWithdrawCashRequest request, bool deposit)
        {
            try
            {
                await CardService.DepositWithdrawCash(request.CardNumber, request.Sum, deposit);
            }
            catch (Exception ex)
            {
                throw;
            }

            var getCardTask = CardService.GetCard(request.CardNumber);
            var getAccountTask = CardService.GetCardBankAccount(request.CardNumber);
            var response = new
            {
                SourceCard = Mapper.Map<Card, CardViewModel>(await getCardTask),
                Sum = request.Sum,
                Balance = (await getAccountTask).Balance,
                DepositTime = DateTime.Now
            };

            return response;
        }
    }
}