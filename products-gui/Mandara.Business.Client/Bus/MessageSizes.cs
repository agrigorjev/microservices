using System.Configuration;

namespace Mandara.Business.Bus
{
    public partial class InformaticaHelper
    {
        private static int _tradesSnapshotPackageSize;

        public static int TradesSnapshotPackageSize
        {
            get
            {
                _tradesSnapshotPackageSize = GetSnapshotPackageSize(
                    _tradesSnapshotPackageSize,
                    "TradeCapturePackageSize",
                    300);

                return _tradesSnapshotPackageSize;
            }
        }

        private static int GetSnapshotPackageSize(int currentValue, string appSettingName, int defaultSize)
        {
            int snapshotPackageSize = currentValue;

            if (currentValue == 0)
            {
                if (!int.TryParse(
                        ConfigurationManager.AppSettings[appSettingName],
                        out snapshotPackageSize))
                {
                    snapshotPackageSize = defaultSize;
                }
            }

            return snapshotPackageSize;
        }

        private static int _fxTradesSnapshotPackageSize;

        public static int FxTradesSnapshotPackageSize
        {
            get
            {
                _fxTradesSnapshotPackageSize = GetSnapshotPackageSize(
                    _fxTradesSnapshotPackageSize,
                    "FxTradePackageSize",
                    300);

                return _fxTradesSnapshotPackageSize;
            }
        }

        private static int _fxExposureSnapshotPackageSize;

        public static int FxExposureSnapshotPackageSize
        {
            get
            {
                _fxExposureSnapshotPackageSize = GetSnapshotPackageSize(
                    _fxExposureSnapshotPackageSize,
                    "FxExposureSnapshotPackageSize",
                    300);

                return _fxExposureSnapshotPackageSize;
            }
        }

        private static int _fxHedgeDetailSnapshotPackageSize;

        public static int FxHedgeDetailSnapshotPackageSize
        {
            get
            {
                _fxHedgeDetailSnapshotPackageSize =
                    GetSnapshotPackageSize(
                        _fxHedgeDetailSnapshotPackageSize,
                        "FxHedgeDetailSnapshotPackageSize",
                        300);

                return _fxHedgeDetailSnapshotPackageSize;
            }
        }

        private static int _dailyReconciliationSnapshotPackageSize;

        public static int DailyReconciliationSnapshotPackageSize
        {
            get
            {
                _dailyReconciliationSnapshotPackageSize =
                    GetSnapshotPackageSize(
                        _dailyReconciliationSnapshotPackageSize,
                        "DailyReconciliationPackageSize",
                        2000);

                return _dailyReconciliationSnapshotPackageSize;
            }
        }

        private static int _positionsSnapshotPackageSize;

        public static int PositionsSnapshotPackageSize
        {
            get
            {
                _positionsSnapshotPackageSize = GetSnapshotPackageSize(
                    _positionsSnapshotPackageSize,
                    "PositionPackageSize",
                    4000);

                return _positionsSnapshotPackageSize;
            }
        }

        private static int _securityDefinitionsSnapshotPackageSize;

        public static int SecurityDefinitionsSnapshotPackageSize
        {
            get
            {
                _securityDefinitionsSnapshotPackageSize =
                    GetSnapshotPackageSize(
                        _securityDefinitionsSnapshotPackageSize,
                        "SecurityDefinitionsSnapshotPackageSize",
                        1000);

                return _securityDefinitionsSnapshotPackageSize;
            }
        }

        private static int _tasTradesSnapshotPackageSize;

        public static int TasTradesSnapshotPackageSize
        {
            get
            {
                _tasTradesSnapshotPackageSize = GetSnapshotPackageSize(
                    _tasTradesSnapshotPackageSize,
                    "TasTradesSnapshotPackageSize",
                    8000);

                return _tasTradesSnapshotPackageSize;
            }
        }

        private static int _tasPricesUpdatePackageSize;

        public static int TasPricesUpdatePackageSize
        {
            get
            {
                _tasPricesUpdatePackageSize = GetSnapshotPackageSize(
                    _tasPricesUpdatePackageSize,
                    "TasPricesUpdatePackageSize",
                    80000);

                return _tasPricesUpdatePackageSize;
            }
        }

        private static int _csvImportPackageSize;

        public static int CsvImportPackageSize
        {
            get
            {
                _csvImportPackageSize = GetSnapshotPackageSize(
                    _csvImportPackageSize,
                    "CsvImportPackageSize",
                    800);

                return _csvImportPackageSize;
            }
        }

        private static int _calcErrorPackageSize;

        public static int CalcErrorPackageSize
        {
            get
            {
                _calcErrorPackageSize = GetSnapshotPackageSize(
                    _calcErrorPackageSize,
                    "CalcErrorPackageSize",
                    100);

                return _calcErrorPackageSize;
            }
        }

        private static int _sourceDataTypeMapPackageSize;

        public static int SourceDataTypeMapPackageSize
        {
            get
            {
                _sourceDataTypeMapPackageSize = GetSnapshotPackageSize(
                    _sourceDataTypeMapPackageSize,
                    "SourceDataTypeMapPackageSize)",
                    500);

                return _sourceDataTypeMapPackageSize;
            }
        }

        private static int _tradesSupportAlertsPackageSize;

        public static int TradesSupportAlertsPackageSize
        {
            get
            {
                _tradesSupportAlertsPackageSize =
                    GetSnapshotPackageSize(
                        _tradesSupportAlertsPackageSize,
                        "TradesSupportAlertsPackageSize",
                        500);

                return _tradesSupportAlertsPackageSize;

            }
        }
    }
}
