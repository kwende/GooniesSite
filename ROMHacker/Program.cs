using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ROMHacker
{
    class Program
    {
        static Dictionary<char, byte> LoadTable(string path)
        {
            Dictionary<char, byte> ret = new Dictionary<char, byte>();

            string[] lines = File.ReadAllLines(path); 

            foreach(string line in lines)
            {
                string[] bits = line.Split('=');
                byte intValue = byte.Parse(bits[0], NumberStyles.HexNumber);
                char value = bits[1][0];

                ret.Add(value, intValue); 
            }


            return ret; 
        }

        static byte[] tableToByteSequence(Dictionary<char, byte> table, string content)
        {
            List<byte> sequence = new List<byte>(); 

            foreach(char c in content)
            {
                if(c == '\n')
                {
                    sequence.Add(0xfe);
                }
                else if(table.ContainsKey(c))
                {
                    sequence.Add(table[c]); 
                }
                else if(table.ContainsKey(Char.ToUpper(c)))
                {
                    sequence.Add(table[Char.ToUpper(c)]);
                }
                else if(c == '\'' && table.ContainsKey('`'))
                {
                    sequence.Add(table['`']);
                }
            }

            return sequence.ToArray(); 
        }

        static void Main(string[] args)
        {

            const string filePath = @"C:/users/ben/desktop/modified.nes";

            // ROOM SCREENS
            //const string SearchText = "you`ve got\na key holder.";
            //const string ToReplace = "you`ve got\na good spouse";
            //const string SearchText = "you`ve got\na hammer";
            //const string ToReplace = "the date is\nfeb 11 .";
            //const string SearchText = "the date is\nfeb 11 .";
            //const string ToReplace = "the date is\nfeb 11. ";
            //const string SearchText = "find the goonies\nwith the magic\nlocator device.";
            //const string ToReplace = "find the goonies\nfor valentines\nday hunt 2022!!";

            // INTRO SCREENS
            const string SearchText = "her.";
            const string ToReplace = "ie. ";

            if (SearchText.Length != ToReplace.Length)
            {
                throw new ArgumentOutOfRangeException(); 
            }

            Dictionary<char, byte> table = LoadTable(@"C:\Users\ben\Desktop\goonies\woman_screen.tbl");

            byte[] searchByteSequence = tableToByteSequence(table, SearchText); 
            byte[] replaceByteSequence = tableToByteSequence(table, ToReplace);

            byte[] bytes = File.ReadAllBytes(filePath);

            int hitCount = 0;
            int sequenceIndex = 0;
            int startIndex = 0;
            bool found = false; 
            for(int c=0;c<bytes.Length;c++)
            {
                byte b = bytes[c]; 
                if(b == searchByteSequence[sequenceIndex])
                {
                    if(sequenceIndex == 0)
                    {
                        startIndex = c; 
                    }

                    sequenceIndex++;
                    hitCount++; 

                    if(hitCount == searchByteSequence.Length)
                    {
                        found = true; 
                        break; 
                    }
                }
                else
                {
                    sequenceIndex = 0;
                    hitCount = 0; 
                }
            }

            if(found)
            {
                for(int c = startIndex, i=0;i< replaceByteSequence.Length; c++,i++)
                {
                    bytes[c] = replaceByteSequence[i]; 
                }
            }

            Console.WriteLine("Hello World!");
            File.WriteAllBytes("C:/users/ben/desktop/modified.nes", bytes); 
        }
    }
}
