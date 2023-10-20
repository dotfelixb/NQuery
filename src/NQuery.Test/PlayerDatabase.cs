namespace NQuery.Test;

public class Player
{
    public string UserName { get; set; } = null!;
    public int Lives { get; set; }
}

public class PlayerDatabase
{
    private readonly Dictionary<string, Player?> _data = new();

    public Task<Player> Insert(string key, Player? player)
    {
        ArgumentNullException.ThrowIfNull(player);
        
        _data[key] = player;

        return Task.FromResult(player);
    }

    public async Task<IEnumerable<Player>> Insert(IEnumerable<Player> players)
    {
        var lst = new List<Player>();
        foreach (var player in players)
        {
            lst.Add(await Insert(player.UserName, player));
        }
        return lst;
    }

    public Task<Player?> Find(string key)
    {
        return _data.TryGetValue(key, out var value) 
            ? Task.FromResult(value) 
            : Task.FromResult<Player?>(null);
    }
    
    public void Update(string key, Player? player)
    {
        _data[key] = player;
    }
}