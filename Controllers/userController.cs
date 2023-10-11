using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using urlShortener.DataModel;
using urlShortener.DBContext;

namespace urlShortener.Controllers
{
    [Route("/")]
    [ApiController]
    public class userController : ControllerBase
    {
        private readonly shortnerDBContext _context;
        public userController(shortnerDBContext context)
        {
            _context = context;
        }

        private static string pwdHash(string pwd, string salt)
        {
            SHA512 hash = SHA512.Create();
            var pwdBytes = Encoding.Default.GetBytes(pwd + salt);
            var HashedPWD = hash.ComputeHash(pwdBytes);
            return Convert.ToHexString(HashedPWD);
        }
        private string apikeyHash(string id)
        {
            var myString = DateTime.Now.Ticks.ToString();
            byte[] saltBytes = Encoding.ASCII.GetBytes(id);
            var dBytes = new Rfc2898DeriveBytes(myString, saltBytes).GetBytes(32);
            var gibString = Convert.ToBase64String(dBytes);

            return gibString.Replace("/", "").Replace("+", "").Replace("=", "");
        }

        /// <summary>
        /// Use this endpoint to create a new user account to get an API Key. Take note of the API key and keep it safe.
        /// </summary>
        [HttpPost("newuser")]
        public async Task<ActionResult> NewUser(string newEmail, string newPassword)
        {
            var checkExistingUser = await _context.user.FirstOrDefaultAsync(u => u.email == newEmail);

            if (checkExistingUser == null)
            {
                var userID = Guid.NewGuid();
                users newuser = new users()
                {
                    UserId = userID,
                    email = newEmail,
                    password = pwdHash(newPassword, userID.ToString()),
                    apiKey = apikeyHash(userID.ToString().Replace("/", "").Replace("+", "").Replace("=", "")).ToLower()
                };

                _context.Add(newuser);
                await _context.SaveChangesAsync();
                return Ok("Your API key is and keep it safe " + newuser.apiKey);
            }
            else
            {
                ModelState.AddModelError("EmailError", newEmail + " already exists!");
                return BadRequest(ModelState);
            }
        }
    }
}
