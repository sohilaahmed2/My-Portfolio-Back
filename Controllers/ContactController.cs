using Microsoft.AspNetCore.Mvc;
using portfolio.Models;
using System.Net;
using System.Net.Mail;

namespace portfolio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        [HttpPost("send")]
        public IActionResult SendEmail([FromBody] ContactForm form)
        {
            try
            {
                var smtpUser = Environment.GetEnvironmentVariable("SMTP_USER");
                var smtpPass = Environment.GetEnvironmentVariable("SMTP_PASS");

                var smtpClient = new SmtpClient("smtp-relay.brevo.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = true,
                    UseDefaultCredentials = false
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUser),
                    Subject = $"New message from {form.Name}",
                    Body = $"From: {form.Name}\nEmail: {form.Email}\n\n{form.Message}",
                    IsBodyHtml = false,
                };

                mailMessage.ReplyToList.Add(new MailAddress(form.Email));
                mailMessage.To.Add(smtpUser);

                smtpClient.Send(mailMessage);

                return Ok("Message sent successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error sending message: " + ex.Message);
            }
        }
    }
}