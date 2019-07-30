using RobloxAutomatization.Models;

namespace RobloxAutomatization.Services
{
    public interface IRobloxAutomatizationService
    {
        void ConfigureAntichatExtension();
        void OpenChrome();
        void RegisterNewUser(ref RobloxUser user, bool isCustomUsername = false);
        void SaveUserCookies(string username);
    }
}
