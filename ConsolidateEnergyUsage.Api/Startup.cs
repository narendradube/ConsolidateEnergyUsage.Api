using ConsolidateEnergyUsage.Api.Controllers;
using ConsolidateEnergyUsage.Api.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ConsolidateEnergyUsage.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            services.AddMvc(setupAction =>
            {
                setupAction.Filters.Add(
                    new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
                setupAction.Filters.Add(
                    new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));
                setupAction.Filters.Add(
                    new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));
                setupAction.Filters.Add(
                    new ProducesDefaultResponseTypeAttribute());
                setupAction.Filters.Add(
                    new ProducesResponseTypeAttribute(StatusCodes.Status401Unauthorized));
                setupAction.ReturnHttpNotAcceptable = true;

                setupAction.OutputFormatters.Add(new XmlSerializerOutputFormatter());

            });
            services.AddControllers(setupAction => { }
            ).AddNewtonsoftJson(setupAction =>
            {
                setupAction.SerializerSettings.ContractResolver =
                   new CamelCasePropertyNamesContractResolver();
            })
             .ConfigureApiBehaviorOptions(setupAction =>
             {
                 setupAction.InvalidModelStateResponseFactory = context =>
                 {
                     var problemDetailsFactory = context.HttpContext.RequestServices
                             .GetRequiredService<ProblemDetailsFactory>();
                     var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(
                             context.HttpContext,
                             context.ModelState);

                     problemDetails.Detail = "See the errors field for details.";
                     problemDetails.Instance = context.HttpContext.Request.Path;

                     var actionExecutingContext =
                               context as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;
                     if ((context.ModelState.ErrorCount > 0) &&
                             (actionExecutingContext?.ActionArguments.Count == context.ActionDescriptor.Parameters.Count))
                     {
                         problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                         problemDetails.Title = "One or more validation errors occurred.";

                         return new UnprocessableEntityObjectResult(problemDetails)
                         {
                             ContentTypes = { "application/problem+json" }
                         };
                     }
                     problemDetails.Status = StatusCodes.Status400BadRequest;
                     problemDetails.Title = "One or more errors on input occurred.";
                     return new BadRequestObjectResult(problemDetails)
                     {
                         ContentTypes = { "application/problem+json" }
                     };
                 };
             });
            services.AddApiVersioning(setupAction =>
            {
                setupAction.AssumeDefaultVersionWhenUnspecified = true;
                setupAction.DefaultApiVersion = new ApiVersion(1, 0);
                setupAction.ReportApiVersions = true;
                setupAction.ApiVersionReader = ApiVersionReader.Combine(
                                       new QueryStringApiVersionReader("api-supported-versions"),
                                       new HeaderApiVersionReader("api-supported-versions"));
                setupAction.Conventions.Controller<EnergyConsumptionController>().HasApiVersion(new ApiVersion(1, 0));

                //setupAction.ApiVersionReader = new HeaderApiVersionReader("api-supported-versions");
            });
            services.AddCors();
            services.AddTransient<IFileProcessor, FileProcessor>();
            services.Configure<MvcOptions>(config =>
            {
                var newtonsoftJsonOutputFormatter = config.OutputFormatters
                      .OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ConsolidateEnergyUsage.Api",
                    Version = "v1",
                    Description = "This Api returns JSON results containing total energy consumption .",
                    Contact = new OpenApiContact()
                    {
                        Email = "narendradubey4@gmail.com",
                        Name = "Narendra Dubey",
                        Url = new Uri("https://github.com/narendradube")
                    }
                });
                c.EnableAnnotations();
                var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

                c.IncludeXmlComments(xmlCommentsFullPath);

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ConsolidateEnergyUsage.Api v1");
                c.DefaultModelExpandDepth(2);
                c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
                c.EnableDeepLinking();
                c.DisplayOperationId();

            }

            );

            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
                });
            });
            app.UseHttpsRedirection();
          
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
