using System;
using System.IO;

namespace WickedFlame.Yaml
{
    //https://docs.ansible.com/ansible/latest/reference_appendices/YAMLSyntax.html
    public class YamlReader
    {
        public T Read<T>(string file) where T : class, new()
        {
            var reader = new YamlNodeReader(typeof(T));

            foreach (var line in ReadAllLines(file))
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                if (line.StartsWith("#"))
                {
                    continue;
                }

                var meta = new YamlLine(line);

                reader.ReadLine(meta);
            }

            return (T)reader.Node;
        }

        public T Read<T>(string[] lines) where T : class, new()
        {
            var reader = new YamlNodeReader(typeof(T));

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                if (line.StartsWith("#"))
                {
                    continue;
                }

                var meta = new YamlLine(line);

                reader.ReadLine(meta);
            }

            return (T)reader.Node;
        }

        private string[] ReadAllLines(string file)
        {
            if (!File.Exists(file))
            {
                file = FindFile(file);
            }

            return File.ReadAllLines(file);
        }

        private string FindFile(string file)
        {
            if (!file.Contains("/") && !file.Contains("\\"))
            {
                var hostingRoot = AppDomain.CurrentDomain.BaseDirectory;
                file = LoadPath(file, hostingRoot);
            }

            return file;
        }

        private string LoadPath(string file, string root)
        {
            var path = Path.Combine(root, file);
            foreach (var f in Directory.GetFiles(root))
            {
                if (path == f)
                {
                    return f;
                }
            }

            foreach (var dir in Directory.GetDirectories(root))
            {
                var f = LoadPath(file, dir);
                if (!string.IsNullOrEmpty(f))
                {
                    return f;
                }
            }

            return null;
        }
    }
}
