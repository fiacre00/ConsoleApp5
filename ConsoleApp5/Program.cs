using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp5
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> urls = new List<string>();

            var lastPage = (int)Math.Ceiling(int.Parse(args[0]) / 20.00);


            for (int i = 1; i <= lastPage; i++)
            {
                var uri = $"https://www.jobserve.com/gb/en/JobListingBasic.aspx?shid={args[1]}&sort=1&view=0&page={i}";
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "GET";
                var response = (HttpWebResponse)request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                List<int> indexes = responseString.ToLower().AllIndexesOf("ir35");

                response.Close();
                response.Dispose();

                foreach (var index in indexes)
                {
                    var toSearch = responseString.Substring(index - 15, 35);
                    //Console.WriteLine(toSearch);
                    if (toSearch.ToLower().IndexOf(" out") > 0)
                    {
                        //Console.WriteLine(toSearch);
                        int endUrl = responseString.LastIndexOf(".jsjob", index);
                        if (endUrl > 0)
                        {
                            int startUrl = responseString.LastIndexOf("/gb/en/", endUrl);
                            if (startUrl > 0)
                            {
                                //Get the description
                                int startDesc = responseString.IndexOf(">", endUrl);
                                int endDesc = responseString.IndexOf("</a>", startDesc);

                                string desc = responseString.Substring(startDesc + 1, (endDesc - (startDesc + 1)));
                                string jobUrl = "https://www.jobserve.com" + responseString.Substring(startUrl, (endUrl - startUrl) + 6);
                                if (!urls.Contains(jobUrl))
                                {
                                    urls.Add(jobUrl);
                                    Console.WriteLine(desc);
                                    Console.WriteLine(jobUrl);
                                    using (StreamWriter w = File.AppendText($"jobserve_{DateTime.Now.ToString("yyyy_MM_dd")}.txt"))
                                    {
                                        w.WriteLine(desc);
                                        w.WriteLine(jobUrl);
                                    }
                                }
                            }
                        }
                    }
                }
                Thread.Sleep(30000);
            }
            Console.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
            Console.ReadLine();
        }

        
    }
}
