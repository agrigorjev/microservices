using AutoMapper;

namespace Mandara.IRM.Server.Extensions
{
    public static class AutoMapperExpressionExtensions
    {
        public static IMappingExpression<TSource, TDest> IgnoreAll<TSource, TDest>
            (this IMappingExpression<TSource, TDest> expression)
        {
            expression.ForAllMembers(opt => opt.Ignore());
            return expression;
        }

    }
}