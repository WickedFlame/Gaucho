using System;
using System.Diagnostics;
using System.Threading;
using Gaucho.Handlers;
using Gaucho.Server.Monitoring;
using NUnit.Framework;
using MeasureMap;

namespace Gaucho.Integration.Tests
{
	public class LoadTests
	{
		private static object _lock = new object();

		[SetUp]
		public void Setup()
		{
		}

		[Test]
		[Explicit]
		public void LoadTest_HeavyWork()
		{
			// start with the warmup...
			var cnt = 0;
			var proc = 0;

			var pipelineId = Guid.NewGuid().ToString();

			var server = new ProcessingServer();
			server.Register(pipelineId, () =>
			{
				var pipeline = new EventPipeline();
				pipeline.AddHandler(new LoadTestOuptuHandler(e =>
				{
					proc += 1;
					// to some heavy work
					Thread.Sleep(500);
				}));

				return pipeline;
			});
			server.Register(pipelineId, new InputHandler<InputItem>());


			var client = new EventDispatcher(server);

			var profiler = ProfilerSession.StartSession()
				.SetIterations(1000)
				.SetThreads(5)
				.Task(() =>
				{
					cnt += 1;
					client.Process(pipelineId, new InputItem
					{
						Value = "StaticServer",
						Name = "test",
						Number = cnt
					});
				}).RunSession();

			server.WaitAll(pipelineId);
			profiler.Trace();

			var monitor = new StatisticsApi(pipelineId);
			var processed = monitor.GetMetricValue<int>(MetricType.ProcessedEvents);
			Assert.AreEqual(processed, cnt, "Processed events are not equal to sent events {0} to {1}", processed, cnt);
		}

		[Test]
		[Explicit]
		public void LoadTest_MetricCounter()
		{
			// start with the warmup...
			var sent = 0;
			var processed = 0;

			var pipelineId = Guid.NewGuid().ToString();

			var server = new ProcessingServer();
			server.Register(pipelineId, () =>
			{
				var pipeline = new EventPipeline();
				pipeline.AddHandler(new LoadTestOuptuHandler(e =>
				{
					lock (_lock)
					{
						processed += 1;
					}

					// to some heavy work
					Thread.Sleep(500);
				}));

				return pipeline;
			});
			server.Register(pipelineId, new InputHandler<InputItem>());


			var client = new EventDispatcher(server);

			var profiler = ProfilerSession.StartSession()
				.SetIterations(1000)
				.SetThreads(5)
				.Task(() =>
				{
					
					client.Process(pipelineId, new InputItem
					{
						Value = "StaticServer",
						Name = "test",
						Number = sent
					});
					sent += 1;
				}).RunSession();

			server.WaitAll(pipelineId);
			profiler.Trace();

			var monitor = new StatisticsApi(pipelineId);
			var metrics = monitor.GetMetricValue<int>(MetricType.ProcessedEvents);
			Assert.AreEqual(metrics, processed, "Metric event counter is not equal to processed events {0} to {1}", metrics, processed);
		}

		[Test]
		[Explicit]
		public void LoadTest_Simple()
		{
			// start with the warmup...
			var cnt = 0;

			var pipelineId = Guid.NewGuid().ToString();

			var server = new ProcessingServer();
			server.Register(pipelineId, () =>
			{
				var pipeline = new EventPipeline();
				pipeline.AddHandler(new LoadTestOuptuHandler(e=>{}));

				return pipeline;
			});
			server.Register(pipelineId, new InputHandler<InputItem>());


			var client = new EventDispatcher(server);

			var profiler  = ProfilerSession.StartSession()
				.SetIterations(1000)
				.SetThreads(5)
				.Task(() =>
				{
					client.Process(pipelineId, new InputItem
					{
						Value = "StaticServer",
						Name = "test",
						Number = 1
					});
					cnt += 1;
				}).RunSession();

			server.WaitAll(pipelineId);
			profiler.Trace();

			var monitor = new StatisticsApi(pipelineId);
			var metrics = monitor.GetMetricValue<int>(MetricType.ProcessedEvents);
			Assert.AreEqual(metrics, cnt, "Processed events are not equal to sent events {0} to {1}", metrics, cnt);
		}
	}

	public class LoadTestOuptuHandler : IOutputHandler
	{
		private Action<Event> _task;

		public LoadTestOuptuHandler(Action<Event> task)
		{
			_task = task;
		}

		public void Handle(Event @event)
		{
			_task(@event);
		}
	}

	public class InputItem
	{
		public string Value { get; set; }

		public string Name { get; set; }

		public int Number { get; set; }
	}
}