using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

    public IActionResult OnPost(string text)
    {
        _logger.LogDebug(text);

        string id = Guid.NewGuid().ToString();

        string textKey = "TEXT-" + id;

        string rankKey = "RANK-" + id;
        //TODO: посчитать rank и сохранить в БД по ключу rankKey 
        _redisStorage.Save(rankKey, GetRank(text));

        string similarityKey = "SIMILARITY-" + id;
        //TODO: посчитать similarity и сохранить в БД по ключу similarityKey 
        _redisStorage.Save(similarityKey, GetSimilarity(text, id).ToString());

        //TODO: сохранить в БД text по ключу textKey 
        _redisStorage.Save(textKey, text);

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

    private static string GetRank(string text)
    {
        double len = text.Length;
        double notLetterCount = 0;
        foreach (char value in text)
        {
            if (!char.IsLetter(value)) 
            {
                notLetterCount++;
            }
        }

        string count = Convert.ToString(notLetterCount / len);

        return count;
    }
}
