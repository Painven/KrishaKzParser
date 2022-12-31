using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace KrishaKzDesktop;

public partial class KrishaParser
{
    private readonly string HOST = "https://krisha.kz";
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

        string uri = GetUri(1);
        var doc = await GetDocument(uri);

        string lastPage = doc.DocumentNode.SelectNodes("//nav[@class='paginator']/a")
            .ToArray()
            .Reverse()
            .Skip(1)
            .FirstOrDefault()
            .GetAttributeValue("data-page", null);

        int currentPage = 1;
        int totalPages = int.Parse(lastPage);

        do
        {
            if (currentPage > 1)
            {
                doc = await GetDocument(GetUri(currentPage));
            }

            var pageAppartments = doc.DocumentNode
                .SelectNodes("//section[contains(@class, 'a-search-list')]/div[@data-id]")
                .Select(div => new Appartment()
                {
                    Id = div.GetAttributeValue("data-id", null),
                    Uri = HOST + div.SelectSingleNode(".//div[contains(@class, 'a-card__header')]/a")?.GetAttributeValue("href", null),
                    Name = div.SelectSingleNode(".//div[contains(@class, 'a-card__header')]/a")?.InnerText.Trim(),
                    PreviewImage = div.SelectSingleNode(".//picture/img")?.GetAttributeValue("src", null)
                })
                .ToArray();

            foreach (var item in pageAppartments)
            {
                if (item.PreviewImage.StartsWith("//krisha.kz"))
                {
                    item.PreviewImage = item.PreviewImage.Replace("//krisha.kz", HOST);
                }
            }

            list.AddRange(pageAppartments);

            await Task.Delay(TimeSpan.FromSeconds(1.25));

            if (currentPage >= 5)
            {
                break;
            }

            currentPage++;

        } while (currentPage <= totalPages);


        return list;
    }

    private string GetUri(int pageNumber)
    {
        //https://krisha.kz/arenda/kvartiry/?das[live.furniture]=1&das[live.rooms]=1&das[live.square][from]=25&das[live.square][to]=35&das[price][from]=120000&das[price][to]=180000

        string uri = $"{HOST}/arenda/kvartiry/?";

        if (WithFurniture)
        {
            uri += "das[live.furniture]=1&";
        }

        uri += $"das[live.rooms]={RoomsCount}&";
        uri += $"das[live.square][from]={MinArea}&das[live.square][to]={MaxArea}&";
        uri += $"das[price][from]={MinPrice}&das[price][to]={MaxPrice}";

        if (pageNumber > 1)
        {
            uri += $"&rent-period-switch=%2Farenda%2Fkvartiry&page={pageNumber}";
        }

        return uri;
    }

    private async Task<HtmlDocument> GetDocument(string uri)
    {
        try
        {
            var doc = new HtmlDocument();
            string data = await client.GetStringAsync(uri);
            doc.LoadHtml(data);

            return doc;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}
