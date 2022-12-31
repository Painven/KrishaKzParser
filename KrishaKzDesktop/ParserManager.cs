using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KrishaKzDesktop.KrishaParser;

namespace KrishaKzDesktop;

public class ParserManager
{
    readonly string fileName = "products.json";

    KrishaParser parser = new KrishaParser();
    JsonSerializerSettings jsonSettings = new JsonSerializerSettings()
    {
        Formatting = Formatting.Indented
    };

    public List<Appartment> appartments { get; private set; }

    public ParserManager()
    {
        Load();
    }

    public async Task Parse(int minPrice, int maxPrice, int minArea, int maxArea, bool withFurniture, int roomsCount, IProgress<Tuple<int, int>> progress)
    {
        parser.MinPrice = minPrice;
        parser.MaxPrice = maxPrice;
        parser.MinArea = minArea;
        parser.MaxArea = maxArea;
        parser.RoomsCount = roomsCount;
        parser.WithFurniture = withFurniture;

        var data = await parser.ParseData(progress);
        Save(data);
    }

    private void Save(List<Appartment> data)
    {
        try
        {
            string json = JsonConvert.SerializeObject(data, jsonSettings);
            File.WriteAllText(fileName, json);
        }
        catch
        {
            throw;
        }
    }

    private void Load()
    {
        try
        {
            string json = File.ReadAllText(fileName);
            appartments = JsonConvert.DeserializeObject<List<Appartment>>(json, jsonSettings);
        }
        catch (Exception ex)
        {

        }
    }
}
