using Mandara.RiskMgmtTool.Spreader.Common;
using System.ComponentModel;

namespace Mandara.RiskMgmtTool.Spreader.MonthlySpreader
{
    public interface IMonthlySpreaderView : ISpreaderViewBase
    {
        void SetInputGrid(BindingList<SimulatedPosition> manualInputPositions);
        void SetCalculatedGrid(BindingList<MonthlySpread> manualInputPositions);
    }
}
