using Newtonsoft.Json.Linq;
using NeteaseCloudMusicApi;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
public class test
{
    //static void Main()
    //{
    //    var scriptStr = @"var hash2 = function(a,b){return a+b}";
    //    var engine = new JavaScriptEngineSwitcher.V8.V8JsEngine();
    //    engine.
    //    engine.Execute(scriptStr);
    //    var result=engine.CallFunction<string>("hash2", 1, 2);
    //    Console.WriteLine(result); 
    //}
}
public class NeteaseTool
{
    CloudMusicApi api = new CloudMusicApi();
    public async Task<SongResult> SearchMusic(string name)
    {
        var reqeust = await api.RequestAsync(CloudMusicApiProviders.Search, new Dictionary<string, object> { ["keywords"] = name });
        var json = reqeust.Item2;
        var result=JsonConvert.DeserializeObject<SongResult>(json.ToString());
        return result;

    }
    public int GetIdByResult(SongResult result)
    {
        if (result.result.songs != null)
        {
            return result.result.songs[0].id;
        }
            return 5221167;
    }
    public string GetMusicUrlById(int id)
    {
        return $"https://music.163.com/song/media/outer/url?id={id}.mp3";
    }
    public string GetMusicUrlOrginById(int id)
    {
        return $"https://music.163.com/#/song?id={id}";
    }
    public string GetMusicCoverByAlbumId(int id)
    {
        var request = api.RequestAsync(CloudMusicApiProviders.Album, new Dictionary<string, object> { ["id"] = id });
        var album = request.Result.Item2["album"];
        return album["blurPicUrl"].ToString();
    }
}
[Serializable]
public class SongResult
{ 
    public Result result;
    public int code;

}
[Serializable]
public class Result
{
    public Songs[] songs;
    public bool hasMore;
    public int songCount;
}
[Serializable]
public class Songs
{
    public int id;
    public string name;
    public Artist[] artists;
    public Album album;
    public int duration;
    public int copyrightId;
    public int status;
    public string[] alias;
    public int rtype;
    public int ftype;
    public int mvid;
    public int fee;
    public string rUrl;
    public long mark;
}
[Serializable]
public class Album
{
    public int id;
    public string name;
    public Artist artist;
    public long publishTime;
    public int size;
    public int copyrightId;
    public int status;
    public long picId;
    public long mark;
}
[Serializable]
public class Artist
{
    public int id;
    public string name;
    public string picUrl;
    public string[] alias;
    public int albumSize;
    public int picId;
    public string img1v1Url;
    public int img1v1;
    public string trans;
}
[Serializable]
public class AlbumResult
{
    bool resourceState;
    AlbumSongs[] songs;
    int code;
    AlbumAlbum album;
}
[Serializable]
public class AlbumSongs
{
    string[] rtUrls;
    AlbumSongsAr[] ar;
    AlbumSongsAl al;
    int st;
    string noCopyrightRcmd;
    string songJumpInfo;
    int djId;
    int fee;
    int no;
    int mv;
    int t;
    int v;
    AlbumSongsH h;
    AlbumSongsL l;
    AlbumSongsSq sq;
    AlbumSongsHr hr;
    int rtype;
    string rurl;
    int pst;
    string[] alia;
    float pop;
    string rt;
    int mst;
    int cp;
    string crbt;
    string cf;
    int dt;
    string rtUrl;
    int ftype;
    string cd;
    string a;
    AlbumSongsM m;
    string name;
    long id;
    AlbumSongsPrivilege privilege;
}
[Serializable]
public class AlbumSongsAr
{
    int id;
    string name;
    string[] tns;
    string[] alia;
}
[Serializable]
public class AlbumSongsAl
{
    int id;
    string name;
    string picUrl;
    string pic_str;
    long pic;
    string[] alia;
}
[Serializable]
public class AlbumSongsH
{
    int br;
    int fid;
    int size;
    float vd;
    int sr;
}
[Serializable]
public class AlbumSongsL
{
    int br;
    int fid;
    int size;
    float vd;
    int sr;
}
[Serializable]
public class AlbumSongsSq
{
    int br;
    int fid;
    int size;
    float vd;
    int sr;
}
[Serializable]
public class AlbumSongsHr
{
    int br;
    int fid;
    int size;
    float vd;
    int sr;
}
[Serializable]
public class AlbumSongsM
{
    int br;
    int fid;
    int size;
    float vd;
    int sr;
}
[Serializable]
public class AlbumSongsPrivilege
{
    long id;
    int fee;
    int payed;
    int st;
    int pl;
    int dl;
    int sp;
    int cp;
    int subp;
    bool cs;
    int maxbr;
    int fl;
    bool toast;
    int flag;
    bool preSell;
    int playMaxbr;
    int downloadMaxbr;
    string maxBrLevel;
    string playMaxBrLevel;
    string downloadMaxBrLevel;
    string plLevel;
    string dlLevel;
    string flLevel;
    string rscl;
    AlbumSongsPrivilegeFreeTrialPrivilege freeTrialPrivilege;
    AlbumSongsPrivilegeChargeInfoList[] chargeInfoList;

}
[Serializable]
public class AlbumSongsPrivilegeFreeTrialPrivilege
{
    bool resConsumable;
    bool userConsumable;
    string listenType;
}
[Serializable]
public class AlbumSongsPrivilegeChargeInfoList
{
    int rate;
    string changeUrl;
    string changeMessage;
    int chargeType;
}
[Serializable]
public class AlbumAlbum
{
    string[] songs;
    bool paid;
    bool onSale;
    int mark;
    int companyId = 0;
    string blurPicUrl;
    string[] alias;
    AlbumSongsArtists[] artists;
    int copyrightId;
    long picId;
    AlbumSongsArtist artist;
    long publishTime;
    string company;
    string briefDesc;
    string picUrl;
    string commentThreadId;
    long pic;
    string description;
    string tags;
    int status;
    string subType;
    string name;
    long id;
    string type;
    int size;
    string picId_str;
    AlbumAlbumInfo info;
}
[Serializable]
public class AlbumSongsArtists
{
    long img1v1Id;
    int topicPerson;
    bool followed;
    string[] alias;
    long picId;
    string briefDesc;
    int musicSize;
    int albumSize;
    string picUrl;
    string img1v1Url;
    string trans;
    string name;
    int id;
    string img1v1Id_str;
}
[Serializable]
public class AlbumSongsArtist
{
    long img1v1Id;
    int topicPerson;
    bool followed;
    string[] alias;
    long picId;
    string briefDesc;
    int musicSize;
    int albumSize;
    string picUrl;
    string img1v1Url;
    string trans;
    string name;
    long id;
    string[] transNames;
}
[Serializable]
public class AlbumAlbumInfo
{
    AlbumAlbumInfoCommentThread commentThread;
    string latestLikedUsers;
    bool liked;
    string comments;
    int resourceType;
    int resourceId;
    int commentCount;
    int likedCount;
    int shareCount;
    string threadId;
}
[Serializable]
public class AlbumAlbumInfoCommentThread
{
    string id;
    AlbumAlbumInfoCommentThreadresourceInfo resourceInfo;
    int resourceType;
    int commentCount;
    int likedCount;
    int shareCount;
    int hotCount;
    int latestLikedUsers;
    long resourceId;
    int resourceOwnerId;
    string resourceTitle;
}
[Serializable]
public class AlbumAlbumInfoCommentThreadresourceInfo
{
    long id;
    int userId;
    string name;
    string imgUrl;
    string creator;
    string encodedId;
    string subTitle;
    string webUrl;
}