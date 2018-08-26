using System;
using System.Collections.Generic;

namespace Common
{
    public class SecurityManager
    {
        public static int AccessKeyHash(string input)
        {
            //Placeholder
            //After initial commit, this file will be added to ignore.
            return 0;
        }

        public static string GetRandomAccessKey()
        {
            Random rand = new Random();
            string key = "";
            for (int i = 0; i < 10; i++)
            {
                key += (char) rand.Next(33, 126);
            }
            return key;
        }
    }
}
