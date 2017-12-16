using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SearchEngine.Models
{
    public class PageRankModel
    {
        public string keywords { get; set; }
        public string[] urls { get; set; }
        public string[] synonyms { get; set; }
    }
}