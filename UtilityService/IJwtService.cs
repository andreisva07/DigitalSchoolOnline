using AppAPI.Models;

namespace AppAPI.UtilityService
{
    public interface IJwtService
    {
        string CreateJwt(User user, string cnp);

    }
}
