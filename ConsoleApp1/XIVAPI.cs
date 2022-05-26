using Flurl;
using Flurl.Http;
using System.Net.Http;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    public class XIVAPI
    {
        //public static async Task Main()
        //{
        //    var api = new XIVAPI();
        //    var list=api.GetMarketBoardListings("无瑕白染剂", "10", "紫水栈桥");
        //    Console.WriteLine(list.Count);
        //}
        public Dictionary<string, string[]> Servers = new Dictionary<string, string[]>
        {
            { "豆豆柴" ,new string[]{ "水晶塔", "银泪湖", "太阳海岸", "伊修加德","红茶川" } },
            { "猫小胖",new string[]{ "紫水栈桥", "延夏" , "静语庄园", "摩杜纳", "海猫茶屋","柔风海湾", "琥珀原" } },
            { "莫古力",new string[]{ "白银乡", "白金幻象" , "神拳痕", "潮风亭", "旅人栈桥", "拂晓之间", "龙巢神殿", "梦羽宝境" } },
            { "陆行鸟",new string[]{ "红玉海", "神意之地", "拉诺西亚", "幻影群岛", "萌芽池", "宇宙和音", "沃仙曦染", "晨曦王座" } },
        };
        public async Task<List<Item>> GetResultByNameAsync(string name)
        {
            var ValueResult = new List<Item>();
            var request = await $"https://cafemaker.wakingsands.com/search?string={name}/".GetAsync();
            var req = request.ResponseMessage;
            var str = req.Content.ReadAsStringAsync().Result;
            dynamic json = JsonConvert.DeserializeObject(str);
            var result = json["Results"];
            var Pagination = json["Pagination"];
            int length = Pagination["Results"];
            for (int i = 0; i < length; i++)
            {
                string Name = result[i]["Name"];
                string ID=result[i]["ID"];
                ValueResult.Add(new Item(Name, ID));
            }
            return ValueResult;
        }
        public List<MarketBoard> GetMarketBoardListings(string id,string count,string serverName)
        {
            var marketBoardList=new List<MarketBoard>();
            var url = GetMarketBoardListingsUrl(id, count, serverName);
            var Json=GetJsonByUrlAsync(url).Result;
            var lists = Json["listings"];
            foreach(dynamic item in lists)
            {
                string retainerName = item["retainerName"];
                int pricePerUnit = item["pricePerUnit"];
                int quantity = item["quantity"];
                marketBoardList.Add(new MarketBoard(retainerName, pricePerUnit, quantity));
            }
            //for (int i = 0; i < lists.Count; i++)
            //{
            //    Console.WriteLine($"name={lists[i]},server={serverName}");
            //    string retainerName= lists[i]["retainerName"];
            //    int pricePerUnit = lists[i]["pricePerUnit"];
            //    int quantity = lists[i]["quantity"];
            //    marketBoardList.Add(new MarketBoard(retainerName, pricePerUnit, quantity));
            //}
            return marketBoardList;
        }
        public string GetMarketBoardListingsUrl(string Id, string count, string serverName)
        {
            return $"https://universalis.app/api/{serverName}/{Id}?listings={count}";
        }
        public async Task<dynamic> GetJsonByUrlAsync(string url)
        {
            var request = await url.GetAsync();
            var req = request.ResponseMessage;
            var str = req.Content.ReadAsStringAsync().Result;
            dynamic json = JsonConvert.DeserializeObject(str);
            return json;
        }
    }
    public class Item
    {
        public Item(string name,string id)
        {
            Name= name;
            ID= id;
        }
        public string Name { get; set; }
        public string ID { get; set; }
    }
    public class ItemValue
    {
        public string server;
        public int value;
    }
    public class MarketBoard
    {
        public MarketBoard(string name, int price, int quantity)
        {
            this.Name= name;
            this.Price= price;
            this.Quantity= quantity;
        }
        public string Name { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
    }
}
[Serializable]
public class PriceResult
{
    public long lastReviewTime;
    public int pricePerUnit;
    public int quantity;
    public int stainID;
    public string creatorName;
    public string creatorID;
    public bool hq;
    public bool isCrafted;
    public string listingID;
    public string[] materia;
    public bool onMannequin;
    public int retainerCity;
    public string retainerID;
    public string retainerName;
    public string sellerID;
    public int total;
} 