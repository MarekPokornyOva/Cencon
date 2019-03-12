#region using
using System.Collections.Generic;
#endregion using

namespace Cencon
{
	//https://msdn.microsoft.com/cs-cz/vstudio/ee672195.aspx
	//https://jacopretorius.net/2010/01/implementing-a-custom-linq-provider.html
	//http://putridparrot.com/blog/creating-a-custom-linq-provider/
	//https://msdn.microsoft.com/en-us/library/bb546158.aspx
	public interface IQueryParameters
	{
		//int Skip { get; }
		string Path { get; }
		IList<string> UrlParameters { get; }
	}
}
