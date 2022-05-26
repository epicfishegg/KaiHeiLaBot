using KaiHeiLa;
using KaiHeiLa.Rest;
using KaiHeiLa.WebSocket;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using JavaScriptEngineSwitcher;
using ConsoleApp1;

class Program
{
    private readonly KaiHeiLaSocketClient _client;
    private readonly string _token = "1/MTEwMTM=/BSbNbZv3ObEcG24DdpAIMg==";
    private readonly ulong _guildId = 4597484086398224;
    private readonly ulong _channelId = 6666953006179651;
    private NeteaseTool _neteaseTool = new NeteaseTool();
    private GetActResultTools _actTools = new GetActResultTools();
    private XIVAPI _XIVAPI = new XIVAPI();
    private bool isSearching = false;
    static void Main(string[] args)
    {
        new Program().MainAsync().GetAwaiter().GetResult();
        var neteaseTool = new NeteaseTool();
    }
    Func<string, SocketUser, IMessage, SocketTextChannel, SocketGuild, Task> lingshi;
    public Program()
    {
        _client = new(new KaiHeiLaSocketConfig() { AlwaysDownloadUsers = true, MessageCacheSize = 100 });

        _client.Log += ClientOnLog;
        _client.MessageReceived += ClientOnMessageReceived;
        _client.MessageReceived += CallMenu;
        _client.MessageReceived += CallMusic;
        _client.MessageReceived += CardDemo;
        _client.MessageReceived += CheckAct;
        _client.MessageReceived += AnalyzeAct;
        _client.MessageReceived += CheckValue;
        _client.MessageReceived += CheckAveragePrice;
        _client.DirectMessageReceived += ClientOnDirectMessageReceived;
        _client.UserConnected += UserjoinedEvent;
        //_client.ready += clientonready;
        _client.MessageDeleted += async (msg, channel) =>
        {
            Console.WriteLine($"Message {(await msg.GetOrDownloadAsync()).CleanContent} deleted in {(await channel.GetOrDownloadAsync()).Name}");
        };
        _client.MessageButtonClicked += CheckPriceDetail;
        _client.MessageButtonClicked += ZeroShiki;
        _client.MessageButtonClicked += ClientOnMessageButtonClicked;
        _client.MessageButtonClicked += ShowSongsList;
        _client.MessageButtonClicked += CallMusicById;
        _client.MessageButtonClicked += CheckItemButtonClick;
        _client.MessageButtonClicked += CheckAveragePriceButtonClick;
        _client.MessageButtonClicked += async (value, user, message, channel, guild) =>
        {
        };
    }
    private Task UserjoinedEvent(SocketUser user, SocketVoiceChannel channel, SocketGuild guild, DateTimeOffset time)
    {
        _client.GetGuild(_guildId).GetTextChannel(_channelId).SendTextMessageAsync($"你好{user.Username}");
        return Task.CompletedTask;
    }
    private Task ClientOnDirectMessageReceived(SocketMessage arg)
    {
        return Task.CompletedTask;
    }
    public async Task MainAsync()
    {
        await _client.LoginAsync(TokenType.Bot, _token);
        await _client.StartAsync();
        await Task.Delay(-1);
    }
    private async Task ClientOnMessageReceived(SocketMessage arg)
    {
        string argCleanContent = arg.CleanContent;
        if (arg.Author.Id == _client.CurrentUser.Id) return;
        if (arg.Author.IsBot == true) return;
        if (arg.Content != "/test") return;
        {
            await arg.Channel.SendTextMessageAsync("你好!");
        }
        await arg.ReloadAsync();
        //await CardDemo(arg);
        await MyCard(arg);
        //await MyCard2(arg);
    }
    private async Task ClientOnReady()
    {
        var asyncEnumerable = _client.GetGuild(1990044438283387).GetUsersAsync();
        IEnumerable<IGuildUser> flattenAsync = await asyncEnumerable.FlattenAsync();
    }
    private Task ClientOnLog(LogMessage arg)
    {
        Console.WriteLine(arg.ToString());
        return Task.CompletedTask;
    }
    private async Task ClientOnMessageButtonClicked(string value, SocketUser user, IMessage msg, SocketTextChannel channel, SocketGuild guild)
    {
        if (value == "Zeroshiki_yes")
        {
            CardBuilder cardBuilder = new CardBuilder()
                .WithSize(CardSize.Large)
                .AddModule(new HeaderModuleBuilder().WithText("今天你第三层过了吗？"))
                .AddModule(new ActionGroupModuleBuilder()
                    .AddElement(new ButtonElementBuilder().WithTheme(ButtonTheme.Info).WithText("过了").WithValue("Zeroshiki_3_yes").WithClick(ButtonClickEventType.ReturnValue))
                    .AddElement(new ButtonElementBuilder().WithTheme(ButtonTheme.Info).WithText("没过").WithValue("Zeroshiki_3_no").WithClick(ButtonClickEventType.ReturnValue)));
            IEnumerable<RestGame> games = await _client.Rest.GetGamesAsync().FlattenAsync().ConfigureAwait(false);
            await _client.GetGuild(guild.Id).GetTextChannel(channel.Id)
                .SendCardMessageAsync(cardBuilder.Build());
        }
        if (value == "Zeroshiki_no")
        {
            CardBuilder cardBuilder = new CardBuilder()
                .WithSize(CardSize.Large)
                .AddModule(new HeaderModuleBuilder().WithText("还TM不去打？"));
            await _client.GetGuild(guild.Id).GetTextChannel(channel.Id)
                .SendCardMessageAsync(cardBuilder.Build());
        }
    }
    private async Task ZeroShiki(string value, SocketUser user, IMessage msg, SocketTextChannel channel, SocketGuild guild)
    {
        if (value == "Zeroshiki_3_yes")
        {
            CardBuilder cardBuilder = new CardBuilder()
                .WithSize(CardSize.Large)
                .AddModule(new HeaderModuleBuilder().WithText("今天你第三层过了吗？"))
                .AddModule(new ActionGroupModuleBuilder()
                    .AddElement(new ButtonElementBuilder().WithTheme(ButtonTheme.Info).WithText("过了").WithValue("Zeroshiki_3_yes").WithClick(ButtonClickEventType.ReturnValue))
                    .AddElement(new ButtonElementBuilder().WithTheme(ButtonTheme.Info).WithText("没过").WithValue("Zeroshiki_3_no").WithClick(ButtonClickEventType.ReturnValue)));
            await _client.GetGuild(guild.Id).GetTextChannel(_channelId).SendCardMessageAsync(cardBuilder.Build());
        }

        if (value == "Zeroshiki_3_no")
        {
            CardBuilder cardBuilder = new CardBuilder()
                .WithSize(CardSize.Large)
                .AddModule(new HeaderModuleBuilder().WithText("还TM不区渡劫"));
            await _client.GetGuild(guild.Id).GetTextChannel(_channelId).SendCardMessageAsync(cardBuilder.Build());
        }
    }
    private async Task MyCard(SocketMessage msg)
    {
        if (msg.Author.Id == _client.CurrentUser.Id) return;
        if (msg.Content != "/test") return;
        CardBuilder cardBuilder = new CardBuilder()
            .WithSize(CardSize.Large)
            .AddModule(new HeaderModuleBuilder().WithText("你今天打零式了吗？"))
            .AddModule(new SectionModuleBuilder()
                .WithText(new ParagraphStructBuilder()
                    .WithColumnCount(2)
                    .AddField(new PlainTextElementBuilder().WithContent("PlainTextElement"))))
            .AddModule(new ActionGroupModuleBuilder()
                .AddElement(new ButtonElementBuilder().WithTheme(ButtonTheme.Info).WithText("打了").WithValue("Zeroshiki_yes").WithClick(ButtonClickEventType.ReturnValue))
                .AddElement(new ButtonElementBuilder().WithTheme(ButtonTheme.Info).WithText("没打").WithValue("Zeroshiki_no").WithClick(ButtonClickEventType.ReturnValue)));
        (Guid MessageId, DateTimeOffset MessageTimestamp) response = await _client.GetGuild(_guildId)
        .GetTextChannel(_channelId)
        .SendCardMessageAsync(cardBuilder.Build(), quote: new Quote(msg.Id));
    }
    private async Task TalkJoke(SocketMessage msg)
    {
        string jocks = "100个经典冷笑话大全爆笑1、一只癞蛤蟆最新茶不思饭不想,连蚊子都不抓着吃了，" +
            "其他的癞蛤蟆都很想知道它到底怎么了。年最长癞蛤蟆说了：”抓只天鹅来，保管好.因为癞蛤蟆想吃天鹅肉呀。”2、" +
            "一次五岁的儿子问我，我手上拿着的东西是什么,我说是手机，他说为什么叫手机那，我那时正忙就随口唱到”左手一只鸡，右手一只鸭.”儿" +
            "子露出更加疑惑的表情：”那为什么不叫手鸭那?”3、我和朋友走到分岔路口，我们以歌作别：”我送你离开,千里之外.”于是，”" +
            "千里之外”就走了。4、我家母狗生了两只小狗，因为老婆是新闻记者，所以我们戏称这两只狗为”狗仔队”,一次我们正要kiss，" +
            "我突然看见”狗仔队”大喊：”狗仔队!”老婆惊吓道:”在哪，不可能，我让他们回去了啊。”5、朝鲜说美国人民生活在水深火热中" +
            "。朝鲜报首版头条,发表的一张美国人爬在海滩上日光浴的照片，配文：美国人很穷，没有衣服穿,吃不饱饭，人都饿死了，也没人管。。" +
            ".。..6、一MM失恋了，几次欲寻短见都被亲友及时发现未能实现.一日趁亲友不备离家出走，急的亲友到处寻找,就在决定报警时，" +
            "收到她发来的短信：你们不必找我了，我在去往死海的路上,我喜欢大海，我决定在那儿结束我的人生.7、" +
            "嫦娥姐姐在吃饭，突然外面一闪，嫦娥一惊，连忙出去看.回来呼了一口气；”杨利伟。。”8、" +
            "阿钜和菲菲都耳背。那天菲菲看阿钜出门；”阿钜，你去散步？””不是啊，我去散步啊!””哦.。" +
            "我以为你去散步呢.”9、阿钜和菲菲商量好走楼梯到他们50层的家。到了10楼，阿钜问菲菲；”" +
            "菲菲你累了吗？”菲菲摇摇头，他们就继续走.到了30楼，阿钜问菲菲:”菲菲你累了吗？”菲菲摇摇头" +
            "，他们就继续走。到了49楼，阿钜问菲菲：”菲菲你累了吗？”菲菲使劲点头。阿钜：”那好，我们走回去乘电梯到家吧" +
            "。”10、阿钜要考试，妈妈问阿钜书看完了吗？阿钜说：”我看完了。”第二天妈妈看到阿钜不及格的卷子大发雷霆，" +
            "”你书都看了为什么考这么差！”阿钜：”妈妈，我那天说的是.。。我看,完了。”@jllxk00111、农夫山泉,有点虫" +
            "。新闻发布会：关于农夫山泉有点虫的问题,是这样的，我们不生产矿泉水的，我们只是大自然的搬运工。这个嘛," +
            "是人人皆知的,在我们的广告中早有声明。既然是大自然,就会有虫虫,大自然如果没有虫虫,那还是大自然吗？最后," +
            "祝福大自然越来越美，虫虫多多益善！12、儿子经常在睡觉前喜欢让妈妈给自己讲故事,听着听着就会慢慢睡着.一天" +
            ",妈妈外出不在，他就让爸爸给讲,爸爸欣然答应了，儿子躺在床上，闭着眼等待爸爸给自己带来的故事，等了好几分钟" +
            ",也没有动静，儿子睁开眼一看爸爸，他已经躺在旁边睡着了...13、温州7。23后。每次和朋友告别时，听到”后会有期" +
            "”这四个字，就紧紧张张的,不得不补充问一句：你坐飞机还是高铁？14、你老公年薪多少?”800万””天那，比我家的强多了" +
            ",好幸福哦你。朋友说.”这是基本工资””做什么的？”朋友又问.”做梦的””.。。。。。”朋友。15" +
            "、拿着鱼竿鱼钩，在江河湖海，默默钓鱼的，叫”钓鱼玩法。”不拿鱼竿鱼钩,在城市人海,默默钓鱼的，叫”钓鱼执法。”" +
            "16、妹妹：”左眼珠要睡觉,右眼珠偏不要。我该怎么办？！”哥哥：”那你就把,心灵的窗户,全部关了，不就ok了。”" +
            "17、春天如同初恋，夏天则是热恋，秋天仿佛黄昏恋，冬天恰似天使之恋.那么,走过四季的我们是什么恋？360度、甲午风云、春夏秋冬、日月轮回、生死恋." +
            "18、新闻报道：现在70%的男人希望发生婚外恋。丈夫听后忙对妻子说:”我一定是那30%的男人！”接着新闻继续报道：其他30%的男人已经有了婚外恋。" +
            "19、阿钜有一天被欺负,哭啊哭啊哭啊哭啊，然后把自己淹死了。。菲菲却没有淹死，为什么呢?因为菲菲会飞啊。." +
            "20、李妈妈年纪大了，特别忌讳一些词,有一次来我家，我倒了一杯茶给她,喝完后我问道：”还喝吗?”李妈妈：”喝!”又一杯下去”还喝吗?””喝!”这样反复好几次，李妈妈胀的不行了,我见状道：”早就不该喝了.”李妈妈气气的说:”我要活！”21、金妹和喜之两姐妹一直没有找到好的归宿，一日，她们看见了今麦郎和喜之郎，于是他们各自结婚了。" +
            "22、农夫山泉有点甜，这是为什么呢？因为有点虫。真滴吗?至于你信不信，我反正是信的.23、哥们你太有才了，人家都是才高八斗，你是身高八斗,穿中三装八个衣兜，@jllxk00111" +
            "、农夫山泉,有点虫。新闻发布会：关于农夫山泉有点虫的问题,是这样的，我们不生产矿泉水的，我们只是大自然的搬运工。这个嘛,是人人皆知的,在我们的广告中早有声明。既然是大" +
            "自然,就会有虫虫,大自然如果没有虫虫,那还是大自然吗？最后,祝福大自然越来越美，虫虫多多益善！12、儿子经常在睡觉前喜欢让妈妈给自己讲故事,听着听着就会慢慢睡着.一天,妈妈" +
            "外出不在，他就让爸爸给讲,爸爸欣然答应了，儿子躺在床上，闭着眼等待爸爸给自己带来的故事，等了好几分钟,也没有动静，儿子睁开眼一看爸爸，他已经躺在旁边睡着了...13、温州7" +
            "。23后。每次和朋友告别时，听到”后会有期”这四个字，就紧紧张张的,不得不补充问一句：你坐飞机还是高铁？14、你老公年薪多少?”800万””天那，比我家的强多了,好幸福哦你。朋"+
            "说.”这是基本工资””做什么的？”朋友又问.”做梦的””.。。。。。”朋友。15、拿着鱼竿鱼钩，在江河湖海，默默钓鱼的，叫”钓鱼玩法。”不拿鱼竿鱼钩,在城市人海,默默钓鱼的，叫”钓"+
            "鱼执法。”16、妹妹：”左眼珠要睡觉,右眼珠偏不要。我该怎么办？！”哥哥：”那你就把,心灵的窗户,全部关了，不就ok了。”17、春天如同初恋，夏天则是热恋，秋天仿佛黄昏恋，冬天恰似天使之恋.那么,走过四季的我们是什么恋？360度、甲午风云、春夏秋冬、日月轮回、生死恋.18、新闻报道：现在70%的男人希望发生婚外恋。丈夫听后忙对妻子说:”我一定是那30%的男人！”接着新闻继续报道：其他30%的男人已经有了婚外恋。19、阿钜有一天被欺负,哭啊哭啊哭啊哭啊，然后把自己淹死了。。菲菲却没有淹死，为什么呢?因为菲菲会飞啊。.20、李妈妈年纪大了，特别忌讳一些词,有一次来我家，我倒了一杯茶给她,喝完后我问道：”还喝吗?”李妈妈：”喝!”又一杯下去”还喝吗?””喝!”这样反复好几次，李妈妈胀的不行了,我见状道：”早就不该喝了.”李妈妈气气的说:”我要活！”21、金妹和喜之两姐妹一直没有找到好的归宿，一日，她们看见了今麦郎和喜之郎，于是他们各自结婚了。22、农夫山泉有点甜，这是为什么呢？因为有点虫。真滴吗?至于你信不信，我反正是信的.23、哥们你太有才了，人家都是才高八斗，你是身高八斗,穿中三装八个衣兜，@jllxk001唱忐忑到高8度,浑身发抖.24、一只鹰觅食，见地上跑着一只小兔子,便倾身飞速下冲，瞬间将小兔子抓起,刚要起飞，小兔子说：”我可什么都看见了。”鹰松下抓子把小兔子放了.。。25、【老公酒醒以后】早上醒来，丈夫对妻子说，咱们家有鬼，昨晚我回到家，去厕所,一开门灯就自己亮了，还有一股寒气逼来！妻子一巴掌打过来:你又尿冰箱里啊!丈夫：。。。...26、英语考试，老师把试题发到同学手中后，一同学举手问到:”老师，名字写在哪？”27、小强到动物园看猴子，猴子惊呼：”二师弟，好久不见.”小强:”你一定认错人了，我不认识你的。”猴子：”还装呢，以为谁不知道呢，自己照照镜子。”28、老鼠嘲笑猫的时候,身旁必有一个洞。你捅马蜂窝的时候，身上也要准备有很多很多很多很多。.。。.。洞。。.。.。29、卖火柴的小女孩为什么有火柴还冻死了?因为她卖的是火柴不是柴火，柴火可以取暖，但火柴很快点完了，所以她就冻死了.30、有一只蚊子在你脸上盯了一下,结果嘴巴拔不出来了,后悔莫及,死后把这件事告诉了所有的蚊子，于是大家都知道你脸皮太厚了。31、你们那儿停电了，这么大夏天别人都热死了你却没有热死为什么呢？因为你收到了我的冷笑话短信。。32、有一个番茄一不小心掉进汤里了，于是就有了番茄汤..一个鸡蛋看见了，想去救番茄，结果也掉进去了，就有了番茄蛋花汤。。33、妈妈老跟我提起她和爸爸当年的浪漫。每次妈妈要去上班，爸爸就说：”你路上小心.”妈妈俏皮的答道：”你放78颗心。”爸爸生气的说：”那还22颗让我悬着啊。”34、我很运气网上抽奖抽到一袋大米,兴冲冲的就跟妈妈说了，正好家里的米快吃完了,我妈也没买就和我一起等待这袋大米的到来，终于咚咚来了，一签收傻眼了,大米500g.35、”爱是什么?””爱就是爱呀，就是喜欢一个人呀。””简单点，爱是什么?两个字。””不知。””笨,爱是你我。””怎么会？””歌里唱的：爱是你我。。。”36、”阿钜和菲菲玩捉迷藏。过了一段时间,菲菲问阿钜藏好了吗,阿钜说好了，菲菲就回家了。。”@jllxk001唱忐忑到高8度,浑身发抖.24、一只鹰觅食，见地上跑着一只小兔子,便倾身飞速下冲，瞬间将小兔子抓起,刚要起飞，小兔子说：”我可什么都看见了。”鹰松下抓子把小兔子放了.。。25、【老公酒醒以后】早上醒来，丈夫对妻子说，咱们家有鬼，昨晚我回到家，去厕所,一开门灯就自己亮了，还有一股寒气逼来！妻子一巴掌打过来:你又尿冰箱里啊!丈夫：。。。...26、英语考试，老师把试题发到同学手中后，一同学举手问到:”老师，名字写在哪？”27、小强到动物园看猴子，猴子惊呼：”二师弟，好久不见.”小强:”你一定认错人了，我不认识你的。”猴子：”还装呢，以为谁不知道呢，自己照照镜子。”28、老鼠嘲笑猫的时候,身旁必有一个洞。你捅马蜂窝的时候，身上也要准备有很多很多很多很多。.。。.。洞。。.。.。29、卖火柴的小女孩为什么有火柴还冻死了?因为她卖的是火柴不是柴火，柴火可以取暖，但火柴很快点完了，所以她就冻死了.30、有一只蚊子在你脸上盯了一下,结果嘴巴拔不出来了,后悔莫及,死后把这件事告诉了所有的蚊子，于是大家都知道你脸皮太厚了。31、你们那儿停电了，这么大夏天别人都热死了你却没有热死为什么呢？因为你收到了我的冷笑话短信。。32、有一个番茄一不小心掉进汤里了，于是就有了番茄汤..一个鸡蛋看见了，想去救番茄，结果也掉进去了，就有了番茄蛋花汤。。33、妈妈老跟我提起她和爸爸当年的浪漫。每次妈妈要去上班，爸爸就说：”你路上小心.”妈妈俏皮的答道：”你放78颗心。”爸爸生气的说：”那还22颗让我悬着啊。”34、我很运气网上抽奖抽到一袋大米,兴冲冲的就跟妈妈说了，正好家里的米快吃完了,我妈也没买就和我一起等待这袋大米的到来，终于咚咚来了，一签收傻眼了,大米500g.35、”爱是什么?””爱就是爱呀，就是喜欢一个人呀。””简单点，爱是什么?两个字。””不知。””笨,爱是你我。””怎么会？””歌里唱的：爱是你我。。。”36、”阿钜和菲菲玩捉迷藏。过了一段时间,菲菲问阿钜藏好了吗,阿钜说好了，菲菲就回家了。。”@jllxk00137、有一个人帽子脏了从来不洗，而是反过来继续戴,你知道这是为什么吗？因为”张(脏）冠李（里）戴”呀！38、职员:这么晚去提款我害怕，白天再去好吗。老板:不行,公司等着用钱那.职员：万一有色狼。.。。老板：你拿手电筒去.职员：这个关什么?老板：遇到色狼，你照一下自己的脸。那就是最好的武器，对于你的安全我很放心。39、小红帽独自走在郊外的小路上,太阳下山前她就往家赶了,可她还是遇见了大灰狼,你猜结果怎么样?结果,小红帽被大灰狼吃了。40、一书生饱读诗书，但手无缚鸡之力,有人问他今生最珍惜的东西是什么，他却说是四支箭，众人不解.答曰：”光阴似(四)箭”耳！41、某日,女朋友打电话来：”快看，快看,外面有流星雨,你看到了吗？”我一看,果不其然，马上回话:”看到了,看到了，你呢？”42、一天,石头口渴，去找苹果打架，结果苹果受伤了，石头拿杯接了一杯苹果汁喝了。后来，石头饿了，去找鸡蛋打架，鸡蛋被石头一脚踢到河里,结果，石头又有鸡蛋汤喝了。43、小时候的某一天,我问爸爸：”爸爸，我是最聪明的孩子吗?”爸爸说:”傻瓜，你当然是最聪明的孩子！”44、妈妈做面膜，听到有人按门铃，对三岁的儿子说：妈妈正做面膜不能见人，你去看看！于是儿子开门让进来人说:我妈妈正在做不能见人的事，你等一会吧！45、玫瑰嘲笑月季:看吧,都说了做花要含蓄,太暴漏了，就是上不了台面.月季听了不悦:就你含蓄，把香艳都包起来，去装刺猬，上了台面也顶多是个带刺的。46、夏天到了,一次儿子要我带他去游泳,我很尴尬的说：”我不会游。”儿子很生气的说：”那小明爸爸怎么会啊。””他老吃鱼所以会游.”儿子撅撅嘴说:”可您总吃鸡，那会下蛋不？”47、每次给姥姥说现在都用电脑了,很方便，也能通过电脑视频看到彼此，姥姥都说:”自己脑子能记住模样，电脑还能真看到，了不起呀，是比人脑厉害呀.”48、老公高中英语就不好，现在有了孩子,为了下一代有好点英语的氛围，我强迫他学习英文歌，一周过去我问道：”学了吗？””学了.”老公自信满满的说。紧接着那催人泪下的旋律传入我耳里：ＡＢＣＤＥＦＧ．．．．．．49、儿子最近刚看完《变形金刚3》就问我一个问题：”为什么说三个臭皮匠抵个诸葛亮，我没看过他三头六臂啊，是不是他跟变形金刚一样会变形啊？”我:”.。。。..”@jllxk00137、有一个人帽子脏了从来不洗，而是反过来继续戴,你知道这是为什么吗？因为”张(脏）冠李（里）戴”呀！38、职员:这么晚去提款我害怕，白天再去好吗。老板:不行,公司等着用钱那.职员：万一有色狼。.。。老板：你拿手电筒去.职员：这个关什么?老板：遇到色狼，你照一下自己的脸。那就是最好的武器，对于你的安全我很放心。39、小红帽独自走在郊外的小路上,太阳下山前她就往家赶了,可她还是遇见了大灰狼,你猜结果怎么样?结果,小红帽被大灰狼吃了。40、一书生饱读诗书，但手无缚鸡之力,有人问他今生最珍惜的东西是什么，他却说是四支箭，众人不解.答曰：”光阴似(四)箭”耳！41、某日,女朋友打电话来：”快看，快看,外面有流星雨,你看到了吗？”我一看,果不其然，马上回话:”看到了,看到了，你呢？”42、一天,石头口渴，去找苹果打架，结果苹果受伤了，石头拿杯接了一杯苹果汁喝了。后来，石头饿了，去找鸡蛋打架，鸡蛋被石头一脚踢到河里,结果，石头又有鸡蛋汤喝了。43、小时候的某一天,我问爸爸：”爸爸，我是最聪明的孩子吗?”爸爸说:”傻瓜，你当然是最聪明的孩子！”44、妈妈做面膜，听到有人按门铃，对三岁的儿子说：妈妈正做面膜不能见人，你去看看！于是儿子开门让进来人说:我妈妈正在做不能见人的事，你等一会吧！45、玫瑰嘲笑月季:看吧,都说了做花要含蓄,太暴漏了，就是上不了台面.月季听了不悦:就你含蓄，把香艳都包起来，去装刺猬，上了台面也顶多是个带刺的。46、夏天到了,一次儿子要我带他去游泳,我很尴尬的说：”我不会游。”儿子很生气的说：”那小明爸爸怎么会啊。””他老吃鱼所以会游.”儿子撅撅嘴说:”可您总吃鸡，那会下蛋不？”47、每次给姥姥说现在都用电脑了,很方便，也能通过电脑视频看到彼此，姥姥都说:”自己脑子能记住模样，电脑还能真看到，了不起呀，是比人脑厉害呀.”48、老公高中英语就不好，现在有了孩子,为了下一代有好点英语的氛围，我强迫他学习英文歌，一周过去我问道：”学了吗？””学了.”老公自信满满的说。紧接着那催人泪下的旋律传入我耳里：ＡＢＣＤＥＦＧ．．．．．．49、儿子最近刚看完《变形金刚3》就问我一个问题：”为什么说三个臭皮匠抵个诸葛亮，我没看过他三头六臂啊，是不是他跟变形金刚一样会变形啊？”我:”.。。。..”@jllxk00150、一次看到老公跟别的女人在一起吃饭，自己吃醋了又不好直说，晚上回来，闷闷不乐，老公问我怎么了,我问道：”女人最爱吃男人的什么啊？”老公坏笑道：”豆腐.”51、一个石头找东西……找啊找……找到一个石头;他们一块接着找…找啊找……又找到一个石头……嗯，于是小‘磊”出现了……52、阴间评选最可爱的鬼,于是吊死鬼得了冠军，因为他死了还不忘吐舌头，多可爱啊。53、其实龟兔赛跑的故事中,还有一段鲜为人知的冷笑话今天分享给您：兔子一路遥遥领先,乌龟在后爬啊爬，忽见一蜗牛，它说：”龟哥我跟你顺路,搭我一个吧。”乌龟点点了头，等乌龟超过了睡觉的兔子之后，又碰见一只蚂蚁被晒得不行的,也上了乌龟壳上，蜗牛搭讪道：”哥们抓好了，速度快，不要被甩出去了。”54、机动车和玩具车在一起夸自己。机动车说道:”我个大力大，能为人们办事，所以人们都很喜欢我!”玩具车不服气的说道:”你个再大，力再大能为人们办事又怎么了？还不是被人们骑被人们坐吗？而我就不同了，人们都得让我坐让我骑。”机动车闻言愣了。55、猜一猜为什么现在大家都喜欢讲冷笑话呢？很热?no。.很无聊?no。。因为.。没有冷哭话.。56、曾以为今生能与你牵手一直走,曾以为能与你相爱日日夜夜,曾以为这辈子心中只装着你一个，直到昨天遇见了你那有钱的二大爷，唉,原谅我!57、一天去开会，进去后互相问候：”你好，同志，哪个部门的?”答：”教育部门的。””哦,幸会幸会。”另一个说,”我们公安部的.””哦,您好您好，久仰久仰。”最后一个说：”我们,那个，嗯。..有关部门的”顿时无语。”？？？天哪!从来只是听说,这回见着活的了！”58、植物大战僵尸谁赢了?植物.因为植物讲了个冷笑话,僵尸冷死了。。植物又大战僵尸谁赢了？僵尸,因为僵尸学会讲冷笑话了。..59、啊刀和啊朱在一个房间，第二天啊朱死了，为什么？猪被刀杀了.。。。。。。。.60、炎炎夏日送你一个冷冰棒的故事，从前有一根冰棒，老呆在冰箱里觉得闷，就跟同伴说：”我出去晒晒太阳。”来到外面,又抱怨道：”我滴亲乖乖,好热哦，脱衣服。”脱着，脱着变成水了.61、为什么蚊香能让蚊子不咬人？因为蚊香蚊香吗，蚊子闻着香，就睡着了,当然就不咬人了呀。62、一天老公回家看到老婆头发染成金黄色的，叹到：甘蔗头变着玉米棒了,@jllxk00150、一次看到老公跟别的女人在一起吃饭，自己吃醋了又不好直说，晚上回来，闷闷不乐，老公问我怎么了,我问道：”女人最爱吃男人的什么啊？”老公坏笑道：”豆腐.”51、一个石头找东西……找啊找……找到一个石头;他们一块接着找…找啊找……又找到一个石头……嗯，于是小‘磊”出现了……52、阴间评选最可爱的鬼,于是吊死鬼得了冠军，因为他死了还不忘吐舌头，多可爱啊。53、其实龟兔赛跑的故事中,还有一段鲜为人知的冷笑话今天分享给您：兔子一路遥遥领先,乌龟在后爬啊爬，忽见一蜗牛，它说：”龟哥我跟你顺路,搭我一个吧。”乌龟点点了头，等乌龟超过了睡觉的兔子之后，又碰见一只蚂蚁被晒得不行的,也上了乌龟壳上，蜗牛搭讪道：”哥们抓好了，速度快，不要被甩出去了。”54、机动车和玩具车在一起夸自己。机动车说道:”我个大力大，能为人们办事，所以人们都很喜欢我!”玩具车不服气的说道:”你个再大，力再大能为人们办事又怎么了？还不是被人们骑被人们坐吗？而我就不同了，人们都得让我坐让我骑。”机动车闻言愣了。55、猜一猜为什么现在大家都喜欢讲冷笑话呢？很热?no。.很无聊?no。。因为.。没有冷哭话.。56、曾以为今生能与你牵手一直走,曾以为能与你相爱日日夜夜,曾以为这辈子心中只装着你一个，直到昨天遇见了你那有钱的二大爷，唉,原谅我!57、一天去开会，进去后互相问候：”你好，同志，哪个部门的?”答：”教育部门的。””哦,幸会幸会。”另一个说,”我们公安部的.””哦,您好您好，久仰久仰。”最后一个说：”我们,那个，嗯。..有关部门的”顿时无语。”？？？天哪!从来只是听说,这回见着活的了！”58、植物大战僵尸谁赢了?植物.因为植物讲了个冷笑话,僵尸冷死了。。植物又大战僵尸谁赢了？僵尸,因为僵尸学会讲冷笑话了。..59、啊刀和啊朱在一个房间，第二天啊朱死了，为什么？猪被刀杀了.。。。。。。。.60、炎炎夏日送你一个冷冰棒的故事，从前有一根冰棒，老呆在冰箱里觉得闷，就跟同伴说：”我出去晒晒太阳。”来到外面,又抱怨道：”我滴亲乖乖,好热哦，脱衣服。”脱着，脱着变成水了.61、为什么蚊香能让蚊子不咬人？因为蚊香蚊香吗，蚊子闻着香，就睡着了,当然就不咬人了呀。62、一天老公回家看到老婆头发染成金黄色的，叹到：甘蔗头变着玉米棒了,@jllxk001@jllxk001第二天,老婆回家把头发又烫成卷发了,老公：哎，玉米炒成爆米花了。63、阿钜感冒咳嗽了，怎么吃药都没用，但是上了一下指客网就好了，为什么呢？因为”止咳”网啊...64、往前看山,大雾遮住半边天，往后看河，久经干涸水难得，往上看看太阳，仿似害羞躲在云里头，没办法，只能低头看地球，发现下面地震了，我飘在风里头。65、”前几天一女孩来买东西，让我着实郁闷,第一天她问:”老板，有一百包面吗?”我:”真抱歉，没有那么多。””这样啊.”女孩垂头丧气地走了.第二天，她有来了”老板，有一百包面吗？”老板:”对不起，还是没有””这样啊.”女孩又垂头丧气地走了.第三天，又来了”老板,有一百包面吗?”老板高兴的说：”有了有了，今天有一百包面了！女孩掏出钱：”太好了，我买两包！””66、你说爬山看日出看到太阳的时候多，还是看到月亮的时候多？答案：看到太阳的时候多一点，因为上山容易下山难.67、最新消息：七月二十三日温州动车追尾事故调查最终结果已经公布，原因居然是铁路没有急转弯道，最终导致出现追尾事故.至于你信不信,我反正信了.68、阿钜以前借了弟弟几百块，最近弟弟要求阿钜还钱,阿钜拿不出钱来，就把自己家的猪给了弟弟，然后阿钜就变成女的了。.为什么？因为”还猪哥哥”。.69、一天，唐僧问悟空：”悟空，你的筋斗云到底有多快?”大圣想了想说:”师傅，也就和现代动车的速度差不多，坐上，嗖，到西天了！”70、A和B在闲聊,说着说着A就说：天气太热啦，我都买不到生鸡蛋了！昨天我买了个凉席，一睡变成电热毯了!汽车不用点火自己着了！刚在路上遇到个陌生人，相视一笑，熟了!桌子太烫了,麻将刚码好,居然糊了。B:最安全的高铁也来给这个高温季节加温，不甘寂寞的时代呀。71、猫和老鹰私奔以后就有了猫头鹰，蜘蛛和大虾私奔以后就有了蜘蛛侠，王功权和王琴私奔以后,羡慕的人少，嫉妒的人多。72、回家很晚的丈夫编了好几个理由。妻子：别编了，我知道你去坐动车了！丈夫争辩说：我没坐,我没坐！妻子大声道:没坐动车,你怎么也出轨了？73、语文课上，老师让小明用”黄河”造句。小明答：”黄河很黄。”老师不悦：”不行，改改！”小明更不开心，把头一扭:”凭什么，我又没有钱买漂白粉！”74、老婆：”老公，你看我长得是不是很漂亮呀？”老公：”漂亮，非常漂亮，就是比凤姐差了一点点。”@jllxk001第二天,老婆回家把头发又烫成卷发了,老公：哎，玉米炒成爆米花了。63、阿钜感冒咳嗽了，怎么吃药都没用，但是上了一下指客网就好了，为什么呢？因为”止咳”网啊...64、往前看山,大雾遮住半边天，往后看河，久经干涸水难得，往上看看太阳，仿似害羞躲在云里头，没办法，只能低头看地球，发现下面地震了，我飘在风里头。65、”前几天一女孩来买东西，让我着实郁闷,第一天她问:”老板，有一百包面吗?”我:”真抱歉，没有那么多。””这样啊.”女孩垂头丧气地走了.第二天，她有来了”老板，有一百包面吗？”老板:”对不起，还是没有””这样啊.”女孩又垂头丧气地走了.第三天，又来了”老板,有一百包面吗?”老板高兴的说：”有了有了，今天有一百包面了！女孩掏出钱：”太好了，我买两包！””66、你说爬山看日出看到太阳的时候多，还是看到月亮的时候多？答案：看到太阳的时候多一点，因为上山容易下山难.67、最新消息：七月二十三日温州动车追尾事故调查最终结果已经公布，原因居然是铁路没有急转弯道，最终导致出现追尾事故.至于你信不信,我反正信了.68、阿钜以前借了弟弟几百块，最近弟弟要求阿钜还钱,阿钜拿不出钱来，就把自己家的猪给了弟弟，然后阿钜就变成女的了。.为什么？因为”还猪哥哥”。.69、一天，唐僧问悟空：”悟空，你的筋斗云到底有多快?”大圣想了想说:”师傅，也就和现代动车的速度差不多，坐上，嗖，到西天了！”70、A和B在闲聊,说着说着A就说：天气太热啦，我都买不到生鸡蛋了！昨天我买了个凉席，一睡变成电热毯了!汽车不用点火自己着了！刚在路上遇到个陌生人，相视一笑，熟了!桌子太烫了,麻将刚码好,居然糊了。B:最安全的高铁也来给这个高温季节加温，不甘寂寞的时代呀。71、猫和老鹰私奔以后就有了猫头鹰，蜘蛛和大虾私奔以后就有了蜘蛛侠，王功权和王琴私奔以后,羡慕的人少，嫉妒的人多。72、回家很晚的丈夫编了好几个理由。妻子：别编了，我知道你去坐动车了！丈夫争辩说：我没坐,我没坐！妻子大声道:没坐动车,你怎么也出轨了？73、语文课上，老师让小明用”黄河”造句。小明答：”黄河很黄。”老师不悦：”不行，改改！”小明更不开心，把头一扭:”凭什么，我又没有钱买漂白粉！”74、老婆：”老公，你看我长得是不是很漂亮呀？”老公：”漂亮，非常漂亮，就是比凤姐差了一点点。”@jllxk00175、狗咬人是真的,人咬狗？嘿嘿!哥只是个传说！人咬狗肉是真的。大夏天的咬什么狗肉呀，不嫌热？（www。siandian.com闪点情话网）嘿嘿！说冷的GG帅，说冷的MM美!76、一男子很羡慕旧社会的男子可以有三妻四妾.新婚之夜他斗胆向新娘说出了自己的理想，新娘善解人意,含情脉脉的说：我一定当好你的贤内助，从现在起我改名叫三妻,只要咱俩一起努力,保证不到一年我就给你生个女儿，起名四妾。这样,你的愿望很快就能实现了。77、小刚开车出去找想小明，看到小明骑摩托就问小明说：”汽车怎么没有摩托车一样的档位显示屏呢?”小明说：”因为摩托没有汽车那么多大啊,没有地方放档杆,汽车的不是在档杆上吗？”78、情人节阿钜给菲菲发短信”情人节快乐”，菲菲很生气,回”为什么只有这五个字，一点诚意也没有！”阿钜”因为我发的是短。。。信啊。。”79、丰胸是一项非常热门的美容项目,那么你知道自我进行丰胸锻炼最好的方法是什么吗？宽容一点就OK了哦，因为”有容乃大”嘛!80、都说”煮熟的鸭子飞了”,那怎样才能让鸭子不会飞走呢?不知道了吧？那就是给它插一只翅膀，因为”插翅难飞”。81、有两个人多少年了一直相爱，却一直不能在一起，某天地震了,他们终于在一起了,他们就是.。天花板和地板..82、孔子有三千弟子，其中三位徒弟特别有名，那就是子贡、子路和子游，可他们三人中有一个不是人，你知道是谁吗？答案当然是子路,因为指鹿（子路)为马呀！83、有两颗球,很喜欢黏在一起,然后就变成葫芦了。。又来了很多球,喜欢跟他们黏在一起,然后就变成糖葫芦了。。84、还记得那时老师说：谁觉得自己很蠢的请站起来.同学们都沉默着，几分钟后,我勇敢的站起来.那时老师你问我：怎么觉得自己蠢？老师笑着抢这说,你那时说:不是呀，我是不忍心你一个人站着.我和老师都哈哈大笑起来。85、我哥的电脑屏幕上有个类似新闻滚动条的东西，上面的文字过得非常快。我好奇问：”是歌词吗?””是呀！””怎么过得这么快?都没看清！”答曰：”是周杰伦的！”86、有个人姓米,别人叫他”小米”，他不乐意，非让人叫他”大米”，叫着叫着，他就被老鼠吃了。87、那次一起在街边摊吃臭豆腐，阿钜和菲菲一人一串，阿钜一个劲的夸:”臭@jllxk001豆腐可是闻着很臭，吃着那叫一个香!好吃！”菲菲撇着嘴吃了口说到：”闻着的确很臭，不过吃着更臭!”88、音乐课上，老师吹奏了一曲《苏武牧羊》，问学生：”你懂音乐吗?””懂。””那我吹的是什么？””笛子……”89、人爱,就是人和人，产生好感，在悲欢离合、风花雪月、纷纷扰扰的人世间,互相付出的那一份说不清,道不明的关怀呵护.爱人，就是你一天把他当儿子、又当孙子，滔滔不绝、杂七杂八、疯癫痴狂，骂他个千二八百遍,却不许别人骂一句的人。90、有一天逛书市,看到一套《高尔基小说选》,非常喜欢其中的一本《童年》，就问售货员:”请问，这是单卖的吗？””对不起，这是前苏联的.”91、人们老说兔子的尾巴长不了，兔子不服气的说：再短也没有女女们的裙子短！92、从前有一只大象，有一天它忽然觉得自己不像大象也不像二象不像三象更不像四象，最后它就成了四不像。.。93、一哥们只要一上课就开始打瞌睡，为此老师已对其”看之任之”了,一天上午，该哥们睡醒了看着黑板上的语文课文板书自语了句：”咱数学老师真牛，语文也能教得这么好！”旁边一同学回日：”不好意思,现在是语文老师在讲课！数学课已经上完了!”94、”英语考场上,一哥们抄得不亦乐乎，监考老师说了句:”you，out!”该考生抬起头望着老师郁闷了一下接着又开始”奋笔疾抄”,老师见状又说了句：”you，out！”见该生没有任何反应，老师接着来了句:”你，出去！”考生答道：”老师，您早说中文不就得了嘛,我听不懂英语！””95、一次吃过饭，小张问曰你去餐厅吃饭没有啊，答曰当然去了,复问曰怎么没看见你啊，答曰我都看见你了，可能是我刚才隐身忘记上线了吧，小张一阵无语。。。96、萧明理完发回寝室的途中，在女生寝室楼下碰见同学，同说”喂油，帅啦！”萧明得意着说”只是剪了个酷头!”二楼一女生探出头来冲楼下大叫”谁拣了裤头？拿出来看一下是不是我丢的那个。”两个人听吧。。.。.97、我正在办公室批改作业，有学生来报告：”老师，外面有个阿姨找您！”我随口问道：”哦，男的,女的？”98、狸猫看到小耗子，就追了过去,可是怎么也追不上小耗子，狸猫就去问狐狸了：”怎么我追耗子追不上呢？”狐狸告诉猫：”耗子是不会走直线的，东躲西藏的，你是不是看到瞎猫撞死耗子就也跑直线了？”@jllxk00199、一同学经常去网吧上网,有一天上微机课，他突然大声喊道:”网管。”另一日在网吧上网,又突然举手叫道：”老师.”100、四岁的阿明,在公园的广场上看一群大哥哥们跳机械舞,高兴的鼓掌欢呼，”大哥哥你们好棒哦.”当跳到慢舞步时，阿明诧异的问:”大哥哥你们卡屏了吗?我爸爸说卡屏了”重启”一下就可以的呦。”";
    }
    //private string[] LoadingJoke(string s)
    //{
    //    var regex = new Regex("(?<=(\\d{2}|\\d))[.\\s\\S]*?(?=(\\d{2}|\\d))");
    //}
    private async Task CallMenu(SocketMessage msg)
    {
        if (msg.Author.Id == _client.CurrentUser.Id) return;
        if (new Regex("^/menu").IsMatch(msg.Content))
        {
            CardBuilder cardBuilder = new CardBuilder()
                .WithSize(CardSize.Large)
                .AddModule(new HeaderModuleBuilder().WithText("菜单"))
                .AddModule(new HeaderModuleBuilder().WithText("(parameter)为必要参数[parameter]为可选参数"))
                .AddModule(new SectionModuleBuilder().WithText(new ParagraphStructBuilder()
                    .WithColumnCount(2)
                    .AddField(new KMarkdownElementBuilder().WithContent("点歌"))
                    .AddField(new KMarkdownElementBuilder().WithContent("/m (歌曲名)")
                    )))
                .AddModule(new SectionModuleBuilder().WithText(new ParagraphStructBuilder()
                    .WithColumnCount(2)
                    .AddField(new KMarkdownElementBuilder().WithContent("查logs，暂时只支持网页中查看"))
                    .AddField(new KMarkdownElementBuilder().WithContent("/l (玩家名) [服务器名]")
                    )))
                .AddModule(new SectionModuleBuilder().WithText(new ParagraphStructBuilder()
                    .WithColumnCount(2)
                    .AddField(new KMarkdownElementBuilder().WithContent("分析logs，暂只支持网页中查看"))
                    .AddField(new KMarkdownElementBuilder().WithContent("/a (logs网址)")
                    )))
                .AddModule(new SectionModuleBuilder().WithText(new ParagraphStructBuilder()
                    .WithColumnCount(2)
                    .AddField(new KMarkdownElementBuilder().WithContent("查询物品板子价格"))
                    .AddField(new KMarkdownElementBuilder().WithContent("/cv (物品名称) [列举数量] [服务器]"))))
                .AddModule(new SectionModuleBuilder().WithText(new ParagraphStructBuilder()
                    .WithColumnCount(2)
                    .AddField(new KMarkdownElementBuilder().WithContent("查询大区物品均价"))
                    .AddField(new KMarkdownElementBuilder().WithContent("/ca (物品名称) [大区服务器]"))));

            await _client.GetGuild(_guildId).GetTextChannel(_channelId).SendCardMessageAsync(cardBuilder.Build());
        }
    }
    private async Task CallMusic(SocketMessage msg)
    {
        if (msg.Author.Id == _client.CurrentUser.Id) return;
        Regex regex = new Regex(@"^/m ");
        if (regex.IsMatch(msg.Content))
        {
            int index = 0;
            var name = msg.Content.ToString().Replace("/m ", "");
            string albumName;
            SongResult result;
            string imageUrl;
            string Url;
            result = _neteaseTool.SearchMusic(name).Result;
            var songs = result.result.songs;
            if (songs != null)
            {
                Url = _neteaseTool.GetMusicUrlById(_neteaseTool.GetIdByResult(result));
                albumName = (songs[0].album.name != "") ? songs[0].album.name : "暂无";
                imageUrl = _neteaseTool.GetMusicCoverByAlbumId(songs[0].album.id);
            }
            else
            {
                result = _neteaseTool.SearchMusic("Never Gonna Give You Up").Result;
                Url = _neteaseTool.GetMusicUrlById(5221167);
                albumName = "Simply The Best Of The 80's";
                imageUrl = "http://p1.music.126.net/JCByvg7bzHn26WOQI7b-AQ==/1766915185845816.jpg?param=130y130";
            }
            string imageLocal = $"./images/{songs[0].name}";
            using (WebClient client = new WebClient())
            {
                if (!File.Exists(imageLocal))
                {
                    byte[] buffer = client.DownloadData(new Uri(imageUrl));
                    using (FileStream fileStream = new FileStream(imageLocal, FileMode.CreateNew))
                    {
                        fileStream.Write(buffer, 0, buffer.Length);
                        fileStream.Close();
                    }
                }
            }   //从网页下载图片到本地
            var imageStream = File.OpenRead(imageLocal);
            imageUrl = _client.Rest.CreateAssetAsync(imageStream, albumName).Result;    //上传图片到KHL服务器，返回Url
            CardBuilder cardBuilder = new CardBuilder()
            .WithSize(CardSize.Large)
            .AddModule(new SectionModuleBuilder()
                    .WithText(new KMarkdownElementBuilder().WithContent($"**歌手**:{result.result.songs[0].artists[0].name}\n**专辑**:{albumName}"))
                    .WithAccessory(new ImageElementBuilder()
                    .WithSource(imageUrl)
                    .WithSize(ImageSize.Small))
                .WithMode(SectionAccessoryMode.Right))
            .AddModule(new AudioModuleBuilder().WithSource(Url))
            .AddModule(new SectionModuleBuilder().WithText(new KMarkdownElementBuilder().WithContent($"[在网页中查看]({_neteaseTool.GetMusicUrlOrginById(result.result.songs[0].id)})")))
            .AddModule(new ActionGroupModuleBuilder()
                .AddElement(new ButtonElementBuilder().WithText("不是这首歌？").WithValue($"不是这首歌_{name}").WithClick(ButtonClickEventType.ReturnValue))
            );
            File.Delete(imageLocal);
            await _client.GetGuild(_guildId).GetTextChannel(_channelId).SendCardMessageAsync(cardBuilder.Build());
        }
    }
    private async Task CheckAct(SocketMessage msg)
    {
        if (msg.Author.Id == _client.CurrentUser.Id) return;
        Regex regex = new Regex("^/l [.\\s\\S]*? [.\\s\\S]*?");
        if (regex.IsMatch(msg.Content))
        {
            var cardBuilder = new CardBuilder();
            var result = msg.Content.Replace("/l ","");
            var data = result.Split(" ");
            var url = _actTools.GetUrlByNameAndServer(data[0], data[1]);
            if (_actTools.GetHtmlByUrl(url).Contains("未找到指定的角色和服务器。可能是由于该角色还没有被记录"))
            {
                var date = msg.Content.Replace("/l ", "").Split(" ");
                var PlayerListUrl = _actTools.GetUrlByName(data[0]);
                var PlayerListHtml = _actTools.GetHtmlByUrl(PlayerListUrl);
                var PlayerList = _actTools.GetPlayersListByHtml(PlayerListHtml);
                var PlayerList2=new List<GetActResultTools.Players>();
                for (int i = 0; i < PlayerList.Count; i++)
                {
                    var regexServer=new Regex("(?<=(- ))[.\\s\\S]*");
                    if (regexServer.Match(PlayerList[i].Server).Value.Equals(data[1]))
                    {
                        PlayerList2.Add(PlayerList[i]);
                    }
                }
                if (PlayerList2.Count == 0)
                {
                    _client.GetGuild(_guildId).GetTextChannel(_channelId).SendTextMessageAsync("查询内容为空");
                    return;
                }
                cardBuilder.AddModule(new HeaderModuleBuilder().WithText("在以下列表中网页里查看")).WithSize(CardSize.Large);
                if (PlayerList2.Count < 10)
                {
                    for (int i = 0; i < PlayerList2.Count; i++)
                    {
                        
                        cardBuilder.AddModule(new SectionModuleBuilder().WithText(new ParagraphStructBuilder()
                            .WithColumnCount(3)
                            .AddField(new KMarkdownElementBuilder().WithContent($"{PlayerList2[i].Name}"))
                            .AddField(new KMarkdownElementBuilder().WithContent($"{PlayerList2[i].Server}"))
                            .AddField(new KMarkdownElementBuilder().WithContent($"[在网页中查看]({PlayerList2[i].Url})")))
                            );
                    }
                }
                if (PlayerList2.Count > 10) for (int i = 0; i < 10; i++)
                    {
                        cardBuilder.AddModule(new SectionModuleBuilder().WithText(new ParagraphStructBuilder()
                            .WithColumnCount(3)
                            .AddField(new KMarkdownElementBuilder().WithContent($"{PlayerList2[i].Name}"))
                            .AddField(new KMarkdownElementBuilder().WithContent($"{PlayerList2[i].Server}"))
                            .AddField(new KMarkdownElementBuilder().WithContent($"[在网页中查看]({PlayerList2[i].Url})")))
                            );
                    }
                _client.GetGuild(_guildId).GetTextChannel(_channelId).SendCardMessageAsync(cardBuilder.Build());
                return;
            }
            cardBuilder
                .WithSize(CardSize.Large)
                .AddModule(new SectionModuleBuilder().WithText($"[{data[0]}]({url})",true));
            _client.GetGuild(_guildId).GetTextChannel(_channelId).SendCardMessageAsync(cardBuilder.Build());
            return;
        }
        var regex2 = new Regex("^/l [.\\s\\S]*?");
        if (regex2.IsMatch(msg.Content))
        {
            var PlayerListUrl=_actTools.GetUrlByName(msg.Content.Replace("/l ",""));
            var playerListHtml = _actTools.GetHtmlByUrl(PlayerListUrl);
            var PlayerList=_actTools.GetPlayersListByHtml(playerListHtml);
            if (PlayerList.Count == 0)
            {
                _client.GetGuild(_guildId).GetTextChannel(_channelId).SendTextMessageAsync("查询内容为空");
                return;
            }
            var cardBuilder = new CardBuilder()
                .AddModule(new HeaderModuleBuilder().WithText("在以下列表中网页里查看")).WithSize(CardSize.Large);
            if (PlayerList.Count < 10)
            {
                for (int i = 0; i < PlayerList.Count; i++)
                {
                    cardBuilder.AddModule(new SectionModuleBuilder().WithText(new ParagraphStructBuilder()
                        .WithColumnCount(3)
                        .AddField(new KMarkdownElementBuilder().WithContent($"{PlayerList[i].Name}"))
                        .AddField(new KMarkdownElementBuilder().WithContent($"{PlayerList[i].Server}"))
                        .AddField(new KMarkdownElementBuilder().WithContent($"[在网页中查看]({PlayerList[i].Url})")))
                        );
                }
            }
            if(PlayerList.Count >10) for (int i = 0; i < 10; i++)
                {
                    cardBuilder.AddModule(new SectionModuleBuilder().WithText(new ParagraphStructBuilder()
                        .WithColumnCount(3)
                        .AddField(new KMarkdownElementBuilder().WithContent($"{PlayerList[i].Name}"))
                        .AddField(new KMarkdownElementBuilder().WithContent($"{PlayerList[i].Server}"))
                        .AddField(new KMarkdownElementBuilder().WithContent($"[在网页中查看]({PlayerList[i].Url})")))
                        );
                }
            _client.GetGuild(_guildId).GetTextChannel(_channelId).SendCardMessageAsync(cardBuilder.Build());
        }
    }
    private async Task AnalyzeAct(SocketMessage msg)
    {
        var regex = new Regex("^/a ");
        var regexLogsBundle = new Regex("(?<=(/reports/))[.\\s\\S]*?(?=(#fight=))");
        var regexLogsBundleId = new Regex("(?<=(#fight=))[.\\s\\S]*?(?=(&type))");
        if (msg.Author.Id == _client.CurrentUser.Id) return;
        if (regex.IsMatch(msg.Content))
        {
            var url = msg.Content.ToString().Replace("/a ", "");
            var logsBundle = regexLogsBundle.Match(url).Value;
            var Id = regexLogsBundleId.Match(url).Value;
            var cardBuilder=new CardBuilder();
            cardBuilder.WithSize(CardSize.Large)
                .AddModule(new SectionModuleBuilder().WithText(new KMarkdownElementBuilder().WithContent($"[在网页中查看](https://xivanalysis.com/fflogs/{logsBundle}/{Id})")));
            await _client.GetGuild(_guildId).GetTextChannel(_channelId).SendCardMessageAsync(cardBuilder.Build());
        }
    }
    private async Task CheckValue(SocketMessage msg)    //data为(名字，列表数，服务器名)
    {
        var regex = new Regex("^/cv ");
        if(msg.Author.Id == _client.CurrentUser.Id) return;
        if (regex.IsMatch(msg.Content))
        {
            var SelectItem = new CardBuilder();
            var str = msg.Content.Replace("/cv ","");
            
            string[] data = { str };
            if (str.Contains(" "))
            {
                data = str.Split(" ");
            }
            if (data.Length == 1)
            {
                var searchResult=await _XIVAPI.GetResultByNameAsync(data[0]);
                for(int i = 0; i < searchResult.Count; i++)
                {
                    SelectItem.AddModule(new ActionGroupModuleBuilder().AddElement(
                        (
                        new ButtonElementBuilder().WithText(searchResult[i].Name)
                        .WithClick(ButtonClickEventType.ReturnValue).WithValue($"CheckItem_{searchResult[i].ID}_20_1043"))));
                }
            }
            if (data.Length == 2)
            {
                var searchResult = await _XIVAPI.GetResultByNameAsync(data[0]);
                for (int i = 0; i < searchResult.Count; i++)
                {
                    SelectItem.AddModule(new ActionGroupModuleBuilder().AddElement(
                        (
                        new ButtonElementBuilder().WithText(searchResult[i].Name)
                        .WithClick(ButtonClickEventType.ReturnValue).WithValue($"CheckItem_{searchResult[i].ID}_{data[1]}_1043"))));
                }
            }
            if (data.Length == 3)
            {
                var searchResult = await _XIVAPI.GetResultByNameAsync(data[0]);
                for (int i = 0; i < searchResult.Count; i++)
                {
                    SelectItem.AddModule(new ActionGroupModuleBuilder().AddElement(
                        (
                        new ButtonElementBuilder().WithText(searchResult[i].Name)
                        .WithClick(ButtonClickEventType.ReturnValue).WithValue($"CheckItem_{searchResult[i].ID}_{data[1]}_{data[2]}"))));
                }
            }
            await _client.GetGuild(_guildId).GetTextChannel(_channelId).SendCardMessageAsync(SelectItem.Build());
        }
    }
    private async Task CheckItemButtonClick(string value, SocketUser user, IMessage msg, SocketTextChannel channel, SocketGuild guild)
    {
        var regex = new Regex("^CheckItem_");
        var itemsCard = new CardBuilder();
        itemsCard.WithSize(CardSize.Large);
        if (regex.IsMatch(value))
        {
            var str = value.Replace("CheckItem_", "");
            var data = str.Split("_");
            var result =_XIVAPI.GetMarketBoardListings(data[0], data[1],data[2]);
            itemsCard.AddModule(new SectionModuleBuilder().WithText(new ParagraphStructBuilder().WithColumnCount(3)
                .AddField(new KMarkdownElementBuilder().WithContent("**卖主**"))
                .AddField(new KMarkdownElementBuilder().WithContent("**单价**"))
                .AddField(new KMarkdownElementBuilder().WithContent("**数量**"))));
            for (int i = 0; i < result.Count; i++)
            {
                itemsCard.AddModule(new SectionModuleBuilder().WithText(new ParagraphStructBuilder().WithColumnCount(3)
                .AddField(new KMarkdownElementBuilder().WithContent($"{result[i].Name}"))
                .AddField(new KMarkdownElementBuilder().WithContent($"{result[i].Price}"))
                .AddField(new KMarkdownElementBuilder().WithContent($"{result[i].Quantity}"))));
            }
            await _client.GetGuild(_guildId).GetTextChannel(_channelId).SendCardMessageAsync(itemsCard.Build());
        }
    }
    private async Task CheckAveragePrice(SocketMessage msg)
    {
        var regex = new Regex("^/ca ");
        if (regex.IsMatch(msg.Content))
        {
            CardBuilder card = new CardBuilder().WithSize(CardSize.Large);
            var str = msg.Content.Replace("/ca ", "");
            string[] data = { str };
            if (str.Contains(" "))
            {
                data = str.Split(" ");
            }
            if (data.Length == 1)
            {
                var searchResult = await _XIVAPI.GetResultByNameAsync(data[0]);
                for (int i = 0; i < searchResult.Count; i++)
                {
                    card.AddModule(new ActionGroupModuleBuilder().AddElement(
                        (
                        new ButtonElementBuilder().WithText(searchResult[i].Name)
                        .WithClick(ButtonClickEventType.ReturnValue).WithValue($"CheckAveragePrice_{searchResult[i].ID}_猫小胖"))));
                }  
                await _client.GetGuild(_guildId).GetTextChannel(_channelId).SendCardMessageAsync(card.Build());
            }
            if (data.Length == 2)
            {
                var searchResult = await _XIVAPI.GetResultByNameAsync(data[0]);
                for (int i = 0; i < searchResult.Count; i++)
                {
                    card.AddModule(new ActionGroupModuleBuilder().AddElement(
                        (
                        new ButtonElementBuilder().WithText(searchResult[i].Name)
                        .WithClick(ButtonClickEventType.ReturnValue).WithValue($"CheckAveragePrice_{searchResult[i].ID}_{data[1]}"))));
                }
                await _client.GetGuild(_guildId).GetTextChannel(_channelId).SendCardMessageAsync(card.Build());
            }
        }
    }
    private async Task CheckAveragePriceButtonClick(string value, SocketUser user, IMessage msg, SocketTextChannel channel, SocketGuild guild)
    {
        var regex = new Regex("^CheckAveragePrice_");
        if (regex.IsMatch(value))
        {
            var card = new CardBuilder().WithSize(CardSize.Large).AddModule(new SectionModuleBuilder().WithText(new ParagraphStructBuilder().WithColumnCount(3)
                .AddField(new KMarkdownElementBuilder().WithContent("**服务器**"))
                .AddField(new KMarkdownElementBuilder().WithContent("**平均价格**"))));
            List<ItemValue> list = new List<ItemValue>();
            var data = value.Replace("CheckAveragePrice_", "").Split("_");
            var servers = _XIVAPI.Servers[data[1]];
            int values=0;
            int sum=0;
            var price=new Dictionary<string, int>();
            for(int i = 0; i < servers.Length; i++)
            {
                var result = _XIVAPI.GetMarketBoardListings(data[0], "10", servers[i]);
                for(int j = 0; j < result.Count; j++)
                {
                    values += Convert.ToInt32(result[j].Price);
                }
                price.Add(servers[i],values/10);
                values = 0;
                card.AddModule(new SectionModuleBuilder().WithText(new ParagraphStructBuilder().WithColumnCount(3)
                .AddField(new KMarkdownElementBuilder().WithContent($"{servers[i]}"))
                .AddField(new KMarkdownElementBuilder().WithContent($"       {price[servers[i]]}"))
                .AddField(new KMarkdownElementBuilder().WithContent(""))).WithMode(SectionAccessoryMode.Right)
                    .WithAccessory(new ButtonElementBuilder().WithText("查询细节").WithClick(ButtonClickEventType.ReturnValue).WithValue($"CheckPriceDetail_{data[0]}_10_{servers[i]}")));
                sum+=price[servers[i]];
            }
            card.AddModule(new SectionModuleBuilder().WithText(new ParagraphStructBuilder().WithColumnCount(2)
                .AddField(new KMarkdownElementBuilder().WithContent($"总均价"))
                .AddField(new KMarkdownElementBuilder().WithContent($"{sum / servers.Length}"))
                .AddField(new KMarkdownElementBuilder().WithContent(""))).WithMode(SectionAccessoryMode.Right));
            await _client.GetGuild(_guildId).GetTextChannel(_channelId).SendCardMessageAsync(card.Build());
        }
    }
    private async Task CheckPriceDetail(string value, SocketUser user, IMessage msg, SocketTextChannel channel, SocketGuild guild)
    {
        var regex = new Regex("^CheckPriceDetail_");
        if (regex.IsMatch(value))
        {
            var card = new CardBuilder().WithSize(CardSize.Large);
            var data = value.Replace("CheckPriceDetail_", "").Split("_");
            var searchResult = _XIVAPI.GetMarketBoardListings(data[0],data[1],data[2]);
            card.AddModule(new SectionModuleBuilder().WithText(new ParagraphStructBuilder().WithColumnCount(3)
                .AddField(new KMarkdownElementBuilder().WithContent("**卖主**"))
                .AddField(new KMarkdownElementBuilder().WithContent("**单价**"))
                .AddField(new KMarkdownElementBuilder().WithContent("**数量**"))));
            for (int i = 0; i < searchResult.Count; i++)
            {
                card.AddModule(new SectionModuleBuilder().WithText(new ParagraphStructBuilder().WithColumnCount(3)
                .AddField(new KMarkdownElementBuilder().WithContent($"{searchResult[i].Name}"))
                .AddField(new KMarkdownElementBuilder().WithContent($"{searchResult[i].Price}"))
                .AddField(new KMarkdownElementBuilder().WithContent($"{searchResult[i].Quantity}"))));
            }
            await _client.GetGuild(_guildId).GetTextChannel(_channelId).SendCardMessageAsync(card.Build());
        }
    }
    private async Task ShowSongsList(string value, SocketUser user, IMessage msg, SocketTextChannel channel, SocketGuild guild)
    {
        var regex = new Regex(@"^不是这首歌_");
        if (regex.IsMatch(value))
        {
            value = value.Replace("不是这首歌_", "");
            await _client.GetGuild(_guildId).GetTextChannel(_channelId).SendTextMessageAsync(value);
            SongResult result;
            result = _neteaseTool.SearchMusic(value).Result;
            var songs = result.result.songs;
            var cardBuilder = new CardBuilder();
            cardBuilder.WithSize(CardSize.Large);
            for (int i = 0; i < songs.Length; i++)
            {
                cardBuilder.AddModule(new SectionModuleBuilder()
                    .WithText(new PlainTextElementBuilder().WithContent($"歌曲名:{songs[i].name}\n歌手：{songs[i].artists[0].name}\n专辑：{songs[i].album.name}"))
                    .WithAccessory(new ButtonElementBuilder().WithText("选择")
                        .WithValue($"CallMusicById_{songs[i].id.ToString()}_{songs[i].artists[0].name}_{songs[i].album.name}_{songs[i].album.id}").WithClick(ButtonClickEventType.ReturnValue))
                            .WithMode(SectionAccessoryMode.Right));
            }
            await _client.GetGuild(_guildId).GetTextChannel(_channelId).SendCardMessageAsync(cardBuilder.Build());
        }
    }
    private async Task CallMusicById(string value, SocketUser user, IMessage msg, SocketTextChannel channel, SocketGuild guild)
    {
        var regex = new Regex(@"^CallMusicById_");
        if (regex.IsMatch(value))
        {
            value = value.Replace("CallMusicById_", "");
            var data = value.Split("_");
            int id = Convert.ToInt32(data[0]);
            var artist = data[1];
            var albumName = data[2];
            var audioUrl = _neteaseTool.GetMusicUrlById(id);
            var imageUrl = _neteaseTool.GetMusicCoverByAlbumId(Convert.ToInt32(data[3]));
            CardBuilder cardBuilder = new CardBuilder()
                .WithSize(CardSize.Large)
                .AddModule(new SectionModuleBuilder()
                    .WithText(new KMarkdownElementBuilder().WithContent($"**歌手**:{artist}\n**专辑**:{albumName}"))
                    .WithAccessory(new ImageElementBuilder()
                    .WithSource(imageUrl)
                    .WithSize(ImageSize.Small))
                    .WithMode(SectionAccessoryMode.Right))
                    .AddModule(new AudioModuleBuilder().WithSource(audioUrl))
                    .AddModule(new SectionModuleBuilder().WithText(new KMarkdownElementBuilder().WithContent($"[在网页中查看]({_neteaseTool.GetMusicUrlOrginById(id)})")));

            await _client.GetGuild(_guildId).GetTextChannel(_channelId).SendCardMessageAsync(cardBuilder.Build());
        }
    }
    private async Task MyCard2(SocketMessage msg)
    {
        CardBuilder cardBuilder = new CardBuilder()
        .WithSize(CardSize.Large)
        .AddModule(new HeaderModuleBuilder().WithText("今天你第三层过了吗？"))
        .AddModule(new ActionGroupModuleBuilder()
        .AddElement(new ButtonElementBuilder().WithTheme(ButtonTheme.Info).WithText("过了").WithValue("Zeroshiki_3_yes").WithClick(ButtonClickEventType.ReturnValue))
        .AddElement(new ButtonElementBuilder().WithTheme(ButtonTheme.Info).WithText("没过").WithValue("Zeroshiki_3_no").WithClick(ButtonClickEventType.ReturnValue)));
        await _client.GetGuild(_guildId).GetTextChannel(_channelId).SendCardMessageAsync(cardBuilder.Build());
    }
    private async Task CardDemo(SocketMessage message)
    {
        if (message.Author.Id == _client.CurrentUser.Id) return;
        if (message.Content != "/test") return;
        CardBuilder cardBuilder = new CardBuilder()
            .WithSize(CardSize.Large)
            .AddModule(new HeaderModuleBuilder().WithText("This is header"))
            .AddModule(new DividerModuleBuilder())
            .AddModule(new SectionModuleBuilder().WithText("**This** *is* ~~kmarkdown~~", true))
            .AddModule(new SectionModuleBuilder()
                .WithText(new ParagraphStructBuilder()
                    .WithColumnCount(2)
                    .AddField(new PlainTextElementBuilder().WithContent("多列文本测试"))
                    .AddField(new KMarkdownElementBuilder().WithContent("**昵称**\n白给Doc"))
                    .AddField(new KMarkdownElementBuilder().WithContent("**在线时间**\n9:00-21:00"))
                    .AddField(new KMarkdownElementBuilder().WithContent("**服务器**\n吐槽中心")))
                .WithAccessory(new ImageElementBuilder()
                    .WithSource("https://img.kaiheila.cn/assets/2021-01/7kr4FkWpLV0ku0ku.jpeg")
                    .WithSize(ImageSize.Small))
                .WithMode(SectionAccessoryMode.Right))
            .AddModule(new SectionModuleBuilder()
                .WithText(new PlainTextElementBuilder().WithContent("您是否认为\"开黑啦\"是最好的语音软件？"))
                .WithAccessory(new ButtonElementBuilder().WithTheme(ButtonTheme.Primary).WithText("完全同意", false))
                .WithMode(SectionAccessoryMode.Right))
            
            .AddModule(new ContainerModuleBuilder()
                .AddElement(new ImageElementBuilder().WithSource("https://img.kaiheila.cn/assets/2021-01/7kr4FkWpLV0ku0ku.jpeg")))
            .AddModule(new ImageGroupModuleBuilder()
                .AddElement(new ImageElementBuilder().WithSource("https://img.kaiheila.cn/assets/2021-01/pWsmcLsPJq08c08c.jpeg"))
                .AddElement(new ImageElementBuilder().WithSource("https://img.kaiheila.cn/assets/2021-01/YIfHfnvxaV0dw0dw.jpg")))
            .AddModule(new ContextModuleBuilder()
                .AddElement(new PlainTextElementBuilder().WithContent("开黑啦气氛组，等你一起来搞事情"))
                .AddElement(new ImageElementBuilder().WithSource("https://img.kaiheila.cn/assets/2021-01/7kr4FkWpLV0ku0ku.jpeg"))
                .AddElement(new ImageElementBuilder().WithSource("https://img.kaiheila.cn/assets/2021-01/7kr4FkWpLV0ku0ku.jpeg"))
                .AddElement(new ImageElementBuilder().WithSource("https://img.kaiheila.cn/assets/2021-01/7kr4FkWpLV0ku0ku.jpeg")))
            .AddModule(new ActionGroupModuleBuilder()
                .AddElement(new ButtonElementBuilder().WithTheme(ButtonTheme.Primary).WithText("确定").WithValue("ok"))
                .AddElement(new ButtonElementBuilder().WithTheme(ButtonTheme.Danger).WithText("取消").WithValue("cancel")))
            .AddModule(new FileModuleBuilder()
                .WithSource("https://img.kaiheila.cn/attachments/2021-01/21/600972b5d0d31.txt")
                .WithTitle("开黑啦介绍.txt"))
            .AddModule(new AudioModuleBuilder()
                .WithSource("https://img.kaiheila.cn/attachments/2021-01/21/600975671b9ab.mp3")
                .WithTitle("命运交响曲")
                .WithCover("https://img.kaiheila.cn/assets/2021-01/rcdqa8fAOO0hs0mc.jpg"))
            .AddModule(new VideoModuleBuilder()
                .WithSource("https://img.kaiheila.cn/attachments/2021-01/20/6008127e8c8de.mp4")
                .WithTitle("有本事别笑"))
            .AddModule(new DividerModuleBuilder())
            .AddModule(new CountdownModuleBuilder().WithMode(CountdownMode.Day).WithEndTime(DateTimeOffset.Now.AddMinutes(1)))
            .AddModule(new CountdownModuleBuilder().WithMode(CountdownMode.Hour).WithEndTime(DateTimeOffset.Now.AddMinutes(1)))
            .AddModule(new CountdownModuleBuilder().WithMode(CountdownMode.Second).WithEndTime(DateTimeOffset.Now.AddMinutes(2)).WithStartTime(DateTimeOffset.Now.AddMinutes(1)));

        (Guid MessageId, DateTimeOffset MessageTimestamp) response = await _client.GetGuild(_guildId)
            .GetTextChannel(_channelId)
            .SendCardMessageAsync(cardBuilder.Build(), quote: new Quote(message.Id));
    }
    private async Task ModifyMessageDemo()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));

        SocketTextChannel channel = _client.GetGuild(_guildId).GetTextChannel(_channelId);
        (Guid MessageId, DateTimeOffset MessageTimestamp) response = await channel
            .SendKMarkdownMessageAsync("BeforeModification");
        await Task.Delay(TimeSpan.FromSeconds(1));

        IUserMessage msg = await channel.GetMessageAsync(response.MessageId) as IUserMessage;
        await msg!.ModifyAsync(properties => properties.Content += "\n==========\nModified");
        await Task.Delay(TimeSpan.FromSeconds(1));

        await msg.DeleteAsync();
        await Task.Delay(TimeSpan.FromSeconds(1));

        response = await channel.SendCardMessageAsync(new CardBuilder()
            .AddModule(new HeaderModuleBuilder().WithText("Test")).Build());
        await Task.Delay(TimeSpan.FromSeconds(1));

        msg = await channel.GetMessageAsync(response.MessageId) as IUserMessage;
        await msg!.ModifyAsync(properties => properties.Cards.Add(new CardBuilder()
            .AddModule(new DividerModuleBuilder())
            .AddModule(new HeaderModuleBuilder().WithText("ModificationHeader")).Build()));
    }
}