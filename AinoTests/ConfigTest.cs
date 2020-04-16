using Aino;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AinoTests
{
    [TestFixture]
    class ConfigTest
    {
        [Test]
        public void TestAddEntryWorks()
        {
            KeyNameListConfig testKeyNameListConfig = new KeyNameListConfig();
            testKeyNameListConfig.AddEntry("test1", "test2");
            Assert.AreEqual("test2", testKeyNameListConfig.GetEntry("test1"));
            Assert.Throws<KeyNotFoundException>(delegate { testKeyNameListConfig.GetEntry("test3"); });
        }

        [Test]
        public void TestAddEntriesWorks()
        {
            KeyNameListConfig testKeyNameListConfig = new KeyNameListConfig();
            Dictionary<string, string> entries = new Dictionary<string, string>();
            entries.Add("test1", "test11");
            entries.Add("test666", "test6661");

            testKeyNameListConfig.AddEntries(entries);

            Assert.AreEqual("test11", testKeyNameListConfig.GetEntry("test1"));
            Assert.AreEqual("test6661", testKeyNameListConfig.GetEntry("test666"));
            Assert.Throws<KeyNotFoundException>(delegate { testKeyNameListConfig.GetEntry("argh"); });
        }

        [Test]
        public void TestEntryExistsWorks()
        {
            KeyNameListConfig testKeyNameListConfig = new KeyNameListConfig();
            testKeyNameListConfig.AddEntry("test9", "test991");
            Assert.IsTrue(testKeyNameListConfig.EntryExists("test9"));
            Assert.IsFalse(testKeyNameListConfig.EntryExists("test911"));
        }

        [Test]
        public void TestThrowsWithDuplicateEntry()
        {
            KeyNameListConfig testKeyNameListConfig = new KeyNameListConfig();
            testKeyNameListConfig.AddEntry("hoi", ":D");
            Assert.Throws<ArgumentException>(delegate { testKeyNameListConfig.AddEntry("hoi", ":D"); });
        }
    }
}
