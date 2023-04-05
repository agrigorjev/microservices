using Optional;

namespace OfficialProductDemoAPI.Services.Contracts
{
    public interface IDataService<T>:IDisposable
    {
        Task InitialLoad();

        List<T> GetList();

        Option<T> GetSingle(string id);



    }
}
