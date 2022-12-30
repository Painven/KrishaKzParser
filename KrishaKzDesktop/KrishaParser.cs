using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace KrishaKzDesktop;

public class KrishaParser
{
    HttpClient client;

    public int MinArea { get; set; }
    public int MaxArea { get; set; }
    public int MinPrice { get; set; }
    public int MaxPrice { get; set; }
    public int RoomsCount { get; set; }
    public bool WithFurniture { get; set; }

    public KrishaParser()
    {
        client = new HttpClient(new HttpClientHandler()
        {
            AllowAutoRedirect = true,
            CookieContainer = new System.Net.CookieContainer(),
            UseCookies = true,
            AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
        });
    }

    public async Task<List<Appartment>> ParseData(IProgress<Tuple<int, int>> progress)
    {
        var list = new List<Appartment>();

        if (new[] { MinArea, MaxArea, MinPrice, MaxPrice, RoomsCount }.Any(i => i == default(int)))
        {
            throw new ArgumentOutOfRangeException();
        }


        return list;
    }
}

public class Appartment
{
    public string Uri { get; set; }
    public string PreviewImage { get; set; }
    public string Name { get; set; }
}
