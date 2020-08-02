using Codenation.Challenge.Models;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
 
namespace Codenation.Challenge.Services
{
    public class PasswordValidatorService: IResourceOwnerPasswordValidator
    {
        private readonly CodenationContext _dbContext;

        public PasswordValidatorService(CodenationContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var validation = _dbContext.Users
                .Where(x => x.Password == context.Password &&
                x.Email == context.UserName)
                .AsNoTracking()
                .FirstOrDefault();

            if (validation == null)
            {
                context.Result = new GrantValidationResult(
                TokenRequestErrors.InvalidGrant, "Invalid username or password");
            }
            else
            {
                context.Result = new GrantValidationResult(
                    subject: validation.Id.ToString(),
                    authenticationMethod: "custom",
                    claims: UserProfileService.GetUserClaims(validation));
            }       

            return Task.CompletedTask;
        }
     
    }
}

