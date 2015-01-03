using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ClientCore
{
    public class MyTypeDescriptorContext : ITypeDescriptorContext
    {
        IServiceProvider serviceProvider;
        //Object instance;
        PropertyDescriptor descriptor;

        public MyTypeDescriptorContext(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public MyTypeDescriptorContext(IServiceProvider serviceProvider, PropertyDescriptor descriptor)
            : this(serviceProvider)
        {
            //this.instance = instance;
            this.descriptor = descriptor;
        }

        #region ITypeDescriptorContext Members

        public IContainer Container
        {
            get
            {
                //throw new NotImplementedException(); 
                return null;
            }
        }

        public object Instance
        {
            get { return null; }
        }

        public void OnComponentChanged()
        {
            //throw new NotImplementedException();
        }

        public bool OnComponentChanging()
        {
            //throw new NotImplementedException();
            return true;
        }

        public PropertyDescriptor PropertyDescriptor
        {
            get { return descriptor; }
        }

        #endregion

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            return serviceProvider.GetService(serviceType);
        }

        #endregion
    }
}
