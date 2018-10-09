using Lab03.common;
using Lab03.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Lab03.Controllers
{
    public class BlogController : Controller
    {
        dbDataContext db = new dbDataContext();
        // GET: Blog
        public ActionResult Index()
        {
            var cate = from s in db.Categories select s;
            return View(cate);
        }

        public ActionResult PostFeed(string type)
        {
            Category category = db.Categories.Where(s => s.Alias.Contains(type)).FirstOrDefault();
            if (category == null)
            {
                return HttpNotFound();
            }
            IEnumerable<Article> posts = (from s in db.Articles where s.Category.Alias.Contains(type) select s).ToList();

            var feed = new SyndicationFeed(category.Name, "RSS Feed",
                        new Uri("https://myblog/RSS/"),
                        Guid.NewGuid().ToString(),
                        DateTime.Now);
            var items = new List<SyndicationItem>();
            foreach (Article art in posts)
            {
                string postUrl = String.Format("https://myblog/RSS/" + art.Alias + "-{0}", art.Id);
                var item = new SyndicationItem(Helper.RemoveIllegalCharacters(art.Title),
                            Helper.RemoveIllegalCharacters(art.Description),
                            new Uri(postUrl),
                            art.Id.ToString(),
                            art.DatePublished.Value);
                items.Add(item);
            }

            feed.Items = items;
            return new RSSApplicationResult { Feed = feed };
        }

        public ActionResult ReadRSS()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ReadRSS(string url)
        {
            WebClient wclient = new WebClient();
            wclient.Encoding = ASCIIEncoding.UTF8;
            string RSSData = wclient.DownloadString(url);

            XDocument xml = XDocument.Parse(RSSData, LoadOptions.PreserveWhitespace);
            var RSSFeedData = (from x in xml.Descendants("item")
                               select new RssFeed
                               {
                                   Title = ((string)x.Element("title")),
                                   Alias = ((string)x.Element("link")),
                                   Description = ((string)x.Element("description")),
                                   DatePublished = ((string)x.Element("pubDate"))
                               });
            ViewBag.RSSFeed = RSSFeedData;
            ViewBag.URL = url;
            return View();
        }
    }
}