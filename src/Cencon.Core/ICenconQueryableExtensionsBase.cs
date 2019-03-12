#region using
using Cencon.Core;
#endregion using

namespace Cencon
{
	public static class ICenconQueryableExtensionsBase
	{
		public static IQueryable<TItemReader> AssetsAll<TItemReader>(this IQueryable<TItemReader> source)
			=> AssetsPath(source, "/assets/all");

		public static IQueryable<TItemReader> AssetId<TItemReader>(this IQueryable<TItemReader> source, string id)
			=> AssetsPath(source, "/assets/asset/id/" + id);

		public static IQueryable<TItemReader> Asset<TItemReader>(this IQueryable<TItemReader> source)
			=> AssetsPath(source, "/assets/asset");

		static IQueryable<TItemReader> AssetsPath<TItemReader>(this IQueryable<TItemReader> source, string path)
		{
			QueryParameters parms = QueryParameters.CloneFrom(source.Parameters);
			parms.Path = path;
			return source.Provider.CreateQuery(source, parms);
		}

		public static IQueryable<TItemReader> AssetType<TItemReader>(this IQueryable<TItemReader> source, string assetType)
			=> AddParm(source, "censhare:asset.type=" + assetType);

		static IQueryable<TItemReader> AddParm<TItemReader>(this IQueryable<TItemReader> source, string parm)
		{
			QueryParameters parms = QueryParameters.CloneFrom(source.Parameters);
			parms.UrlParameters.Add(parm);
			return source.Provider.CreateQuery(source, parms);
		}
	}
}
