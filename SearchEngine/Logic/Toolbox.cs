using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace SearchEngine.Logic
{
    public class Toolbox
    {
        List<string> checkedUrls;

        private string[] _synonymData;

        public Toolbox()
        {

        }

        public string readUrlContent(string url)
        {
            string html = "";
            string urlData = "";

            using (var client = new WebClient { Encoding = System.Text.Encoding.UTF8 })
            {
                try
                {
                    html = client.DownloadString(url);
                }
                catch(Exception e)
                {
                    Debug.Print(e.StackTrace);
                    Debug.Print("+++++++++ Problem at URL +++++++++ ---> " + url);
                }
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            try
            {
                urlData = doc.DocumentNode.SelectSingleNode("//body").InnerText;
            }
            catch(Exception e)
            {
                Debug.Print(e.Message);
            }

            return toEnglishCharacters(urlData);
        }

        public int isInTitleTag(string url, string[] keywords)
        {
            string html = "";
            string titleTag = "";
            int score = 0;

            using (var client = new WebClient { Encoding = System.Text.Encoding.UTF8 })
            {
                try
                {
                    html = client.DownloadString(url);
                }
                catch (Exception e)
                {
                    Debug.Print("+++++++++ Problem at URL +++++++++ ---> " + url);
                }
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            try
            {
                titleTag = doc.DocumentNode.Descendants("title").FirstOrDefault().InnerText;

                foreach(string keyword in keywords)
                {
                    if (titleTag.ToLower().Contains(keyword.ToLower())) score += 5;
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }

            return score;

        }

        public List<string> getSubUrls(string url)
        {
            List<string> urls = new List<string>();
            Debug.Print("Root URL: " + url);

            try
            {
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(url);
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
                {
                    string hrefValue = link.GetAttributeValue("href", String.Empty);
                    if (!checkedUrls.Contains(hrefValue))
                    {
                        string absolutePath = getAbsoluteUrl(url, hrefValue);
                        if (absolutePath.StartsWith(url))
                        {
                            if(!absolutePath.EndsWith(".pdf") && !absolutePath.EndsWith(".doc"))
                                urls.Add(absolutePath);
                        }

                        checkedUrls.Add(hrefValue);
                    }
                }
            }
            catch(Exception e)
            {
                Debug.Print(e.StackTrace);
                Debug.Print(e.Message);
                Debug.Print("Can't get subURLs");
            }

            return urls;
        }

        private static string getAbsoluteUrl(string baseUrl, string url)
        {
            string absoluteUrl = "";
            try
            {
                var uri = new Uri(url, UriKind.RelativeOrAbsolute);
                if (!uri.IsAbsoluteUri)
                    uri = new Uri(new Uri(baseUrl), uri);
                absoluteUrl = uri.AbsoluteUri;
            }catch(Exception e)
            {
                Debug.Print(e.Message);
            }
            return absoluteUrl;
        }

        public void clearCheckedUrls()
        {
            checkedUrls = new List<string>();
        }

        public void isUrlValid(string url)
        {
            try
            {
                WebRequest req = WebRequest.Create(url);

                WebResponse res = req.GetResponse();

                Debug.Print("URL is valid");
            }
            catch(WebException e)
            {
                if (e.Message.Contains("remote name could not be resolved"))
                {
                    Debug.Print("Invalid URL");
                }
            }
        }

        public double standartDeviation(int[] dataSet)
        {
            if (dataSet.Length == 1) return 1;

            double average = dataSet.Average();
            double sumOfDerivation = 0;
            foreach (double value in dataSet)
            {
                sumOfDerivation += (value) * (value);
            }
            double sumOfDerivationAverage = sumOfDerivation / (dataSet.Length - 1);
            double standartDeviation = Math.Sqrt(sumOfDerivationAverage - (average * average));

            if (double.IsNaN(standartDeviation))
            {
                return 1;
            }

            return standartDeviation;
        }

        private float getAverage(int[] dataSet)
        {
            int total = 0;

            for(int i = 0; i < dataSet.Length; i++)
            {
                total += dataSet[i];
            }

            return total / dataSet.Length;
        }

        public string toEnglishCharacters(string text)
        {
            string[] olds = { "ğ", "ç", "ü", "ş", "ı", "ö", "î", "â"};
            string[] news = { "g", "c", "u", "s", "i", "o", "i", "a"};

            for(int i = 0; i < olds.Length; i++)
            {
                text = text.Replace(olds[i], news[i]);
            }

            return text;
        }

        public string[] getSynonyms(string[] keywords)
        {
            string urlData = "";
            string[] synonyms = new string[keywords.Length];

            using (var client = new WebClient { Encoding = System.Text.Encoding.UTF8 })
            {
                try
                {
                    if(_synonymData == null)
                    {
                        urlData = client.DownloadString("https://raw.githubusercontent.com/maidis/mythes-tr/master/veriler/kelime-esanlamlisi.txt");
                        _synonymData = urlData.Split('\n');
                    }
                    
                    for(int i = 0; i < keywords.Length; i++)
                    {
                        for(int j = 0; j < _synonymData.Length; j++)
                        {
                            string tempKeyword = toEnglishCharacters(keywords[i]);
                            string tempLine = toEnglishCharacters(_synonymData[j]);

                            if (tempLine.StartsWith(tempKeyword + "\t"))
                            {
                                string withoutSpaces = _synonymData[j].Replace("\t", "");
                                string synonym = withoutSpaces.Remove(0, keywords[i].Length);
                                synonyms[i] = synonym;
                                Debug.Print("Synonym of " + keywords[i] + " : " + synonym);
                                j = _synonymData.Length;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Print(e.Message);                  
                }
            }

            return synonyms;
        }
    }
}