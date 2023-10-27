using NQuery.Core;

namespace NQuery.Test;

public class QueryTests
{
    private readonly NQueryConfiguration _configuration = new()
    {
        UseInMemory = true,
        CacheDuration = TimeSpan.FromMinutes(5)
    };
    private Core.NQuery _nQuery;
    private PlayerDatabase playerDatabase;

    [SetUp]
    public async Task Setup()
    {
        _nQuery = Core.NQuery.Create(_configuration);
        playerDatabase  = new PlayerDatabase();
        
        var player = new Player
        {
            UserName = "some-string",
            Lives = 1
        }; 
       await _nQuery.QueryAsync(player.UserName, async () => await playerDatabase.Insert(player.UserName, player));
    }

    [Test]
    public async Task TestQuery()
    { 
        var rst = await _nQuery.QueryAsync("some-string", async () => await playerDatabase.Find("some-string"));
        Assert.That(rst?.Lives, Is.EqualTo(1));
    }
    
    [Test]
    public async Task TestQueryAfterUpdate()
    { 
        playerDatabase.Update("some-string", new Player { UserName = "some-string", Lives = 2 });
        Thread.Sleep(_configuration.CacheDuration);
        
        var rst = await _nQuery.QueryAsync("some-string", async () => await playerDatabase.Find("some-string"));
        Assert.That(rst?.Lives, Is.EqualTo(2));
    }
}