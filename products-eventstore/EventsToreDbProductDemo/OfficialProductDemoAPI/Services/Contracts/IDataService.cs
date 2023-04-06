using EventStore.Client;
using Optional;
using System.Text;

namespace OfficialProductDemoAPI.Services.Contracts
{
    public interface IDataService<T>:IDisposable
    {
        Task InitialLoad();

        List<T> GetList();

        Option<T> GetSingle(string id);

        string StreamName { get; }


       

    }
}
