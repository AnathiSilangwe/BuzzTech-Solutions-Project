using BusinessBuzz.Core.Entities;

namespace BusinessBuzz.Core.Services;

public interface IAuthenticationService
{
    Task<SystemUser?> RegisterAsync(
        string email,
        string password,
        int roleId);

    Task<SystemUser?> LoginAsync(
        string email,
        string password);
}