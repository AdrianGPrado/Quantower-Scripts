// Copyright QUANTOWER LLC. © 2017-2023. All rights reserved.

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TradingPlatform.BusinessLayer;

namespace Oscillators;

public sealed class IndicatorRLarryWilliams : Indicator, IWatchlistIndicator
{
    // Displays Input Parameter as input field (or checkbox if value type is bolean).
    [InputParameter("Period", 0, 1, 999, 1, 0)]
    public int Period = 14;

    public int MinHistoryDepths => this.Period;
    public override string ShortName => $"RLW ({this.Period})";
    public override string HelpLink => "https://help.quantower.com/analytics-panels/chart/technical-indicators/oscillators/r-larry-williams";
    public override string SourceCodeLink => "https://github.com/Quantower/Scripts/blob/main/Indicators/IndicatorQstick.cs";

    // Highest 
    private readonly List<double> highest;
    // Lowest 
    private readonly List<double> lowest;

    /// <summary>
    /// Indicator's constructor. Contains general information: name, description, LineSeries etc. 
    /// </summary>
    public IndicatorRLarryWilliams()
        : base()
    {
        this.highest = new List<double>();
        this.lowest = new List<double>();

        // Serves for an identification of related indicators with different parameters.
        this.Name = "%R Larry Williams";
        this.Description = "Uses Stochastic to determine overbought and oversold levels";

        // Defines line on demand with particular parameters.
        this.AddLineSeries("RLW Line", Color.Blue, 1, LineStyle.Solid);
        this.AddLineLevel(-20, "Upper Limit", Color.Red, 1, LineStyle.Solid);
        this.AddLineLevel(-80, "Lower Limit", Color.Red, 1, LineStyle.Solid);

        this.SeparateWindow = true;
    }

    /// <summary>
    /// Calculation entry point. This function is called when a price data updates. 
    /// Will be runing under the HistoricalBar mode during history loading. 
    /// Under NewTick during realtime. 
    /// Under NewBar if start of the new bar is required.
    /// </summary>
    /// <param name="args">Provides data of updating reason and incoming price.</param>
    protected override void OnUpdate(UpdateArgs args)
    {
        if (args.Reason != UpdateReason.NewTick)
        {
            this.highest.Insert(0, 0);
            this.lowest.Insert(0, 0);
        }

        this.highest[0] = this.GetPrice(PriceType.High);
        this.lowest[0] = this.GetPrice(PriceType.Low);

        if (this.Count < this.MinHistoryDepths)
            return;

        double highestPrice = this.highest.Take(this.Period).Max();
        double lowestPrice = this.lowest.Take(this.Period).Min();

        if (highestPrice - lowestPrice > 1e-7)
            this.SetValue(-100 * (highestPrice - this.GetPrice(PriceType.Close)) / (highestPrice - lowestPrice));
    }
}