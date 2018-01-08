# Bitfinex websocket API version 2.0 client

This is a C# implementation of the Bitfinex websocket API version 2.0 (BETA) found here:

https://bitfinex.readme.io/v2/docs

### License: 
    Apache License 2.0

### Features

* instalation via NuGet ([Bitfinex.Client.Websocket](https://www.nuget.org/packages/Bitfinex.Client.Websocket/0.0.1))
* public and authenticated API
* reactive extensions ([Rx.NET](https://github.com/Reactive-Extensions/Rx.NET))

### Usage

```csharp
var exitEvent = new ManualResetEvent(false);
var url = BitfinexValues.ApiWebsocketUrl;

using (var communicator = new BitfinexWebsocketCommunicator(url))
{
    using (var client = new BitfinexWebsocketClient(communicator))
    {
        client.PongReceived.Subscribe(pong =>
        {
            Console.WriteLine($"Pong received! Id: {pong.Cid}") // Pong received! Id: 123456
            exitEvent.Set();
        });

        await communicator.Start();
        await client.Send(new PingRequest() {Cid = 123456});

        exitEvent.WaitOne(TimeSpan.FromSeconds(30));
    }
}
```
 
Donations gratefully accepted.
Bitcoin: 1HfxKZhvm68qK3gE8bJAdDBWkcZ2AFs9pw
Litecoin: LftdENE8DTbLpV6RZLKLdzYzVU82E6dz4W
Ethereum: 0xb9637c56b307f24372cdcebd208c0679d4e48a47