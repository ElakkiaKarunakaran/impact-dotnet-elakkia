using Xunit;

public class GradeStrategyTests
{
	// ── PercentageStrategy ────────────────────────
	[Fact]
	public void Percentage_PassingScore_ContainsPass()
	{
		var strategy = new PercentageStrategy();
		var result = strategy.Calculate(75);
		Assert.Contains("Pass", result);
		Assert.Contains("75", result);
	}

	[Fact]
	public void Percentage_FailingScore_ContainsFail()
	{
		var strategy = new PercentageStrategy();
		var result = strategy.Calculate(40);
		Assert.Contains("Fail", result);
	}

	[Fact]
	public void Percentage_BoundaryScore_IsPass()
	{
		var strategy = new PercentageStrategy();
		var result = strategy.Calculate(50); // exactly 50 = pass
		Assert.Contains("Pass", result);
	}

	// ── GpaStrategy ──────────────────────────────
	[Fact]
	public void Gpa_Score100_Returns4Point0()
	{
		var strategy = new GpaStrategy();
		var result = strategy.Calculate(100);
		Assert.Contains("4.0", result);
	}

	[Fact]
	public void Gpa_Score50_Returns2Point0()
	{
		var strategy = new GpaStrategy();
		var result = strategy.Calculate(50);
		Assert.Contains("2.0", result);
	}

	[Fact]
	public void Gpa_ResultContainsGpa()
	{
		var strategy = new GpaStrategy();
		var result = strategy.Calculate(75);
		Assert.Contains("GPA", result);
	}

	// ── Factory selection ─────────────────────────
	[Fact]
	public void Factory_Percentage_ReturnsPercentageStrategy()
	{
		var strategy = GradeStrategyFactory.GetStrategy("percentage");
		Assert.IsType<PercentageStrategy>(strategy);
	}

	[Fact]
	public void Factory_Gpa_ReturnsGpaStrategy()
	{
		var strategy = GradeStrategyFactory.GetStrategy("gpa");
		Assert.IsType<GpaStrategy>(strategy);
	}

	[Fact]
	public void Factory_Unknown_ReturnsDefaultPercentage()
	{
		var strategy = GradeStrategyFactory.GetStrategy("unknown");
		Assert.IsType<PercentageStrategy>(strategy); // default
	}

	[Fact]
	public void Factory_Null_ReturnsDefaultPercentage()
	{
		var strategy = GradeStrategyFactory.GetStrategy(null);
		Assert.IsType<PercentageStrategy>(strategy);
	}

	[Fact]
	public void Factory_CaseInsensitive_Works()
	{
		var strategy = GradeStrategyFactory.GetStrategy("GPA");
		Assert.IsType<GpaStrategy>(strategy);
	}
}