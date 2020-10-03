using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ELKsetup
{
    class ActiveStart
    {
        public void ELK()
        {
            string[] checkList = new string[] { "elasticsearch", "logstash", "kibana", "filebeat" };

            for(byte i = 0; i < checkList.Length; i++)
            {

                bool breakFlag = true;
                bool nameCheck = true;
                do
                {

                    Console.WriteLine($"{checkList[i]} 설정 재시도중");
                    TerminalCommand.Command($"sudo  service {checkList[i]} status > /etc/setting{checkList[i]}.txt ; exit");
                    Thread.Sleep(1000);
                    string[] settingArr = File.ReadAllLines(@$"/etc/setting{checkList[i]}.txt");


                    for (int ii = 0; ii < settingArr.Length; ii++)
                    {
                        if (settingArr[ii].Contains(checkList[i]))
                        {
                            Console.WriteLine("> "+settingArr[ii]);
                            nameCheck = false;
                        }
                        if (settingArr[ii].Contains("Active: active (running)"))
                        {
                            Console.WriteLine(">>" +settingArr[ii]);
                            breakFlag = false;
                            break;
                        }
                        else if(settingArr[ii].Contains("Active:"))
                        {
                            nameCheck = true;
                            breakFlag = true;
                            TerminalCommand.Command($"sudo  service {checkList[i]} start && exit;");
                            break;
                        }
                        
                        
                    }
                    Task.Delay(3000);

                } while (breakFlag || nameCheck);

            }
        }
    }
}
