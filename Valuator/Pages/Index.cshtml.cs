using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NATS.Client;
using System.Text;
using Valuator.Redis;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IRedisStorage _redisStorage;

    public IndexModel(ILogger<IndexModel> logger, IRedisStorage storage)
    {
        _logger = logger;
        _redisStorage = storage;
    }

    public void OnGet()
    {

    }

    static async Task ProduceAsync(CancellationToken ct, string id)
    {
        ConnectionFactory cf = new ConnectionFactory();

        using (IConnection c = cf.CreateConnection())
        {
            byte[] data = Encoding.UTF8.GetBytes(id);
            c.Publish("valuator.processing.rank", data);
            await Task.Delay(1000);
            c.Drain();

            c.Close();
        }
    }

    public IActionResult OnPost(string text)
    {
        _logger.LogDebug(text);

        string id = Guid.NewGuid().ToString();

        string textKey = "TEXT-" + id;

        string similarityKey = "SIMILARITY-" + id;
        //TODO: посчитать similarity и сохранить в БД по ключу similarityKey 
        _redisStorage.Save(similarityKey, GetSimilarity(text, id).ToString());

        //TODO: сохранить в БД text по ключу textKey 
        _redisStorage.Save(textKey, text);

        CancellationTokenSource cts = new CancellationTokenSource();
        ProduceAsync(cts.Token, id);
        cts.Cancel();

        return Redirect($"summary?id={id}");
    }

    private int GetSimilarity(string text, string id)
    {
        var keys = _redisStorage.GetKeys();
        string textPrefix = "TEXT-";

        foreach(var value in keys)
        {
            if (value.StartsWith(textPrefix) && _redisStorage.Get(value) == text)
            {
                return 1;
            }
        }

        return 0;

    }

    
}
