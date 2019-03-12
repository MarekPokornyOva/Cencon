namespace System.Collections.Generic
{
	public interface IEnumerableAsync<out T>
	{
		IEnumeratorAsync<T> GetEnumerator();
	}
}
