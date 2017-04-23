/*--------------------------------------------------------*\
|   ______    __   |
|  |  __  |  |  |  |
|  | |  | |  |  |  |
|  | !__! |  |  |  |
|  !______!  !__!  |  binary : tech art
|
|  Defines the model for article contribution summary.
|----------------------------------------------------------
|  Copyright(C) 2017 Valeriy Novytskyy
\*---------------------------------------------------------*/

namespace ZeroWeb.Api.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Article contribution summary.
    /// </summary>
    public class ContributionSummary
    {
        /// <summary>
        /// Gets or sets the months to display for each page.
        /// </summary>
        public IList<PageSummary> Pages { get; private set; }

        /// <summary>
        /// Gets or sets the monthly contribution summary.
        /// </summary>
        public MonthDictionary Months { get; private set; }

        /// <summary>
        /// Gets or sets the max. article count for any month.
        /// </summary>
        public int Max { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContributionSummary"/> class.
        /// </summary>
        public ContributionSummary()
        {
            this.Pages = new List<PageSummary>();
            this.Months = new MonthDictionary();
        }

        /// <summary>
        /// Aggregate count for an article tag at all summary levels.
        /// </summary>
        /// <param name="articleKey">The unique article key used in permalinks.</param>
        /// <param name="articleTitle">The article title.</param>
        /// <param name="date">The date when an article with this tag was posted.</param>
        /// <param name="tag">The article tag.</param>
        /// <returns>A self-reference for chaining.</returns>
        public ContributionSummary Aggregate(string articleKey, string articleTitle, DateTime date, string tag)
        {
            MonthSummary monthSummary = this.GetOrCreateMonth(date);
            int monthWeekMax = monthSummary.Aggregate(articleKey, articleTitle, date, tag);

            if (this.Max < monthWeekMax)
            {
                this.Max = monthWeekMax;
            }

            return this;
        }

        /// <summary>
        /// Paginate the contribution summary.
        /// </summary>
        /// <param name="maxArticles">Max. articles per page.</param>
        /// <param name="maxDays">Max. days per page.</param>
        /// <returns>A self-reference for chaining.</returns>
        public ContributionSummary Paginate(int maxArticles, int maxDays)
        {
            DateTime? startWeek = null;
            string startMonthName = null;
            string monthName = null;
            int startWeekIndex = -1;
            int articleCount = 0;
            DateTime[] weeks = null;

            foreach (DateTime month in this.Months.Keys.ToList().OrderByDescending(key => key))
            {
                MonthSummary monthSummary = this.Months[month];
                monthName = month.ToString("MMM").ToLower();
                weeks = monthSummary.Weeks.Keys.OrderByDescending(key => key).ToArray();

                for (int weekIndex = 0; weekIndex < weeks.Length; weekIndex++)
                {
                    DateTime firstDayOfWeek = weeks[weekIndex];
                    DateTime lastDayOfWeek = month.AddDays(7);
                    WeekSummary weekSummary = monthSummary.Weeks[firstDayOfWeek];
                    int dayCount = startWeek.HasValue ?
                        (int)(startWeek.Value - lastDayOfWeek).TotalDays : 0;

                    if (!startWeek.HasValue)
                    {
                        startWeek = firstDayOfWeek;
                        startWeekIndex = weekIndex;
                        startMonthName = monthName;
                    }

                    articleCount += weekSummary.Articles.Count;

                    if (articleCount > maxArticles ||
                        (articleCount == maxArticles && dayCount > maxDays))
                    {
                        this.Pages.Add(new PageSummary(
                            new WeekMapping(startMonthName, startWeekIndex),
                            new WeekMapping(monthName, weekIndex),
                            articleCount - 1));

                        articleCount = 1;
                        startWeek = null;
                    }
                }
            }

            if (startWeek.HasValue)
            {
                this.Pages.Add(new PageSummary(
                    new WeekMapping(startMonthName, startWeekIndex),
                    new WeekMapping(monthName, weeks.Length - 1), 0));
            }

            return this;
        }

        /// <summary>
        /// Gets or creates a month entry.
        /// </summary>
        /// <param name="date">The date to create the month for.</param>
        /// <returns>The existing or new month summary.</param>
        private MonthSummary GetOrCreateMonth(DateTime date)
        {
            DateTime firstOfMonth = new DateTime(date.Year, date.Month, 1);

            if (this.Months.ContainsKey(firstOfMonth))
            {
                return this.Months[firstOfMonth];
            }

            return this.Months[firstOfMonth] = new MonthSummary();
        }
    }
}