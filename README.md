# Bitfinex websocket API version 2.0 client [![Build Status](https://travis-ci.org/Marfusios/bitfinex-client-websocket.svg?branch=master)](https://travis-ci.org/Marfusios/bitfinex-client-websocket) [![NuGet version](https://badge.fury.io/nu/Bitfinex.Client.Websocket.svg)](https://badge.fury.io/nu/Bitfinex.Client.Websocket)

This is a C# implementation of the Bitfinex websocket API version 2.0 (BETA) found here:

https://bitfinex.readme.io/v2/docs

### License: 
    Apache License 2.0

### Features

* instalation via NuGet ([Bitfinex.Client.Websocket](https://www.nuget.org/packages/Bitfinex.Client.Websocket))
* public and authenticated API
* targeting .NET Standard 2.0 (.NET Core, Linux/MacOS compatible)
* reactive extensions ([Rx.NET](https://github.com/Reactive-Extensions/Rx.NET))
* integrated logging ([Serilog](https://serilog.net/))

### Usage

```csharp
var exitEvent = new ManualResetEvent(false);
var url = BitfinexValues.ApiWebsocketUrl;

using (var communicator = new BitfinexWebsocketCommunicator(url))
{
    using (var client = new BitfinexWebsocketClient(communicator))
    {
        client.Streams.InfoStream.Subscribe(info =>
        {
            Log.Information($"Info received, version: {info.Version}, reconnection happened, resubscribing to streams");
            
            client.Streams.PongStream.Subscribe(pong =>
            {
                Console.WriteLine($"Pong received! Id: {pong.Cid}") // Pong received! Id: 123456
                exitEvent.Set();
            });

        });

        await communicator.Start();
        await client.Send(new PingRequest() {Cid = 123456});

        exitEvent.WaitOne(TimeSpan.FromSeconds(30));
    }
}
```

More usage examples:
* integration tests ([link](test_integration/Bitfinex.Client.Websocket.Tests.Integration))
* console sample ([link](test_integration/Bitfinex.Client.Websocket.Sample/Program.cs))

### API coverage

| PUBLIC                 |    Covered     |  
|------------------------|:--------------:|
| Info                   |  ✔            |
| Ping-Pong              |  ✔            |
| Errors                 |  ✔            |
| Channel subscribing    |  ✔            |
| Channel unsubscribing  |  ✔            |
| Ticker                 |  ✔            |
| Trades                 |  ✔            |
| Books                  |  ✔            |
| Raw books              |                |
| Candles                |  ✔            |

| AUTHENTICATED          |    Covered     |  
|------------------------|:--------------:|
| Account info           |  ✔            |
| Orders                 |  ✔            |
| Positions              |                |
| Trades                 |                |
| Funding                |                |
| Wallets                |  ✔            |
| Balance                |                |
| Notifications          |                |

| AUTHENTICATED - INPUT  |    Covered     |  
|------------------------|:--------------:|
| New order              |  ✔            |
| Cancel order           |  ✔            |
| Cancel order multi     |                |
| Calc                   |                |

**Pull Requests are welcome!**

Donations gratefully accepted.
* Bitcoin: 1HfxKZhvm68qK3gE8bJAdDBWkcZ2AFs9pw
* Litecoin: LftdENE8DTbLpV6RZLKLdzYzVU82E6dz4W
* Ethereum: 0xb9637c56b307f24372cdcebd208c0679d4e48a47