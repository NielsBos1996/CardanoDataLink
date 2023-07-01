using CardanoDataLink.Domain.DataHandler;
using CardanoDataLink.Domain.Entities;
using CardanoDataLink.Domain.Gleif;
using CardanoDataLink.Infra.Gleif;
using Moq;

namespace CardanoDataLink.Tests.Domain.DataHandler;

public class DataEnricherTest
{
    [Fact]
    public async Task EnrichData_ReturnsEnrichedTransactions()
    {
        // Arrange
        var mockGleifClient = new Mock<IGleifClientInterface>();
        var gleifData = new LeiRecords
        {
            data = new[]
            {
                new DataItem
                {
                    attributes = new Attributes
                    {
                        lei = "123",
                        bic = new[] { "FakeBic" },
                        entity = new Entity
                        {
                            legalAddress = new LegalAddress { country = "NL" },
                            legalName = new LegalName { name = "Test Corp" },
                        }
                    }
                }
            }
        };
        var transactions = new List<Transaction>
        {
            new Transaction { Lei = "123", Notional = 1000, Rate = 2 }
        };

        mockGleifClient.Setup(x => x.GetByIdentifier(It.IsAny<IEnumerable<string>>())).ReturnsAsync(gleifData);

        var dataEnricher = new DataEnricher(mockGleifClient.Object);

        // Act
        var result = await dataEnricher.EnrichData(transactions);

        // Assert
        var enrichedTransaction = result.First();
        Assert.Equal(500, enrichedTransaction.TransactionCost);
        Assert.Equal("Abs(1000 * (1 / 2) - 1000)", enrichedTransaction.TransactionCostReason);
        Assert.Equal("Test Corp", enrichedTransaction.LegalName);
        Assert.Equal("FakeBic", enrichedTransaction.Bic);
    }
}
