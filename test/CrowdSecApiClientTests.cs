using System.Net;
using AspNetCore.CrowdSec;
using Moq;
using Moq.Protected;

namespace AspNet.CrowdSec.Tests;

public class CrowdSecApiClientTests
{
    [Fact]
    public async Task QueryIpAddressAsync_ApiErrorResponse_ThrowsInvalidOperationExceptionExceptions()
    {
        // Arrange
        const string errors = "This is an error";
        const string message = "The message";
        const string apiHostAddress = "http://crowdsec.api/";
        var ipAddress = IPAddress.Parse("1.2.3.4");
        
        var httpClientFactory = new Mock<IHttpClientFactory>();
        var messageHandler = new Mock<HttpMessageHandler>();
        messageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("{\"message\": \" " + message + "\"," +
                                            " \"errors\": \""+ errors +"\"}")
            });

        var httpClient = new HttpClient(messageHandler.Object);
        httpClient.BaseAddress = new Uri(apiHostAddress);
        httpClientFactory.Setup(f => 
                f.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);
        
        // Act
        ICrowdSecApiClient client = new CrowdSecApiClient(httpClientFactory.Object);
        
        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await client.QueryIpAddressAsync(ipAddress));
    }
    
    [Fact]
    public async Task QueryIpAddressAsync_ApiSuccessResponse_ReturnsCrowdSecApiSuccessResponse()
    {
        // Arrange
        var ipAddress = IPAddress.Parse("1.2.3.4");
        const string apiHostAddress = "http://crowdsec.api/";
        
        var httpClientFactory = new Mock<IHttpClientFactory>();
        var messageHandler = new Mock<HttpMessageHandler>();
        messageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"duration\": \"3h59m57.641708614s\"," +
                                            " \"id\": 2410,      " +
                                            "\"origin\": \"cscli\"," +
                                            "\"scenario\": \"manual 'ban' from '939972095cf1459c8b22cc608eff85daEb4yoi2wiTD7Y3fA'\", " +
                                            "\"scope\": \"Ip\"," +
                                            "\"type\": \"ban\"," +
                                            "\"value\": \"3.3.3.4\"}")
            });

        var httpClient = new HttpClient(messageHandler.Object);
        httpClient.BaseAddress = new Uri(apiHostAddress);
        httpClientFactory.Setup(f => 
                f.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);
        
        // Act
        ICrowdSecApiClient client = new CrowdSecApiClient(httpClientFactory.Object);
        var queryResult = await client.QueryIpAddressAsync(ipAddress);
        
        // Assert
        Assert.True(queryResult == IpAddressQueryResult.Banned);
    }
}