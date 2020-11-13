using System.Threading.Tasks;

namespace SheetsEF.Interfaces
{
    public interface IData
    {
        T GetData<T>(string key);
        abstract Task UpdatingSheetsAsync();
    }
}
