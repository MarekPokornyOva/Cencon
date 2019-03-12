#region using
using System;
#endregion using

namespace Cencon.Core
{
	public class CenconException : Exception
	{
		public CenconException(string message) : base(message)
		{ }
	}
}
