using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Mandara.Date.Time;
using Newtonsoft.Json;

namespace Mandara.Entities.ErrorReporting
{
    public class Error
    {
        public String DisplayType
        {
            get
            {
                switch (Type)
                {
                    case ErrorType.DataError:
                        return "Data Error";
                    case ErrorType.Exception:
                        return "Exception";
                    case ErrorType.CalculationError:
                        return "Calculation Error";
                    case ErrorType.TradeError:
                        return "Trade Error";
                    case ErrorType.ImportError:
                        return "Import Error";
                    case ErrorType.Information:
                        return "Information";
                    case ErrorType.TradeOnHolidayDate:
                        return "Trade On Holiday";
                    default:
                        return "Unknown";
                }
            }
        }

        public string DisplayLevel
        {
            get
            {
                return Level.ToString();
            }
        }

        public ErrorType Type { get; set; }

        public String Message { get; set; }

        public String Source { get; set; }

        public String ObjectId { get; set; }

        [JsonProperty(TypeNameHandling = TypeNameHandling.Objects)]
        public object Object { get; set; }

        public ErrorLevel Level { get; set; } 

        public DateTime Date { get; set; }

        public String UserName { get; set; }

        public List<int> PortfolioIds { get; set; }

        /// <summary>
        /// Indicates if error should display in support Window
        /// </summary>
        public bool HandleBySupport { get; set; }

        public Error()
        {
            PortfolioIds = new List<int>();
        }

        public Error(string userName, 
            String source, 
            ErrorType errorType, 
            String message, 
            String objectId = null, 
            Object o = null, 
            ErrorLevel level = ErrorLevel.Normal,
            bool handleBySupport=false)
            : this(source, errorType, message, objectId, o, level)
        {
            HandleBySupport = handleBySupport;
            UserName = userName;
        }

        public Error(String source, 
            ErrorType errorType, 
            String message, 
            String objectId = null, 
            Object o = null, 
            ErrorLevel level = ErrorLevel.Normal,
            bool handleBySupport=false)
            :this()
        {
            HandleBySupport = handleBySupport;
            Source = source;
            Type = errorType;
            Message = message;
            ObjectId = objectId;
            Object = o;
            Date = SystemTime.Now();
            Level = level;
        }
    }
}
