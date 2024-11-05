﻿using EchoSync;
using EchoSyncTest.Mocks;

namespace EchoSyncTest;

public class EchoServerTests
{
    private EchoServer _echoServer;
    
    [SetUp]
    public void Setup()
    {
        _echoServer = new EchoServer(new ServerMock());    
    }

    [TearDown]
    public void TearDown()
    {
        _echoServer.Dispose();
    }
    
    [Test]
    public void CanCreate()
    {
        Assert.Pass();
    }
}