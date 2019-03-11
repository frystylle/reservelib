
using System.Threading.Tasks;

namespace storege.Interfaces
{
    public interface IRepositoryDb
    {
         Task<string> SetReserve(int idUser, string productName, int count);
    }
}
