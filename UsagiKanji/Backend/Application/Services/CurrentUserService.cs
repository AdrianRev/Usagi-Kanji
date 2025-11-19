using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Application.Services
{
	public class CurrentUserService : ICurrentUserService
	{
		private readonly IHttpContextAccessor _contextAccessor;

		public CurrentUserService(IHttpContextAccessor contextAccessor)
		{
			_contextAccessor = contextAccessor;
		}

		public Guid? UserId
		{
			get
			{
				var user = _contextAccessor.HttpContext?.User;

				var id = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
					  ?? user?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

				return Guid.TryParse(id, out var guid) ? guid : null;
			}
		}
	}
}
