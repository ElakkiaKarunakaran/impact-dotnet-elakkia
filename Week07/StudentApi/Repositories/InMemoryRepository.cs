public class InMemoryRepository<T> : IRepository<T> where T : class
{
	private List<T> items = new List<T>();
	private int nextId = 1;

	// We need a way to get ID from any entity
	private Func<T, int> getId;
	private Action<T, int> setId;

	public InMemoryRepository(Func<T, int> getId, Action<T, int> setId)
	{
		this.getId = getId;
		this.setId = setId;
	}

	public IEnumerable<T> GetAll() => items;

	public T GetById(int id)
		=> items.FirstOrDefault(i => getId(i) == id);

	public void Add(T entity)
	{
		setId(entity, nextId++);
		items.Add(entity);
	}

	public void Update(T entity)
	{
		var existing = GetById(getId(entity));
		if (existing != null)
		{
			items.Remove(existing);
			items.Add(entity);
		}
	}

	public void Delete(int id)
	{
		var item = GetById(id);
		if (item != null) items.Remove(item);
	}

	public void Seed(List<T> seedData)
	{
		seedData.ForEach(Add);
	}
}