using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrishaKzDesktop;

public class ParserManager
{
    readonly string fileName = "products.json";

    KrishaParser parser = new KrishaParser();

    List<Appartment> appartments;

    public ParserManager()
    {
        Load();
    }

    public async Task Parse(IProgress<Tuple<int, int>> progress)
    {
        var data = await parser.ParseData(progress);
        Save(data);
    }

    private void Save(List<Appartment> data)
    {
        try
        {
            string json = JsonConvert.SerializeObject(data);
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
            appartments = JsonConvert.DeserializeObject<List<Appartment>>(json);
        }
        catch (Exception ex)
        {

        }
    }
}
