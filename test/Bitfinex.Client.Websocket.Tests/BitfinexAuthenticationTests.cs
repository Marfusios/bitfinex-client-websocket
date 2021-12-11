using Bitfinex.Client.Websocket.Utils;
using Xunit;

namespace Bitfinex.Client.Websocket.Tests;

public class BitfinexAuthenticationTests
{
    [Fact]
    public void CreateSignature_ShouldReturnCorrectString()
    {
        var nonce = BitfinexAuthentication.CreateAuthNonce(123456);
        var payload = BitfinexAuthentication.CreateAuthPayload(nonce);
        var signature = BitfinexAuthentication.CreateSignature(payload, "api_secret");

        Assert.Equal("cbe5ac2d70f8bb8246e31906d872408c29097df8c97a935ce367a93f57e47af51e033939a07cd5ecf1e078e7f7c9a344", signature);
    }
}