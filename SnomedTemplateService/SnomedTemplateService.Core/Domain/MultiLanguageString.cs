using System.Collections.Generic;
using System.Collections.Immutable;
using System;
using System.Linq;

public class MultiLanguageString
{
    private readonly ImmutableDictionary<string, string> translations;

    public MultiLanguageString()
    {
    }
    public MultiLanguageString(IDictionary<string, string> translations)
    {
        this.translations = translations.ToImmutableDictionary(StringComparer.InvariantCultureIgnoreCase); 
    }
    public string this[string key]
    {
        get
        {
            string t = null;
            translations?.TryGetValue(key, out t);
            return t;
        }
    }
    
    public IEnumerable<string> LanguagesForWhichStringIsTranslated => 
        translations.Where(kv => !string.IsNullOrWhiteSpace(kv.Value)).Select(kv => kv.Key);

    public bool IsTranslatedFor(string lang)
    {
        return translations.ContainsKey(lang) && !string.IsNullOrWhiteSpace(translations[lang]);
    }
}