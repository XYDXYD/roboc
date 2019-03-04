using System;

[Serializable]
public struct HashInfo
{
	public string hash;

	public int fileSize;

	public HashInfo(string hsh, int size)
	{
		hash = hsh;
		fileSize = size;
	}
}
