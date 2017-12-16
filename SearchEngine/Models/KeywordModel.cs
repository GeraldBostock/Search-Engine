using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SearchEngine.Models
{
    public class KeywordModel
    {
        [Display(Name = "Keywords: ")]
        public string keywords { get; set; }
        [Display(Name = "URL: ")]
        public string url { get; set; }
        public int keywordCount { get; set; }
    }
}