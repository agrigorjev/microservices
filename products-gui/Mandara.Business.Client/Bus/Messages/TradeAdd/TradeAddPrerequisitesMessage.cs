using AutoMapper;
using JetBrains.Annotations;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Business.Bus.Messages.EditTrades;
using Mandara.Business.Bus.Messages.Historical;
using Mandara.Business.Bus.Messages.Portfolio;
using Mandara.Business.TradeAdd;
using Mandara.Entities;
using Mandara.Entities.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Mandara.Business.Bus.Messages.TradeAdd
{
    public class TradeAddPrerequisitesRequestMessage : MessageBase
    {
        public int UserId { get; set; }
        public int TradeCaptureId { get; set; }
        public bool IsDuplicateMode { get; set; }
        public bool IsMasterToolMode { get; set; }
    }

    /*
    public class TradeAddPrerequisitesRequestMessageDto : MessageBase
    {
        public int UserId { get; set; }
        public int TradeCaptureId { get; set; }
        public bool IsDuplicateMode { get; set; }
        public bool IsMasterToolMode { get; set; }
    }
    */

    public class TradeAddPrerequisitesResponseMessage : MessageBase, INotifyPropertyChanged
    {
        public List<Unit> Units { get; set; }
        public List<Instrument> Instruments { get; set; }
        public List<Entities.Portfolio> Portfolios { get; set; }
        public List<Exchange> Exchanges { get; set; }
        public List<CompanyAlias> Brokers { get; set; }
        public Entities.Portfolio DefaultUserPortfolio { get; set; }

        private List<Instrument> _availableInstrumentsForExchange;
        private List<string> _expiryExchanges;

        public List<Instrument> AvailableInstrumentsForExchange
        {
            get { return _availableInstrumentsForExchange; }
            set
            {
                if (_availableInstrumentsForExchange == null && value == null)
                    return;

                bool changed = _availableInstrumentsForExchange == null || value == null;

                if (!changed)
                {
                    changed = _availableInstrumentsForExchange.Count != value.Count;

                    for (int i = 0; i < _availableInstrumentsForExchange.Count && !changed; i++)
                    {
                        Instrument oldInstrument = _availableInstrumentsForExchange[i];
                        Instrument newInstrument = value[i];

                        changed = oldInstrument.Id != newInstrument.Id;
                    }
                }

                _availableInstrumentsForExchange = value;
                
                if (changed)
                    OnPropertyChanged(nameof(AvailableInstrumentsForExchange));
            }
        }

        public List<string> ExpiryExchanges
        {
            get { return _expiryExchanges; }
            set
            {
                if (_expiryExchanges == null && value == null)
                    return;

                bool changed = _expiryExchanges == null || value == null;

                if (!changed)
                {
                    changed = _expiryExchanges.Count != value.Count;

                    for (int i = 0; i < _expiryExchanges.Count && !changed; i++)
                    {
                        string oldExpiryExchange = _expiryExchanges[i];
                        string newExpiryExchange = value[i];

                        changed = oldExpiryExchange != newExpiryExchange;
                    }
                }

                _expiryExchanges = value;

                if (changed)
                    OnPropertyChanged("ExpiryExchanges");
            }
        }

        public List<TradeTemplate> TradeTemplates { get; set; }
        public TradeAddDetails TradeAddDetails { get; set; }
        public string ErrorMessage { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class TradeAddPrerequisitesResponseMessageDto : MessageBase
    {
        public List<Unit> Units { get; set; }
        public List<Instrument> Instruments { get; set; }
        public List<PortfolioDto> Portfolios { get; set; }
        public List<ExchangeDto> Exchanges { get; set; }
        public List<CompanyAliasDto> Brokers { get; set; }
        public PortfolioDto DefaultUserPortfolio { get; set; }
        public List<TradeTemplateDto> TradeTemplates { get; set; }
        public TradeAddDetailsDto TradeAddDetails { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class TradeTemplateDto
    {
        public string TemplateName { get; set; }
        public ExchangeDto Exchange { get; set; }
        public PortfolioDto Portfolio { get; set; }
        public OfficialProductDto OfficialProduct { get; set; }
        public decimal Volume { get; set; }
        public Unit Unit { get; set; }
    }

    [Serializable]
    public class Instrument
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> AvailableUnits { get; set; }
        public List<int> BalmoUnits { get; set; }
        public List<string> Exchanges { get; set; }
        public bool HasBalmo { get; set; }
        public bool HasFutures { get; set; }
        public Dictionary<string, ExchangeUnits> ExchangeUnits { get; set; }
        public List<string> ExpiryExchanges { get; set; }
        public bool HasNonTas { get; set; }
        public bool HasTas { get; set; }
        public bool HasMops { get; set; }
        public bool HasMm { get; set; }
        public bool HasMoc { get; set; }
        public bool HasDailySwaps { get; set; }
        public bool HasDailyDiffs { get; set; }
        public Dictionary<string, List<int>> DailySwapUnits { get; set; }
        public Dictionary<string, List<int>> DailyDiffUnits { get; set; }

        public List<string> FxTradesExchanges { get; set; }

        public string FxSpecifiedCurrency { get; set; }

        public string FxAgainstCurrency { get; set; }

        public string Currency { get; set; }
        public bool IsCalcPnlFromLegs { get; set; }

        public override bool Equals(object obj)
        {
            Instrument anotherInstrument = obj as Instrument;

            if (anotherInstrument == null)
                return false;

            return Id == anotherInstrument.Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    [Serializable]
    public class ExchangeUnits
    {
        public List<int> AvailableUnits { get; set; }
        public List<int> BalmoUnits { get; set; }
        public bool HasBalmo { get; set; }
    }
}