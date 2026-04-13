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
        [HttpPost("send")]
        public IActionResult SendEmail([FromBody] ContactForm form)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("sohila24ahmed@gmail.com", "lezf tstz ndid eriz"),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("sohila24ahmed@gmail.com"),
                    Subject = $"New message from {form.Name}",
                    Body = $"From: {form.Name}\nEmail: {form.Email}\n\n{form.Message}",
                    IsBodyHtml = false,
                };
                mailMessage.ReplyToList.Add(new MailAddress(form.Email));
                mailMessage.To.Add("sohila24ahmed@gmail.com");

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
