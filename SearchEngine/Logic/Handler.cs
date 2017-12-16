using SearchEngine.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace SearchEngine.Logic
{
    public class Handler
    {
        static private Toolbox _toolbox = new Toolbox();
        private List<string> checkedUrls;
        public PageRankModel _pageRank;
        public PageRankResultModel[] rankingResults;

        public KeywordResultModel[] getKeywordCountResult(string keywords, string url)
        {
            string[] words = keywords.Split(' ');
            KeywordResultModel[] result = new KeywordResultModel[words.Length];

            for (int i = 0; i < words.Length; i++)
            {
                words[i] = _toolbox.toEnglishCharacters(words[i]);
                result[i] = new KeywordResultModel();
                result[i].keyword = words[i];
                result[i].keywordTotal = getKeywordCount(result[i].keyword, url);
            }

            return result;
        }

        private int[] getKeywordCountArray(string[] keywords, string url, int index)
        {
            int[] countArray = new int[keywords.Length];

            try
            {
                string urlContent = _toolbox.readUrlContent(url).ToLower();

                for (int i = 0; i < countArray.Length; i++)
                {
                    keywords[i] = _toolbox.toEnglishCharacters(keywords[i]);
                    string pattern = String.Format(@"\b{0}\b", keywords[i].ToLower());

                    countArray[i] = Regex.Matches(urlContent, pattern).Count;
                }
            }catch(Exception e)
            {
                Debug.Print(e.Message);
            }

            return countArray;
        }

        private int getKeywordCount(string keyword, string url)
        {
            keyword = _toolbox.toEnglishCharacters(keyword);

            string urlContent = _toolbox.readUrlContent(url).ToLower();

            string pattern = String.Format(@"\b{0}\b", keyword.ToLower());

            return Regex.Matches(urlContent, pattern).Count;
        }

        public void rankPages()
        {
            initializeRankingResults();
            calculateUrlScores();
        }

        public void rankSites()
        {
            _toolbox.clearCheckedUrls();
            Debug.Print("Initializing ranking results");
            initializeRankingResults();
            Debug.Print("Initiazling done");
            string[] words = _pageRank.keywords.Split(' ');

            List<string> subUrls = new List<string>();

            try
            {
                Debug.Print("Starting sub URL search");
                for (int i = 0; i < rankingResults.Length; i++)
                {
                    rankingResults[i].subUrls = new List<PageRankResultModel>();

                    subUrls = _toolbox.getSubUrls(rankingResults[i].url);
                    foreach (string url in subUrls)
                    {
                        if (!checkedUrls.Contains(url))
                        {
                            checkedUrls.Add(url);
                            PageRankResultModel subUrlModel = new PageRankResultModel();
                            subUrlModel.subUrls = new List<PageRankResultModel>();
                            subUrlModel.url = url;
                            subUrlModel.keywords = words;
                            subUrlModel.score = 0;
                            subUrlModel.keywordCounts = getKeywordCountArray(words, url, i);

                            List<string> subberUrls = new List<string>();
                            subberUrls = _toolbox.getSubUrls(url);
                            foreach(string subberUrl in subberUrls)
                            {
                                if(!checkedUrls.Contains(subberUrl))
                                {
                                    PageRankResultModel subberUrlModel = new PageRankResultModel();
                                    subberUrlModel.url = subberUrl;
                                    subberUrlModel.keywords = words;
                                    subberUrlModel.score = 0;
                                    subberUrlModel.keywordCounts = getKeywordCountArray(words, subberUrl, i);

                                    subUrlModel.subUrls.Add(subberUrlModel);
                                    checkedUrls.Add(subberUrl);
                                }
                            }

                            rankingResults[i].subUrls.Add(subUrlModel);
                        }
                    }

                    calculateUrlScores();

                }
                Debug.Print("Sub URL search done");
            }catch(Exception e)
            {
                Debug.Print("------------Exception at sub url search---------------");
                Debug.Print(e.Source);
                Debug.Print(e.StackTrace);
                Debug.Print(e.Message);
                Debug.Print("------------Exception at sub url search---------------");
            }
        }

        private void calculateUrlScores()
        {
            for(int i = 0; i < rankingResults.Length; i++)
            {
                int[] keywordCounts = new int[rankingResults[0].keywords.Length];
                for(int j = 0; j < keywordCounts.Length; j++)
                {
                    keywordCounts[j] = 0;
                }

                int keywordTotal = 0;
                for (int j = 0; j < rankingResults[i].keywords.Length; j++)
                {
                    keywordTotal += rankingResults[i].keywordCounts[j];
                    keywordCounts[j] += rankingResults[i].keywordCounts[j];
                }

                for(int j = 0; j < rankingResults[i].subUrls.Count; j++)
                {
                    for(int k = 0; k < rankingResults[i].keywords.Length; k++)
                    {
                        keywordTotal += rankingResults[i].subUrls[j].keywordCounts[k];
                        keywordCounts[k] += rankingResults[i].subUrls[j].keywordCounts[k];
                    }

                    for(int x = 0; x < rankingResults[i].subUrls[j].subUrls.Count; x++)
                    {
                        for(int y = 0; y < rankingResults[i].keywords.Length; y++)
                        {
                            keywordTotal += rankingResults[i].subUrls[j].subUrls[x].keywordCounts[y];
                            keywordCounts[y] += rankingResults[i].subUrls[j].subUrls[x].keywordCounts[y];
                        }
                    }
                }
                foreach (int count in keywordCounts) Debug.Print("Keyword count: " + count.ToString());
                Debug.Print("Standart Deviation: " + _toolbox.standartDeviation(keywordCounts).ToString());
                rankingResults[i].score += keywordTotal / _toolbox.standartDeviation(keywordCounts);
                Debug.Print(rankingResults[i].score.ToString());
            }

            Array.Sort(rankingResults, delegate (PageRankResultModel model1, PageRankResultModel model2)
            {
                return model2.score.CompareTo(model1.score);
            });
        }

        public void setPageRankModel(PageRankModel model)
        {
            _pageRank = new PageRankModel();
            _pageRank.urls = new string[model.urls.Length];
            _pageRank.keywords = model.keywords;

            for(int i = 0; i < model.urls.Length; i++)
            {
                _pageRank.urls[i] = model.urls[i];
            }
        }

        public void clearUrlList()
        {
            checkedUrls = new List<string>();
        }

        private void initializeRankingResults()
        {
            rankingResults = new PageRankResultModel[_pageRank.urls.Length];
            string[] words = _pageRank.keywords.Split(' ');

            for(int i = 0; i < rankingResults.Length; i++)
            {
                rankingResults[i] = new PageRankResultModel();

                rankingResults[i].url = _pageRank.urls[i];
                rankingResults[i].keywords = words;
                rankingResults[i].score = _toolbox.isInTitleTag(_pageRank.urls[i], words);
                rankingResults[i].keywordCounts = getKeywordCountArray(words, _pageRank.urls[i], i);

                rankingResults[i].subUrls = new List<PageRankResultModel>();

                checkedUrls.Add(_pageRank.urls[i]);
            }
        }

        public string[] semanticAnalysis(string keywords)
        {
            string[] words = keywords.Split(' ');

            return _toolbox.getSynonyms(words);
        }

        public PageRankModel getPageRankModel()
        {
            return this._pageRank;
        }

        public PageRankResultModel[] getRankingResults()
        {
            return this.rankingResults;
        }
    }
}