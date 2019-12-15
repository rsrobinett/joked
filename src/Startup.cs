using Joked.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Joked
{
	public class Startup
	{
	private string name = "Joked";

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddCors(options =>
			{
				options.AddPolicy("CorsPolicy", builder => builder
					.WithOrigins("http://localhost:3000", "http://localhost:4200")
					.AllowAnyMethod()
					.AllowAnyHeader()
					.AllowCredentials());
			});

			services.AddSignalR()
				.AddJsonProtocol(options =>
				{
					options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
				});

			services.AddHttpContextAccessor();

			services.AddHttpClient<IJokeHttpClient, JokeHttpClient>();

			services.AddControllers()
				.SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1",
					new OpenApiInfo
					{
						Title = name,
						Version = "v1"

					});
				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				c.IncludeXmlComments(xmlPath);
			});

			//services.AddSingleton<IHostedService, RandomJokeTimerService>();
			services.AddHostedService<RandomJokeTimerService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", name);
				c.DocumentTitle = name;
				//c.RoutePrefix = string.Empty;
			});

			app.UseRouting();

			app.UseCors("CorsPolicy");

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapHub<JokeHub>("/randomjokes");
			});
		}
	}
}


