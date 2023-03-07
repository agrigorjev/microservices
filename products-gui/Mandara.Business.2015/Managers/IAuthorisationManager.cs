using Mandara.Entities;
using Mandara.Extensions.Option;
using System.Collections.Generic;

namespace Mandara.Business.Managers
{
    /**
     * <summary>
     * IAuthorisationManager interface was introduced to provide data access to user configuration data.
     *  Interface responsibilities should be limited to accessing and editing user accounts and permissioons
     * <summary>
     */
    public interface IAuthorisationManager
    {
        /**
         * <summary>
         * GetUsers returns a collection of all users currently set up in the system
         * <summary>
         */
        IEnumerable<User> GetUsers();
        TryGetResult<User> TryGetUserForAlias(string userAlias);
    }
}
