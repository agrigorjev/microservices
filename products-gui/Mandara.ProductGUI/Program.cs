using System;
using System.Threading;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Mandara.Business;
using Mandara.Business.Authorization;
using Mandara.Business.Bus;
using Mandara.ProductGUI.Log;
using Ninject;
using Ninject.Extensions.Logging;

namespace Mandara.ProductGUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var kernel = CreateKernel();
            IoC.Initialize(kernel);
            BusClientProfile.InitAutomapper();

            AppDomain.CurrentDomain.UnhandledException += OnCurrentDomain_UnhandledException;
            Application.ThreadException += OnApplication_ThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ProductListForm());
        }

        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();

            kernel.Load(new LogModule(), new MainModule());

            return kernel;
        }

        private static void OnApplication_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            OnException(e.Exception);
        }

        private static void OnCurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception;

            if (exception != null)
                OnException(exception);
        }

        private static void OnException(Exception exception)
        {
            string errorText = "\r\n\r\nError Details:\r\n";

            while (exception != null)
            {
                errorText += exception.Message + "\r\n";

                exception = exception.InnerException;
            }


            XtraMessageBox.Show("Product Management Tool encounter an error and will be closed." + errorText,
                                "Product Management Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Application.Exit();
        }
    }
}