using System.Linq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using SCM.Model;
using SRRC.Service.Repository.Authentication;
using SRRC.UOW;
using SRRC.Common;
using SRRC.Model;
using SRRC.DomainClasses.Entities.Authentication;

namespace SRRC.Web.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("CorsPolicy")]
    [Authorize(Policy = CustomRoles.Admin)]
    public class AccountController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly ITokenStoreService _tokenStoreService;
        private readonly IUnitOfWork _uow;
        private readonly IAntiForgeryCookieService _antiforgery;
        private readonly ITokenFactoryService _tokenFactoryService;
        
        public AccountController(
            IUsersService usersService,
            ITokenStoreService tokenStoreService,
            ITokenFactoryService tokenFactoryService,
            IUnitOfWork uow,
            IAntiForgeryCookieService antiforgery,
            ISecurityService securityService)
        {
            _usersService = usersService;
            _usersService.CheckArgumentIsNull(nameof(usersService));

            _tokenStoreService = tokenStoreService;
            _tokenStoreService.CheckArgumentIsNull(nameof(tokenStoreService));

            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(_uow));

            _antiforgery = antiforgery;
            _antiforgery.CheckArgumentIsNull(nameof(antiforgery));

            _tokenFactoryService = tokenFactoryService;
            _tokenFactoryService.CheckArgumentIsNull(nameof(tokenFactoryService));
        }

        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        [HttpPost("[action]")]
        // [ValidateCaptcha(ErrorMessage = "متن نوشته شده در تصویر را به عدد وارد کنید",
        //                     IsNumericErrorMessage = "تصویر را به عدد وارد کنید",
        //                     CaptchaGeneratorLanguage = Language.Persian)]
        public async Task<IActionResult> Login([FromBody] LoginUserModel loginUser)
        {

            if (loginUser == null)
            {
                return BadRequest("نام کاربری و کلمه عبور را وارد کنید.");
            }
            // if (!ModelState.IsValid) // If `ValidateDNTCaptcha` fails, it will set a `ModelState.AddModelError`.
            // {
            //     return BadRequest("کد امنیتی وارد شده صحیح نمی باشد");
            // }
            var user = await _usersService.FindUserAsync(loginUser.Username, loginUser.Password);
            if (user == null || !user.IsActive)
            {
                return Unauthorized();
            }
            try
            {
                var result = await _tokenFactoryService.CreateJwtTokensAsync(user);
                await _tokenStoreService.AddUserTokenAsync(user, result.RefreshTokenSerial, result.AccessToken, null);
                await _uow.SaveChangesAsync();

                _antiforgery.RegenerateAntiForgeryCookies(result.Claims);

                return Ok(new { access_token = result.AccessToken, refresh_token = result.RefreshToken });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> RefreshToken([FromBody] JToken jsonBody)
        {
            var refreshTokenValue = jsonBody.Value<string>("refreshToken");
            if (string.IsNullOrWhiteSpace(refreshTokenValue))
            {
                return BadRequest("refreshToken is not set.");
            }

            var token = await _tokenStoreService.FindTokenAsync(refreshTokenValue);
            if (token == null)
            {
                return Unauthorized();
            }

            var result = await _tokenFactoryService.CreateJwtTokensAsync(token.User);
            await _tokenStoreService.AddUserTokenAsync(token.User, result.RefreshTokenSerial, result.AccessToken, _tokenFactoryService.GetRefreshTokenSerial(refreshTokenValue));
            await _uow.SaveChangesAsync();

            _antiforgery.RegenerateAntiForgeryCookies(result.Claims);

            return Ok(new { access_token = result.AccessToken, refresh_token = result.RefreshToken });
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<bool> Logout(string refreshToken)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userIdValue = claimsIdentity.FindFirst(ClaimTypes.UserData)?.Value;

            // The Jwt implementation does not support "revoke OAuth token" (logout) by design.
            // Delete the user's tokens from the database (revoke its bearer token)
            await _tokenStoreService.RevokeUserBearerTokensAsync(userIdValue, refreshToken);
            await _uow.SaveChangesAsync();

            _antiforgery.DeleteAntiForgeryCookies();

            return true;
        }


        [HttpPost("[action]")]
        public async Task<User> RegisterAdmin([FromBody] RegisterModel register)
        {
            User user = new User
            {
                Username = register.Username,
                Password = register.Password,
                DisplayName = register.DisplayName,
                SerialNumber = Guid.NewGuid().ToString(),
                IsActive = register.IsActive
            };
            await _usersService.RegisterAdmin(user);
            await _uow.SaveChangesAsync();

            if (register.IsAdmin)
            {
                _usersService.AddAdminUserRole(user.Id);
                await _uow.SaveChangesAsync();
            }
            
            if (register.Groups != null)
            {
                foreach (var item in register.Groups)
                {
                    await _uow.UserGroups.AddAsync(new UserGroup
                    {
                        GroupId = item,
                        UserId = user.Id
                    });
                }
            }

            await _uow.SaveChangesAsync();
            return user;
        }
        public async Task<IActionResult> ValidateUserName(string userName)
        {
            if (!await UserExists(userName))
            {
                return Json(true);
            }
            else
            {
                return Json("قبلا کاربری یا این نام کاربری تعریف شده است");
            }
        }
        [HttpGet("[action]"), HttpPost("[action]")]
        public bool IsAuthenticated()
        {
            return User.Identity.IsAuthenticated;
        }

        [HttpGet("[action]"), HttpPost("[action]")]
        public IActionResult GetUserInfo()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            return Json(new { Username = claimsIdentity.Name });
        }

        // GET: api/Users
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUseSRRCsync()
        {
            return await _usersService.GetUseSRRCsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RegisterModel>> GetUser(int id)
        {
            var user = await _usersService.FindUserAsync(id);
            var userGroups = _uow.UserGroups
                .GetAll()
                .Where(x => x.UserId == id)
                .Select(x => x.GroupId).ToArray();
            
            RegisterModel m = new RegisterModel
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                IsActive = user.IsActive,
                Password = user.Password,
                Username = user.Username,
                Groups = userGroups
            };

            m.IsAdmin = _usersService.IsAdminUser(id);

            if (user == null)
            {
                return NotFound();
            }

            return m;
        }

        // PUT: api/Users/5

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRegister(int id, [FromBody] RegisterModel user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }
            var currentUser = await _usersService.FindUserAsync(id);


            currentUser.IsActive = user.IsActive;
            currentUser.DisplayName = user.DisplayName;

            if (user.IsAdmin)
            {
                if (!_usersService.IsAdminUser(id))
                {
                    _usersService.AddAdminUserRole(id);
                }
            }
            else
            {
                if (_usersService.IsAdminUser(id))
                {
                    _usersService.DeleteAdminUserRole(id);
                }
            }

            List<UserGroup> usergroupList = _uow.UserGroups.GetAll().Where(x => x.UserId == id).ToList();
            foreach (var item in usergroupList.ToList())
            {
                _uow.UserGroups.Remove(item);
            }
            if (user.Groups != null)
            {
                foreach (var item in user.Groups)
                {
                    await _uow.UserGroups.AddAsync(new UserGroup
                    {
                        GroupId = item,
                        UserId = user.Id
                    });
                }
                _usersService.Update(currentUser);
            }
            
            try
            {
                await _uow.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // if (!UserExists(id))
                // {
                //     return NotFound();
                // }
                // else
                // {
                //     throw;
                // }
            }
            catch (Exception)
            {
                // ignored
            }

            return NoContent();
        }


        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _usersService.FindUserAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            _usersService.Remove(user);
            await _uow.SaveChangesAsync();
            return user;
        }

        private async Task<bool> UserExists(string username)
        {
            var users = await _usersService.GetUseSRRCsync();
            var model = users.Where(e => e.Username == username);
            if (model.Count() > 0)
                return true;
            else
                return false;
        }

    }

}