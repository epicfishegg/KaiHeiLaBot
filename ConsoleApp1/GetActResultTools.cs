using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;


public class GetActResultTools
{
    //public static void Main()
    //{
    //    var regexLogsBundle = new Regex("(?<=(/reports/))[.\\s\\S]*?(?=(#fight=))");
    //    var regexLogsBundleId = new Regex("(?<=(#fight=))[.\\s\\S]*?(?=(&type))");
    //    var tools = new GetActResultTools();
    //    var a = regexLogsBundle.Match("https://cn.fflogs.com/reports/TYt3AynjPqVx498F#fight=44&type=damage-done");
    //    var b = regexLogsBundleId.Match("https://cn.fflogs.com/reports/TYt3AynjPqVx498F#fight=44&type=damage-done");
    //    Console.WriteLine($"{a}\n{b}");
    //}
    public string GetUrlByName(string name)
    {
        return $"https://cn.fflogs.com/search/?term={name}";
    }
    public string GetUrlByNameAndServer(string name, string server)
    {
        return $"https://cn.fflogs.com/character/cn/{server}/{name}";
    }
    public string GetHtmlByUrl(string Url)
    {
        var request = WebRequest.Create(Url);
        var response = request.GetResponse();
        Stream stream = response.GetResponseStream();
        StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("UTF-8"));
        var html = reader.ReadToEnd();
        return html;
    }

    public List<Players> GetPlayersListByHtml(string html)
    {
        if (html.Contains("未找到指定的角色和服务器。可能是由于该角色还没有被记录"))
        {
            return new List<Players>();
        }
        var regexCut = new Regex("(?<=(<div id=\"characters-title\" class=\"dialog-title\">角色</div>))[.\\s\\S]*");
        html = regexCut.Match(html).Value;
        var regex = new Regex("(?<=(<div class=\"search-item\"><div class=\"name\"><a href=))[.\\s\\S]*?(?=(</div></div>))");
        var regexName = new Regex("(?<=(class=\"NPC\">))[.\\s\\S]*?(?=(</a></div><div))");
        var regexServer = new Regex("(?<=(class=\"server\">))[.\\s\\S]*");
        var regexUrl = new Regex("(?<=(\"))[.\\s\\S]*?(?=(\"))");
        var list = new List<Players>();
        var matches = regex.Matches(html);
        for (int i = 0; i < matches.Count; i++)
        {
            list.Add(new Players(
                regexName.Match(matches[i].ToString()).ToString(),
                regexServer.Match(matches[i].ToString()).ToString(),
                regexUrl.Match(matches[i].ToString()).ToString()
            ));
        }
        return list;
    }
    //public Players GetPlayerByUrl(string url)
    //{

    //}
    public class Players
    {
        public Players(string name, string server, string url)
        {
            Name = name; ;
            Server = server;
            Url = url;
        }
        public string Name;
        public string Server;
        public string Url;
    }
    public class ActResult
    {

    }
}