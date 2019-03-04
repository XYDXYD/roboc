using System.Collections;

namespace Services
{
	internal interface IDataSource<T> where T : class
	{
		IEnumerator GetDataAsync(T data);
	}
}
