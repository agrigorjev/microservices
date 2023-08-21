using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Messages;
using Mandara.Business.Bus.Messages.Base;
using com.latencybusters.lbm;
//using LumenWorks.Framework.IO.Csv;
using System.Threading.Tasks;
using Mandara.Entities.ErrorReporting;
using Mandara.Entities.Exceptions;
using System.Globalization;
using Mandara.Date;

namespace Mandara.Business.Bus.Commands
{
    /// <summary>
    /// Csv import command class. Performs reading od csv file., check for data validity and send csv data to bus service
    /// </summary>
    class CsvImportClientCommand : BusCommandBase
    {
        private readonly CSVSourceImportMessage _message;
        private readonly Action<MessageStatusCode> _callback;
        private LBMSource _updateSource;

        /// <summary>
        /// Constructior odfcommand class
        /// </summary>
        /// <param name="filename">Csv file path</param>
        /// <param name="effectiveDate">Date of data actual for</param>
        /// <param name="dataType">Type of data to import (Seals ,Open positions, or Trade activity)</param>
        /// <param name="callback">Function to call on command accomplished</param>
        public CsvImportClientCommand(
            string filename,
            DateTime effectiveDate,
            Mandara.Entities.SourceDataType dataType,
            Action<MessageStatusCode> callback)
        {
            //instantiate message
            _message = new CSVSourceImportMessage();
            _message.SnapshotId = Guid.NewGuid();
            _message.StatusCode = MessageStatusCode.UpdateData;

            //Get data from files
            if (dataType == Mandara.Entities.SourceDataType.Seals)
            {
                _message.Data = getSealsData(filename);
            }
            else
            {
                _message.Data = getFileData(filename);
            }

            _message.FileName = filename;

            //if data to import empty.
            if (_message.Data.Count < 2)
                throw new ArgumentException("Nothing to import", "CSV file");

            _message.EffectiveDate = effectiveDate;
            _message.DataType = (int)dataType;
            _callback = callback;
            TopicName = InformaticaHelper.CsvDataImportTopicName;
        }

        //Headers should exist in Seals data files
        private readonly string[] referenceHeaders =
        {
            "0",
            "ExchangeId",
            "BuySell",
            "Volume",
            "Market_Expiry",
            "Market_Product",
            "Price",
            "ExchangeTradeNumber",
            "ExchangeOrderNumber"
        };
        //Headers should exist in Trade Activity and Open positions data files
        private readonly string[] referenceHeadersPositions =
        {
            "0",
            "Currency Symbol",
            "Expiry Date",
            "Multiplier",
            "customer name",
            "Process Date",
            "Instrument Description",
            "Trade Price",
            "Quantity",
            "Buy/Sell",
            "Account Number",
            "Exchange Code",
            "Trade Date",
            "Trade Type",
            "Clearing Fee",
            "Commission",
            "Exchange Fee",
            "NFA Fee",
            "Futures Code"
        };

        /// <summary>
        /// Read data from Open Positio or Trade activity file
        /// </summary>
        /// <param name="filename">Csv file path</param>
        /// <returns>Collection of arrays representing single csv row.</returns>
        private List<string[]> getFileData(string filename)
        {
            List<string[]> lines = new List<string[]>();
            string missingHeaders = string.Empty;

            using (CsvHelper.CsvReader csv = new CsvHelper.CsvReader(new System.IO.StreamReader(filename), CultureInfo.InvariantCulture))            
            {
                List<DataImportException> formatErrors = new List<DataImportException>();
                csv.Read();
                string[] headers = csv.ReadHeader()
                    ? csv.HeaderRecord
                    : new string[0];

                //Check if required columns presented in given file
                if (checkColumns(headers, referenceHeadersPositions.Skip(1).ToArray(), out missingHeaders))
                {
                    //Add headers to return data 
                    int lineNumber = 1;
                    lines.Add(referenceHeadersPositions);

                    while (csv.Read())
                    {
                        lines.Add(BuildOutputLine(csv, formatErrors, lineNumber++));
                    }

                    //If data invalid  - break data import
                    if (formatErrors.Count > 0)
                    {
                        formatErrors.ForEach(
                            ex => ErrorReportingHelper.ReportError(
                                "Source Data Import",
                                ErrorType.ImportError,
                                ex.Message,
                                ex.RowNumber.ToString(),
                                ex,
                                ErrorLevel.Critical));

                        throw new ArgumentException($"Failed to import {filename}", "CSV file");
                    }
                }
                else
                {
                    //If colum required not presented- break data import
                    throw new DataImportException(
                        $"Following columns not present in import file: {missingHeaders}. ",
                        0);
                }
            }

            return lines;
        }

        private string[] BuildOutputLine(CsvHelper.CsvReader csv, List<DataImportException> formatErrors, int lineNumber)
        {
            string[] raw = new string[referenceHeadersPositions.Length];
            raw[0] = lineNumber.ToString();

            for (int i = 1; i < referenceHeadersPositions.Length; i++)
            {
                string colName = referenceHeadersPositions[i];
                string rawValue = csv[colName];
                
                raw[i] = rawValue;
                IsValueParseable(rawValue, colName, lineNumber, formatErrors);
            }

            return raw;
        }

        //TODO: Change this so that the list isn't passed in and mutated.
        private void IsValueParseable(string rawValue, string colName, int col, List<DataImportException> formatErrors)
        {
            switch (colName)
            {
                case "Quantity":
                case "Trade Price":
                case "Multiplier":
                {
                    Check(CheckDecimal);
                    break;
                }

                case "Process Date":
                case "Trade Date":
                {
                    Check(CheckDateTime);
                    break;
                }

                case "Futures Code":
                case "Exchange Code":
                {
                    Check(toCheck => !string.IsNullOrWhiteSpace(toCheck));
                    break;
                }

                case "Clearing Fee":
                case "Commission":
                case "Exchange Fee":
                case "NFA Fee":
                {
                    Check(CheckNullableDecimal);
                    break;
                }
            }

            void Check(Func<string, bool> check)
            {
                if (!check(rawValue))
                {
                    formatErrors.Add(new DataImportException(ParseFailureMsg(), col));
                }
            }

            string ParseFailureMsg()
            {
                return $"[{rawValue ?? ""}] could not be parsed as {colName}";
            }
        }

        /// <summary>
        /// Read data from Seals csv file
        /// </summary>
        /// <param name="filename">Csv file path</param>
        /// <returns>Collection of arrays representing single csv row.</returns>
        private List<string[]> getSealsData(string filename)
        {
            List<string[]> lines = new List<string[]>();

            using (CsvHelper.CsvReader csv = new CsvHelper.CsvReader(new System.IO.StreamReader(filename), CultureInfo.InvariantCulture))
            {

                string[] headers = csv.ReadHeader() ? csv.HeaderRecord : throw new DataImportException(string.Format("No Headers present in import file"), 0);
                
                string notFoundheaders = string.Empty;
                List<DataImportException> formatErrors = new List<DataImportException>();

                //Check if required headers exists. 
                //If not - we suppose that file is wrong and break import
                if (checkColumns(headers, referenceHeaders.Skip(1).ToArray(), out notFoundheaders))
                {
                    //Add headres with row number =0
                    string[] nHeaders = new string[headers.Length + 1];
                    nHeaders[0] = "0";

                    Array.Copy(
                        headers,
                        0,
                        nHeaders,
                        1,
                        headers.Length);

                    lines.Add(nHeaders);
                    int n = 1;

                    while (csv.Read())
                    {
                        //Process eacgh row
                        string[] raw = new string[nHeaders.Length];
                        raw[0] = n++.ToString();

                        //Chek if data format invalid for several fields
                        for (int i = 1; i < nHeaders.Length; i++)
                        {
                            raw[i] = csv[nHeaders[i]];

                            switch (nHeaders[i])
                            {
                                case "Volume":
                                    if (!CheckDecimal(raw[i]))
                                    {
                                        formatErrors.Add(
                                            new DataImportException(
                                                string.Format(
                                                    "The following string could not be parsed as {0} [{1}]",
                                                    nHeaders[i],
                                                    raw[i]),
                                                n));
                                    }

                                    break;

                                case "Price":
                                    if (!CheckPrice(raw[i]))
                                    {
                                        formatErrors.Add(
                                            new DataImportException(
                                                string.Format(
                                                    "The following string could not be parsed as {0} [{1}]",
                                                    nHeaders[i],
                                                    raw[i]),
                                                n));
                                    }

                                    break;

                                case "Market_Expiry":
                                    if (!CheckMarketExpiry(raw[i]))
                                    {
                                        formatErrors.Add(
                                            new DataImportException(
                                                string.Format(
                                                    "The following string could not be parsed as {0} [{1}]",
                                                    nHeaders[i],
                                                    raw[i]),
                                                n));
                                    }

                                    break;
                            }
                        }

                        lines.Add(raw);
                    }

                    //If there was format errors - report to user
                    if (formatErrors.Count > 0)
                    {
                        formatErrors.ForEach(
                            ex => ErrorReportingHelper.ReportError(
                                "Source Data Import",
                                ErrorType.ImportError,
                                ex.Message,
                                ex.RowNumber.ToString(),
                                ex,
                                ErrorLevel.Critical));
                    }
                }
                else
                {
                    throw new DataImportException(
                        string.Format("Following columns not present in import file: {0}. ", notFoundheaders),
                        0);
                }
            }

            return lines;
        }

        /// <summary>
        /// Actual sen message
        /// </summary>
        public override void Execute()
        {
            _updateSource = BusClient.Instance.InformaticaHelper.GetSource(TopicName);
            SendSnapshotMessage(_message, _message.Data.Count, InformaticaHelper.CsvImportPackageSize);

            SendMessage(
                new EndOfSnapshotMessage() { SnapshotId = _message.SnapshotId.Value },
                _updateSource,
                TopicName,
                false);

            //After transmission complete we wait for service returns results.
            StartTaskWithErrorHandling(
                () => getUpdateResult2(_message.SnapshotId.Value, MessageStatusCode.ResultRequest, new List<Error>()));
        }

        /// <summary>
        /// Get result of import from service
        /// </summary>
        /// <param name="reqId">Id of original message request</param>
        /// <param name="code">Result sode</param>
        public void getUpdateResult(Guid reqId, MessageStatusCode code)
        {
            if (reqId == null)
            {
                throw new Exception("reqId can't be null");
            }

            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));

            switch (code)
            {
                case MessageStatusCode.ResultRequest:
                case MessageStatusCode.UpdateData:
                case MessageStatusCode.UpdateInProgress:
                    //Still in progress try one more time
                    BusClient.Instance.CommandManager.AddCommand(
                        new UpdateRequestCommand(reqId, _updateSource, TopicName, getUpdateResult));

                    break;

                //We gote result - announce caller.
                case MessageStatusCode.UpdateOK:
                case MessageStatusCode.UpdateFailed:
                    StartTaskWithErrorHandling(
                        () =>
                        {
                            if (_callback != null) _callback(code);
                        });

                    break;
            }
        }

        /// <summary>
        /// Get result from service importing data. Allows return not only codes but also import errors
        /// </summary>
        /// <param name="reqId">Id of original message request</param>
        /// <param name="code">Result code</param>
        /// <param name="lst">Import errors from service</param>
        public void getUpdateResult2(Guid reqId, MessageStatusCode code, List<Error> lst)
        {
            if (reqId == null)
            {
                throw new Exception("reqId can't be null");
            }

            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));

            switch (code)
            {
                case MessageStatusCode.ResultRequest:
                case MessageStatusCode.UpdateData:
                case MessageStatusCode.UpdateInProgress:
                    BusClient.Instance.CommandManager.AddCommand(
                        new UpdateRequestCommand(reqId, _updateSource, TopicName, getUpdateResult2));

                    break;

                case MessageStatusCode.UpdateOK:
                case MessageStatusCode.UpdateFailed:
                    StartTaskWithErrorHandling(
                        () =>
                        {
                            if (_callback != null) _callback(code);
                        });

                    ErrorReportingHelper.GlobalQueue.AddRange(lst);

                    break;
            }
        }

        /// <summary>
        /// Sending data message
        /// </summary>
        /// <param name="message">Message itself</param>
        /// <param name="numEntities">Total rows in data</param>
        /// <param name="packSize">Number of records to transmit with single sending</param>
        private void SendSnapshotMessage(CSVSourceImportMessage message, int numEntities, int packSize)
        {
            List<string[]> entities = message.Data;
            int numPacks = (numEntities / packSize) + (numEntities % packSize > 0 ? 1 : 0);

            for (int i = 0; i < numPacks; i++)
            {
                int skip = i * packSize;
                List<string[]> pack = entities.Skip(skip).Take(packSize).ToList();

                if (pack.Count > 0)
                {
                    message.Data = pack;
                    message.ContolCount = numPacks;
                    SendMessage(message, _updateSource, TopicName, false);
                }
            }
        }

        private Task StartTaskWithErrorHandling(Action action)
        {
            return Task.Factory.StartNew(action)
                       .ContinueWith(t => ReportError(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
        }

        private void ReportError(Exception ex)
        {
            ErrorReportingHelper.ReportError(
                "IRM",
                ErrorType.Exception,
                string.Format("Update Receiver encounter an error [Topic={0}]", TopicName),
                null,
                ex,
                ErrorLevel.Critical);
        }

        //Series of function to check validity of csv data fro specific datatype

    #region Validate csv file

        /// <summary>
        /// Check if array of columns contains all required fields
        /// </summary>
        /// <param name="array">Column names array</param>
        /// <param name="check">Array of names to find out</param>
        /// <param name="notFound">If not found we need to know which one field missed</param>
        /// <returns>Boolean value. If given array contains all of the required fields</returns>
        private bool checkColumns(string[] array, string[] check, out string notFound)
        {
            StringBuilder sb = new StringBuilder();
            check.Where(c => !array.Contains(c)).ToList().ForEach(ss => sb.AppendFormat("{0}, ", ss));
            notFound = sb.ToString().TrimEnd(',', ' ');

            return string.IsNullOrEmpty(notFound);
        }

        /// <summary>
        /// If string represents valid decimal
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool CheckPrice(string input)
        {
            decimal? price = SealsPriceParser.ParsePrice(input);

            return price.HasValue;
        }

        /// <summary>
        /// Check if string represents a valid decimal
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool CheckDecimal(string input)
        {
            try
            {
                Convert.ToDecimal(input, new CultureInfo("en-US"));

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Check if string is valid decimal or contains nothing
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool CheckNullableDecimal(string input)
        {
            if (string.IsNullOrEmpty(input))
                return true;

            try
            {
                Convert.ToDecimal(input, new CultureInfo("en-US"));

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Check if string represent valid datetime value
        /// </summary>
        /// <param name="toCheck"></param>
        /// <returns></returns>
        private bool CheckDateTime(string toCheck)
        {
            try
            {
                return DateTime.TryParseExact(
                           toCheck,
                           Formats.SortableShortDate,
                           CultureInfo.InvariantCulture,
                           DateTimeStyles.None,
                           out DateTime _)
                       || DateParse.IsValidDayFirstDate(toCheck);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Check if string represent valid datetime value. 
        /// Specific for Msrket_Expiry field: there may be 00 values for day component of datetime,
        /// which breaks traditional conversion but valid for seals.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool CheckMarketExpiry(string input)
        {
            try
            {
                DateTime r;

                if (!DateTime.TryParseExact(
                    input,
                    Formats.SortableShortDate,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out r))
                {
                    if (input.Length == 8 && input.EndsWith("00"))
                    {
                        string checkDate = input.Substring(0, 7) + "1";

                        if (DateTime.TryParseExact(
                            checkDate,
                            Formats.SortableShortDate,
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out r))
                        {
                            r = r.AddMonths(1).AddDays(-1);
                        }
                        else
                        {
                            r = DateTime.Parse(input, CultureInfo.GetCultureInfo("en-GB"));
                        }
                    }
                    else
                    {
                        r = DateTime.Parse(input, CultureInfo.GetCultureInfo("en-GB"));
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

    #endregion
    }
}