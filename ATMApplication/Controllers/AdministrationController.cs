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
    public class AdministrationController : Controller
    {
        IUserService UserService;
        IRepositoryFactory RepositoryFactory;

        public AdministrationController(
            [FromServices] IUserService userService,
            [FromServices] IRepositoryFactory repositoryFactory)
        {
            UserService = userService;
            RepositoryFactory = repositoryFactory;
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

            // Аутентификация пользователя
            (_, var token) = await UserService.SignInUser(model.Login, model.Password);

            return Ok(new { Token = token });
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginEditModel model)
        {
            // Проверяем, присутствуют ли в моделе ошибки
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            (var validationResult, var token) = await UserService.SignInUser(model.Login, model.Password);

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
                return BadRequest(ModelState);
            }

            return Ok(new { token = token });
        }
    }
}
