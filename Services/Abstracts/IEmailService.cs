﻿namespace UniqloMvc.Services.Abstracts;

public interface IEmailService
{
    Task SendEmailConfirmationAsync(string receiver, string userName, string token);
    Task SendForgotPasswordAsync(string receiver, string userName, string token);
}
