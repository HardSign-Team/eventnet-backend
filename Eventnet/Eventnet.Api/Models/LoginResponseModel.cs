using Eventnet.DataAccess;

namespace Eventnet.Models;

public class LoginResponseModel
{
    public string AccessToken { get; set; }
    public DateTime ExpiredAt { get; set; }
    public ApplicationUser User { get; set; }
    public IList<string> UserRoles { get; set; }
}