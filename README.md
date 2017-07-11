# NetCoreMaintenanceService
A simple service to add and run maintenance tasks on a background thread


Add to startup.cs:

'''

public class Startup
{
	private MaintenanceService maintenance;
	...
  	public void ConfigureServices(IServiceCollection services)
  	{
     		...
      		service.AddScoped<IService, ServiceImplementationWithIMaintainable>();
      
      		var serviceProvider = services.BuildServiceProvider();
		maintenance = new MaintenanceService(serviceProvider, timeout_in_ms);
		maintenance.Add<IService>();
  }

  public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
  {
    ...
    // Finish the initiating
    ...
    
    maintenance.Start();
  }
'''
