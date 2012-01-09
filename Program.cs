using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

//Plan
//Build a dictionary list of commonly misspelled words
//Build a ranked list of common misspellings
    //for any new list, check if it's in the dictionary
    //if not record misspellings in List



namespace SpellChecker2
{
    class Program
    {
        static void Main(string[] args)
        {
            WordChecker test = new WordChecker();

            Console.Write("Populating List...\n");

            test.CompileErrata("../../Lists/Wikipedia.txt", 1);
            //test.CompileErrata("../../Lists/spell-errors.txt", 2);             
            //test.outputErrata();
            
            test.createList();

            while (true)
            {
                Console.WriteLine("Enter a word:  ");
                test.checkWord(Console.ReadLine());
            }
            
            
            //WordChecker.testErrata("abcess", "abscess");
            return;
        }

    }
}