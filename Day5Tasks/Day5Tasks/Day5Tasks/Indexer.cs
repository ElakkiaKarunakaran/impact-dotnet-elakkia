public class Playlist
{
	private string[] songs = new string[10];
	private int count = 0;

	public void AddSong(string song)
	{
		if (count < songs.Length)
			songs[count++] = song;
	}

	// int indexer — access by position number
	public string this[int index]
	{
		get
		{
			if (index < 0 || index >= count)
				throw new IndexOutOfRangeException($"Index {index} is out of range.");
			return songs[index];
		}
		set
		{
			if (index < 0 || index >= count)
				throw new IndexOutOfRangeException($"Index {index} is out of range.");
			songs[index] = value;
		}
	}

	// string indexer — search by song name, return its position
	public int this[string songName]
	{
		get
		{
			for (int i = 0; i < count; i++)
				if (songs[i] == songName) return i;
			return -1; // not found
		}
	}
}