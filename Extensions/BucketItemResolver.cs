using System;
using Sitecore.Pipelines.HttpRequest;
using Sitecore;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using System.Linq;

namespace Sitecore.Foundation.SitecoreExtensions.Extensions
{
    public class BucketItemResolver : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            if (Context.Item != null || Context.Database == null || args.Url.ItemPath.Length == 0)
                return;
            var requestUrl = args.Url.ItemPath.TrimEnd('/');

            var index = requestUrl.LastIndexOf('/');

            var bucketPath = requestUrl.Substring(0, index);
            //bucketPath = MainUtil.DecodeName(bucketPath);
            var bucketItem = args.GetItem(bucketPath);
            if (bucketItem != null)
            {
                var itemName = requestUrl.Substring(index + 1);
                var searchIndex = ContentSearchManager.GetIndex(Sitecore.Configuration.Settings.GetSetting("Sitecore.Foundation.SitecoreExtensions.ArticleIndexName"));
                using (var searchContext = searchIndex.CreateSearchContext())
                {
                    Sitecore.Data.ID bucketTemplateID = new Sitecore.Data.ID(new Guid(Sitecore.Configuration.Settings.GetSetting("Sitecore.Foundation.SitecoreExtensions.ArticleTemplateId")));
                    var result = searchContext.GetQueryable<SearchResultItem>().Where(
                                x => x.TemplateId == bucketTemplateID && (x.Name == itemName || x.Name == itemName.Replace("-", " "))).FirstOrDefault();

                    if (result != null)
                    {
                        var item = result.GetItem();
                        if (item.Language == Context.Language)
                        {
                            Context.Item = result.GetItem();
                        }
                        else
                        {
                            var langItem = Sitecore.Context.Database.GetItem(result.ItemId, Context.Language);
                            if (langItem != null)
                            {
                                Context.Item = langItem;
                            }
                        }
                    }
                }
            }
        }
    }
}