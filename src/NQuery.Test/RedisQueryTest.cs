namespace NQuery.Test;

public class RedisQueryTest
{
    private readonly NQueryConfiguration _configuration = new()
    {
        UseInMemory = false, // <-- change to false for this test
        CacheDuration = 5,
        RedisConfiguration = new RedisConfiguration(new[]
        {
            new RedisEndpoint { Host = "127.0.0.1", Port = 6379 }
        })
    };
    private NQuery _query;
    private PlayerDatabase playerDatabase;
    
    [SetUp]
    public async Task Setup()
    {
        _query = NQuery.Create(_configuration);
        playerDatabase  = new PlayerDatabase();
        
        var player = new Player
        {
            UserName = "some-string",
            Lives = 1
        };
        await _query.QueryAsync(player.UserName, async () => await playerDatabase.Insert(player.UserName, player));
    }
    
    [Test]
    public async Task SetKey()
    {
        var player = new Player
        {
            UserName = "some-string",
            Lives = 1
        }; 
        var rst = await _query.QueryAsync(player.UserName, async () => await playerDatabase.Insert(player.UserName, player));
        Assert.That(rst?.Lives, Is.EqualTo(1));
    }
    
    [Test]
    public async Task GetKey()
    {
        var rst = await _query.QueryAsync(
            "some-string", 
            async () => await playerDatabase.Find("some-string"));
        Assert.That(rst?.Lives, Is.EqualTo(1));
    }
    
    [Test]
    public async Task SetKeyList()
    {
        var players = new List<Player>
        {
            new() { UserName = "some-string", Lives = 1 },
            new() { UserName = "some-string2", Lives = 2 },
        };
        var rst = await _query.QueryAsync(
            "some-list", 
            async () => await playerDatabase.Insert(players));
        Assert.That(rst.Count(), Is.EqualTo(2));
    }
}