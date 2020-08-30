using System;
using System.IO;

namespace ELKsetup
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("1 : Download\n" +
                "2 : Setup\n" +
                "3 : Setting\n" +
                "4 : Start\n");


            Console.Write("input : ");
            string input = Console.ReadLine();


            if (input.Equals("1"))
            {
                Console.WriteLine("### ELK Download Start ###");
                ELK_DownLoad();
            }
            else if (input.Equals("2"))
            {
                Console.WriteLine("### ELK Setup ###");
                ELK_setup();
            }
            else if (input.Equals("3"))
            {
                Console.WriteLine("### ELK Setting ###");
                ELK_setting();
            }else if(input.Equals("4"))
            {
                Console.WriteLine("### ELK Start ###");
                ELK_Start();
            }


            Console.ReadLine();
        }
        static string DownloadDirChack()
        {
            string path = System.IO.Directory.GetCurrentDirectory();
            path += "\\DownloadELK";
            Console.WriteLine(path);

            DirectoryInfo dirChack = new DirectoryInfo(path);
            if (dirChack.Exists == false) dirChack.Create(); //폴더가없다면 생성

            return path;
        }

        static async void ELK_DownLoad()
        {
            ELKDownload eLKDownload = new ELKDownload();
            await eLKDownload.DownLoad();
        }
        static void ELK_setup()
        {
            string path = System.IO.Directory.GetCurrentDirectory();

            TerminalCommand.Command($"gnome-terminal -- bash -ic 'cd $HOME; cd {path}; " +
                $"apt-get install -y openjdk-8-jdk;" +
                $"dpkg -i elasticsearch-7.1.0-amd64.deb; " +
                $"dpkg -i logstash-7.1.0.deb;" +
                $"dpkg -i kibana-7.1.0-amd64.deb;" +
                $"dpkg -i filebeat-7.1.0-amd64.deb;" +
                $"bash'");           
        }

        static void ELK_setting()
        {
            Setting.ELKSetting();
        }

        static void ELK_Start()
        {
            TerminalCommand.Command($"gnome-terminal -- bash -ic 'cd $HOME; " +
                $"service elasticsearch restart;" +
                $"service kibana restart; " +
                $"service filebeat restart;" +
                $"bash'");
        }




    }
}
