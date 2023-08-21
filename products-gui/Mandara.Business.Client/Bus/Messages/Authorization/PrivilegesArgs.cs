using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages
{
    using System.Linq;
    using Mandara.Business.Authorization;
    using Mandara.Entities;
    using Mandara.Entities.Enums;

    public class PrivilegesArgs : MessageBase
    {
        public string SystemUserName { get; set; }
        public string MasterPasswordHash { get; set; }
        public bool CheckMasterPassword { get; set; }
    }

    public class UserDataMessage : MessageBase
    {
        public UserData UserData { get; set; }
    }
    public class UserData
    {
        public UserData()
        {
            Permissions = new List<int>();
            Groups = new List<GroupData>();
            PortfolioPermissions = new List<PortfolioPermissionsData>();
        }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool Locked { get; set; }

        public List<int> Permissions { get; set; }

        public List<GroupData> Groups { get; set; }

        public PortfolioData Portfolio { get; set; }

        public List<PortfolioPermissionsData> PortfolioPermissions { get; set; }

        public PasswordChangeReason ForcePasswordChange { get; set; }  

        public static UserData InvalidUser { get { return new UserData {UserId = -1}; } }
    }

    public class GroupData
    {
        public string GroupName { get; set; }

        public int GroupId { get; set; }
    }

    public class PortfolioData
    {
        public int PortfolioId { get; set; }

        public string Name { get; set; }
    }

    public class PortfolioPermissionsData
    {
        public int PortfolioId { get; set; }

        public bool CanViewRisk { get; set; }

        public bool CanViewPnl { get; set; }

        public bool CanAddEditTrades { get; set; }

        public bool CanUseMasterTool { get; set; }

        public bool CanAddEditBooks { get; set; }

        public bool CanAmendBrokerage { get; set; }
    }
}