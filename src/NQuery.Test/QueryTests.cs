namespace NQuery.Test;

public class QueryTests
{
    private QueryConfiguration configuration = new()
    {
        UseInMemory = true,
        CacheDuration = 5000
    };
    private Query query;
    private PlayerDatabase playerDatabase;

    [SetUp]
    public async Task Setup()
    {
        query = Query.Create(configuration);
        playerDatabase  = new();
        
        var player = new Player
        {
            UserName = "some-string",
            Lives = 1
        }; 
       await query.QueryAsync(player.UserName, async () => await playerDatabase.Insert(player.UserName, player));
    }

    [Test]
    public async Task TestQuery()
    { 
        var rst = await query.QueryAsync("some-string", async () => await playerDatabase.Find("some-string"));
        Assert.That(rst?.Lives, Is.EqualTo(1));
    }
    
    [Test]
    public async Task TestQueryAfterUpdate()
    { 
        playerDatabase.Update("some-string", new Player { UserName = "some-string", Lives = 2 });
        Thread.Sleep(configuration.CacheDuration + 1000);
        
        var rst = await query.QueryAsync("some-string", async () => await playerDatabase.Find("some-string"));
        Assert.That(rst?.Lives, Is.EqualTo(2));
    }
}