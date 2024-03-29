﻿using System;
using System.Collections.Generic;

namespace Gaucho.Dashboard.Monitoring
{
	/// <summary>
	/// 
	/// </summary>
    public class PipelineMetric
    {
        private readonly List<DashboardMetric> _metrics = new List<DashboardMetric>();
        private Dictionary<string, DashboardElements> _elements;

		/// <summary>
		/// Creates a new instance of the PipelineMetric
		/// </summary>
		/// <param name="pipelineId"></param>
		/// <param name="server"></param>
		/// <param name="heartbeat"></param>
        public PipelineMetric(string pipelineId, string server, DateTime? heartbeat)
        {
            PipelineId = pipelineId;
			ServerName = server;
			Hearbeat = heartbeat;
        }

		/// <summary>
		/// Gets the servername
		/// </summary>
        public string ServerName { get; }

		/// <summary>
		/// Gets the pipelineid
		/// </summary>
        public string PipelineId { get; }

		/// <summary>
		/// Gets the last heartbeat of the Pipeline
		/// </summary>
		public DateTime? Hearbeat { get; }

		/// <summary>
		/// Gets a value indicating if the Pipeline has had a heartbeat in the last 5 minutes
		/// </summary>
		public bool IsActive => Hearbeat == null || Hearbeat.Value.AddMinutes(5) > DateTime.Now; 

		/// <summary>
		/// Gets a list of all <see cref="DashboardMetric"/>
		/// </summary>
        public IEnumerable<DashboardMetric> Metrics => _metrics;

		/// <summary>
		/// Gets a list of all <see cref="DashboardElements"/>
		/// </summary>
        public Dictionary<string, DashboardElements> Elements => _elements ?? (_elements = new Dictionary<string, DashboardElements>());

        internal void AddMetric(string key, string title, object value)
        {
            _metrics.Add(new DashboardMetric {Key = key,  Title = title, Value = value});
        }

        internal void AddElement(string key, string title, object value)
        {
	        if (!Elements.ContainsKey(key))
	        {
		        Elements.Add(key, new DashboardElements {Key = key, Title = title});
	        }

	        Elements[key].Elements.Add(value);
        }
    }
}
