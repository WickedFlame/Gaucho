using System;
using System.Collections.Generic;
using System.Text;
using WickedFlame.Yaml;

namespace Gaucho.Configuration
{
    public static class YamlReaderExtensions
    {
        public static PipelineConfiguration Read(this YamlReader reader, string fileName)
        {
            return reader.Read<PipelineConfiguration>(fileName);
        }
    }
}
