﻿using System.Data.Entity;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.SignalR;
using MvcContrib.Binders;
using MvcEnumFlags;
using RegisterKeeper.Web.App_Start;
using RegisterKeeper.Web.Models;

namespace RegisterKeeper.Web
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			// Register the default hubs route: ~/signalr
			RouteTable.Routes.MapHubs(new HubConfiguration { EnableDetailedErrors = true });

			AreaRegistration.RegisterAllAreas();

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			AuthConfig.RegisterAuth();

			Database.SetInitializer(new RegisterKeeperDbInitialiser());

			ModelBinders.Binders.Add(typeof(Distance), new EnumFlagsModelBinder());

			BundleMobileConfig.RegisterBundles(BundleTable.Bundles);

			DerivedTypeModelBinderCache.RegisterDerivedTypes(
				typeof(Shot),
				new[]
					{
						typeof(SightingShot), 
						typeof(ScoringShot)
					});
		}
	}
}