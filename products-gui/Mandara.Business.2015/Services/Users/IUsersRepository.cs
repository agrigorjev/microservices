using System.Collections.Generic;
using Mandara.Entities;
using Optional;

namespace Mandara.Business.Services.Users
{
    public interface IUsersRepository
    {
        Option<User> TryGetUser(string name);
        Option<User> TryGetUser(int id);
        ICollection<User> GetUsers();
        void SaveUser(User toSave);
    }
}