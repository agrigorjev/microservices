using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mandara.Business.Bus.Commands.Base
{
    public interface ICommandManager
    {
        void AddCommand<T>(Action<T> commandInitializer) where T : ICommand;

        void AddCommand(ICommand command);
    }
}
