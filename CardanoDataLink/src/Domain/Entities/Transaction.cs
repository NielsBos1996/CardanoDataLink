using CsvHelper.Configuration.Attributes;

namespace CardanoDataLink.Domain.Entities;

public class Transaction
{
    [Name("transaction_uti")]
    public string TransactionUti { get; set; }
    [Name("isin")]
    public string Isin { get; set; }
    [Name("notional")]
    public double Notional { get; set; }
    [Name("notional_currency")]
    public string NotionalCurrency { get; set; }
    [Name("transaction_type")]
    public string TransactionType { get; set; }
    [Name("transaction_datetime")]
    public DateTime TransactionDatetime { get; set; }
    [Name("rate")]
    public double Rate { get; set; }
    [Name("lei")]
    public string Lei { get; set; }

    [Optional]
    [Name("legal_name")]
    public string? LegalName { get; set; }
    
    [Optional]
    [Name("bic")]
    public string? Bic { get; set; }
    
    [Optional]
    [Name("reason")]
    public string? Reason { get; set; }
    
    [Optional]
    [Name("transaction_cost")]
    public double? TransactionCost { get; set; }

    public bool CanBeProcessed()
    {
        return Notional > 0;
    }
}