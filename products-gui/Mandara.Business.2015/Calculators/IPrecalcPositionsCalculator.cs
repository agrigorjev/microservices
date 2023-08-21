using Mandara.Entities;
using System;
using System.Collections.Generic;

namespace Mandara.Business.Calculators
{
    public interface IPrecalcPositionsCalculator
    {
        /// <summary>
        /// This method will force recalculation of a custom period trade capture's precalc details.  Security 
        /// definition precalc details will not be recalculated.
        /// </summary>
        /// <param name="trade">The <see cref="TradeCapture"/> for which precalc details are being recalculated.</param>
        /// <param name="secDef">The <see cref="SecurityDefinition"/> for which precalc details are being
        /// recalculated.</param>
        Tuple<ICollection<PrecalcTcDetail>, ICollection<PrecalcSdDetail>> ForceCalculatePrecalcPositions(
            TradeCapture trade,
            SecurityDefinition secDef);

        /// <summary>
        /// Calculate the precalc details for either a custom period trade capture or the security definition if the
        /// trade is for fixed period (month, quarter, year).  It is assumed that the <see cref="Product"/> is 
        /// referenced from the <see cref="SecurityDefinition"/>
        /// </summary>
        /// <param name="trade">The <see cref="TradeCapture"/> for which precalc details are needed if it's a custom 
        /// period trade.</param>
        /// <param name="secDef">The <see cref="SecurityDefinition"/> for which precalc details are needed for a month-
        /// based period trade.</param>
        Tuple<ICollection<PrecalcTcDetail>, ICollection<PrecalcSdDetail>> CalculatePrecalcPositions(
            TradeCapture trade,
            SecurityDefinition secDef);

        /// <summary>
        /// Calculate the precalc details for either a custom period trade capture or the security definition if the
        /// trade is for fixed period (month, quarter, year).  The <see cref="Product"/> is provided in case it's not 
        /// referenced directly from the <see cref="SecurityDefinition" />.
        /// </summary>
        /// <param name="trade">The <see cref="TradeCapture"/> for which precalc details are needed if it's a custom 
        /// period trade.</param>
        /// <param name="secDef">The <see cref="SecurityDefinition"/> for which precalc details are needed for a month-
        /// based period trade.</param>
        /// <param name="product">The <see cref="Product"/> needed for the calculations.</param>
        Tuple<ICollection<PrecalcTcDetail>, ICollection<PrecalcSdDetail>> CalculatePrecalcPositions(
            TradeCapture trade,
            SecurityDefinition secDef,
            Product product);
    }
}
