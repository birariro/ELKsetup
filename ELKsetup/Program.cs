using System;
using System.Threading.Tasks;

namespace ELKsetup
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("### ELK Download Start ###");
            ELK_DownLoad();
        }

        

        static async void ELK_DownLoad()
        {
            ELKDownload eLKDownload = new ELKDownload();
            await eLKDownload.DownLoad();
        }

    }
}
