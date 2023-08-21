using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mandara.Business.Managers;

namespace Mandara.Business.Configuration
{
    public class FxSecDefIdReader
    {

        public static bool ReadAndValidateFXSecDefID()
        {
            string fxSecDefIdSetting = ConfigurationManager.AppSettings["FxSecurityDefinitionId"];

            if (fxSecDefIdSetting == null)
            {
                return false;
            }

            int fxSecDefId;

            if (!int.TryParse(fxSecDefIdSetting, out fxSecDefId))
            {
                return false;
            }

            SecurityDefinitionsManager.FxSecDefId = fxSecDefId;
            return true;
        }
    }
}
