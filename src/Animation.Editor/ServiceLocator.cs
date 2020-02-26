using Animation.Editor.ViewModel;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animation.Editor
{
    public static class ServiceLocator
    {
        static ServiceLocator()
        {
            SimpleIoc.Default.Register<MainViewModel>();
        }

        public static MainViewModel Main
        {
            get
            {
                return SimpleIoc.Default.GetInstance<MainViewModel>();
            }
        }
    }
}
