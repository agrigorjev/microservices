using System;
using System.Collections.Generic;
using System.Linq;
using Mandara.Business.Bus.Messages;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Business.Contracts;
using Mandara.Entities;
using Mandara.Entities.ErrorReporting;
using Mandara.Entities.Exceptions;
using Mandara.Import;

namespace Mandara.Business.Services
{
    public class CsvImportService : ICsvImportService
    {
        private readonly object _lockResults = new object();
        private readonly object _lockCsvSrc = new object();
        private Dictionary<Guid, Tuple<int, CSVSourceImportMessage>> _updateCsv;
        private Dictionary<Guid, Tuple<MessageStatusCode, bool>> _updateResult;
        private Dictionary<Guid, List<Error>> _expiredImport;

        public void AddCsvImportPackage(Guid msgId, CSVSourceImportMessage data)
        {
            lock (_lockCsvSrc)
            {
                bool allinPlace = false;
                if (_updateCsv == null)
                {
                    _updateCsv = new Dictionary<Guid, Tuple<int, CSVSourceImportMessage>>();
                }
                if (!_updateCsv.Keys.Contains(msgId))
                {
                    _updateCsv.Add(msgId, new Tuple<int, CSVSourceImportMessage>(1, data));
                    allinPlace = data.ContolCount == 1;
                }
                else
                {
                    CSVSourceImportMessage current = _updateCsv[msgId].Item2;
                    current.Data.AddRange(data.Data);
                    int cCount = _updateCsv[msgId].Item1 + 1;
                    _updateCsv[msgId] = new Tuple<int, CSVSourceImportMessage>(cCount, current);
                    allinPlace = cCount == data.ContolCount;
                }
                SetResult(msgId, MessageStatusCode.UpdateInProgress, allinPlace);
            }
        }

        public void SaveCsvImport(Guid msgId, string userName)
        {
            //to send on error notification per import
            bool errorLogged = false;
            if (_updateCsv == null || !_updateCsv.Keys.Contains(msgId) || _updateCsv[msgId] == null ||
                _updateCsv[msgId].Item2.Data.Count == 0)
            {
                SetResult(msgId, MessageStatusCode.UpdateFailed, false);
                ErrorReportingHelper.GlobalQueue.Enqueue(new Error(userName, "Csv Import", ErrorType.DataError,
                    "Empty prices collection", null, null, ErrorLevel.Critical));
                return;
            }

            SourceDataImport importer = new SourceDataImport();
            CSVSourceImportMessage message = _updateCsv[msgId].Item2;

            try
            {
                IEnumerable<DataImportException> notMappedProducts;
                int resultCnt = 0;
                List<Error> expiredErrors = new List<Error>();
                if (message.DataType == (int) SourceDataType.Seals)
                {
                    List<Tuple<List<DataImportException>, SealDetail>> lst =
                        importer.ImportSealSourceDataFromArray(message.Data, message.EffectiveDate,
                            (SourceDataType) message.DataType);
                    notMappedProducts = lst.Where(a => a.Item1.Count > 0).SelectMany(n => n.Item1).ToList();
                    resultCnt = lst.Count;
                }
                else
                {
                    List<Tuple<DataImportException, SourceDetail>> lst = importer.ImportSourceDataFromArray(
                        message.Data, message.EffectiveDate, (SourceDataType) message.DataType, out expiredErrors);

                    if (expiredErrors.Count > 0)
                    {
                        //We change the logic how those error handled - it was returned to client.
                        //Now we throw those error to the event log for specific user
                        expiredErrors.ForEach(e =>
                        {
                            e.UserName = userName;
                            ErrorReportingHelper.GlobalQueue.Enqueue(e);
                        });

                        errorLogged = true;
                    }
                    notMappedProducts = lst.Where(a => a.Item1 != null).Select(n => n.Item1).ToList();
                    resultCnt = lst.Count;
                }

                if (resultCnt > 0)
                {
                    SetResult(msgId, MessageStatusCode.UpdateOK, true);
                }
                else
                {
                    SetResult(msgId, MessageStatusCode.UpdateFailed, true);
                    if (!errorLogged)
                    {
                        errorLogged = true;
                    }
                }

                if (notMappedProducts.Count() > 0)
                {
                    foreach (DataImportException failedProduct in notMappedProducts)
                    {
                        ErrorReportingHelper.GlobalQueue.Enqueue(new Error(userName, "Source Data Import",
                            ErrorType.ImportError,
                            failedProduct.Message, failedProduct.RowNumber.ToString(), failedProduct,
                            failedProduct.ErrorLevel));
                    }

                    if (notMappedProducts.Count(x => x.ErrorLevel != ErrorLevel.Low) > 0)
                    {
                        if (!errorLogged)
                        {
                            errorLogged = true;
                        }
                    }
                }

                _updateCsv.Remove(msgId);
            }
            catch (Exception ex)
            {
                SetResult(msgId, MessageStatusCode.UpdateFailed, true);
                ErrorReportingHelper.GlobalQueue.Enqueue(new Error(userName, "Source Data Import", ErrorType.DataError,
                    ex.Message, null, null, ErrorLevel.Critical));
            }
        }

        /// <summary>
        /// returns import csv result with list of expired products errors.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public MessageStatusCode GetUpdateResult(Guid id, out List<Error> errors)
        {
            errors = new List<Error>();
            MessageStatusCode res = MessageStatusCode.ResultRequest;
            if (_updateResult != null && _updateResult.Keys.Contains(id))
            {
                res = _updateResult[id].Item1;
            }

            if (res == MessageStatusCode.UpdateOK || res == MessageStatusCode.UpdateFailed)
            {
                _updateResult.Remove(id);
                if (_expiredImport != null && _expiredImport.ContainsKey(id))
                {
                    errors = _expiredImport[id];
                    _expiredImport.Remove(id);
                }
            }

            return res;
        }

        public bool AllDataRecieved(Guid id)
        {
            bool res = false;
            if (_updateResult != null && _updateResult.Keys.Contains(id))
            {
                res = _updateResult[id].Item2;
            }
            return res;
        }

        private void SetResult(Guid id, MessageStatusCode code, bool gotAll)
        {
            lock (_lockResults)
            {
                if (_updateResult == null) _updateResult = new Dictionary<Guid, Tuple<MessageStatusCode, bool>>();
                if (_updateResult.Keys.Contains(id))
                {
                    _updateResult[id] = new Tuple<MessageStatusCode, bool>(code, gotAll);
                }
                else
                {
                    _updateResult.Add(id, new Tuple<MessageStatusCode, bool>(code, gotAll));
                }
            }
        }
    }
}
