using System;
using System.Threading;
using Gaucho.Diagnostics;

namespace Gaucho.Server.Monitoring
{
	/// <summary>
	/// A background task processor that exutes a recurring task in a defined interval
	/// </summary>
	public class BackgroundProcess : IDisposable
	{
		private bool _isDisposed;
		private readonly Timer _timer;
		private readonly Action _task;

		/// <summary>
		/// Creates a new instance of a BackgroundProcessor
		/// </summary>
		/// <param name="task"></param>
		/// <param name="interval"></param>
		public BackgroundProcess(Action task, int interval)
		{
			_task = task ?? throw new ArgumentNullException(nameof(task));
			if(interval <= 0)
			{
				interval = 120000;
			}

			_timer = new Timer(Execute, null, 0, interval);
		}

		private void Execute(object state)
		{
			try
			{
				_task.Invoke();
			}
			catch (Exception e)
			{
				var logger = LoggerConfiguration.Setup();
				logger.Write(e.Message, Category.Log, LogLevel.Error, "BackgroundProcess");
			}
		}

		/// <summary>
		/// Dispose the processor
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (_isDisposed)
			{
				return;
			}

			if (disposing)
			{
				_timer.Dispose();
			}

			_isDisposed = true;
		}
	}
}
