using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SearchEngine.Models
{
    public class PageRankResultModel
    {
        public string url { get; set; }
        public string[] keywords { get; set; }
        public int[] keywordCounts { get; set; }
        public double score { get; set; }

        public List<PageRankResultModel> subUrls { get; set; }

        public void addScore(int score)
        {
            this.score += score;
        }
    }
}