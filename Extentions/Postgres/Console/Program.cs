using System;
using Hcs.Store;

namespace PostgresCons
{
    class Program
    {
        static void Main(string[] args)
        {
            HomeController item = new HomeController();
            item.GenPostgr();
            Console.WriteLine("-------------------------------------------");
            Console.ReadLine();
        }
    }
}
