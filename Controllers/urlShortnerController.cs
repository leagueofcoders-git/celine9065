using System;
using System.Data;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using urlShortener.DataModel;
using urlShortener.DBContext;
using urlShortener.Utilities;

namespace urlShortener.Controllers
{
    [Route("/")]
    [ApiController]
    public class urlShortnerController : ControllerBase
    {
        private readonly shortnerDBContext _context;
        public urlShortnerController(shortnerDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Return the original URL based on friendly URL parameter. Paste the shorten url to a browser and hit enter
        /// </summary>

        [HttpGet("/getlongurl/{shortid}")]
        public async Task<ActionResult> GetLongURL(string shortid)
        {
            var idExist = await _context.urlShortner.FirstOrDefaultAsync(r => r.URLparam == shortid);

            if (idExist is null)
            {
                return BadRequest("Invalid friendly URL parameter.");
            }

            return Redirect(idExist.longURL);
        }

        /// <summary>
        /// Use Postman to access this endpoint https://celine9065.leagueofcoders.app/getallurlsbyuser. Create a header in Postman named "x-api-key" and put your api key as value. Copy the shortURL and open a browser and paste it.
        /// </summary>
        [HttpGet("getallurlsbyuser")]
        [ServiceFilter(typeof(ApiKeyHeaderAttribute))]
        public async Task<ActionResult> GetAllURLsByUser()
        {
            var getApiKey = Request.Headers["x-api-key"].ToString();
            var userExist = _context.user.Where(r => r.apiKey == getApiKey).Select(r => r.UserId).FirstOrDefault();

            if (userExist == Guid.Empty)
            {
                return BadRequest("Invalid account credential.");
            }

            var records = await _context.urlShortner.Where(u => u.userId.UserId == userExist).Select(p => new
            {
                p.longURL,
                p.shortURL,
                p.URLparam
            }).ToListAsync();

            return Ok(records);
        }
        /// <summary>
        /// Use Postman to access this endpoint https://celine9065.leagueofcoders.app/createshortener. Create a header in Postman named "x-api-key" and put your api key as value. 
        /// </summary>
        /// <remarks>
        /// In the post Body of the Postman, type **"https://www.someurl.com"** of your choice.
        /// </remarks>

        [HttpPost("createshortener")]
        [ServiceFilter(typeof(ApiKeyHeaderAttribute))]
        public async Task<ActionResult> CreateShortner([FromBody] string longURL)
        {
            if (!IsAbsoluteUrl(longURL))
            {
                longURL = "https://" + longURL;
            }

            if (validURL(longURL))
            {
                var urlExist = await _context.urlShortner.FirstOrDefaultAsync(r => r.longURL == longURL);
                var domain = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + "/";

                if (urlExist == null)
                {
                    var param = getParam(longURL);
                    var getApiKey = Request.Headers["x-api-key"].ToString();

                    shortener insertNewRecord = new shortener()
                    {
                        // URLId auto created for new records
                        longURL = longURL,
                        shortURL = domain + param,
                        URLparam = param,
                        userId = await _context.user.FirstOrDefaultAsync(u => u.apiKey == getApiKey)
                };

                    _context.Add(insertNewRecord);
                    await _context.SaveChangesAsync();

                    return Ok($"The shortened URL is {insertNewRecord.shortURL}");
                }

                return Ok($"The URL already has a unique URLShortner generated: {urlExist.shortURL}");

            }
            
            return BadRequest("Input is not a valid URL");

        }

        private string getParam(string longurl)
        {
            var datetime = DateTime.Now.Ticks.ToString();
            byte[] saltBytes = Encoding.ASCII.GetBytes(longurl);
            var dBytes = new Rfc2898DeriveBytes(datetime, saltBytes).GetBytes(16);
            var shortenedURL = Convert.ToBase64String(dBytes); 

            return shortenedURL.Substring(0, 8);
        }
        
        private bool validURL(string url)
        {
            if (!IsAbsoluteUrl(url))
            {
                url = "https://" + url; // automatically add in https if relative URL is given
            }

            return Uri.IsWellFormedUriString(url, UriKind.Absolute);

        }

        private bool IsAbsoluteUrl(string url)
        {
            Uri result;
            return Uri.TryCreate(url, UriKind.Absolute, out result);
        }

    }
}
