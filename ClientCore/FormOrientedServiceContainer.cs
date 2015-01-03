using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace COTES.ISTOK.ClientCore
{
    /// <summary>
    /// Сервисконтейнер, отдельный для каждой формы.
    /// На данный момент вся его роль заключается в связи с UnitProvider через интерфейсы IUnitProviderSupplier и IUnitNodeProvider.
    /// Если не нашёл нужного класса, запрашивает к следующему, в цепи, элементу ISite.
    /// </summary>
    public class FormOrientedServiceContainer : ISite
    {
        ISite parentSite;

        List<Object> services;

        public FormOrientedServiceContainer(ISite parentSite)
        {
            services = new List<object>();
            this.parentSite = parentSite;
        }

        public void AddService(Object service)
        {
            if (service != null)
                services.Add(service);
        }

        #region ISite Members

        public IComponent Component
        {
            get { return null; }
        }

        public IContainer Container
        {
            get { return null; }
        }

        public bool DesignMode
        {
            get { return false; }
        }

        public string Name
        {
            get;
            set;
        }

        #endregion

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            var service = (from s in services where serviceType.IsInstanceOfType(s) select s).FirstOrDefault();

            if (service != null)
                return service;

            if (parentSite != null)
                return parentSite.GetService(serviceType);

            return null;
        }

        #endregion
    }
}
