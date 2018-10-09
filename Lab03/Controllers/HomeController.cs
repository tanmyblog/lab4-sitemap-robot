using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Lab03.Models;
using System.IO;
using System.Xml.Linq;
using System.Globalization;
using System.Text;

namespace Lab03.Controllers
{
    public class HomeController : Controller
    {
        dbDataContext db = new dbDataContext();

        [Route("robots.txt", Name = "GetRobotsText"), OutputCache(Duration = 86400)]
        public ContentResult RobotsText()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("user-agent: *");
            stringBuilder.AppendLine("disallow: /error/");
            stringBuilder.AppendLine("allow: /error/foo");
            stringBuilder.Append("sitemap: ");
            stringBuilder.AppendLine(this.Url.RouteUrl("GetSitemapXml", null, this.Request.Url.Scheme).TrimEnd('/'));

            return this.Content(stringBuilder.ToString(), "text/plain", Encoding.UTF8);
        }

        [Route("sitemap.xml", Name = "GetSitemapXml"), OutputCache(Duration = 86400)]
        public ActionResult SitemapXml()
        {
            var sitemapNodes = GetSitemapNodes();
            string xml = GetSitemapDocument(sitemapNodes);
            return this.Content(xml, "text/xml", Encoding.UTF8);
        }

        public ActionResult Index()
        {
            var cate = from s in db.Articles select s;
            return View(cate);
        }

        public ActionResult Details(string catealias, string alias, int Id)
        {
            Category ct = db.Categories.SingleOrDefault(x => x.Id == Id);
            if(ct == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.News = new SelectList(db.Articles.ToList().OrderBy(y => y.CategoryId), "Alias", "Category");
            return View();

        }

        

        //public ActionResult Robot()
        //{
        //    var fileStream = new FileStream("~/robots.txt/", FileMode.Open, FileAccess.Read);
        //    var fsResult = new FileStreamResult(fileStream, "application/txt");
        //    return fsResult;
        //}

        public string GetSitemapDocument(IEnumerable<SitemapNode> sitemapNodes)
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XElement root = new XElement(xmlns + "urlset");
            foreach (SitemapNode sitemapNode in sitemapNodes)
            {
                XElement urlElement = new XElement(
                xmlns + "url",
                new XElement(xmlns + "loc", Uri.EscapeUriString(sitemapNode.Url)),
                sitemapNode.LastModified == null ? null : new XElement(
                xmlns + "lastmod",
                sitemapNode.LastModified.Value.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:sszzz")),
                sitemapNode.Frequency == null ? null : new XElement(
                xmlns + "changefreq",
                sitemapNode.Frequency.Value.ToString().ToLowerInvariant()),
                sitemapNode.Priority == null ? null : new XElement(
                xmlns + "priority",
                sitemapNode.Priority.Value.ToString("F1", CultureInfo.InvariantCulture)));
                root.Add(urlElement);
            }
            XDocument document = new XDocument(root);
            return document.ToString();
        }

        public IReadOnlyCollection<SitemapNode> GetSitemapNodes()
        {
            List<SitemapNode> nodes = new List<SitemapNode>();

            var dsSanPham = from s in db.Articles select s;

            foreach (var news in dsSanPham)
            {
                nodes.Add(
                   new SitemapNode()
                   {
                       Url = Url.Action("Details", "Home", new { catealias = news.Category.Alias, alias = news.Alias, Id = news.Id }, Request.Url.Scheme),
                       Frequency = SitemapFrequency.Weekly,
                       Priority = 0.8
                   });
            }
            return nodes;
        }
    }
}