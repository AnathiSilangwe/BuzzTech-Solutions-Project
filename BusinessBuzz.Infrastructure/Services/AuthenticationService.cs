using BusinessBuzz.Core.Entities;
using BusinessBuzz.Core.Services;
using BusinessBuzz.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BusinessBuzz.Infrastructure.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly BusinessBuzzDbContext _context;
    private readonly Microsoft.AspNetCore.Identity.PasswordHasher<SystemUser> _passwordHasher;

    public AuthenticationService(BusinessBuzzDbContext context)
    {
        _context = context;
        _passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<SystemUser>();
    }

    public async Task<SystemUser?> RegisterAsync(
        string name,
        string email,
        string password,
        int roleId)
    {
        var existingUser = await _context.SystemUsers
            .FirstOrDefaultAsync(u => u.Email == email);

        if (existingUser != null)
        {
            return null;
        }

        var user = new SystemUser
        {
            Name = name.Trim(),
            Email = email.Trim(),
            RoleId = roleId
        };

        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            password);

        _context.SystemUsers.Add(user);

        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<SystemUser?> LoginAsync(
        string email,
        string password)
    {
        var user = await _context.SystemUsers
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            return null;
        }

        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            password);

        if (result == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed)
        {
            return null;
        }

        return user;
    }
}