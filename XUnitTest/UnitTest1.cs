using Xunit;
using storege;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace XUnitTest
{
    public class UnitTest1
    {
        private static bool _answer;
        private static int _end;
        
        [Fact]
        public void Test1()
        {
            _answer = false;
            _end = 0;
            List<Thread> threadList = new List<Thread>();

            for (var ii = 0; ii < 10; ++ii) // �������������� 10 ������� ��� �������������
            {
                threadList.Add(new Thread(new ParameterizedThreadStart(StartNewUserThread)));
            }       

            for (var ii = 0; ii < 10; ++ii) // ��������� ��� 10 ������������� ��� ������������
            {
                threadList[ii].Start(ii);
            }
            while (_end != 10);
            Assert.True(_answer);
            
        }

        private static void StartNewUserThread(object id)
        {
            ShopStorage sp = new ShopStorage((int)id); // �������������� ������������ �� id

            for (var ii = 0; ii < 100; ++ii) // ��������� ��������� ���������� ������ ������ 
            {
               string ss = sp.Reserve("Table", 1).Result;
                if (ss.Equals("out of stock"))//����� ���������� 
                {
                    break;
                }
                else if (!ss.Equals("success update") && !ss.Equals("success insert")) //�������� ������ ������ �����, ���� �� �������
                {
                    _answer = false;
                    break;
                }
            }
            _answer = true;
            ++_end;
        }
    }
}


