using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

public class MultiLanguageString
{
    private readonly ImmutableDictionary<string, string> translations;

    public MultiLanguageString()
    {
    }
    public MultiLanguageString(IDictionary<string, string> translations)
    {
        this.translations = translations.ToImmutableDictionary(); 
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
}