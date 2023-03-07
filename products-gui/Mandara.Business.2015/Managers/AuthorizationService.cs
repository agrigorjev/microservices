using System;
using System.Collections.Generic;
using System.Linq;
using Mandara.Business.Managers;
using Mandara.Entities;

namespace Mandara.Business.Authorization
{
    public static class AuthorizationService
    {
        private static readonly AuthorizationManager _authManager = new AuthorizationManager();

        public static AuthorizationManager AuthorizationManager { get { return _authManager; } }

        static AuthorizationService()
        {
        }

        public static bool IsUserAuthorizedTo(User user, PermissionType permissionType)
        {
            if (user == null || user.Groups == null)
                return false;

            if (_authManager.GetUserPermissions(user).Contains(permissionType))
                return true;

            return false;
        }
    }
}