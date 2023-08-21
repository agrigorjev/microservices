namespace Mandara.Entities.EntityPieces
{
    public sealed class SecurityDefPieces
    {
        public SecurityDefinition SecurityDef { get; }
        public Product Product { get; }

        public SecurityDefPieces(SecurityDefinition secDef) : this(secDef, secDef.Product)
        {

        }

        public SecurityDefPieces(SecurityDefinition secDef, Product product)
        {
            NullTester.ThrowIfNullArgument(secDef, nameof(secDef), "SecurityDefPieces");
            NullTester.ThrowIfNullArgument(product, nameof(product), "SecurityDefPieces");

            SecurityDef = secDef;
            Product = product;
        }

        public override string ToString()
        {
            return $"Security - [{SecurityDef}]; Product - [{Product}]";
        }
    }
}
