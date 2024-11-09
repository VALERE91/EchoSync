using EchoSync;
using EchoSync.Replication;
using EchoSync.Replication.Client;
using EchoSyncTest.Mocks;

namespace EchoSyncTest;

public class EchoClientTests
{
    private EchoClient _echoClient;
    
    [SetUp]
    public void Setup()
    {
        _echoClient = new EchoClient(new ClientMock(), "key");    
    }

    [TearDown]
    public void TearDown()
    {
        _echoClient.Dispose();
    }

    [Test]
    public void CanCreate()
    {
        Assert.Pass();
    }
}