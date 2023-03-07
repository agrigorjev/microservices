using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mandara.Entities.Calculation;
using Mandara.Entities.MatchingDummies;
using Mandara.Entities.Trades;
using Newtonsoft.Json;

namespace Mandara.Entities
{
    [Serializable]
    [Table("support_notification_log")]
    public partial class TradeSupportAlert
    {
        [Column("notification_id")]
        [Key]
        public Guid Id { get; set; }

        [Column("message_type")]
        public short Type { get; set; }

        [Column("message")]
        [Required]
        [StringLength(250)]
        public string Message { get; set; }

        [Column("parameters")]
        public string Params { get; set; }

        [Column("date_submitted")]
        public DateTime Submitted { get; set; }

        [Column("is_acknowledged")]
        public bool IsAcknowledged { get; set; }

        [Column("acknowledged_at")]
        public DateTime? AcknowledgedAt { get; set; }

        [Column("acknowleged_by")]
        [StringLength(100)]
        public string AcknowledgedBy { get; set; }

        [Column("acknowledged_ip")]
        [StringLength(50)]
        public string AcknowledgedIP { get; set; }

        public enum AlertType
        {
            Expiring_products,
            RollOff,
            TAS,
            Dummies,
            Import,
            Product_Definition,
            TradeOnHolidayDate,
            [Obsolete("This functionality was removed and not supported anymore")]
            CustomSpread,
            Brokerage
        }

        [NotMapped]
        public string DisplayType
        {
            get
            {
                switch ((AlertType)this.Type)
                {
                    case AlertType.TAS:
                        return "TAS warning";
                    case AlertType.RollOff:
                        return "Roll Off Notification";
                    case AlertType.Product_Definition:
                        return "Product Definition";
                    case AlertType.Import:
                        return "Import error";
                    case AlertType.Expiring_products:
                        return "Expiring Product";
                    case AlertType.Dummies:
                        return "Matching dummies";
                    case AlertType.TradeOnHolidayDate:
                        return "Trade On Holiday";
                    case AlertType.CustomSpread:
                        return "Custom Spread";
                    case AlertType.Brokerage:
                        return "Brokerage";
                    default:
                        return string.Empty;
                }
            }
        }

        [NotMapped]
        public AlertType AType
        {
            set
            {
                this.Type = (short)value;
            }
            get
            {
                return (AlertType)Type;
            }
        }

        private object _parameters;

        [NotMapped]
        public Object Parameters
        {
            get
            {
                if (_parameters == null && !string.IsNullOrEmpty(this.Params))
                {
                    switch ((AlertType)this.Type)
                    {
                        case AlertType.TAS:
                            _parameters = JsonConvert.DeserializeObject<List<TasCheckDetail>>(this.Params);
                            break;
                        case AlertType.RollOff:
                            _parameters = JsonConvert.DeserializeObject<List<RolloffDetail>>(this.Params);
                            break;
                        case AlertType.Dummies:
                            _parameters = JsonConvert.DeserializeObject<MatchingDummiesObjectCollection>(this.Params);
                            break;
                        case AlertType.Brokerage:
                            _parameters = JsonConvert.DeserializeObject<TradeView>(this.Params);
                            break;
                    }
                }

                return _parameters;
            }
            set
            {
                if (value != null)
                {
                    _parameters = value;
                    if (_parameters != null)
                    {
                        this.Params = JsonConvert.SerializeObject(_parameters,
                                                        new JsonSerializerSettings
                                                        {
                                                            NullValueHandling = NullValueHandling.Ignore,
                                                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore

                                                        });
                    }
                    else
                    {
                        this.Params = string.Empty;
                    }
                }
            }
        }

        private string _details = "View Details";
        
        [NotMapped]
        public string Details
        {
            get
            {
                if (this.Id != null && this.Id != Guid.Empty && this.AType != AlertType.TradeOnHolidayDate)
                {
                    return _details;
                }
                else if ((this.Id == null || this.Id == Guid.Empty) && this.AType == AlertType.Import)
                {
                    return _details;
                }
                else
                {
                    return null;
                }
            }
            set { _details = value; }
        }

        private string _ignoreMessages = "Ignore messages";
       
        [NotMapped]
        public string IgnoreMessages
        {
            get { return _ignoreMessages; }
            set { _ignoreMessages = value; }
        }

        private string _acknowledge = "Acknowledge";

        [NotMapped]
        public string Acknowledge
        {
            get
            {
                if (this.Id != null && this.Id != Guid.Empty)
                {
                    return _acknowledge;
                }
                else
                {
                    return null;
                }
            }
            set { _acknowledge = value; }
        }

        public T GetParams<T>()
        {
            try
            {
                _parameters = JsonConvert.DeserializeObject<T>(this.Params);
                return (T)_parameters;
            }
            catch
            {
                return default(T);
            }
        }

        [NotMapped]
        public string Username { get; set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            TradeSupportAlert entity = obj as TradeSupportAlert;
            if (entity == null)
            {
                return false;
            }

            return Id == entity.Id;
        }

        [NotMapped]
        public AlertType MessageType
        {
            set { Type = (short)value; }
            get { return (AlertType)Type; }
        }

        public void SetParameters(object val)
        {
            Parameters = JsonConvert.SerializeObject(val,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
        }

    }
}
