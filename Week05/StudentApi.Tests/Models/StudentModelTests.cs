using Xunit;

public class StudentModelTests
{
	[Fact]
	public void Age_ValidValue_SetsCorrectly()
	{
		var s = new Student();
		s.Age = 21;
		Assert.Equal(21, s.Age);
	}

	[Fact]
	public void Age_TooLow_ThrowsArgumentException()
	{
		var s = new Student();
		Assert.Throws<ArgumentException>(() => s.Age = 4);
	}

	[Fact]
	public void Age_TooHigh_ThrowsArgumentException()
	{
		var s = new Student();
		Assert.Throws<ArgumentException>(() => s.Age = 101);
	}

	[Theory]
	[InlineData(5)]
	[InlineData(50)]
	[InlineData(100)]
	public void Age_BoundaryValues_Accepted(int age)
	{
		var s = new Student();
		s.Age = age;
		Assert.Equal(age, s.Age);
	}
}