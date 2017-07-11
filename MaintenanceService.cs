using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class MaintenanceService
{
	public interface IMaintainable
	{
		void Maintain(IServiceProvider Services);
	}

	[System.Serializable]
	public class MaintenanceThreadException : ApplicationException
	{
		public MaintenanceThreadException() { }
		public MaintenanceThreadException(string message) : base(message) { }
		public MaintenanceThreadException(string message, Exception inner) : base(message, inner) { }
		protected MaintenanceThreadException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}

	private readonly IServiceProvider _services;
	private readonly Thread _thread;
	private readonly ConcurrentBag<IMaintainable> _strategies;
	private readonly int _sleepingMiliseconds;
	private List<Type> _types;

	public MaintenanceService(IServiceProvider ServiceProvider, int SleepingTimeMinutes)
	{
		_sleepingMiliseconds = Convert.ToInt32(TimeSpan.FromMinutes(SleepingTimeMinutes).TotalMilliseconds);
		_services = ServiceProvider;
		_thread = new Thread(ThreadMethod);
		_thread.Name = "MaintenanceThread";
		_strategies = new ConcurrentBag<IMaintainable>();
		_types = new List<Type>();
	}

	public void Start()
	{
		if (_thread.ThreadState != ThreadState.Unstarted)
			throw new ApplicationException();
		_thread.Start();
	}
	public void Add(IMaintainable Maintainable)
	{
		_strategies.Add(Maintainable);
	}
	public void Add<T>()
	{
		_types.Add(typeof(T));
	}

	void ThreadMethod()
	{
		Thread.Sleep(_sleepingMiliseconds);
		while (true)
		{
			foreach (var type in _types)
			{
				try
				{
					var maintainable = _services.GetService(type) as IMaintainable;
					maintainable.Maintain(_services);
				}
				catch (Exception)
				{
					
				}
			}

			var arr = _strategies.ToArray();
			foreach (var item in arr)
			{
				try
				{
					item.Maintain(_services);
				}
				catch (Exception)
				{
				}
			}
			Thread.Sleep(_sleepingMiliseconds);
		}
	}
}

