using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Decoherence.CommandLineSerialization.Test
{
    public class MicsTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestSplitCommandLine1()
        {
            var argList = new List<string>(ImplUtil.SplitCommandLine("1 2 'a b \\\\ \\ \\\" \\1' \"1 \\\" \\\\ \\ \\1\" '1 2'\" 3\""));
            
            foreach (var arg in argList)
            {
                Console.WriteLine(arg);
            }
            
            Assert.IsTrue(argList[0] == "1");
            Assert.IsTrue(argList[1] == "2");
            Assert.IsTrue(argList[2] == "a b \\ \\ \\\" \\1");
            Assert.IsTrue(argList[3] == "1 \" \\ \\ \\1");
            Assert.IsTrue(argList[4] == "1 2 3");
        }
        
        [Test]
        public void TestSplitCommandLine2()
        {
            var argList = new List<string>(ImplUtil.SplitCommandLine("1 2 \\"));
            
            foreach (var arg in argList)
            {
                Console.WriteLine(arg);
            }
            
            Assert.IsTrue(argList[0] == "1");
            Assert.IsTrue(argList[1] == "2");
        }
        
        [Test]
        public void TestSplitCommandLine3()
        {
            var argList = new List<string>(ImplUtil.SplitCommandLine("1 2\\"));
            
            foreach (var arg in argList)
            {
                Console.WriteLine(arg);
            }
            
            Assert.IsTrue(argList[0] == "1");
            Assert.IsTrue(argList[1] == "2");
        }
        
        [Test]
        public void TestSplitCommandLine4()
        {
            try
            {
                new List<string>(ImplUtil.SplitCommandLine("\"1 2"));
            }
            catch (InvalidOperationException)
            {
                Assert.Pass();
                return;
            }

            Assert.True(false);
        }
    }
}