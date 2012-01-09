using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SpellChecker2
{
    class WordChecker
    {
        private WordDictionary real_words; //real_words is a dictionary of all known words
        private WordDictionary fake_words; //fake_words is a list of known misspelled words
        private List<Misspelling> subs1; //subs1 is a list of substitutions that turn a misspelled word into a correctly spelled one

        public WordChecker()
        {
            real_words = new WordDictionary("../../Lists/2of12.txt");
            fake_words = new WordDictionary();
            subs1 = new List<Misspelling>();

            //Console.WriteLine(subs1.Exists(delegate(Misspelling x) { return x.orig == "o" && x.sub == "ou"; }));

        }

        //Attempts all substitutions on a misspelled word to turn it into a correct word. Calls recursiveCheck words to perform up to three 
        //substitutions on a word.
        public void checkWord(string word)
        {
            word = "(" + word + ")";
            List<Misspelling> possibleSubs = new List<Misspelling>();
            HashSet<string> found = new HashSet<string>();
            

            foreach (Misspelling s in subs1)
            {
                if (word.Contains(s.orig))
                {
                    if(s.orig != s.sub)
                        possibleSubs.Add(s);
                }
            }

            int index = 0;
            foreach (Misspelling s in possibleSubs)
            {
                string replacement = word.Replace(s.orig, s.sub);
                if (real_words.contains(replacement.Substring(1, replacement.Length - 2)))
                    //Console.WriteLine("Found " + replacement.Substring(1, replacement.Length - 2) + " at a depth of 0");
                    found.Add(convertToString((word.Replace(s.orig, (s.orig.Substring(0, 1) + "/" + index + "/" + s.orig.Substring(s.orig.Length - 1, 1)))), possibleSubs));
                else
                {
                    string temp = word.Replace(s.orig, (s.orig.Substring(0, 1) + "/" + index + "/" + s.orig.Substring(s.orig.Length - 1, 1)));
                    recursiveCheckWord(temp, 1, possibleSubs, ref found);
                }
                ++index;
            }
            List<string> found2 = found.ToList();
            found2.Sort();

            foreach(string s in found2)
            {
                Console.WriteLine(s);
            }
            Console.WriteLine("Finished Checking");

        }

        private void recursiveCheckWord(string word, int depth, List<Misspelling> possibleSubs, ref HashSet<string> found)
        {
            if (depth >= 3)
                return;
            else
            {
                int index = 0;
                foreach (Misspelling s in possibleSubs)
                {
                    if (word.Contains(s.orig))
                    {
                        string from = s.orig;
                        string to = s.sub;
                        string replacement = word.Replace(s.orig, s.sub);
                        if (real_words.contains(convertToString(replacement,possibleSubs)))
//                            Console.WriteLine("Found " + convertToString(replacement, possibleSubs) + " at a depth of " + depth + ".     " + word + ", " + from + "->" + to);
                            found.Add(convertToString((word.Replace(s.orig, (s.orig.Substring(0, 1) + "/" + index + "/" + s.orig.Substring(s.orig.Length - 1, 1)))), possibleSubs));
                        else
                        {
                            string temp = word.Replace(s.orig, (s.orig.Substring(0, 1) + "/" + index + "/" + s.orig.Substring(s.orig.Length - 1, 1)));
                            recursiveCheckWord(temp, depth+1, possibleSubs, ref found);
                        }
                        //Console.WriteLine(e.orig + " -> " + e.sub);
                    }
                    ++index;
                }
            }
        }

        private string convertToString(string word, List<Misspelling> possibleSubs)
        {
            while (word.Contains('/'))
            {
                int index1 = word.IndexOf('/');
                int length = 0;
                for (int i = index1 + 1; word[i] != '/'; ++i)
                {
                    ++length;
                }
                int index2 = Convert.ToInt32(word.Substring(index1+1,length));
                word = word.Replace("/" + index2 + "/", possibleSubs[index2].sub.Substring(1, possibleSubs[index2].sub.Length - 2));
            }
            return word.Substring(1,word.Length-2);
        }

        //The purpose of this method is to open a text file and parse through a list of commonly misspelled
        //finding a substitution for earch misspelled word that turns it into the correct word.
        public void CompileErrata(string filepath, int method)
        {
            FileStream fin = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fin);

            while (!sr.EndOfStream)
            {
                if (method == 1)
                {
                    string[] lineparse = sr.ReadLine().ToLower().Split('(');
                    lineparse[1] = lineparse[1].Replace(')', ' ');
                    if (lineparse[1].Contains(';'))
                        lineparse[1] = lineparse[1].Remove(lineparse[1].IndexOf(';'));
                    if (lineparse[1].Contains('[') && lineparse[1].Contains(']'))
                    {
                        int from = lineparse[1].IndexOf('[');
                        int to = lineparse[1].IndexOf(']');
                        lineparse[1] = lineparse[1].Remove(from, (to - from));
                    }
                    lineparse[1] = lineparse[1].Replace("british", " ");
                    lineparse[1] = lineparse[1].Replace("american", " ");
                    lineparse[1] = lineparse[1].Replace("acceptable variant of", " ");
                    lineparse[1] = lineparse[1].Replace("variant of", " ");
                    lineparse[0] = lineparse[0].Trim();
                    lineparse[1] = lineparse[1].Trim();

                    if (!fake_words.contains(lineparse[0]))
                    {
                        //Console.Write(lineparse[0] + "\t");
                        string[] correct = lineparse[1].Split(',');
                        for (int i = 0; i < correct.Length; ++i)
                        {
                            if (real_words.contains(correct[i].Trim()))
                            {
                                //Console.Write(correct[i] + "\t");
                                //this.createErrata(lineparse[0], correct[i].Trim());
                                this.createErrata(","+lineparse[0]+",",","+correct[i].Trim()+",");
                            }
                        }
                        fake_words.add(lineparse[0]);
                        //Console.Write("\n");
                    }
                }
                if (method == 2)
                {
                    string[] lineparse = sr.ReadLine().ToLower().Split(':');
                    lineparse[0] = lineparse[0].Trim();
                    lineparse[1] = lineparse[1].Trim();
                    if (real_words.contains(lineparse[0].Trim()))
                    {
                        //Console.Write(lineparse[0] + "\t");
                        string[] incorrect = lineparse[1].Split(',');
                        for (int i = 0; i < incorrect.Length; ++i)
                        {
                            if (!fake_words.contains(incorrect[i]))
                            {
                                //Console.Write(incorrect[i] + "\t");
                                //this.createErrata(incorrect[i].Trim(), lineparse[0]);
                                this.createErrata("("+incorrect[i].Trim()+")", "("+lineparse[0]+")");
                            }
                        }
                        fake_words.add(lineparse[0]);
                        //Console.Write("\n");
                    }

                }
            }

            subs1.Sort(delegate(Misspelling m1, Misspelling m2)
            {
                return m2.count - m1.count;
            });
        }


        //Outputs known substitutions to a text file in the root called output.txt
        public void createList()
        {
            FileStream fin = new FileStream("../../output.txt", FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fin);

            while (!sr.EndOfStream)
            {
                string[] line = sr.ReadLine().Split(' ');

                if (line.Length == 3)
                    subs1.Add(new Misspelling(line[0], line[1], Int32.Parse(line[2])));
            }
            sr.Close();

            //outputErrata();
            return;
        }

        //looks at two words and finds a substitution that will turn one
        private void createErrata(string word1, string word2)
        {
            string longer, shorter, sub1, sub2;
            int left, right;
            bool switched = false;
            if (word2.Length > word1.Length)
            {
                longer = word2;
                shorter = word1;
                switched = true;
            }
            else
            {
                longer = word1;
                shorter = word2;
            }
            left = 0;

            for (int i = 0; i < shorter.Length; ++i)
            {
                if (longer[i] != shorter[i])
                {
                    left = i-1;
                    break;
                }
            }
            right = 0;
            for (int i = 0; i < shorter.Length - left; ++i)
            {
                right = i - 1;
                if (longer[longer.Length - i - 1] != shorter[shorter.Length - i - 1])
                {
                    right = i-1;
                    break;
                }
            }
            int method = 1;
            if (method == 1)
            {
                sub1 = shorter.Substring(left, shorter.Length - (left + right));
                sub2 = longer.Substring(left, longer.Length - (left + right));
                if (switched)
                    addSubstitute(sub1, sub2);
                else
                    addSubstitute(sub2, sub1);
            }
            else if (method == 2)
            {
                sub1 = shorter.Substring(left, shorter.Length - (left + right) - 1);
                sub2 = longer.Substring(left, longer.Length - (left + right) - 1);
                if (switched)
                    addSubstitute(sub1, sub2);
                else
                    addSubstitute(sub2, sub1);

                sub1 = shorter.Substring(left + 1, shorter.Length - (left + right) - 1);
                sub2 = longer.Substring(left + 1, longer.Length - (left + right) - 1);
                if (switched)
                    addSubstitute(sub1, sub2);
                else
                    addSubstitute(sub2, sub1);
            }
            else
            {
                sub1 = shorter.Substring(left+1, shorter.Length - (left + right)-2);
                sub2 = longer.Substring(left+1, longer.Length - (left + right)-2);
                if (switched)
                    addSubstitute(sub1, sub2);
                else
                    addSubstitute(sub2, sub1);
            }


            return;
        }

        private void createErrataShort(string word1, string word2)
        {
            string longer, shorter, sub1, sub2;
            int left, right;
            bool switched = false;
            if (word2.Length > word1.Length)
            {
                longer = word2;
                shorter = word1;
                switched = true;
            }
            else
            {
                longer = word1;
                shorter = word2;
            }
            left = 0;
            for (int i = 0; i < shorter.Length; ++i)
            {
                left = i;
                if (longer[i] != shorter[i])
                {
                    break;
                }
            }
            right = 0;
            for (int i = 0; i < shorter.Length - left; ++i)
            {
                right = i;
                if (longer[longer.Length - i - 1] != shorter[shorter.Length - i - 1])
                {
                    break;
                }
            }



            if (left + right == shorter.Length)
            {
                if (left != 0)
                {
                    sub1 = shorter.Substring(left - 1, shorter.Length - (left + right) + 1);
                    sub2 = longer.Substring(left - 1, longer.Length - (left + right) + 1);
                    if (switched)
                        addSubstitute(sub1, sub2);
                    else
                        addSubstitute(sub2, sub1);
                }

                if (right != 0)
                {
                    sub1 = shorter.Substring(left, shorter.Length - (left + right) + 1);
                    sub2 = longer.Substring(left, longer.Length - (left + right) + 1);
                    if (switched)
                        addSubstitute(sub1, sub2);
                    else
                        addSubstitute(sub2, sub1);
                }
            }
            else
            {
                sub1 = shorter.Substring(left, shorter.Length - (left + right));
                sub2 = longer.Substring(left, longer.Length - (left + right));
                if (switched)
                    addSubstitute(sub1, sub2);
                else
                    addSubstitute(sub2, sub1);
            }


            return;
        }

        private void addSubstitute(string sub1, string sub2)
        {
            int index = subs1.FindIndex(delegate(Misspelling x) { return x.orig == sub1 && x.sub == sub2; });
            if (index != -1)
                subs1[index].increment();
            else
                subs1.Add(new Misspelling(sub1, sub2));
        }

        public void outputErrata()
        {
            StreamWriter sw = new StreamWriter("../../output.txt");

            int count = 0;
            foreach (Misspelling e in subs1)
            {
                ++count;
                //Console.WriteLine(e.orig + "\t" + e.sub + "\t" + e.count);
                sw.WriteLine(e.orig + " " + e.sub + " " + e.count);
            }
            Console.WriteLine("Total size: " + count);
            sw.Close();
        }
    }
}
