using COTES.ISTOK.WebClient.Models;
using COTES.ISTOK.WebClient.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace COTES.ISTOK.WebClient
{
    public class SessionKeeperModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.Model != null)
                throw new InvalidOperationException();
            SessionKeeper sessionKeeper = (SessionKeeper)controllerContext.HttpContext.Session[Resources.strSessionKeeperKey];
            if (sessionKeeper == null)
            {
                sessionKeeper = new SessionKeeper(controllerContext.HttpContext.Session);
                controllerContext.HttpContext.Session[Resources.strSessionKeeperKey] = sessionKeeper;
            }

            return sessionKeeper;
        }
    }
}