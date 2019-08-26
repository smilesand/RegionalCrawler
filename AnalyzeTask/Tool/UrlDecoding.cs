using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzeTask.Tool
{
    public class UrlDecoding
    {
        public static string Decoding(string Url)
        {
            string url = "";
            for (var i = 0; i < (int)(Url.Length / 2) - 1; i++)
            {
                url += Url.Substring(i * 2, i * 2 + 2) + "/";
            }
            url += Url + ".html";
            return url;
        }
    }
}
