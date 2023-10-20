namespace NQuery.Test;

public class RedisQueryTest
{
    private QueryConfiguration configuration = new()
    {
        CacheDuration = 1,
        RedisConfiguration = new RedisConfiguration(new[]
        {
            new RedisEndpoint { Host = "localhost", Port = 6379 }
        })
    };
    private Query query;
    private PlayerDatabase playerDatabase;
    
    [SetUp]
    public Task Setup()
    {
        query = Query.Create(configuration);
        playerDatabase  = new PlayerDatabase();
        
        var player = new Player
        {
            UserName = "some-string",
            Lives = 1
        };

        return Task.CompletedTask;
    }
    
    [Test]
    public async Task SetKey()
    {
        var player = new Player
        {
            UserName = "some-string",
            Lives = 1
        }; 
        var rst = await query.QueryAsync(player.UserName, async () => await playerDatabase.Insert(player.UserName, player));
        Assert.That(rst?.Lives, Is.EqualTo(1));
    }
    
    [Test]
    public async Task GetKey()
    {
        var rst = await query.QueryAsync(
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
        var rst = await query.QueryAsync(
            "some-string", 
            async () => await playerDatabase.Insert(players));
        Assert.That(rst.Count(), Is.EqualTo(2));
    }
}