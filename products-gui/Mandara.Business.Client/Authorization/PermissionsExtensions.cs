using System.Collections.Generic;
using System.Linq;
using Mandara.Entities;

namespace Mandara.Business.Authorization
{
    public static class PermissionsExtensions
    {
        public static bool Contains(this IEnumerable<Permission> perms, PermissionType permissionType)
        {
            return perms.Any(x => x.PermissionId == permissionType.GetPermissionId());
        }

        public static void Add(this List<Permission> perms, PermissionType permissionType)
        {
            perms.Add(new Permission { PermissionId = permissionType.GetPermissionId() });
        }

        public static int GetPermissionId(this PermissionType permissionType)
        {
            var memInfo = typeof(PermissionType).GetMember(permissionType.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(EntityIdAttribute), false);

            return ((EntityIdAttribute)attributes[0]).Id;
        }
    }
}