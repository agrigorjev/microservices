using Mandara.Business.Bus.Messages.TradeAdd;
using Mandara.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mandara.Business.Bus
{
    /// <summary>
    /// Build new <see cref="Instrument"/> based on information from <see cref="OfficialProduct"/>. 
    /// If object is created for Master Tool all products are considered, if not then only products allowed for
    /// manual trades are considered.
    /// </summary>

    public interface IOfficialProductToInstrument
    {
        /// <summary>
        /// Build new <see cref="Instrument"/> for Master Tool. All products are considered.
        /// </summary>
        /// <param name="officialProduct"><see cref="OfficialProduct"/> object</param>
        /// <returns><see cref="Instrument"/> object</returns>
        Instrument ConvertOfficialProductToInstrumentForMasterToolMode(OfficialProduct officialProduct);

        /// <summary>
        /// Build new <see cref="Instrument"/>. Products allowed for manual trades are considered.
        /// </summary>
        /// <param name="officialProduct"><see cref="OfficialProduct"/> object</param>
        /// <returns><see cref="Instrument"/> object</returns>
        Instrument ConvertOfficialProductToInstrument(OfficialProduct officialProduct);
    }
}
