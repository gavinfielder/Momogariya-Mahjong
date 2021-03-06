﻿using System;
using System.Collections.Generic;

namespace Mahjong
{
    public interface ISecured
    {
        void SetOwner(int newAccessKey);
        void ReleaseOwnership(int accessKey);
    }

    public class Security
    {
        public const int NO_OWNER = 0;
        public const int PUBLIC_READ = 0x287019bf; //TODO: replace all "0 means public read" in the code/comments
        public const int PUBLIC_READ_WRITE = 0x5c81b2db;

        public static int AccessKeyHash(string input)
        {
            //null input always returns null hash
            if (input == null || input == "") return 0;
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
