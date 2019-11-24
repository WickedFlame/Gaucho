using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MessageMap.Configuration
{
    public class ConfigurationReader
    {
        private Dictionary<string, ITestDataReader> _dataReaders;

        public ConfigurationReader()
        {
            _dataReaders = new Dictionary<string, ITestDataReader>();
            _dataReaders.Add("default", new PropertyReader());
            _dataReaders.Add("inputhandler", new InputHandlerReader());
            _dataReaders.Add("outputhandler", new OutputHandlerReader());
            _dataReaders.Add("filters", new FilterReader());
        }

        public string ReadString(string file)
        {
            return string.Join(Environment.NewLine, ReadAllLines(file));
        }

        public string[] ReadAllLines(string file)
        {
            if (!file.Contains('.'))
            {
                file = $"{file}.testdata";
            }

            if (!File.Exists(file))
            {
                file = FindTestFile(file);
            }

            return File.ReadAllLines(file);
        }

        private string FindTestFile(string file)
        {
            if (!file.Contains("/") && !file.Contains("\\"))
            {
                //var hostingRoot = System.Web.Hosting.HostingEnvironment.IsHosted
                //    ? System.Web.Hosting.HostingEnvironment.MapPath("~/")
                //    : AppDomain.CurrentDomain.BaseDirectory;
                
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

        public PipelineConfiguration Read(string file)
        {
            var config = new PipelineConfiguration();

            var reader = _dataReaders["default"];

            foreach (var line in ReadAllLines(file))
            {
                if (line.StartsWith("'"))
                {
                    continue;
                }

                if (line == string.Empty)
                {
                    reader = null;
                    continue;
                }

                var key = line.Trim().ToLower();
                if (_dataReaders.ContainsKey(key))
                {
                    reader = _dataReaders[key];
                    reader.Initialize();
                }

                if (reader == null)
                {
                    continue;
                }

                reader.ReadLine(line, config);
            }

            return config;
        }
    }

    public interface ITestDataReader
    {
        void Initialize();

        void ReadLine(string line, PipelineConfiguration item);
    }

    public class PropertyReader : ITestDataReader
    {
        private readonly PropertyMapper<PipelineConfiguration> _mapper;

        public PropertyReader()
        {
            _mapper = new PropertyMapper<PipelineConfiguration>();
        }

        public void Initialize()
        {
        }

        public void ReadLine(string line, PipelineConfiguration item)
        {
            _mapper.ReadLine(line, item);
        }
    }

    public class FilterReader : ITestDataReader
    {
        private readonly PropertyMapper<HandlerNode> _mapper;

        public FilterReader()
        {
            _mapper = new PropertyMapper<HandlerNode>();
        }

        public void Initialize()
        {
        }

        public void ReadLine(string line, PipelineConfiguration item)
        {
            var handler = item.OutputHandlers.LastOrDefault();
            if (handler == null)
            {
                return;
            }

            if (handler.Filters == null)
            {
                handler.Filters = new List<FilterNode>();
            }

            if (line.Contains("->"))
            {
                var idx = line.IndexOf("->");
                var source = line.Substring(0, idx).Trim();
                var destination = line.Substring(idx + 2).Trim();

                handler.Filters.Add(new FilterNode(source, destination));
            }
        }
    }

    public class InputHandlerReader : ITestDataReader
    {
        private readonly PropertyMapper<HandlerNode> _mapper;

        public InputHandlerReader()
        {
            _mapper = new PropertyMapper<HandlerNode>();
        }

        public void Initialize()
        {
        }

        public void ReadLine(string line, PipelineConfiguration item)
        {
            if (item.InputHandler == null)
            {
                item.InputHandler = new HandlerNode();
            }

            _mapper.ReadLine(line, item.InputHandler);
        }
    }

    public class OutputHandlerReader : ITestDataReader
    {
        private readonly PropertyMapper<HandlerNode> _mapper;
        private HandlerNode _handler;

        public OutputHandlerReader()
        {
            _mapper = new PropertyMapper<HandlerNode>();
        }

        public void Initialize()
        {
            _handler = null;
        }

        public void ReadLine(string line, PipelineConfiguration item)
        {
            if (_handler == null)
            {
                _handler = new HandlerNode();

                if (item.OutputHandlers == null)
                {
                    item.OutputHandlers = new List<HandlerNode>();
                }

                item.OutputHandlers.Add(_handler);
            }

            _mapper.ReadLine(line, _handler);
        }
    }

    public class PropertyMapper<T>
    {
        private readonly PropertyInfo[] _properties;

        public PropertyMapper()
        {
            _properties = typeof(T).GetProperties();
        }

        public void ReadLine(string line, T item)
        {
            var index = line.IndexOf(':');
            if (index < 0)
            {
                return;
            }

            var propertyName = line.Substring(0, index).TrimStart();
            var propertyInfo = _properties.FirstOrDefault(p => p.Name == propertyName);
            if (propertyInfo == null)
            {
                return;
            }

            var value = line.Substring(index + 1);

            PropertyMapper.ParsePrimitive(propertyInfo, item, value);
        }
    }

    public class PropertyMapper
    {
        public static void ParsePrimitive(PropertyInfo prop, object entity, object value)
        {
            if (prop.PropertyType == typeof(string))
            {
                prop.SetValue(entity, value.ToString().Trim(), null);
            }
            else if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?))
            {
                if (value == null)
                {
                    prop.SetValue(entity, null, null);
                }
                else
                {
                    prop.SetValue(entity, ParseBoolean(value.ToString()), null);
                }
            }
            else if (prop.PropertyType == typeof(long))
            {
                prop.SetValue(entity, long.Parse(value.ToString()), null);
            }
            else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
            {
                if (value == null)
                {
                    prop.SetValue(entity, null, null);
                }
                else
                {
                    prop.SetValue(entity, int.Parse(value.ToString()), null);
                }
            }
            else if (prop.PropertyType == typeof(decimal))
            {
                prop.SetValue(entity, decimal.Parse(value.ToString()), null);
            }
            else if (prop.PropertyType == typeof(double) || prop.PropertyType == typeof(double?))
            {
                var isValid = double.TryParse(value.ToString(), out _);
                if (isValid)
                {
                    prop.SetValue(entity, double.Parse(value.ToString()), null);
                }
            }
            else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(Nullable<DateTime>))
            {
                var isValid = DateTime.TryParse(value.ToString(), out var date);
                if (isValid)
                {
                    prop.SetValue(entity, date, null);
                }
                else
                {
                    isValid = DateTime.TryParseExact(value.ToString(), "yyyyMMdd", new CultureInfo("de-CH"), DateTimeStyles.AssumeLocal, out date);
                    if (isValid)
                    {
                        prop.SetValue(entity, date, null);
                    }
                }
            }
            else if (prop.PropertyType == typeof(Guid))
            {
                var isValid = Guid.TryParse(value.ToString(), out var guid);
                if (isValid)
                {
                    prop.SetValue(entity, guid, null);
                }
                else
                {
                    isValid = Guid.TryParseExact(value.ToString(), "B", out guid);
                    if (isValid)
                    {
                        prop.SetValue(entity, guid, null);
                    }
                }
            }
            else if (prop.PropertyType == typeof(Type))
            {
                var type = Type.GetType(value.ToString());
                prop.SetValue(entity, type, null);
            }
        }

        public static bool ParseBoolean(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return false;
            }

            switch (value.ToString().ToLowerInvariant())
            {
                case "1":
                case "y":
                case "yes":
                case "true":
                    return true;

                case "0":
                case "n":
                case "no":
                case "false":
                default:
                    return false;
            }
        }
    }
}
