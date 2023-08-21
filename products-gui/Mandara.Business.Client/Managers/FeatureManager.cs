using System;
using System.Collections.Generic;
using System.Linq;
using Mandara.Entities;

namespace Mandara.Business.Client.Managers
{
    public static class FeatureManager
    {
        //features list
        public static string LiveTradesPastPnl = "live_trades_past_pnl";
        public static string OvernightPnlPastPnl = "overnight_pnl_past_pnl";
        public static string DoNotTriggerAlertsOnWeekends = "do_not_trigger_alerts_on_weekends";
        public static string FuturesExpiryTime = "futures_expiry_time";
        public static string InsertedTradesIdsInMessage = "inserted_trades_ids_in_message";
        public static string NewMasterTool = "new_master_tool";
        public static string LiveTradesTimeFilter = "live_trades_time_filter";
        public static string NymexTasPnl = "nymex_tas_pnl";
        public static string DailySwaps = "daily_swaps";
        public static string DeleteDummy = "delete_dummy";
        public static string SpreaderOptimization = "spreader_optimization";
        public static string TreatTimespreadStripAsLegs = "treat_timespread_strip_as_legs";
        public static string GroupQuarters = "group_quarters";
        public static string TransferErrorsAlerts = "transfer_errors_alerts";
        public static string UpdateLegsOnClients = "update_legs_on_clients";
        public static string CategoryOverride = "category_override";
        public static string ExportSourceData = "export_source_data";
        
        // members
        private static IEnumerable<Feature> _features;

        // props
        public static IEnumerable<Feature> Features
        {
            get { return _features; }
        }

        // methods
        public static void Init(IEnumerable<Feature> features)
        {
            if (_features == null)
                _features = features ?? new List<Feature>();
        }

        public static bool IsFeatureEnabled(string featureName)
        {
            if (_features == null)
                return false;

            Feature feature = _features.FirstOrDefault(f => f.Name.Equals(featureName, StringComparison.InvariantCultureIgnoreCase));

            if (feature == null)
            {
                return false;
            }

            return feature.IsEnabled;
        }
    }
}