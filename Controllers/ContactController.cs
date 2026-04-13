using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using portfolio.Models;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Cors;

namespace portfolio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class ContactController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ContactController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] ContactForm form)
        {
            try
            {
                var email = _config["EmailSettings:Email"];
                var password = _config["EmailSettings:Password"];

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(email, password),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(email),
                    Subject = $"New message from {form.Name}",
                    Body = $"From: {form.Name}\nEmail: {form.Email}\n\n{form.Message}",
                    IsBodyHtml = false,
                };

                mailMessage.To.Add(email);
                mailMessage.ReplyToList.Add(new MailAddress(form.Email));

                smtpClient.SendMailAsync(mailMessage).GetAwaiter().GetResult();

                return Ok("Message sent successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error sending message: " + ex.Message);
            }
        }

    }
    }
