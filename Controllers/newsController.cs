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
        public IActionResult Index()
        {
            string excludeTag = Tags.Story.ToString();

            return View(this.store
                .GetItems(Tags.Story)
                .Select(item => new
                {
                    id = item.Id,
                    title = item.Title,
                    date = item.Date,
                    author = item.Author.Name,
                    tags = item.Metadata
                        .Where(metadata => metadata.Tag.Name.ToLower() != excludeTag)
                        .Select(metadata => metadata.Tag.Name).ToArray()
                })
                .ToArray()
                .Select(result =>
                {
                    dynamic story = new ExpandoObject();
                    story.id = result.id;
                    story.title = result.title;
                    story.date = result.date;
                    story.author = result.author;
                    story.tags = result.tags;
                    return story;
                }));
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
