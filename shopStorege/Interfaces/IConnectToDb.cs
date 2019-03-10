
using System.Threading.Tasks;

namespace storege.Interfaces
{
    public interface IConnectToDb
    {
         Task<string> SetReserve(string token, string productName, int count);
         Task<string> SetDeleteReserve(string token, string productName, int count);
    }
}
