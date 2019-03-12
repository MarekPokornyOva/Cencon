namespace Cencon.Interception
{
	public interface IQueryInterceptor
	{
		void Querying(QueryingContext context);
		void Queryed(QueryedContext context);
		void Parsed(ParsedContext context);
	}
}
