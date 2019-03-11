using storege.Interfaces;
using System;
using System.Threading.Tasks;

namespace storege
{
    public class ShopStorage
    {
        IRepositoryDb _connect;
        int _id;
        /**  принимает на вход 
         *  @int id - Учетный номер пользователя
         **/
        public ShopStorage(int id)
        {
            _connect = new RepositoryDb();
            _id = id;
        }

        /**  принимает на вход 
         *  @string productName - Имя продукта строкой
         *  @int count - Количество продукта желаемого для брони
         *  
         *  @return Task<string> - Строка ответа возвращает результаты:
         *  "out of stok" - товара нет в наличии в указанном количестве
         *  "success update" - товар уже был забронированн и бронь обновлена с учетом нового запроса
         *  "sucsess insert" - запрись о брони товара создана
         *   "Exceptions: ..." - остальное коды ошибок
         **/
        public async Task<string> Reserve(string productName, int count) 
        {
            try
            {
                return await _connect.SetReserve(_id, productName, count);
            }
            catch (Exception ex)
            {
                return await Task.FromResult($"Exceptions: {ex}");
            }
        }
    }
}
