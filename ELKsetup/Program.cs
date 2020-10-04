using ELKsetup.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ELKsetup
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("1 : Download\n" +
                "2 : Setup\n" +
                "3 : Setting\n" +
                "4 : Start\n" +
                "5 : X-Pack Setting\n");


            Console.Write("input : ");
            string input = Console.ReadLine();
            string version = null;



            switch(input)
            {
                case "1":
                    Console.WriteLine("### ELK Download Start ###");
                    Console.Write("ELK Version Input (Default 7.9.1) : "); //7.9.1
                    version = Console.ReadLine();
                    if (version.Length < 5 || !string.IsNullOrWhiteSpace(version)) version = "7.9.1";
                    ELK_DownLoad(version);
                    break;

                case "2":
                    Console.Write("ELK Version Input (Default 7.9.1) : ");
                    version = Console.ReadLine();
                    if (version.Length < 5 || !string.IsNullOrWhiteSpace(version)) version = "7.9.1";
                    Console.WriteLine("### ELK Setup ###");
                    ELK_setup(version);
                    break;

                case "3":
                    Console.WriteLine("### ELK Setting ###");
                    ELK_setting();
                    break;

                case "4":
                    Console.WriteLine("### ELK Start ###");
                    ELK_Start();
                    break;

                case "5":
                    Console.WriteLine("### Xpack Setting ###");
                    Xpack_setting();
                    break;
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

        static async void ELK_DownLoad(string version)
        {
            ELKDownload eLKDownload = new ELKDownload();
            await eLKDownload.DownLoad(version);
        }
        static void ELK_setup(string version)
        {
            string path = System.IO.Directory.GetCurrentDirectory();
            TerminalCommand.Command($"gnome-terminal -- bash -ic ' " +
                $"apt-get install -y openjdk-8-jdk &&" +
                $"sudo dpkg -i elasticsearch-{version}-amd64.deb && " +
                $"sudo dpkg -i logstash-{version}.deb &&" +
                $"sudo dpkg -i kibana-{version}-amd64.deb &&" +
                $"sudo dpkg -i filebeat-{version}-amd64.deb && exit;" +
                $"bash'");

        }

        static void ELK_setting()
        {
            string[] serverIPList = GetServiceIP(); //서버의 ip를 얻어온다.
            string serverIP;
            if (serverIPList.Length >= 2) serverIP = SelectIP(serverIPList); //ip가 여러개라면 선택할수있게한다.
            else
            {
                if(serverIPList[0]!=null) serverIP = serverIPList[0]; //서버 ip가 하나라면 그대로 간다.
                else //서버 ip를 구하지못하였을때는 사용자에게 입력하게한다.
                {
                    Console.Write("Input IP : ");
                    serverIP= Console.ReadLine();
                }
            }

            Console.WriteLine("Select Servce ip : " + serverIP);
            Setting.ELKSetting(serverIP);
        }

        static void ELK_Start()
        {
            /*TerminalCommand.Command($"gnome-terminal -- bash -ic 'cd $HOME; " +
                $"sudo service elasticsearch start &&" +
                $"sudo service kibana start &&" +
                $"sudo service filebeat start &&" +
                $"sudo service logstash start && exit;'");*/
            ActiveStart activeStart = new ActiveStart();
            activeStart.ELK();



        }
        static void Xpack_setting()
        {
            Setting.XpackSetting();
        }

        static string SelectIP(string[] IPList) //ip중 하나를 선택
        {

            ConsoleKeyInfo keys;
            byte selectPosition = 0; //선택된 위치 
            bool selectEnd = false;
            Console.WriteLine("Input '↑'or '↓' , Choose 'Enter' , Direct input 'ESC' ");
            for (byte i = 0; i < IPList.Length; i++)
            {
                if (i == selectPosition)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("> " + IPList[i]);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("-" + IPList[i]);
                }

            }
            do
            {
                if (Console.KeyAvailable)
                {
                    keys = Console.ReadKey(true);

                    switch (keys.Key)
                    {
                        case ConsoleKey.UpArrow:
                            if (selectPosition > 0)
                            {
                                Console.Clear();
                                Console.WriteLine("Input '↑'or '↓' , Choose 'Enter' , Direct input 'ESC' ");
                                selectPosition--;
                                for (byte i = 0; i < IPList.Length; i++)
                                {
                                    if (i == selectPosition)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("> " + IPList[i]);
                                        Console.ForegroundColor = ConsoleColor.White;
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.White;
                                        Console.WriteLine("- " + IPList[i]);
                                    }
                                }
                            }
                            break;
                        case ConsoleKey.DownArrow:
                            if (selectPosition < (IPList.Length - 1))
                            {
                                Console.Clear();
                                Console.WriteLine("Input '↑'or '↓' , Choose 'Enter' , Direct input 'ESC' ");
                                selectPosition++;
                                for (ushort i = 0; i < IPList.Length; i++)
                                {
                                    if (i == selectPosition)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("> " + IPList[i]);
                                        Console.ForegroundColor = ConsoleColor.White;
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.White;
                                        Console.WriteLine("- " + IPList[i]);
                                    }
                                }
                            }
                            break;
                        case ConsoleKey.Enter:
                            selectEnd = true;
                            break;
                        case ConsoleKey.Escape: //원하는 ip가 없는경우
                            Console.Clear();
                            Console.Write("Input IP : ");
                            return Console.ReadLine();
                    }
                }

            } while (!selectEnd);

            return IPList[selectPosition];
        }
        static string[] GetServiceIP() //서버의 ip를 가져온다.
        {
            TerminalCommand.Command($"sudo ifconfig > /etc/setting.txt ; exit");
            Thread.Sleep(500);
            string[] settingText = File.ReadAllLines(@"/etc/setting.txt");

            List<string> ipListTmp = new List<string>();

            for (int i = 0; i < settingText.Length; i++)
            {
                Console.Clear();
                if (settingText[i].Contains("inet")&& settingText[i].Contains("netmask"))  //모든 ip를 잘라낸다.
                {
                    string ip = StrCut.StrChange(settingText[i], "inet", "netmask", true);
                    ipListTmp.Add(ip);
                }
            }

            string[] ipList = new string[ipListTmp.Count];
            for(short i =0; i < ipListTmp.Count; i++)
            {
                ipList[i] = ipListTmp[i];
            }
            return ipList; //모든 ip 반환
        } 



    }
}
