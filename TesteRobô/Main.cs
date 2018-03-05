using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RobotTest
{
    public partial class TestForm : Form
    {
        List<SearchResult> SearchResultsData = new List<SearchResult>();
        string bingUrl = "http://www.bing.com/";

        public TestForm()
        {
            InitializeComponent();
        }

        private void TestBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if(TestBrowser.Document.Url.Equals("http://www.bing.com/"))
            {
                SearchForABSCard();
            }
            else if(TestBrowser.Document.Url.ToString().Contains("http://www.bing.com/search?q="))
            {
                var resultsElements = TestBrowser.Document.GetElementById("b_results").Children;

                foreach(HtmlElement result in resultsElements)
                {
                    if (result.OuterHtml.Contains("b_algo"))
                    {
                        string desc;
                        var title = result.Children[0].InnerText.Trim();
                        var url = result.Children[1].Children[0].InnerText.Trim();
                        if (result.Children[1].Children.Count > 1) {desc = result.Children[1].Children[1].InnerText.Trim();}
                        else {desc = result.Children[2].Children[0].InnerText.Trim(); }

                        var newSearchResult = new SearchResult
                        {
                            Title = title, Url=url, Description = desc
                            
                        };

                        SearchResultsData.Add(newSearchResult);
                    }
                      
                }

                WriteDataInCsv();
            }
        }

        public void SearchForABSCard()
        {
            var searchBox = TestBrowser.Document.GetElementById("sb_form_q");
            searchBox.InnerText = "ABSCard Gestão de Benefícios";

            var searchButton = TestBrowser.Document.GetElementById("sb_form_go");
            searchButton.InvokeMember("click");
        }

        public void WriteDataInCsv()
        {
            var sw = new StreamWriter(Application.StartupPath + "\\SearchResults.csv");
            foreach (var searchResult in SearchResultsData)
            {
                var title = searchResult.Title;
                sw.WriteLine("Título: " +title);
                var url = searchResult.Url;
                if (url == "www.dmsistemas.com.br")
                {
                    sw.WriteLine("--> Item 1 encontrado:");
                    sw.WriteLine("url: " + url);
                    sw.WriteLine("--> Item 2 encontrado:");
                    
                }
                else { sw.WriteLine("url: " + url); }
                
                var desc = searchResult.Description;
                sw.WriteLine("descrição: " + desc);
                sw.WriteLine("    ");
                


            }
            sw.Dispose();

            Process.Start("notepad.exe", Application.StartupPath + "\\SearchResults.csv");
            Close();
        }
    }
}
