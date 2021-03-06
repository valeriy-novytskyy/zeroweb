/*--------------------------------------------------------*\
|   ______    __   |
|  |  __  |  |  |  |
|  | |  | |  |  |  |
|  | !__! |  |  |  |
|  !______!  !__!  |  binary : tech art
|
|  News server-side controller.
|----------------------------------------------------------
|  Copyright(C) 2016 Valeriy Novytskyy
\*---------------------------------------------------------*/

using System;
using System.Dynamic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace ZeroWeb.Controllers
{
    /// <summary>
    /// News controller.
    /// </summary>
    public class NewsController : Controller
    {
        /// <summary>
        /// The data store for getting news stories.
        /// </summary>
        IDataStore store;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewsController"/> class.
        /// </summary>
        /// <param name="store">The data store</param>
        public NewsController(IDataStore store)
        {
            this.store = store;
        }

        /// <summary>
        /// Default action.
        /// </summary>
        /// <param name="story">Display a page with the specified story.</param>
        /// <param name="page">Display the specified page.</param>
        /// <param name="year">Display contributions for the specified year.</param>
        public IActionResult Index([FromQuery]string story, [FromQuery]int? page, [FromQuery]int? year)
        {
            var typeTag = Shared.FormatTag(TypeTags.Story);
            var excludeIpAddress = Shared.GetRequestIpAddress(this.Request);
            var stories = this.store
                .GetArticles(typeTag, page, year, story)
                .Select(article => new
                {
                    id = article.Id,
                    title = article.Title,
                    key = article.Key,
                    date = article.Date,
                    author = article.Author.Name,
                    views = article.Views.Count,
                    stars = article.Stars.Count,
                    starsReadOnly = article.Stars.Any(star => star.IpAddress == excludeIpAddress),
                    location = article.LocationName,
                    latitude = article.LocationLatitude,
                    longitude = article.LocationLongitude,
                    zoom = article.LocationZoom,
                    tags = article.Metadata
                        .Where(metadata => metadata.Tag.Name.ToLower() != typeTag)
                        .Select(metadata => metadata.Tag.Name).ToArray(),
                    content = article.Content
                })
                .ToArray()
                .Select(result =>
                {
                    dynamic article = new ExpandoObject();
                    article.id = result.id;
                    article.title = result.title;
                    article.key = result.key;
                    article.date = result.date;
                    article.author = result.author;
                    article.views = result.views;
                    article.stars = result.stars;
                    article.starsReadOnly = result.starsReadOnly;
                    article.location = result.location;
                    article.latitude = result.latitude;
                    article.longitude = result.longitude;
                    article.zoom = result.zoom;
                    article.tags = result.tags;
                    article.content = result.content;
                    return article;
                });

            dynamic news = new ExpandoObject();
            news.Page = page.HasValue ? page.Value : 1;
            news.Year = year;
            news.Stories = stories;
            
            return this.View(news);
        }

        /// <summary>
        /// Error action.
        /// </summary>
        public IActionResult Error()
        {
            return View();
        }
    }
}
