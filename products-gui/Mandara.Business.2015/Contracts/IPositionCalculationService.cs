using System;
using System.Collections.Generic;
using Mandara.Business.Model;
using Mandara.Entities;

namespace Mandara.Business.Contracts
{
    public interface IPositionCalculationService
    {
        List<CalculationDetailModel> CalculatePositions(TradeCapture trade, 
            DateTime? riskDateParam = null, 
            bool checkProductValidation = false,
            bool splitWeekends = false);

        List<CalculationDetailModel> CalculatePositions(List<TradeModel> trades, 
            List<SdQuantityModel> sdQuantities, 
            DateTime? riskDateParam = null, 
            bool splitWeekends = false);
    }
}