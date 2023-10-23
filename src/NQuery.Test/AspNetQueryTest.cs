using Microsoft.Extensions.DependencyInjection;
using NQuery.Interfaces;

namespace NQuery.Test;

public class AspNetQueryTest
{
    readonly ServiceCollection _services = new ServiceCollection();
    
    [SetUp]
    public void Setup()
    {
        _services.AddNQuery(cfg =>
        {
            // cfg.RedisConfiguration = new RedisConfiguration(new[]
            // {
            //     new RedisEndpoint { Host = "localhost", Port = 6379 }
            // });
            cfg.UseInMemory = true;
        }); 
    }

    [Test]
    public void TestAspNetCoreServices()
    {
        var provider = _services.BuildServiceProvider();
        var queryType = provider.GetRequiredService<INQuery>();
        Assert.That(queryType, Is.InstanceOf<INQuery>());
    }
}