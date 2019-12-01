using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Configuration;
using NUnit.Framework;
using WickedFlame.Yaml;

namespace Gaucho.Test.Configuration
{
    [TestFixture]
    public class YamlReaderTests
    {
        [Test]
        public void Gaucho_ConfigurationReader_SimpleProperty_Root()
        {
            var reader = new YamlReader();
            var data = reader.Read<YamlRoot>("YamlTest.yml");

            Assert.AreEqual("id", data.Id);
        }

        [Test]
        public void Gaucho_ConfigurationReader_ObjectProperty_Root()
        {
            var reader = new YamlReader();
            var data = reader.Read<YamlRoot>("YamlTest.yml");

            Assert.IsNotNull(data.SimpleObject);
            Assert.AreEqual("Object name", data.SimpleObject.Name);
        }

        [Test]
        public void Gaucho_ConfigurationReader_ObjectProperty_Nested()
        {
            var reader = new YamlReader();
            var data = reader.Read<YamlRoot>("YamlTest.yml");

            Assert.IsNotNull(data.Nested);
            Assert.IsNotNull(data.Nested.Nested);
            Assert.AreEqual("nested object", data.Nested.Nested.Name);
        }

        [Test]
        public void Gaucho_ConfigurationReader_ObjectProperty_AfterNested()
        {
            var reader = new YamlReader();
            var data = reader.Read<YamlRoot>("YamlTest.yml");

            Assert.AreEqual("first object", data.Nested.Name);
        }

        [Test]
        public void Gaucho_ConfigurationReader_StringList()
        {
            var reader = new YamlReader();
            var data = reader.Read<YamlRoot>("YamlTest.yml");

            Assert.AreEqual("first object", data.Nested.Name);
        }
    }

    public class YamlRoot
    {
        public string Id { get; set; }

        public SimpleObject SimpleObject { get; set; }

        public NestedObject Nested { get; set; }

        public List<string> StringList { get; set; }

        public IEnumerable<string> EnumerableList { get; set; }
    }

    public class SimpleObject
    {
        public string Name { get; set; }
    }

    public class NestedObject
    {
        public string Name { get; set; }

        public NestedObject Nested { get; set; }
    }
}
