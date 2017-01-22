using System.Reflection;
using System.Web.Http;

using Autofac;
using Autofac.Integration.WebApi;
using Owin;
using Pets.Misc;
using Pets.Repositories;
using Swashbuckle.Application;

namespace Pets
{
    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            var config = new HttpConfiguration();
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            config
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "Docs");
                    c.DescribeAllEnumsAsStrings();
                    c.IncludeXmlComments(GetXmlCommentsPath());
                })
                .EnableSwaggerUi(c => c.DisableValidator());

            // Web API routes
            config.MapHttpAttributeRoutes();

            // DI
            var builder = new ContainerBuilder();
            builder.RegisterType(typeof(UserRepository)).As<IUserRepository>().SingleInstance();
            builder.RegisterType(typeof(PetRepository)).As<IPetRepository>().SingleInstance();
            builder.RegisterType(typeof(PetAttributeContainer)).As<IPetAttributeContainer>().SingleInstance();
            builder.RegisterType(typeof(PetLifetimeService)).As<PetLifetimeService>().SingleInstance();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            appBuilder.UseAutofacMiddleware(container);
            appBuilder.UseAutofacWebApi(config);
            appBuilder.UseWebApi(config);

            // create PetLifetimeService
            var service = container.Resolve(typeof(PetLifetimeService));
        }

        protected static string GetXmlCommentsPath()
        {
            return $@"{System.AppDomain.CurrentDomain.BaseDirectory}\Pets.XML";
        }
    }
}