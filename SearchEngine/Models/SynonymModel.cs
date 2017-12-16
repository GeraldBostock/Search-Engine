using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SearchEngine.Models
{
    public class SynonymModel
    {
        public string[] keywords { get; set; }
        public string[] synonyms { get; set; }
        public int arraySize { get; set; }
    }
}