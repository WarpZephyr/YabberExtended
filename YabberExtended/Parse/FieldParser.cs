using System.Collections.Generic;

namespace YabberExtended.Parse
{
    internal class FieldParser
    {
        internal string Delimiter { get; set; }
        internal ValueDictionary FieldDictionary { get; set; }

        internal FieldParser(string delimiter = "=")
        {
            Delimiter = delimiter;
            FieldDictionary = [];
        }

        internal void ParseField(string field)
        {
            if (!field.Contains(Delimiter))
            {
                throw new ParseException($"{nameof(Delimiter)} \"{Delimiter}\" could not be found in the Field: {field}");
            }

            string[] strs = field.Split(Delimiter, System.StringSplitOptions.TrimEntries);
            if (strs.Length < 2)
            {
                throw new ParseException($"Could not get both parts of Field: {field}");
            }
            else if (strs.Length > 2)
            {
                throw new ParseException($"Field cannot have more than one value set: {field}");
            }

            string name = strs[0];
            string value = strs[1];
            if (!FieldDictionary.TryAdd(name, value))
            {
                FieldDictionary[name] = value;
            }
        }

        internal void ParseFields(IList<string> fields)
        {
            foreach (string field in fields)
            {
                ParseField(field);
            }
        }
    }
}
