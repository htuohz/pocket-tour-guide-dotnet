using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace pocket_tour_guide_dotnet.Controllers
{
    [Route("api/[controller]")]
    public class DescriptionController : Controller
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            yield return "";
        }

        // GET api/values/5
        [HttpGet("getbyname/{name}")]
        public IActionResult Get(string name)
        {
            string url = "https://en.m.wikipedia.org/wiki/"+HttpUtility.UrlEncode(name).Replace("+", "%20");
            string description = getDescriptionFromUrl(url);
            if (description != "")
            {
                return Ok(description);
            }
            return NotFound();
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private string getDescriptionFromUrl(string url)
        {
            WebRequest myReq = WebRequest.Create(url);
            myReq.Method = "GET";
            myReq.ContentType = "application/json; charset=UTF-8";
            UTF8Encoding enc = new UTF8Encoding();
            myReq.Headers.Remove("auth-token");
            var text = "";
            try
            {
                WebResponse wr = myReq.GetResponse();
                Stream receiveStream = wr.GetResponseStream();
                StreamReader reader = new StreamReader(receiveStream, Encoding.UTF8);
                string content = reader.ReadToEnd();
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(content);
                try
                {
                    var collection = document.DocumentNode.SelectNodes("//div[@id='mw-content-text']");
                    foreach (var node in collection.First().DescendantNodes().ToList())
                    {
                        if(node.Name == "sup")
                        {
                            node.Remove();
                        }                
                    }
                    foreach (var node in collection.First().DescendantNodes().ToList())
                    {
                        if (node.Name == "p")
                        {
                            text = text + node.InnerText;
                        }
                    }
                    //text = document.DocumentNode.SelectNodes("//div[@id='mw-content-text']").First().InnerText;

                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
            catch(Exception e)
            {
                return "";
            }
            
            var rgx = new Regex(@"[\r\n'/\\]");
            text = rgx.Replace(text, string.Empty);
            return text;
        }
    }
}
