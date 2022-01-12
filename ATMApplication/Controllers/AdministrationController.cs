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

namespace ATMApplication.Controllers
{
    [ApiController]
    public class AdministrationController : ControllerBase
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
                registerResult.ErrorMessages.ForEach(error => ModelState.AddModelError(error.Key, error.Message));

                return BadRequest(ModelState);
            }

            (_, _, var token) = await UserService.SignInUser(model.Login, model.Password);

            return Ok(new { Token = token });
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginEditModel model)
        {
            // Проверяем, присутствуют ли в моделе ошибки
            if (ModelState.IsValid)
            {
                var result = await UserService.SignInUser(model.Login, model.Password);

                // Если во время аутентификации были получены ошибки
                if (result.HasErrors)
                {
                    // Добавить все ошибки в ModelState
                    result.ErrorMessages.ForEach(error => ModelState.AddModelError(error.Key, error.Message));
                    return BadRequest(ModelState);
                }
                else
                {
                    return Ok(new { token = result.Token });
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}
