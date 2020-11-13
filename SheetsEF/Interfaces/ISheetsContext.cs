using System.Threading.Tasks;

namespace SheetsEF.Interfaces
{
    public interface ISheetsContext
    {
        Task<bool> GetDataFromSheetsAsync(string propName);
        bool BackupDataToSheets();
    }
}
