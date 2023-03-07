using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mandara.Date.Time;
using Newtonsoft.Json;

namespace Mandara.Entities
{
    [Table("adm_alerts")]
    public class AdministrativeAlert
    {
        [Column("alert_id")]
        [Key]
        public int AlertId { get; set; }

        [Column("portfolio_id")]
        public int? PortfolioId { get; set; }

        [Column("alert_type")]
        public int AlertType { get; set; }

        [Column("alert_title")]
        [StringLength(150)]
        public string Title { get; set; }

        [Column("boundary")]
        public short Boundary { get; set; }

        [Column("threshold")]
        public decimal Threshold { get; set; }

        [Column("escalation_time")]
        public int EscalationTime { get; set; }

        [Column("level_one_group_id")]
        public int? LevelOneGroupId { get; set; }

        [Column("level_two_group_id")]
        public int? LevelTwoGroupId { get; set; }

        [Column("level_one_subject")]
        [StringLength(250)]
        public string Level1Subject { get; set; }

        [Column("level_one_message")]
        public string Level1Message { get; set; }

        [Column("level_two_subject")]
        [StringLength(250)]
        public string Level2Subject { get; set; }

        [Column("level_two_message")]
        public string Level2Message { get; set; }

        [Column("alert_active")]
        public bool Active { get; set; }

        [Column("product_id")]
        public int? ProductID { get; set; }

        [Column("level3_subject")]
        [StringLength(250)]
        public string Level3Subject { get; set; }

        [Column("level3_message")]
        public string Level3Message { get; set; }

        [Column("level4_subject")]
        [StringLength(250)]
        public string Level4Subject { get; set; }

        [Column("level4_message")]
        public string Level4Message { get; set; }

        [Column("level3_group_id")]
        public int? Level3GroupId { get; set; }

        [Column("level4_group_id")]
        public int? Level4GroupId { get; set; }

        [Column("level1_escalation_time")]
        public int? Level1EscalationTime { get; set; }

        [Column("level2_escalation_time")]
        public int? Level2EscalationTime { get; set; }

        [Column("level3_escalation_time")]
        public int? Level3EscalationTime { get; set; }

        [Column("level4_escalation_time")]
        public int? Level4EscalationTime { get; set; }

        [Column("condition_check_count")]
        public int? ConditionCheckCount { get; set; }

        [Column("product_group_id")]
        public int? ProductGroupId { get; set; }

        [Column("range_threshold_end")]
        public decimal? RangeThresholdEnd { get; set; }

        [Column("email_subject_template")]
        [StringLength(250)]
        public string EmailSubjectTemplate { get; set; }

        [Column("email_message_template")]
        public string EmailMessageTemplate { get; set; }

        [Column("start_time")]
        public DateTime? StartTimeDb { get; set; }

        [Column("is_level1_active")]
        public bool? IsLevel1ActiveDb { get; set; }

        [Column("is_level2_active")]
        public bool? IsLevel2ActiveDb { get; set; }

        [Column("is_level3_active")]
        public bool? IsLevel3ActiveDb { get; set; }

        [Column("is_level4_active")]
        public bool? IsLevel4ActiveDb { get; set; }

        [Column("do_not_trigger_on_weekends")]
        public bool? DoNotTriggerOnWeekendsDb { get; set; }

        [Column("custom_properties")]
        public string CustomPropertiesJson { get; set; }

        [ForeignKey("LevelOneGroupId")]
        public virtual AdministrativeAlertGroup AlertGroup1
        {
            get => _alertGroup1;
            set
            {
                _alertGroup1 = value;
                LevelOneGroupId = _alertGroup1?.GroupId;
            }
        }

        [ForeignKey("LevelTwoGroupId")]
        public virtual AdministrativeAlertGroup AlertGroup2
        {
            get => _alertGroup2;
            set
            {
                _alertGroup2 = value;
                LevelTwoGroupId = _alertGroup2?.GroupId;
            }
        }

        [ForeignKey("Level3GroupId")]
        public virtual AdministrativeAlertGroup AlertGroup3
        {
            get => _alertGroup3;
            set
            {
                _alertGroup3 = value;
                Level3GroupId = _alertGroup3?.GroupId;
            }
        }

        [ForeignKey("Level4GroupId")]
        public virtual AdministrativeAlertGroup AlertGroup4
        {
            get => _alertGroup4;
            set
            {
                _alertGroup4 = value;
                Level4GroupId = _alertGroup4?.GroupId;
            }
        }

        [ForeignKey("PortfolioId")]
        public virtual Portfolio Portfolio
        {
            get => _portfolio;
            set
            {
                _portfolio = value;
                PortfolioId = _portfolio?.PortfolioId;
            }
        }

        [ForeignKey("ProductID")]
        public virtual Product Product
        {
            get => _product;
            set
            {
                _product = value;
                ProductID = _product?.ProductId;
            }
        }

        [ForeignKey("ProductGroupId")]
        public virtual ProductCategory ProductGroup
        {
            get => _productGroup;
            set
            {
                _productGroup = value;
                ProductGroupId = _productGroup?.CategoryId;
            }
        }


        private Dictionary<string, string> _customProperties;
        private AdministrativeAlertGroup _alertGroup1;
        private AdministrativeAlertGroup _alertGroup2;
        private Portfolio _portfolio;
        private AdministrativeAlertGroup _alertGroup3;
        private AdministrativeAlertGroup _alertGroup4;
        private ProductCategory _productGroup;
        private Product _product;

        /// <summary>
        /// Enumeration for alert type
        /// </summary>
        public enum AdmAlertType
        {
            NaN = 0,
            PnL = 1,
            VaR = 2,
            Flat_Price_Position = 3,
            Trade_Time = 4,
            Expiring_Products = 5,
            TransferServiceErrors = 6,
            TradeInPortfolio = 7
        }

        public enum BoundaryType
        {
            LessThan = 1,
            GreaterThan = 2,
            InRange = 3,
        }

        public class TimeRange
        {
            public TimeRange(TimeSpan start, TimeSpan end)
            {
                Start = start;
                End = end;
            }

            public TimeSpan Start { get; private set; }
            public TimeSpan End { get; private set; }

            public bool Contains(TimeSpan time)
            {
                if (Start <= End)
                {
                    return time >= Start && time <= End; // regular range
                }
                else
                {
                    return time >= Start || time <= End; // range that contains midnight
                }
            }
        }

        /// <summary>
        /// Db type to enumeration type
        /// </summary>
        [NotMapped]
        public AdmAlertType TypeOfAlert
        {
            get
            {
                try
                {
                    return (AdmAlertType)AlertType;
                }
                catch
                {
                    return AdmAlertType.NaN;
                }
            }
            set => AlertType = (int)value;
        }

        /// <summary>
        /// Alert type to show in grid
        /// </summary>
        [NotMapped]
        public string TypeAlertString
        {
            get
            {
                switch (TypeOfAlert)
                {
                    case AdmAlertType.Flat_Price_Position:
                        {
                            return "Flat Price Position";
                        }

                    case AdmAlertType.PnL:
                        {
                            return "PnL";
                        }

                    case AdmAlertType.Trade_Time:
                        {
                            return "Trade Time";
                        }

                    case AdmAlertType.VaR:
                        {
                            return "VaR";
                        }

                    case AdmAlertType.Expiring_Products:
                        {
                            return "Expiring Products";
                        }

                    case AdmAlertType.TransferServiceErrors:
                        {
                            return "Transfer Errors";
                        }

                    case AdmAlertType.TradeInPortfolio:
                        {
                            return "Trade in Portfolio";
                        }

                    default:
                        {
                            return string.Empty;
                        }
                }

            }
        }

        [NotMapped]
        public BoundaryType TypeOfBoundary
        {
            get => (BoundaryType)Boundary;
            set => Boundary = (short)value;
        }

        /// <summary>
        /// Interpret threshold in terms of Trade_Time type of alert
        /// </summary>
        [NotMapped]
        public TimeSpan TradeTime
        {
            get => TypeOfAlert == AdmAlertType.Trade_Time ? TimeSpan.FromMinutes((double)Threshold) : TimeSpan.Zero;
            set => Threshold = (decimal)value.TotalMinutes;
        }

        [NotMapped]
        public TimeRange TradeTimeRange
        {
            get =>
                (TypeOfAlert == AdmAlertType.Trade_Time && TypeOfBoundary == BoundaryType.InRange)
                    ? new TimeRange(TradeTime, TradeTimeRangeEnd ?? TimeSpan.MaxValue)
                    : null;
            set
            {
                TradeTime = value?.Start ?? TimeSpan.Zero;
                TradeTimeRangeEnd = value?.End;
            }
        }

        /// <summary>
        /// Interpret threshold in terms of Trade_Time type of alert
        /// </summary>
        [NotMapped]
        public TimeSpan? TradeTimeRangeEnd
        {
            get =>
                (TypeOfAlert == AdmAlertType.Trade_Time && TypeOfBoundary == BoundaryType.InRange)
                    ? TimeSpan.FromMinutes((double)(RangeThresholdEnd ?? decimal.Zero))
                    : default(TimeSpan?);

            set => RangeThresholdEnd = value != null ? (decimal)value.Value.TotalMinutes : (decimal?)null;
        }

        /// <summary>
        /// Interpret threshold in terms of decimal value (for PnL, VaR and Positions alert types)
        /// </summary>
        [NotMapped]
        public decimal ThresholdValue
        {
            get => Threshold;
            set
            {
                if (TypeOfAlert != AdmAlertType.Trade_Time)
                {
                    Threshold = value;
                }
            }
        }

        /// <summary>
        /// Get/set escalation value as timespan
        /// </summary>
        [NotMapped]
        public TimeSpan Escalation
        {
            get => TimeSpan.FromMinutes(Convert.ToDouble(EscalationTime));
            set => EscalationTime = Convert.ToInt32(value.TotalMinutes);
        }

        /// <summary>
        /// Get/set level1 escalation value as timespan
        /// </summary>
        [NotMapped]
        public TimeSpan? Level1Escalation
        {
            get
            {
                if (Level1EscalationTime == null)
                {
                    return null;
                }

                return TimeSpan.FromMinutes(Convert.ToDouble(Level1EscalationTime));
            }
            set => Level1EscalationTime = GetEscalationTime(value);
        }

        private int? GetEscalationTime(TimeSpan? delay)
        {
            return (int?)delay?.TotalMinutes;
        }

        /// <summary>
        /// Get/set level2s escalation value as timespan
        /// </summary>
        [NotMapped]
        public TimeSpan? Level2Escalation
        {
            get
            {
                if (Level2EscalationTime == null)
                {
                    return null;
                }

                return TimeSpan.FromMinutes(Convert.ToDouble(Level2EscalationTime));
            }
            set => GetEscalationTime(value);
        }

        /// <summary>
        /// Get/set level3 escalation value as timespan
        /// </summary>
        [NotMapped]
        public TimeSpan? Level3Escalation
        {
            get
            {
                if (Level3EscalationTime == null)
                {
                    return null;
                }

                return TimeSpan.FromMinutes(Convert.ToDouble(Level3EscalationTime));
            }
            set => GetEscalationTime(value);
        }

        /// <summary>
        /// Get/set level4 escalation value as timespan
        /// </summary>
        [NotMapped]
        public TimeSpan? Level4Escalation
        {
            get
            {
                if (Level4EscalationTime == null)
                {
                    return null;
                }

                return TimeSpan.FromMinutes(Convert.ToDouble(Level4EscalationTime));
            }
            set => GetEscalationTime(value);
        }

        /// <summary>
        /// Get readable threshold value
        /// </summary>
        [NotMapped]
        public string ThresholdValueString
        {
            get
            {
                switch (TypeOfAlert)
                {
                    case AdmAlertType.Flat_Price_Position:
                    case AdmAlertType.PnL:
                    case AdmAlertType.VaR:
                        {
                            return ThresholdValue.ToString("F2");
                        }

                    case AdmAlertType.Trade_Time:
                    {
                        return TypeOfBoundary == BoundaryType.InRange
                            ? $"From {TradeTime} to {TradeTimeRangeEnd}"
                            : TradeTime.ToString();
                    }

                    case AdmAlertType.Expiring_Products:
                    {
                        return $"{ThresholdValue:F0} days to expire";
                    }
                }

                return string.Empty;
            }
        }

        [NotMapped]
        public DateTime StartTime
        {
            get => StartTimeDb ?? SystemTime.Today();
            set => StartTimeDb = SystemTime.Today().Add(value.TimeOfDay);
        }

        [NotMapped]
        public bool IsLevel1Active
        {
            get => IsLevel1ActiveDb ?? true;
            set => IsLevel1ActiveDb = value;
        }

        [NotMapped]
        public bool IsLevel2Active
        {
            get => IsLevel2ActiveDb ?? true;
            set => IsLevel2ActiveDb = value;
        }

        [NotMapped]
        public bool IsLevel3Active
        {
            get => IsLevel3ActiveDb ?? true;
            set => IsLevel3ActiveDb = value;
        }

        [NotMapped]
        public bool IsLevel4Active
        {
            get => IsLevel4ActiveDb ?? true;
            set => IsLevel4ActiveDb = value;
        }

        [NotMapped]
        public bool DoNotTriggerOnWeekends
        {
            get => DoNotTriggerOnWeekendsDb ?? false;
            set => DoNotTriggerOnWeekendsDb = value;
        }

        public bool ShouldStartNow()
        {
            return SystemTime.Now().TimeOfDay >= StartTime.TimeOfDay;
        }

        [NotMapped]
        public Dictionary<string, string> CustomProperties
        {
            get
            {
                if (_customProperties == null)
                {
                    if (CustomPropertiesJson != null)
                    {
                        _customProperties =
                            JsonConvert.DeserializeObject<Dictionary<string, string>>(CustomPropertiesJson);
                    }

                    if (_customProperties == null)
                    {
                        _customProperties = new Dictionary<string, string>();
                    }
                }

                return _customProperties;
            }
        }

        public string this[string name]
        {
            get
            {
                CustomProperties.TryGetValue(name, out string value);

                return value;
            }
            set
            {
                if (CustomProperties.ContainsKey(name))
                {
                    CustomProperties.Remove(name);
                }

                CustomProperties.Add(name, value);
                CustomPropertiesJson = JsonConvert.SerializeObject(CustomProperties);
            }
        }

        public static class CustomProperty
        {
            public const string TransferErrorsTypes = "TransferErrorsTypes";
        }

        private const int DefaultAlertId = -1;

        public static readonly AdministrativeAlert Default = new AdministrativeAlert()
        {
            Active = false,
            AlertId = DefaultAlertId,
        };

        public bool IsDefault()
        {
            return DefaultAlertId == AlertId;
        }
    }
}
