using ATMApplication.Models;
using ATMApplication.Services;
using ATMApplication.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using System.Web.Http;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace ATMApplication.Controllers
{
    public class SignInController : Controller
    {
        IUserService UserService;
        IRepositoryFactory RepositoryFactory;
        SignInManager SignInManager;

        public SignInController(
            [FromServices] IUserService userService,
            [FromServices] IRepositoryFactory repositoryFactory,
            [FromServices] SignInManager signInManager)
        {
            UserService = userService;
            RepositoryFactory = repositoryFactory;
            SignInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register(RegisterEditModel model)
        {
            // Присутствуют ли в моделе ошибки
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Регистрация пользователя
            var registerResult = await UserService.RegisterUser(model);

            // Если во время регистрации были получены ошибки
            if (registerResult.HasErrors)
            {
                // Добавить все ошибки в ModelState
                foreach (var kvp in registerResult.ErrorMessages)
                {
                    foreach (var message in kvp.Value)
                    {
                        ModelState.AddModelError(kvp.Key, message);
                    }
                }

                return BadRequest(ModelState);
            }
            
            return Ok($"{model.FirstName} успешно {(model.Gender == Gender.MALE ? "зарегистрирован" : "зарегистрирована")}");
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginEditModel model)
        {
            // Проверяем, присутствуют ли в моделе ошибки
            if (!ModelState.IsValid)
            {
                return Ok(ModelState);
            }

            (var validationResult, var user) = await UserService.GetLogInUser(model.Login, model.Password);

            // Если во время аутентификации были получены ошибки
            if (validationResult.HasErrors)
            {
                // Добавить все ошибки в ModelState
                foreach (var kvp in validationResult.ErrorMessages)
                {
                    foreach (var message in kvp.Value)
                    {
                        ModelState.AddModelError(kvp.Key, message);
                    }
                }

                foreach (var message in validationResult.CommonMessages)
                {
                    ModelState.AddModelError(string.Empty, message);
                }

                return Ok(ModelState);
            }

            var principal = await UserService.CreateUserPrincipal(user);

            await SignInManager.LogIn().UsingClaims(HttpContext, principal);
            
            return Ok();
        }

        [HttpPost("/logout")]
        public async Task<IActionResult> Logout()
        {
            await SignInManager.LogOut().UsingClaims(HttpContext);

            return Ok();
        }
    }
}
