using Sitecore.Buckets.Managers;
using Sitecore.Buckets.Extensions;
using Sitecore.IO;
using Sitecore.Links;
using Sitecore.Links.UrlBuilders;

namespace Sitecore.Foundation.SitecoreExtensions.Extensions
{
    public class BucketLinkManager : ItemUrlBuilder
    {
        public BucketLinkManager(DefaultItemUrlBuilderOptions defaultOptions) : base(defaultOptions)
        {
        }

        public override string Build(Sitecore.Data.Items.Item item, ItemUrlBuilderOptions options)
        {
            if (BucketManager.IsItemContainedWithinBucket(item))
            {
                var bucketItem = item.GetParentBucketItemOrParent();
                if (bucketItem != null && bucketItem.IsABucket())
                {
                    var bucketUrl = base.Build(bucketItem, options);
                    if ((bool)options.AddAspxExtension)
                    {
                        bucketUrl = bucketUrl.Replace(".aspx", string.Empty);
                    }

                    return FileUtil.MakePath(bucketUrl, item.Name).Replace(" ", "-") +
                            ((bool)options.AddAspxExtension ? ".aspx" : string.Empty);
                }
            }

            return Sitecore.StringUtil.EnsurePostfix('/', base.Build(item, options));
        }

        //public override string GetItemUrl(Sitecore.Data.Items.Item item, ItemUrlBuilderOptions options)
        //{
        //    if (BucketManager.IsItemContainedWithinBucket(item))
        //    {
        //        var bucketItem = item.GetParentBucketItemOrParent();
        //        if (bucketItem != null && bucketItem.IsABucket())
        //        {
        //            var bucketUrl = base.GetItemUrl(bucketItem, options);
        //            if (options.AddAspxExtension)
        //            {
        //                bucketUrl = bucketUrl.Replace(".aspx", string.Empty);
        //            }

        //            return FileUtil.MakePath(bucketUrl, item.Name).Replace(" ", "-") +
        //                    (options.AddAspxExtension ? ".aspx" : string.Empty);
        //        }
        //    }

        //    return Sitecore.StringUtil.EnsurePostfix('/', base.GetItemUrl(item, options));
        //}
    }
}