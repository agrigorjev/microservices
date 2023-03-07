using System;
using System.Collections.Generic;
using Mandara.Business.Bus.Messages;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities.ErrorReporting;

namespace Mandara.Business.Contracts
{
    public interface ICsvImportService
    {
        void AddCsvImportPackage(Guid msgId, CSVSourceImportMessage data);
        void SaveCsvImport(Guid msgId, string userName);
        bool AllDataRecieved(Guid id);
        MessageStatusCode GetUpdateResult(Guid id, out List<Error> errors);
    }
}
