using System;
using storege;

namespace Lbda
{
    class Program
    {
        static void Main(string[] args)
        {
            var token = "qazWSX1";
            var productName = "Table";
            var count = 10;

            var ShopStorage = new ShopStorage(token);
            var res = ShopStorage.Reserve(productName, count);
            Console.WriteLine(res.Result);
            Console.ReadKey();
        }
    }
}
