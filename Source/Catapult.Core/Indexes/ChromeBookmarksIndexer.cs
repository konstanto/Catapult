﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Catapult.Core.Indexes
{
    public class ChromeBookmarksIndexer
    {
        public BookmarkItem[] GetBookmarkItems()
        {
            string bookmarksFilePath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\AppData\Local\Google\Chrome\User Data\Default\Bookmarks");

            if (!File.Exists(bookmarksFilePath))
            {
                return new BookmarkItem[0];
            }

            var bookmarkCollectionJson = File.ReadAllText(bookmarksFilePath);
            var bookmarkCollection = JsonConvert.DeserializeObject<ChromeBookmarkCollection>(bookmarkCollectionJson);

            ChromeBookmark[] urlBookmarks = bookmarkCollection.Flatten().Where(x => string.Equals(x.Type, "url", StringComparison.InvariantCultureIgnoreCase)).ToArray();

            return urlBookmarks.Select(x => new BookmarkItem(x.Name, x.Url, "Chrome bookmark")).ToArray();
        }

        internal class ChromeBookmarkCollection
        {
            public Dictionary<string, ChromeBookmark> Roots { get; set; }

            public IEnumerable<ChromeBookmark> Flatten()
            {
                foreach (ChromeBookmark child in Roots?.Values.ToArray() ?? new ChromeBookmark[0])
                {
                    foreach (ChromeBookmark bookmark in child.Flatten())
                    {
                        yield return bookmark;
                    }
                }
            }
        }

        internal class ChromeBookmark
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string Url { get; set; }
            public ChromeBookmark[] Children { get; set; }

            public IEnumerable<ChromeBookmark> Flatten()
            {
                yield return this;

                foreach (ChromeBookmark child in Children ?? new ChromeBookmark[0])
                {
                    foreach (ChromeBookmark bookmark in child.Flatten())
                    {
                        yield return bookmark;
                    }
                }
            }
        }
    }
}