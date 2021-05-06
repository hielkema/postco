using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Concurrent;
using System.Linq;

namespace SnomedTemplateService.Core.Domain
{
    public class TemplateData
    {
        private ImmutableList<Author> authors;
        private ImmutableDictionary<string, Item> items;
        private ImmutableList<string> tagIds;
        private MultiLanguageString description;
        private MultiLanguageString title;
        private MultiLanguageString stringFormat;

        public TemplateData(
            string id,
            string timestamp,
            string snomedVersion,
            string snomedBranch,
            string defaultLanguage,
            MultiLanguageString title,
            string etl
            )
        {
            if (string.IsNullOrEmpty(snomedVersion))
            {
                throw new ArgumentException($"'{nameof(snomedVersion)}' cannot be null or empty", nameof(snomedVersion));
            }
            if (string.IsNullOrEmpty(snomedBranch))
            {
                throw new ArgumentException($"'{nameof(snomedBranch)}' cannot be null or empty", nameof(snomedBranch));
            }
            if (string.IsNullOrEmpty(defaultLanguage))
            {
                throw new ArgumentException($"'{nameof(defaultLanguage)}' cannot be null or empty", nameof(defaultLanguage));
            }
            if (string.IsNullOrEmpty(etl))
            {
                throw new ArgumentException($"'{nameof(etl)}' cannot be null or empty", nameof(etl));
            }
            if (title == null)
            {
                throw new ArgumentNullException(nameof(title));
            }
            if (!title.IsTranslatedFor(defaultLanguage))
            {
                throw new ArgumentException($"'{nameof(title)}' should be translated for the default language.");
            }
            Id = id;
            DefaultLanguage = defaultLanguage;
            Title = title;
            TimeStamp = timestamp;
            SnomedVersion = snomedVersion.Trim();
            SnomedBranch = snomedBranch.Trim();
            Etl = etl.Trim();
        }
        public string Id { get; }
        public string TimeStamp { get; }
        public IList<Author> Authors
        {
            get => authors;
            set => authors = value.ToImmutableList() ?? throw new ArgumentNullException(nameof(Authors));
        }

        public string DefaultLanguage
        {
            get;
        }
        public MultiLanguageString Title
        {
            get
            {
                title ??= new MultiLanguageString();
                return title;
            }
            set => title = value switch
            {
                null => throw new ArgumentNullException(nameof(Title)),
                var t => t
            };
        }
        public MultiLanguageString Description
        {
            get
            {
                description ??= new MultiLanguageString();
                return description;
            }
            set => description = value switch
            {
                null => throw new ArgumentNullException(nameof(Description)),
                var d => d
            };
        }
        public string SnomedVersion { get; }
        public string SnomedBranch { get; }
        public MultiLanguageString StringFormat
        {
            get
            {
                stringFormat ??= new MultiLanguageString();
                return stringFormat;
            }
            set => stringFormat = value switch
            {
                null => throw new ArgumentNullException(nameof(StringFormat)),
                var sf => sf
            };
        }
        public string Etl { get; }
        public IList<string> TagIds
        {
            get {
                tagIds ??= ImmutableList.Create<string>();
                return tagIds;
            }
            set => tagIds = value switch
            {
                null => throw new ArgumentNullException(nameof(TagIds)),
                var tagIds => tagIds.Select((tagId, i) => tagId switch
                {
                    null => throw new ArgumentNullException($"{nameof(TagIds)}[{i}]"),
                    var id => id
                }
                ).ToImmutableList()
            };
        }
        public IDictionary<string, Item> ItemData
        {
            get
            {
                items ??= ImmutableDictionary.Create<string, Item>();
                return items;
            }
            set => items = value switch
            {
                null => throw new ArgumentNullException(nameof(ItemData)),
                var items => items.Select(kv => kv.Value switch
                {
                    null => throw new ArgumentNullException($"{nameof(ItemData)}[{kv.Key}]"),
                    var t => kv
                }).ToImmutableDictionary(kv => kv.Key, kv => kv.Value)
            };
        }

        public bool RequiredPropertiesAreTranslated(string lang)
        {
            return !string.IsNullOrWhiteSpace(title[lang]) &&
                items.All(kv => !string.IsNullOrWhiteSpace(kv.Value.Title[lang]));
        }

        public ICollection<string> SupportedLanguages
        {
            get
            {
                return title.LanguagesForWhichStringIsTranslated.Where(
                    l => RequiredPropertiesAreTranslated(l)
                    ).ToList();
            }
        }

        public class Item
        {
            public Item(MultiLanguageString title, MultiLanguageString description)
            {
                Title = title ?? throw new ArgumentNullException(nameof(title));
                Description = description ?? new MultiLanguageString();
            }
            public MultiLanguageString Title { get; }
            public MultiLanguageString Description { get; }
        }

        public class Author
        {
            public string Name { get; }
            public Author(string name)
            {
                
                Name = name ?? throw new ArgumentNullException(nameof(name));
            }
            public string Contact { get; set; }
        }
    }
}
