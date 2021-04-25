﻿using System.Diagnostics.CodeAnalysis;
using Library.Publisher.Data.Repositories;
using Library.Publisher.Service;
using Library.Publisher.Service.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Swagger;

namespace Library.Publisher.Api
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private const string Url = "/swagger/v1/swagger.json";
        private const string Title = "Library OS Publisher API";
        private const string Version = "v1";
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddMvc(x => x.EnableEndpointRouting = false);
            
            AddSwagger(services);
            AddContext(services, Configuration);
            AddRepositories(services);
            AddComponents(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            // global cors policy
            // app.UseCors(x => x
            //                 .AllowAnyOrigin()
            //                 .AllowAnyMethod()
            //                 .AllowAnyHeader()
            //                 .AllowCredentials());

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(Url, Title);
                c.RoutePrefix = string.Empty;
            });

            app.UseMvc();
        }
        
        private void AddSwagger(IServiceCollection services)
        {
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c => { c.SwaggerDoc(Version, new OpenApiInfo {Title = Title, Version = Version}); });
        }
        private void AddContext(IServiceCollection services, IConfiguration configuration)
        {
            var mongoConnection = configuration.GetSection(PublisherServiceConstants.MongoSettingsConstants)
                .Get<MongoSettings>();

            services.AddSingleton<IMongoClient, MongoClient>(t => new MongoClient(mongoConnection.Url));
        }
        private void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IPublisherRepository, PublisherRepository>();
        }

        private void AddComponents(IServiceCollection services)
        {
            services.AddScoped<IPublisherService, PublisherService>();
        }

    }
}