using EchoSync;
using EchoSyncTest.Mocks;

namespace EchoSyncTest;

public class EchoClientTests
{
    private EchoClient _echoClient;
    
    [SetUp]
    public void Setup()
    {
        _echoClient = new EchoClient(new ClientMock());    
    }

    [Test]
    public void CanCreate()
    {
        Assert.Pass();
    }
}