using Mandara.Business.Bus.Messages.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mandara.Business.Bus.Messages.Pnl
{
    public class VarMessage : MessageBase
    {
        //Todo Why are VaR message Data values doubles when VaR Engine calculates in decimal
        //Todo change using VarType to data structure
        public class Data
        {
            public int PortfolioId { get; set; }
            public double Var { get; set; }
            public byte VarType { get; set; }
            public double VarContribution { get; set; }
            public double VarContributionRatio { get; set; }
            public double Ratio { get; set; }
        }

        public List<Data> VarValues { get; set; }
    }
}
