namespace NQuery.Test;

public class Player
{
    public string UserName { get; set; } = null!;
    public int Lives { get; set; }
}

public class PlayerDatabase
{
    private readonly Dictionary<string, Player> _data = new();

    public Task<Player> Insert(string key, Player player)
    {
        _data[key] = player;

        return Task.FromResult(player);
    }

    public Task<Player?> Find(string key)
    {
        return _data.TryGetValue(key, out var value) 
            ? Task.FromResult<Player?>(value) 
            : Task.FromResult<Player?>(null);
    }
    
    public void Update(string key, Player player)
    {
        _data[key] = player;
    }
}