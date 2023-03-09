using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Extensions.Logging;

namespace Mandara.ProductGUI.Log
{
    public class LogModule : LoggerModuleBase
    {
        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>
        public override void Load()
        {
            this.Bind<ILoggerFactory>().ToConstant(new LoggerFactory());
            base.Load();
        }
    }
}
