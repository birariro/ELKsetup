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
            string[] checkList = new string[] { "elasticsearch", "logstash", "filebeat", "kibana" };

            for (byte i = 0; i < checkList.Length; i++) //각 모든 설치파일을 동작시킨다.
            {
                bool breakFlag = true;
                TerminalCommand.Command($"sudo  service {checkList[i]} start && exit;");
                Console.WriteLine("명령 실행 완료 슬립 시작");
                Thread.Sleep(5000); //실행후  대기 너무 빠르게 실행되면 문제가 될 가능성이있다.
                do 
                {
                    TerminalCommand.Command($"sudo  service {checkList[i]} status > /etc/setting{checkList[i]}.txt ; exit");
                    string[] settingArr = File.ReadAllLines(@$"/etc/setting{checkList[i]}.txt");


                    for (int ii = 0; ii < settingArr.Length; ii++)
                    {
                        
                        if (settingArr[ii].Contains("Active: active (running)")) //실행중이라면 종료해라
                        {
                            Console.WriteLine($"{checkList[i]} 실행 확인!");
                            breakFlag = false;
                            break;
                        }
                        else if (settingArr[ii].Contains("Active:"))
                        {
                            breakFlag = true;
                            Console.WriteLine($"{checkList[i]} 실행 대기중...");
                            Thread.Sleep(5000); //실행후  대기
                            break;
                        }


                    }


                } while (breakFlag);
              

            }
        }
    }
}
