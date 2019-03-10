using storege.Interfaces;
using System;
using System.Threading.Tasks;

namespace storege
{
    public class ShopStorage
    {
        IConnectToDb _connect;
        string _token;

        public ShopStorage(string token)
        {
            _connect = new ConnectToDb();
            _token = token;
        }

        /**  принимает на вход 
         *  @string productName - Имя продукта строкой
         *  @int count - Количество продукта желаемого для брони
         *  
         *  @return Task<string> - Строка ответа возвращает результаты:
         *  "out of stok" - товара нет в наличии в указанном количестве
         *  "sucsess update" - товар уже был забронированн и бронь обновлена с учетом нового запроса
         *  "sucsess insert" - запрись о брони товара создана
         *   остальное коды ошибок...
         * */
        public async Task<string> Reserve(string productName, int count) 
        {
            try
            {
                return await _connect.SetReserve(_token, productName, count);
            }
            catch (Exception ex)
            {
                return await Task.FromResult($"Exceptions: {ex}");
            }
        }

        /**  принимает на вход 
        *  @string productName - Имя продукта строкой
        *  @int count - Количество продукта желаемого для снятия из брони:
        **  0 или число более числа имеющейся брони - Снять всю бронь
        *  
        *  @return Task<string> - число снятой брони
        * */
        public async Task<string> DeleteReserve(string productName, int count)
        {
            try
            {
                return await _connect.SetDeleteReserve(_token, productName, count);
            }
            catch (Exception ex)
            {
                return await Task.FromResult($"Exceptions: {ex}");
            }
        }

    }
}
