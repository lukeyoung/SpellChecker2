using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SpellChecker2
{
    class WordDictionary
    {
        private Dictionary<string, int> dict;
        private int count;

        public WordDictionary(string filepath)
        {
            FileStream fin = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fin);
            dict = new Dictionary<string, int>();
            count = 0;

            while(!sr.EndOfStream)            
            {
                dict.Add(sr.ReadLine(), count);
                ++count;
            }
        }

        public WordDictionary()
        {
            dict = new Dictionary<string, int>();
            count = 0;
        }

        public bool contains(string word)
        {
            //d.TryGetValue("key", out value)
            return dict.ContainsKey(word.ToLower());
        }

        public void add(string word)
        {
            if (!this.contains(word))
            {
                dict.Add(word, count);
                ++count;
            }
        }

    }
}
