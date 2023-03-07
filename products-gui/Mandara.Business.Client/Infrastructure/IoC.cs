using System.Linq;
using Ninject;

namespace Mandara.Business
{
    public static class IoC
    {
        private static IKernel _kernel;

        public static IKernel Kernel
        {
            get { return _kernel; }
        }

        public static T Get<T>()
        {
            return _kernel.Get<T>();
        }

        public static void Initialize(IKernel kernel)
        {
            _kernel = kernel;
        }
    }
}
