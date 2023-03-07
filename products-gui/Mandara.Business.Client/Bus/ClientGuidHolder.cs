using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mandara.Business.Bus
{
    /// <summary>
    /// Stores the ClientGuid. Can be move to BusClient when the BusClient will be one for all clients (RiscTool, AlertService and etc)
    /// </summary>
    public class ClientGuidHolder
    {
        public static readonly Guid ClientGuid = Guid.NewGuid();
    }
}
