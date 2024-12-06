using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using UniqloMvc.Constants;
using UniqloMvc.Helpers;
using UniqloMvc.Services.Abstracts;

namespace UniqloMvc.Services.Implements;

public class EmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;
    private readonly MailAddress _from;
    private readonly HttpContext _httpContext;

    public EmailService(IOptions<SmtpOptions> options, IHttpContextAccessor context)
    {
        SmtpOptions opt = options.Value;
        _from = new MailAddress(opt.Sender, "Ruslan Bayramov");

        _smtpClient = new SmtpClient(opt.Host, opt.Port);
        _smtpClient.EnableSsl = true;
        _smtpClient.Credentials = new NetworkCredential(opt.Sender, opt.Password);

        _httpContext = context.HttpContext!;
    }

    public Task SendEmailConfirmationAsync(string receiver, string userName, string token)
    {
        MailAddress to = new MailAddress(receiver);
        MailMessage msg = new MailMessage(_from, to);

        string url = _httpContext.Request.Scheme + "://" + _httpContext.Request.Host + "/Account/VerifyEmail" + $"?token={token}" + $"&user={userName}"; 

        msg.Body = EmailTemplates.ConfirmTemplate.Replace("__$userName", userName).Replace("__$verifyLink", url);
        msg.Subject = "Email Confirmation";
        msg.IsBodyHtml = true;

        _smtpClient.Send(msg);

        return Task.CompletedTask;
    }

    public Task SendForgotPasswordAsync(string receiver, string userName, string token)
    {
        MailAddress to = new MailAddress(receiver);
        MailMessage msg = new MailMessage(_from, to);

        string url = _httpContext.Request.Scheme + "://" + _httpContext.Request.Host + "/Account/ForgotPassword" + $"?token={token}" + $"&user={userName}";

        msg.Body = EmailTemplates.ForgotTemplate.Replace("__$userName", userName).Replace("__$verifyLink", url);
        msg.Subject = "Email Confirmation";
        msg.IsBodyHtml = true;

        _smtpClient.Send(msg);
        
        return Task.CompletedTask; 
    }
}
