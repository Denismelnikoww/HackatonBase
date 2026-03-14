using Domain.Models.User;
using Infrastructure.DbContexts;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Application.Services
{
    //public class ResetPasswordService : IResetPasswordService
    //{
    //    private readonly IVerificationCacheProvider _cacheProvider;
    //    private readonly IEmailService _emailService;
    //    private readonly IEmailTemplateBuilder _emailTemplateBuilder;
    //    private readonly IPasswordHasher _passwordHasher;
    //    private readonly UserDbContext _context;
    //    private readonly DbSet<User> _users;

    //    public ResetPasswordService(IEmailTemplateBuilder emailTemplateBuilder,
    //        IPasswordHasher passwordHasher,
    //        IVerificationCacheProvider cacheProvider,
    //        IEmailService emailService,
    //        UserDbContext context)
    //    {
    //        _emailTemplateBuilder = emailTemplateBuilder;
    //        _passwordHasher = passwordHasher;
    //        _context = context;
    //        _cacheProvider = cacheProvider;
    //        _emailService = emailService;
    //    }

        //public async Task SendLink(string email, CancellationToken ct)
        //{
        //    var user = await _users.AsNoTracking()
        //        .FirstOrDefaultAsync(x => x.Email == email);

        //    if (user == null)
        //        throw new Exception();

        //    _cacheProvider.GenerateToken()
        //    var html = "";

        //    await _emailService.SendAsync(email, "Смена пароля", html, ct);
        //}

        //public async Task ResetPassword(string email, string token,
        //    string password, CancellationToken ct)
        //{
        //    if (await _tokensProvider.VerifyPasswordResetToken(email, token, ct))
        //        throw new Exception();

        //    var user = await _users.FirstOrDefaultAsync(u => u.Email == email);
        //    user.PasswordHash = _passwordHasher.Hash(password);
        //    await _context.SaveChangesAsync();
        //    await _emailService.SendAsync(email, "Пароль успешно сброшен", "текст", ct);
        //}
    //}
}