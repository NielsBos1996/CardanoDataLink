using CardanoDataLink.Domain.Exceptions;
using CardanoDataLink.Infra.Gleif;
using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace CardanoDataLink.Tests.Infra.Gleif;

public class HttPGleifRepositoryTest
{
    private readonly Mock<ILogger<HttPGleifRepository>> _mockLogger;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;

    public HttPGleifRepositoryTest()
    {
        _mockLogger = new Mock<ILogger<HttPGleifRepository>>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
    }

    [Fact]
    public async Task GetByIdentifier_Success()
    {
        // Arrange
        SetupHttpMessageHandler(HttpStatusCode.OK, JsonResponseSuccess);

        var repo = new HttPGleifRepository(_mockLogger.Object, CreateHttpClient());

        // Act
        var result = await repo.GetByIdentifier(new List<string> {"fake-lei1", "fake-lei2"});

        // Assert
        Assert.NotNull(result.data);

        var data = result.data;
        Assert.Equal(2, data.Length);
        Assert.Equal("MX", data[0].attributes.entity.legalAddress.country);
        Assert.Equal("ASOCIACION MEXICANA DE ESTANDARES PARA EL COMERCIO ELECTRONICO AC", data[0].attributes.entity.legalName.name);
        Assert.Equal("4469000001AVO26P9X86", data[0].attributes.lei);
    }

    [Fact]
    public async Task GetByIdentifier_Throws_When_Not_OK_Response()
    {
        // Arrange
        SetupHttpMessageHandler(HttpStatusCode.BadRequest, "Bad request");

        var repo = new HttPGleifRepository(_mockLogger.Object, CreateHttpClient());

        // Act & Assert
        await Assert.ThrowsAsync<GleifUnavailalbeException>(() => repo.GetByIdentifier(new List<string> {"12345"}));
    }

    [Fact]
    public async Task GetByIdentifier_Throws_When_Invalid_Json()
    {
        // Arrange
        SetupHttpMessageHandler(HttpStatusCode.OK, "Invalid JSON");

        var repo = new HttPGleifRepository(_mockLogger.Object, CreateHttpClient());

        // Act & Assert
        await Assert.ThrowsAsync<GleifUnavailalbeException>(() => repo.GetByIdentifier(new List<string> {"12345"}));
    }

    private void SetupHttpMessageHandler(HttpStatusCode statusCode, string content)
    {
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content)
            });
    }

    private HttpClient CreateHttpClient()
    {
        return new HttpClient(_mockHttpMessageHandler.Object);
    }
    
    private const string JsonResponseSuccess = "{\"meta\":{\"goldenCopy\":{\"publishDate\":\"2023-06-29T08:00:00Z\"},\"pagination\":{\"currentPage\":1,\"perPage\":15,\"from\":1,\"to\":2,\"total\":2,\"lastPage\":1}},\"links\":{\"first\":\"https:\\/api.gleif.org/api/v1/lei-records?filter%5Blei%5D=261700K5E45DJCF5Z735%2C4469000001AVO26P9X86&page%5Bnumber%5D=1&page%5Bsize%5D=15\",\"last\":\"https:\\/api.gleif.org/api/v1/lei-records?filter%5Blei%5D=261700K5E45DJCF5Z735%2C4469000001AVO26P9X86&page%5Bnumber%5D=1&page%5Bsize%5D=15\"},\"data\":[{\"type\":\"lei-records\",\"id\":\"4469000001AVO26P9X86\",\"attributes\":{\"lei\":\"4469000001AVO26P9X86\",\"entity\":{\"legalName\":{\"name\":\"ASOCIACION MEXICANA DE ESTANDARES PARA EL COMERCIO ELECTRONICO AC\",\"language\":\"es\"},\"otherNames\":[{\"name\":\"GS1 M\u00c9XICO\",\"language\":\"es\",\"type\":\"TRADING_OR_OPERATING_NAME\"}],\"transliteratedOtherNames\":[],\"legalAddress\":{\"language\":\"es\",\"addressLines\":[\"BOULEVARD TOLUCA 46\",\"El Conde\",\"Naucalpan de Ju\u00e1rez\"],\"addressNumber\":\"46\",\"addressNumberWithinBuilding\":null,\"mailRouting\":null,\"city\":\"Naucalpan de Ju\u00e1rez\",\"region\":\"MX-MEX\",\"country\":\"MX\",\"postalCode\":\"53500\"},\"headquartersAddress\":{\"language\":\"es\",\"addressLines\":[\"BOULEVARD TOLUCA 46\",\"El Conde\",\"Naucalpan de Ju\u00e1rez\"],\"addressNumber\":\"46\",\"addressNumberWithinBuilding\":null,\"mailRouting\":null,\"city\":\"Naucalpan de Ju\u00e1rez\",\"region\":\"MX-MEX\",\"country\":\"MX\",\"postalCode\":\"53500\"},\"registeredAt\":{\"id\":\"RA000449\",\"other\":null},\"registeredAs\":\"AME9806027F9\",\"jurisdiction\":\"MX\",\"category\":\"GENERAL\",\"legalForm\":{\"id\":\"5AWU\",\"other\":null},\"associatedEntity\":{\"lei\":null,\"name\":null},\"status\":\"ACTIVE\",\"expiration\":{\"date\":null,\"reason\":null},\"successorEntity\":{\"lei\":null,\"name\":null},\"successorEntities\":[],\"creationDate\":\"1998-06-02T05:00:00Z\",\"subCategory\":null,\"otherAddresses\":[{\"fieldType\":\"TransliteratedOtherAddress\",\"language\":\"es\",\"type\":\"AUTO_ASCII_TRANSLITERATED_LEGAL_ADDRESS\",\"addressLines\":[\"BOULEVARD TOLUCA 46\",\"El Conde\",\"Naucalpan de Juarez\"],\"addressNumber\":\"46\",\"city\":\"Naucalpan de Juarez\",\"region\":\"MX-MEX\",\"country\":\"MX\",\"postalCode\":\"53500\"},{\"fieldType\":\"TransliteratedOtherAddress\",\"language\":\"es\",\"type\":\"AUTO_ASCII_TRANSLITERATED_HEADQUARTERS_ADDRESS\",\"addressLines\":[\"BOULEVARD TOLUCA 46\",\"El Conde\",\"Naucalpan de Juarez\"],\"addressNumber\":\"46\",\"city\":\"Naucalpan de Juarez\",\"region\":\"MX-MEX\",\"country\":\"MX\",\"postalCode\":\"53500\"}],\"eventGroups\":[]},\"registration\":{\"initialRegistrationDate\":\"2017-02-02T01:37:30Z\",\"lastUpdateDate\":\"2023-02-09T04:37:12Z\",\"status\":\"ISSUED\",\"nextRenewalDate\":\"2024-02-02T01:37:30Z\",\"managingLou\":\"4469000001AVO26P9X86\",\"corroborationLevel\":\"FULLY_CORROBORATED\",\"validatedAt\":{\"id\":\"RA000449\",\"other\":null},\"validatedAs\":\"AME9806027F9\",\"otherValidationAuthorities\":[]},\"bic\":null,\"mic\":null,\"ocid\":null,\"spglobal\":[\"529128148\"]},\"relationships\":{\"managing-lou\":{\"links\":{\"related\":\"https:\\/api.gleif.org/api/v1/lei-records/4469000001AVO26P9X86/managing-lou\"}},\"lei-issuer\":{\"links\":{\"related\":\"https:\\/api.gleif.org/api/v1/lei-records/4469000001AVO26P9X86/lei-issuer\"}},\"field-modifications\":{\"links\":{\"related\":\"https:\\/api.gleif.org/api/v1/lei-records/4469000001AVO26P9X86/field-modifications\"}},\"direct-parent\":{\"links\":{\"reporting-exception\":\"https:\\/api.gleif.org/api/v1/lei-records/4469000001AVO26P9X86/direct-parent-reporting-exception\"}},\"ultimate-parent\":{\"links\":{\"reporting-exception\":\"https:\\/api.gleif.org/api/v1/lei-records/4469000001AVO26P9X86/ultimate-parent-reporting-exception\"}}},\"links\":{\"self\":\"https:\\/api.gleif.org/api/v1/lei-records/4469000001AVO26P9X86\"}},{\"type\":\"lei-records\",\"id\":\"261700K5E45DJCF5Z735\",\"attributes\":{\"lei\":\"261700K5E45DJCF5Z735\",\"entity\":{\"legalName\":{\"name\":\"APIR SYSTEMS LIMITED\",\"language\":\"en\"},\"otherNames\":[],\"transliteratedOtherNames\":[],\"legalAddress\":{\"language\":\"en\",\"addressLines\":[\"LEVEL 2\",\"33 AINSLIE PLACE\"],\"addressNumber\":null,\"addressNumberWithinBuilding\":null,\"mailRouting\":null,\"city\":\"CANBERRA\",\"region\":\"AU-ACT\",\"country\":\"AU\",\"postalCode\":\"2601\"},\"headquartersAddress\":{\"language\":\"en\",\"addressLines\":[\"LEVEL 2\",\"33 AINSLIE PLACE\"],\"addressNumber\":null,\"addressNumberWithinBuilding\":null,\"mailRouting\":null,\"city\":\"CANBERRA\",\"region\":\"AU-ACT\",\"country\":\"AU\",\"postalCode\":\"2601\"},\"registeredAt\":{\"id\":\"RA000014\",\"other\":null},\"registeredAs\":\"081 044 957\",\"jurisdiction\":\"AU\",\"category\":\"GENERAL\",\"legalForm\":{\"id\":\"R4KK\",\"other\":null},\"associatedEntity\":{\"lei\":null,\"name\":null},\"status\":\"ACTIVE\",\"expiration\":{\"date\":null,\"reason\":null},\"successorEntity\":{\"lei\":null,\"name\":null},\"successorEntities\":[],\"creationDate\":\"1997-12-12T00:00:00Z\",\"subCategory\":null,\"otherAddresses\":[],\"eventGroups\":[]},\"registration\":{\"initialRegistrationDate\":\"2015-09-21T00:14:18Z\",\"lastUpdateDate\":\"2023-01-03T05:47:51Z\",\"status\":\"ISSUED\",\"nextRenewalDate\":\"2024-02-24T00:00:00Z\",\"managingLou\":\"529900T8BM49AURSDO55\",\"corroborationLevel\":\"FULLY_CORROBORATED\",\"validatedAt\":{\"id\":\"RA000014\",\"other\":null},\"validatedAs\":\"081 044 957\",\"otherValidationAuthorities\":[]},\"bic\":null,\"mic\":null,\"ocid\":\"au/081044957\",\"spglobal\":[\"117524173\"]},\"relationships\":{\"managing-lou\":{\"links\":{\"related\":\"https:\\/api.gleif.org/api/v1/lei-records/261700K5E45DJCF5Z735/managing-lou\"}},\"lei-issuer\":{\"links\":{\"related\":\"https:\\/api.gleif.org/api/v1/lei-records/261700K5E45DJCF5Z735/lei-issuer\"}},\"field-modifications\":{\"links\":{\"related\":\"https:\\/api.gleif.org/api/v1/lei-records/261700K5E45DJCF5Z735/field-modifications\"}},\"direct-parent\":{\"links\":{\"reporting-exception\":\"https:\\/api.gleif.org/api/v1/lei-records/261700K5E45DJCF5Z735/direct-parent-reporting-exception\"}},\"ultimate-parent\":{\"links\":{\"reporting-exception\":\"https:\\/api.gleif.org/api/v1/lei-records/261700K5E45DJCF5Z735/ultimate-parent-reporting-exception\"}}},\"links\":{\"self\":\"https:\\/api.gleif.org/api/v1/lei-records/261700K5E45DJCF5Z735\"}}]}";
}
