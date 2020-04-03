using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aino
{
    public class KeyNameListConfig
    {
        private Dictionary<string, string> entries = new Dictionary<string, string>();

        /**
         * Returns value based on key.
         *
         * @param key key to search for
         * @return value corresponding to key
         */
        public string GetEntry(string key)
        {
            if (entries.TryGetValue(key, out string resultvalue))
            {
                return resultvalue;
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }

        /**
         * Adds key-value pair to this object.
         * @param key key
         * @param value value
         */
        public void AddEntry(String key, String value)
        {

            if (entries.ContainsKey(key))
            {
                throw new ArgumentException("Key already exists!");
            }
            else
            {
                entries.Add(key, value);
            }
        }

        /**
         * Batch add multiple key-value pairs from Map.
         *
         * @param operationsMap Map containing key-value pairs to be added.
         */
        public void addEntries(Dictionary<string, string> operationsMap)
        {
            foreach (string omkey in operationsMap.Keys)
            {
                if (entries.ContainsKey(omkey))
                {
                    entries[omkey] = operationsMap[omkey];
                }
                else
                {
                    entries.Add(omkey, operationsMap[omkey]);
                }
            }
        }

        /**
         * Checks if key exists.
         *
         * @param key key to check
         * @return true if key was found
         */
        public bool EntryExists(string key)
        {
            return entries.ContainsKey(key);
        }

        /**
         * Checks if value exists.
         *
         * @param name value to check
         * @return true if value was found
         */
        public bool nameExists(string name)
        {
            return entries.ContainsValue(name);
        }
    }
}