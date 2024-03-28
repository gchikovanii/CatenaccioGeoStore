using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using CatenaccioStore.API.Infrastructure.Models;
using Microsoft.Extensions.Options;

namespace CatenaccioStore.API.Controllers
{
    public class ContactController : BaseController
    {

        private readonly GoogleAppSettings _googleAppSettings;
        public ContactController(IOptions<GoogleAppSettings> googleAppSettings)
        {
            _googleAppSettings = googleAppSettings.Value;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ContactForm formData)
        {
            try
            {
                string googleAppPassword = _googleAppSettings.GoogleAppPassword;
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(formData.Email);
                mailMessage.To.Add("catenaccio.geo@gmail.com");
                mailMessage.Subject = formData.Subject;
                mailMessage.Body = $"Name: {formData.Name}\nEmail: {formData.Email}\nPhone Number: {formData.Phone}\nMessage: {formData.Message}";
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential("catenaccio.geo@gmail.com", googleAppPassword);
                smtpClient.Send(mailMessage);
                return Ok("Message sent successfully");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
