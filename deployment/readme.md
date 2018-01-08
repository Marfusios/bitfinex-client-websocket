# How to deploy

### Manually

* `cd ../src/Bitfinex.Client.Websocket`
* `dotnet pack -c Release`
* `cd bin/Release`
* upload `Bitfinex.Client.Websocket.<version>.nupkg` via web [nuget.org](https://www.nuget.org/packages/manage/upload)