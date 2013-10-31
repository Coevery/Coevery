using System;
using System.Web.Mvc;
using Coevery.Core.Feeds.Models;

namespace Coevery.Core.Feeds {
    public interface IFeedBuilder {
        ActionResult Process(FeedContext context, Action populate);

        FeedItem<TItem> AddItem<TItem>(FeedContext context, TItem contentItem);
        void AddProperty(FeedContext context, FeedItem feedItem, string name, string value);
    }
}