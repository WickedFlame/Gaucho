using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Gaucho.Configuration
{
    public class ConfigurationReader
    {
        private readonly Dictionary<string, INodeReader<PipelineConfiguration>> _dataReaders;

        public ConfigurationReader()
        {
            _dataReaders = new Dictionary<string, INodeReader<PipelineConfiguration>>
            {
                {"default", new PropertyReader<PipelineConfiguration>()},
                {"inputhandler", new InputHandlerReader()},
                {"outputhandler", new OutputHandlerReader()}
            };
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

                if(reader  == null)
                {
                    var key = line.Trim().ToLower();
                    if (_dataReaders.ContainsKey(key))
                    {
                        reader = _dataReaders[key];
                        reader.Initialize();
                    }
                }

                if (reader == null)
                {
                    //TODO: Log message
                    continue;
                }

                reader.ReadLine(line, config);
            }

            return config;
        }
    }

    public interface INodeReader<T>
    {
        void Initialize();

        void ReadLine(string line, T item);
    }

    public class PropertyReader<T> : INodeReader<T>
    {
        private readonly PropertyMapper<T> _mapper;

        public PropertyReader()
        {
            _mapper = new PropertyMapper<T>();
        }

        public void Initialize()
        {
        }

        public void ReadLine(string line, T item)
        {
            _mapper.ReadLine(line, item);
        }
    }

    public class FilterReader : INodeReader<HandlerNode>
    {
        private readonly PropertyMapper<HandlerNode> _mapper;

        public FilterReader()
        {
            _mapper = new PropertyMapper<HandlerNode>();
        }

        public void Initialize()
        {
        }

        public void ReadLine(string line, HandlerNode handler)
        {
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

    public class InputHandlerReader : INodeReader<PipelineConfiguration>
    {
        private readonly Dictionary<string, INodeReader<HandlerNode>> _dataReaders;
        private INodeReader<HandlerNode> _reader;

        public InputHandlerReader()
        {
            _dataReaders = new Dictionary<string, INodeReader<HandlerNode>>
            {
                {"default", new PropertyReader<HandlerNode>()}, 
                {"filters", new FilterReader()}
            };
        }

        public void Initialize()
        {
            _reader = _dataReaders["default"];
        }

        public void ReadLine(string line, PipelineConfiguration item)
        {
            if (item.InputHandler == null)
            {
                item.InputHandler = new HandlerNode();
            }

            var key = line.Trim().ToLower();
            if (_dataReaders.ContainsKey(key))
            {
                _reader = _dataReaders[key];
                _reader.Initialize();
            }

            _reader.ReadLine(line, item.InputHandler);
        }
    }

    public class OutputHandlerReader : INodeReader<PipelineConfiguration>
    {
        private readonly Dictionary<string, INodeReader<HandlerNode>> _dataReaders;
        private INodeReader<HandlerNode> _reader;
        private HandlerNode _handler;

        public OutputHandlerReader()
        {
            _dataReaders = new Dictionary<string, INodeReader<HandlerNode>>
            {
                {"default", new PropertyReader<HandlerNode>()}, 
                {"filters", new FilterReader()}
            };
        }

        public void Initialize()
        {
            _handler = null;
            _reader = _dataReaders["default"];
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

            var key = line.Trim().ToLower();
            if (_dataReaders.ContainsKey(key))
            {
                _reader = _dataReaders[key];
                _reader.Initialize();
            }

            _reader.ReadLine(line, _handler);
        }
    }

    
}
