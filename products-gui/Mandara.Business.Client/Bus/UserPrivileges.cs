using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mandara.Business.Authorization;
using Mandara.Business.Bus.Messages;
using Mandara.Entities.Enums;

namespace Mandara.Business.Bus
{
    public class UserPrivileges : IDisposable
    {
        private BackgroundWorker _privilegesWorker;
        private readonly BusClient _busClient;
        public event EventHandler OnGetPermission;
        private bool _forcePermissionsRequest;
        private static readonly int ForceRequestPermissionsInterval = (int)TimeSpan.FromMinutes(60).TotalSeconds;
        private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);

        public UserPrivileges(BusClient busClient)
        {
            _busClient = busClient;
        }

        public void StartPrivilegeWorker()
        {
            _privilegesWorker = new BackgroundWorker();
            _privilegesWorker.WorkerSupportsCancellation = true;
            _privilegesWorker.DoWork += DoWork;
            _privilegesWorker.RunWorkerAsync();
        }

        public void Stop()
        {
            _privilegesWorker?.CancelAsync();
        }

        public void ForcePermissionsRequest()
        {
            _forcePermissionsRequest = true;
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            if (worker == null)
            {
                return;
            }

            while (!worker.CancellationPending)
            {
                //TODO: We should later change it to send message from server
                for (int i = 0;
                     i < ForceRequestPermissionsInterval && !worker.CancellationPending && !_forcePermissionsRequest;
                     i++)
                {
                    Thread.Sleep(OneSecond);
                }

                _forcePermissionsRequest = false;
                GetPermissionsFromServer();
            }
        }

        public void GetPermissionsFromServer()
        {
            UserData userData = _busClient.GetUserData();

            if (userData == null)
            {
                return;
            }

            PrivilegesArgs user = new PrivilegesArgs
            {
                SystemUserName = userData.UserName,
                CheckMasterPassword = false
            };

            _busClient.GetPermissionSnapshot(user, PermissionCallback);
        }

        private void PermissionCallback(UserDataMessage userDataMessage)
        {
            if (userDataMessage == null)
            {
                return;
            }

            _busClient.SetUserData(userDataMessage.UserData);

            _forcePermissionsRequest = false;
            if (OnGetPermission != null)
            {
                Task.Factory.StartNew(() => OnGetPermission(this, EventArgs.Empty));
            }
        }

        public bool IsCurrentUserAuthorizedTo(PermissionType permissionType)
        {
            UserData userData = _busClient.GetUserData();
            if ((userData != null) && (userData.Permissions != null))
            {
                if (userData.Permissions.Contains(permissionType.GetPermissionId()))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsCurrentUserAuthorizedToPortfolio(int portfolioId, PortfolioPermission permissionType)
        {
            UserData userData = _busClient.GetUserData();

            if ((userData != null) && (userData.PortfolioPermissions != null))
            {
                Func<PortfolioPermissionsData, bool> permissionPredicate = GetPermissionPredicate(permissionType);
                return userData.PortfolioPermissions.Any(p => (p.PortfolioId == portfolioId) && permissionPredicate(p));
            }

            return false;
        }

        private static Func<PortfolioPermissionsData, bool> GetPermissionPredicate(PortfolioPermission permissionType)
        {
            switch (permissionType)
            {
                case PortfolioPermission.CanViewRisk:
                {
                    return permission => permission.CanViewRisk;
                }

                case PortfolioPermission.CanViewPnl:
                {
                    return permission => permission.CanViewPnl;
                }

                case PortfolioPermission.CanAddEditTrades:
                {
                    return permission => permission.CanAddEditTrades;
                }

                case PortfolioPermission.CanUseMasterTool:
                {
                    return permission => permission.CanUseMasterTool;
                }

                case PortfolioPermission.CanAddEditBooks:
                {
                    return permission => permission.CanAddEditBooks;
                }

                case PortfolioPermission.CanAmendBrokerage:
                {
                    return permission => permission.CanAmendBrokerage;
                }

                default:
                {
                    throw new ArgumentOutOfRangeException("permission");
                }
            }
        }

        public void Dispose()
        {
            Stop();
            _privilegesWorker?.Dispose();
        }
    }
}