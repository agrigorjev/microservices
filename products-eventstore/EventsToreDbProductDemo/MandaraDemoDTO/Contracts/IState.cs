using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandaraDemoDTO.Contracts
{

    public enum State
    {
        UNSPECIFIED,
        REMOVED
    }
    public interface IState
    {
        

        State Status{get;set;}

    }
}
