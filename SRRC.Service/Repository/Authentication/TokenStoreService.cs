using System;
using System.Linq;
using System.Threading.Tasks;

using SRRC.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SRRC.DataLayer.Database;
using SRRC.DomainClasses.Entities.Authentication;

namespace SRRC.Service.Repository.Authentication
{
    public interface ITokenStoreService
    {
        Task AddUserTokenAsync(UserToken userToken);
        Task AddUserTokenAsync(User user, string refreshTokenSerial, string accessToken, string refreshTokenSourceSerial);
        Task<bool> IsValidTokenAsync(string accessToken, int userId);
        Task DeleteExpiredTokensAsync();
        Task<UserToken> FindTokenAsync(string refreshTokenValue);
        Task DeleteTokenAsync(string refreshTokenValue);
        Task DeleteTokensWithSameRefreshTokenSourceAsync(string refreshTokenIdHashSource);
        Task InvalidateUserTokensAsync(int userId);
        Task RevokeUserBearerTokensAsync(string userIdValue, string refreshTokenValue);
    }

    public class TokenStoreService : ITokenStoreService
    {
        private readonly ISecurityService _securityService;
        private readonly DbSet<UserToken> _tokens;
        private readonly IOptionsSnapshot<BearerTokensOptions> _configuration;
        private readonly ITokenFactoryService _tokenFactoryService;

        public TokenStoreService(
            SRRCDbContext context,
            ISecurityService securityService,
            IOptionsSnapshot<BearerTokensOptions> configuration,
            ITokenFactoryService tokenFactoryService)
        {
            var context1 = context;
            context1.CheckArgumentIsNull(nameof(context1));

            _securityService = securityService;
            _securityService.CheckArgumentIsNull(nameof(_securityService));

            _tokens = context1.Set<UserToken>();

            _configuration = configuration;
            _configuration.CheckArgumentIsNull(nameof(configuration));

            _tokenFactoryService = tokenFactoryService;
            _tokenFactoryService.CheckArgumentIsNull(nameof(tokenFactoryService));
        }

        public async Task AddUserTokenAsync(UserToken userToken)
        {
            if (!_configuration.Value.AllowMultipleLoginsFromTheSameUser)
            {
                await InvalidateUserTokensAsync(userToken.UserId);
            }
            await DeleteTokensWithSameRefreshTokenSourceAsync(userToken.RefreshTokenIdHashSource);
            _tokens.Add(userToken);
        }

        public async Task AddUserTokenAsync(User user, string refreshTokenSerial, string accessToken, string refreshTokenSourceSerial)
        {
            var now = DateTimeOffset.UtcNow;
            var token = new UserToken
            {
                UserId = user.Id,
                // Refresh token handles should be treated as secrets and should be stored hashed
                RefreshTokenIdHash = _securityService.GetSha256Hash(refreshTokenSerial),
                RefreshTokenIdHashSource = string.IsNullOrWhiteSpace(refreshTokenSourceSerial) ?
                                           null : _securityService.GetSha256Hash(refreshTokenSourceSerial),
                AccessTokenHash = _securityService.GetSha256Hash(accessToken),
                RefreshTokenExpiresDateTime = now.AddMinutes(_configuration.Value.RefreshTokenExpirationMinutes),
                AccessTokenExpiresDateTime = now.AddMinutes(_configuration.Value.AccessTokenExpirationMinutes)
            };
            await AddUserTokenAsync(token);
        }

        public async Task DeleteExpiredTokensAsync()
        {
            var now = DateTimeOffset.UtcNow;
            await _tokens.Where(x => x.RefreshTokenExpiresDateTime < now)
                         .ForEachAsync(userToken =>
                         {
                             _tokens.Remove(userToken);
                         });
        }

        public async Task DeleteTokenAsync(string refreshTokenValue)
        {
            var token = await FindTokenAsync(refreshTokenValue);
            if (token != null)
            {
                _tokens.Remove(token);
            }
        }

        public async Task DeleteTokensWithSameRefreshTokenSourceAsync(string refreshTokenIdHashSource)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenIdHashSource))
            {
                return;
            }
            await _tokens.Where(t => t.RefreshTokenIdHashSource == refreshTokenIdHashSource)
                         .ForEachAsync(userToken =>
                         {
                             _tokens.Remove(userToken);
                         });
        }

        public async Task RevokeUserBearerTokensAsync(string userIdValue, string refreshTokenValue)
        {
            if (!string.IsNullOrWhiteSpace(userIdValue) && int.TryParse(userIdValue, out int userId))
            {
                if (_configuration.Value.AllowSignoutAllUserActiveClients)
                {
                    await InvalidateUserTokensAsync(userId);
                }
            }

            if (!string.IsNullOrWhiteSpace(refreshTokenValue))
            {
                var refreshTokenSerial = _tokenFactoryService.GetRefreshTokenSerial(refreshTokenValue);
                if (!string.IsNullOrWhiteSpace(refreshTokenSerial))
                {
                    var refreshTokenIdHashSource = _securityService.GetSha256Hash(refreshTokenSerial);
                    await DeleteTokensWithSameRefreshTokenSourceAsync(refreshTokenIdHashSource);
                }
            }

            await DeleteExpiredTokensAsync();
        }

        public Task<UserToken> FindTokenAsync(string refreshTokenValue)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenValue))
            {
                return null;
            }

            var refreshTokenSerial = _tokenFactoryService.GetRefreshTokenSerial(refreshTokenValue);
            if (string.IsNullOrWhiteSpace(refreshTokenSerial))
            {
                return null;
            }

            var refreshTokenIdHash = _securityService.GetSha256Hash(refreshTokenSerial);
            return _tokens.Include(x => x.User).FirstOrDefaultAsync(x => x.RefreshTokenIdHash == refreshTokenIdHash);
        }

        public async Task InvalidateUserTokensAsync(int userId)
        {
            await _tokens.Where(x => x.UserId == userId)
                         .ForEachAsync(userToken =>
                         {
                             _tokens.Remove(userToken);
                         });
        }

        public async Task<bool> IsValidTokenAsync(string accessToken, int userId)
        {
            var accessTokenHash = _securityService.GetSha256Hash(accessToken);
            var userToken = await _tokens.FirstOrDefaultAsync(
                x => x.AccessTokenHash == accessTokenHash && x.UserId == userId);
            return userToken?.AccessTokenExpiresDateTime >= DateTimeOffset.UtcNow;
        }
    }
}