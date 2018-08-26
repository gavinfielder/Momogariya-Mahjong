using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Mahjong.Tests
{
    public class SecurityTests
    {
        [Test]
        public void Output_Random_access_key()
        {
            Debug.Log("Test output: Random access key: " + Common.SecurityManager.GetRandomAccessKey());
        }
    }
}