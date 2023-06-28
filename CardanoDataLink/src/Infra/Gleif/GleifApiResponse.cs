﻿namespace CardanoDataLink.Infra.Gleif;

public class LeiRecords
{
    public DataItem[] data { get; set; }
}

public class DataItem
{
    public Attributes attributes { get; set; }
}

public class Attributes
{
    public Entity entity { get; set; }
    public string[]? bic { get; set; }
}

public class Entity
{
    public LegalName legalName { get; set; }
}

public class LegalName
{
    public string? name { get; set; }
}