using System.Net;
using System.Net.Mail;
using ApiSolutionTestVentas.Entities;
using ApiSolutionTestVentas.Services.Abstracciones;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ApiSolutionTestVentas.Services.Implementaciones;

public class EmailService : IEmailService
{
    private readonly IOptions<AppSettings> _options;
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger, IOptions<AppSettings> options)
    {
        this._logger = logger;
        this._options = options;
    }
    
    public async Task SendEmailAsync(string email, string subject, string message)
    {
        try
        {
            var smtp = _options.Value.SmtpConfiguration;
            var mailMessage = new MailMessage(new MailAddress(smtp.UserName, smtp.FromName),
                new MailAddress(email));

            mailMessage.Subject = subject;
            mailMessage.Body = message;
            mailMessage.IsBodyHtml = true;

            var smtpClient = new SmtpClient(smtp.Server, smtp.PortNumber);
            smtpClient.Credentials = new NetworkCredential(smtp.UserName, smtp.Password);
            smtpClient.EnableSsl = smtp.EnableSsl;
            smtpClient.UseDefaultCredentials = false;

            await smtpClient.SendMailAsync(mailMessage);

            _logger.LogInformation($"Se envió correctamente el correo a {email}");
        }
        catch (SmtpException ex)
        {
            _logger.LogWarning($"No se puede enviar el correo:  {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, $"Error al enviar el correo a {email} - Mensaje: {ex.Message}");
        }
    }
   
}