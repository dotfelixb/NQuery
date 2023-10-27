using NQuery.Core;

namespace NQuery.Test;

public class MutationTest
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
    public async Task TestMutation()
    {
        var newPlayer = new Player { UserName = "some-key", Lives = 2 };
        
        var rst = await _nQuery.MutationAsync(
            "some-string", 
            async () => await playerDatabase.Insert("some-key", newPlayer));
        
        Assert.That(rst.Lives, Is.EqualTo(2));
    }
}