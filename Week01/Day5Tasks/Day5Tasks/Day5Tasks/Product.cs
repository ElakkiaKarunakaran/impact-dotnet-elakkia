public enum Category
{
	Electronics,
	Clothing,
	Food
}

public class Product
{
	public string Name { get; set; }
	public double Price { get; set; }
	public Category Category { get; set; }

	public Product(string name, double price, Category category)
	{
		Name = name;
		Price = price;
		Category = category;
	}
}