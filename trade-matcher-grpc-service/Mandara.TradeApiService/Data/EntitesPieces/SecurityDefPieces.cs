namespace Mandara.TradeApiService.Data;

public sealed class SecurityDefPieces
{
    public SecurityDefinition SecurityDef { get; }
    public Product Product { get; }

    public SecurityDefPieces(SecurityDefinition secDef) : this(secDef, secDef.Product)
    {

    }

    public SecurityDefPieces(SecurityDefinition secDef, Product product)
    {
        SecurityDef = secDef;
        Product = product;
    }

    public override string ToString()
    {
        return $"Security - [{SecurityDef}]; Product - [{Product}]";
    }
}
