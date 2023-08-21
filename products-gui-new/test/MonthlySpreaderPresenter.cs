using Mandara.Business.Bus.Messages.Spreader;
using Mandara.Common.Settings;
using Mandara.Date;
using Mandara.Date.Time;
using Mandara.Entities;
using Mandara.Entities.Calculation;
using Mandara.RiskMgmtTool.Infrastructure;
using Mandara.RiskMgmtTool.Spreader.Common;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;

namespace Mandara.RiskMgmtTool.Spreader.MonthlySpreader
{
    internal class MonthlySpreaderPresenter : SpreaderPresenterBase
    {
		private const int YearsInFuture = 4;

		private readonly string[] _monthSpreadsMap =
		{
			"fg", "gh", "hj", "jk", "km", "mn", "nq", "qu", "uv", "vx", "xz", "zf"
		};

		private readonly ILogger _logger = new NLogLogger(typeof(MonthlySpreaderPresenter));

		public IMonthlySpreaderView View => (SpreaderViewBase as IMonthlySpreaderView);

		public BindingList<SimulatedPosition> InputPositions { get; set; } = new BindingList<SimulatedPosition>();

		public BindingList<MonthlySpread> CalculatedSpreads { get; set; } = new BindingList<MonthlySpread>();

		public MonthlySpreaderPresenter(
			IObservableBusClient busClient,
			IMessageBoxDisplay messageBoxDisplay,
			IMonthlySpreaderView view,
			SettingsHelper parent,
			ISchedulerProvider schedulerProvider) : base(busClient, messageBoxDisplay, view, parent, schedulerProvider)
		{

		}

        protected override string GetSpreaderTitle(ISpreaderViewBase view)
        {
            return view.FormId.Replace("Monthly", string.Empty).Replace("Form", " ");
		}

		protected override void UpdateUserSettings(SpreaderSettings settings)
		{
			var firstOrDefault = UserSettings.Instance.SpreaderSettings.FirstOrDefault(x => x.Index == settings.Index);
			if (firstOrDefault != null)
			{
				UserSettings.Instance.SpreaderSettings.Remove(firstOrDefault);
			}

			UserSettings.Instance.SpreaderSettings.Add(settings);
		}

		protected override SpreaderSettings GetSettingsOrDefault()
		{
			return UserSettings.Instance.SpreaderSettings.FirstOrDefault(
					   spreaderSettings => spreaderSettings.Index == (int)View.Tag)
				   ?? base.GetSettingsOrDefault();
		}

		protected override void RefreshGridView()
		{
			base.RefreshGridView();
			RefreshInputGridView();
			RefreshOutputGridView();
		}

		private void RefreshInputGridView()
		{
			List<SimulatedPosition> inputTemplate = GetInputGridTemplate(YearsInFuture);
			InputPositions = new BindingList<SimulatedPosition>(inputTemplate);
			View.SetInputGrid(InputPositions);
		}

		private List<SimulatedPosition> GetInputGridTemplate(int yearsInFuture)
		{
			return MasterTradeEditForm.GenerateStrips(yearsInFuture)
				.Select(strip => new SimulatedPosition { Strip = strip })
				.ToList();
		}

		protected override void RefreshOutputGridView()
		{
			List<MonthlySpread> outputTemplate = GetOutputGridTemplate(YearsInFuture);
			CalculatedSpreads = new BindingList<MonthlySpread>(outputTemplate);
			View.SetCalculatedGrid(CalculatedSpreads);
		}

		private List<MonthlySpread> GetOutputGridTemplate(int yearsInFuture)
		{
			return MasterTradeEditForm.GenerateMonths(yearsInFuture)
				.Select(month =>
					new MonthlySpread
					{
						Month = month,
						Spreads = _monthSpreadsMap[month.DateValue.Month - 1]
					})
				.ToList();
		}

		protected override void Load(List<Portfolio> portfolios, List<ProductCategory> productCategories, ICollection<CalculationDetail> positions)
		{
			SetupLocalSpreaderCalculationSubscription();
			SetupServerSpreaderCalculationSubscription();
			base.Load(portfolios, productCategories, positions);
		}

		protected override IEnumerable<Product> FilterAvailableSimProducts(IEnumerable<Product> inputProducts)
		{
			return inputProducts.Where(x => x.Type != ProductType.DailySwap);
		}

		private void SetupServerSpreaderCalculationSubscription()
        {
			_subscriptions.Add(
				ServerSpreadCalculationRequests
					.Sample(ThrottlingPeriod, _schedulerProvider.ThreadPool)
					.WithLatestFrom(BusClient.Positions, (cmd, pos) => (command: cmd, allPositions: pos))
					.Do(data =>
					{
						_logger.Trace("{0}->{1}: Sequence fired with {2} positions because {3}",
							View.FormId,
							nameof(ServerSpreadCalculationRequests),
							data.allPositions.Count,
							data.command.Source);
					})
					.Select(data =>
					{
						List<CalculationDetail> filteredPositions = GetPositionsForSelections(
							SelectedPortfolio,
							PositionProducts,
							data.allPositions.ToList());

						return (data.command, positions: filteredPositions);
					})
					.SelectMany(data =>
					{
						return Observable.Return(data)
							.Zip(
								ObservableServerCalculation(SelectedSimulationProduct,
									InternalTime.LocalToday().FirstDayOfMonth(),
									GetEndDate(data.positions)),
								(reqAndPos, calc) =>
									(reqAndPos.command, reqAndPos.positions, serverSpreadsOutput: calc));
					})
					.Do(data =>
					{
						_logger.Trace("{0}->{1}: Server Calc returned with {2} filtered because {3}",
							View.FormId,
							nameof(ServerSpreadCalculationRequests),
							data.positions.Count,
							data.command.Source);
					})
					.ObserveOn(_schedulerProvider.UI)
					.Subscribe(data =>
					{
						_logger.Trace("{0}: Update Spreads because {1}", View.FormId, data.command.Source);

						SpreaderCalculations.UpdateFuturesEquivalent(data.serverSpreadsOutput, CalculatedSpreads);
						SpreaderCalculations.UpdateSpreads(
							RecalculateLocallyForObservable(data.positions), CalculatedSpreads);
						View.UpdateView(null);
					},
						error => OnError(error, "Calculation Request Failure")));
		}

		private DateTime GetEndDate(List<CalculationDetail> positions)
		{
			int yearsInFuture = YearsInFuture;
			DateTime maxPositionDate = DateTime.MinValue;
			if (positions.Count == 0)
			{
				return maxPositionDate;
			}

			maxPositionDate = positions.Max(p => p.CalculationDate);


			DateTime maxGridDate = SystemTime.Now().AddYears(yearsInFuture - 1).FirstDayOfMonth();

			while (maxPositionDate > maxGridDate)
			{
				yearsInFuture++;
				maxGridDate = maxGridDate.AddYears(1);
			}

			return maxGridDate;
		}

		private IObservable<List<SpreaderOutput>> ObservableServerCalculation(
			Product product,
			DateTime startDate,
			DateTime endDate)
		{
			return TaskObservableExtensions.ToObservable(RequestServerCalculation(product, startDate, endDate))
				.Select(msg => msg != null ? msg.Output : new List<SpreaderOutput>());
		}

		private Task<SpreaderResponseMessage> RequestServerCalculation(
			Product product,
			DateTime startDate,
			DateTime endDate)
		{
			List<SpreaderInput> manualInput = GetPositionsFromManualInput();

			return BusClient.CalculateSpreader(manualInput, product.ProductId, startDate, endDate);
		}

		private List<SpreaderInput> GetPositionsFromManualInput()
		{
			return InputPositions
				.Where(position => position.FuturesAmount.HasValue || position.SwapAmount.HasValue)
				.Select(CreateSpreaderConfig)
				.ToList();

			SpreaderInput CreateSpreaderConfig(SimulatedPosition position)
			{
				return new SpreaderInput
				{
					Month = position.Strip.DateValue,
					DateType = GetStripDateType(position),
					FuturesAmount = position.FuturesAmount,
					SwapAmount = position.SwapAmount
				};
			}

			ProductDateType GetStripDateType(SimulatedPosition position)
			{
				return position.Strip.StringValue.Contains("Q") ? ProductDateType.Quarter :
					position.Strip.StringValue.Contains("Cal") ? ProductDateType.Year : ProductDateType.MonthYear;
			}
		}

		private void SetupLocalSpreaderCalculationSubscription()
		{
			_subscriptions.Add(
				LocalSpreadCalculationRequests
					.Sample(ThrottlingPeriod)
					.WithLatestFrom(BusClient.Positions, (cmd, pos) => (command: cmd, allPositions: pos))
					.Select(data =>
					{
						List<CalculationDetail> pos = GetPositionsForSelections(SelectedPortfolio,
							PositionProducts,
							data.allPositions.ToList());
						return (data.command, positions: pos);
					})
					.ObserveOn(_schedulerProvider.UI)
					.Subscribe(data =>
					{
						_logger.Trace("{0}: Update Spreads because {1}", View.FormId, data.command.Source);
						SpreaderCalculations.UpdateSpreads(RecalculateLocallyForObservable(data.positions),
							CalculatedSpreads);
						View.UpdateView(null);
					},
						error => OnError(error, "Spreader Calculation Failure")));
		}

		private List<SpreaderOutput> RecalculateLocallyForObservable(List<CalculationDetail> positions)
		{
			List<SpreaderInput> input = GetInputFromPositions(positions);

			return CalculateSpreads(input);
		}


		private List<SpreaderOutput> CalculateSpreads(List<SpreaderInput> positionsInput)
		{
			Func<List<SpreaderInput>, List<MonthlySpread>, int, List<SpreaderOutput>> spreadCalculator =
				IncludeSimulatedPositions
					? SpreaderCalculations.GetSpreadsWithSimulatedPositions
					: (Func<List<SpreaderInput>, List<MonthlySpread>, int, List<SpreaderOutput>>)SpreaderCalculations
						.GetSpreads;

			return spreadCalculator(positionsInput, CalculatedSpreads.ToList(), View.ExitRow);
		}

		private List<SpreaderInput> GetInputFromPositions(List<CalculationDetail> positions)
		{
			List<MonthlySpread> outputTemplate = GetOutputGridTemplate(YearsInFuture);

			return outputTemplate.Select(
					spread =>
					{
						CalculationDetail detail =
							positions.FirstOrDefault(
								position => position.CalculationDate == spread.Month.DateValue.Date);

						return new SpreaderInput
						{
							Month = spread.Month.DateValue,
							DateType = ProductDateType.MonthYear,
							FuturesAmount = (detail?.Amount / 1000)
						};
					})
				.ToList();
		}

		private List<CalculationDetail> GetPositionsForSelections(
			Portfolio portfolio,
			Dictionary<int, ProductOrCategory> selectableProducts,
			List<CalculationDetail> positions)
		{
			return AggregatePositionsWithoutCategory(
					GetPositionsForPortfolio(positions.ToList(), portfolio)
						.Where(position =>
							selectableProducts.TryGetValue(
								position.ProductId.GetProductIdForTreeList(),
								out var selectablePoc) &&
							selectablePoc.Checked))
				.ToList();
		}

		private IEnumerable<CalculationDetail> AggregatePositionsWithoutCategory(
			IEnumerable<CalculationDetail> positions)
		{
			return from position in positions
				   group position by
					   new
					   {
						   position.CalculationDate.Year,
						   position.CalculationDate.Month,
					   }
				   into g
				   select new CalculationDetail
				   {
					   DetailId = Guid.NewGuid(),

					   ProductId = g.First().ProductId,
					   SourceProductId = g.First().SourceProductId,

					   CalculationDate = new DateTime(g.Key.Year, g.Key.Month, 1),

					   Amount = g.Sum(x => x.Amount),
					   AmountInner = g.Sum(x => x.AmountInner),
				   };
		}
	}
}
