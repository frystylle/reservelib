using System;
using storege;

namespace Lbda
{
    class Program
    {
        static void Main(string[] args)
        {
            var id = 0;
            var productName = "Table";
            var count = 10;

            var ShopStorage = new ShopStorage(id);
            var res = ShopStorage.Reserve(productName, count);
            Console.WriteLine(res.Result);
            Console.ReadKey();
        }
    }
}
