using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpellChecker2
{
    class Misspelling
    {
        public string orig;
        public string sub;
        public int count;

        public Misspelling(string _orig, string _sub)
        {
            orig = _orig;
            sub = _sub;
            count = 1;
        }

        public Misspelling(string _orig, string _sub, int _count)
        {
            orig = _orig;
            sub = _sub;
            count = _count;
        }

        public void increment()
        {
            ++count;
        }
    }
}
