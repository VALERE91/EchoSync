using EchoSync;
using EchoSync.Replication;
using EchoSync.Replication.Client;
using EchoSync.Utils;
using EchoSyncTest.Mocks;

namespace EchoSyncTest;

public class EchoClientTests
{
    private EchoClient _echoClient;
    
    [SetUp]
    public void Setup()
    {
        ServiceLocator.InitializeDefaultServices();
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