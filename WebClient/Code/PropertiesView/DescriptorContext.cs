using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace WebClient
{
    public class DescriptorContext : ITypeDescriptorContext
    {
        IServiceProvider serviceProvider;
        Object instance;
        PropertyDescriptor descriptor;

        public DescriptorContext(IServiceProvider serviceProvider, Object instance, PropertyDescriptor descriptor)
        {
            this.serviceProvider = serviceProvider;
            this.instance = instance;
            this.descriptor = descriptor;
        }

        //public TypeConverter PropertyConverter { get; set; }

        #region ITypeDescriptorContext Members

        public IContainer Container
        {
            get { return null; }
        }

        public object Instance
        {
            get { return instance; }
        }

        public void OnComponentChanged()
        { }

        public bool OnComponentChanging()
        {
            return true;
        }

        public PropertyDescriptor PropertyDescriptor
        {
            get { return descriptor; }
        }

        #endregion

        #region IServiceProvider Members

        object IServiceProvider.GetService(Type serviceType)
        {
            return serviceProvider.GetService(serviceType);
        }

        #endregion
    }
}
