using System;
using System.Collections.Generic;
using Mandara.Entities;

namespace Mandara.Business.Trades
{
    /// <summary>
    /// Reads <see cref="TradeCapture"/> from a database. 
    /// </summary>
    public interface ITradesDataProvider
    {
        /// <summary>
        /// Reads a list of Filled trades from a database taking into account
        /// CalculatePnlOnLegs flag on products - it should apply <see cref="ICalcPnlFromLegsParentReplacer"/>
        /// to list of trades. Also sets security definition and product references on trades from storages to 
        /// have all details available for further use.
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>List of Filled trades.</returns>
        List<TradeCapture> GetFilledTrades(DateTime startDate, DateTime endDate);
        List<TradeCapture> GetFilledTrades(DateTime startDate, DateTime endDate, List<int> portfolioIds);
    }
}