using Microsoft.AspNetCore.Mvc;
using portfolio.Models;
using System.Text;
using System.Text.Json;

namespace portfolio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] ContactForm form)
        {
            try
            {
                var apiKey = Environment.GetEnvironmentVariable("BREVO_API_KEY");

                if (string.IsNullOrEmpty(apiKey))
                {
                    return StatusCode(500, "BREVO_API_KEY is missing");
                }

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("api-key", apiKey);

                var body = new
                {
                    sender = new { email = "YOUR_VERIFIED_EMAIL@domain.com", name = "Portfolio" },
                    to = new[] { new { email = "YOUR_VERIFIED_EMAIL@domain.com" } },
                    subject = $"New message from {form.Name}",
                    htmlContent = $@"
                        <h3>New Contact Message</h3>
                        <p><b>Name:</b> {form.Name}</p>
                        <p><b>Email:</b> {form.Email}</p>
                        <p><b>Message:</b> {form.Message}</p>
                    "
                };

                var json = JsonSerializer.Serialize(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(
                    "https://api.brevo.com/v3/smtp/email",
                    content
                );

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode(500, error);
                }

                return Ok("Message sent successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }
    }
}