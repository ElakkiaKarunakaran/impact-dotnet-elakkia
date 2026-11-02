public interface IGradeStrategy
{
	string Calculate(double score);
}

public class PercentageStrategy : IGradeStrategy
{
	public string Calculate(double score)
		=> $"{score}% — {(score >= 50 ? "Pass" : "Fail")}";
}

public class GpaStrategy : IGradeStrategy
{
	public string Calculate(double score)
	{
		double gpa = score / 25.0; // convert to 4.0 scale
		return $"GPA: {gpa:F1}";
	}
}

public class GradeStrategyFactory
{
	public static IGradeStrategy GetStrategy(string type)
	{
		return type?.ToLower() switch
		{
			"gpa" => new GpaStrategy(),
			"percentage" => new PercentageStrategy(),
			_ => new PercentageStrategy() // default
		};
	}
}