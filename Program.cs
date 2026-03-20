using App_deploy.Api;
using Microsoft.Extensions.Configuration;



var config = new ConfigurationBuilder()
	.SetBasePath(AppContext.BaseDirectory)
	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
	.Build();

var apiOptions = config.GetSection("Api").Get<ApiOptions>() ?? new ApiOptions();
await ApiServer.RunAsync(apiOptions.ListenUrl);
