using Xunit;
using storege;
using System.Collections.Generic;
using System.Threading;

namespace XUnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var answer = true;
            List<Thread> threadList = new List<Thread>();

            for (var ii = 0; ii < 10; ++ii) // »нициализируем 10 потоков дл€ пользователей
            {
                threadList.Add(new Thread(new ParameterizedThreadStart(fornewthread)));
            }       

            for (var ii = 0; ii < 10; ++ii) // «апускаем все 10 пользователей дл€ бронировани€
            {
                threadList[ii].Start(ii + 1);
            }       

            Thread.Sleep(2000);
            Assert.True(answer);
        }

        public static void fornewthread(object numUser)
        {
            ShopStorage user = new ShopStorage($"qazWSX{(int)numUser}"); // »нициализируем пользовател€ по Token

            for (var ii = 0; ii < 100; ++ii) // Ѕронируем указанное количество товара подр€д 
            {
               string ss = user.Reserve("Table", 1).Result;
                if (ss.Equals("out of stock")) //“овар закончилс€
                {
                    break;
                }
            }
        }
    }
}


