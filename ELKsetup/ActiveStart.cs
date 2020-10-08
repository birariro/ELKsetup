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

            for(byte i = 0; i < checkList.Length; i++) //각 모든 설치파일을 동작시킨다.
            {

                bool breakFlag = true;
                //bool nameCheck = true;
                do //실행 성공할때까지 반복
                {

                    
                    TerminalCommand.Command($"sudo  service {checkList[i]} status > /etc/setting{checkList[i]}.txt ; exit");
                    
                    string[] settingArr = File.ReadAllLines(@$"/etc/setting{checkList[i]}.txt");


                    for (int ii = 0; ii < settingArr.Length; ii++)
                    {
                        /*if (settingArr[ii].Contains(checkList[i])) // 이름이 맞는지 왜검사하지?
                        {
                            Console.WriteLine("> "+settingArr[ii]);
                            nameCheck = false;
                        }*/
                        if (settingArr[ii].Contains("Active: active (running)")) //실행중이라면 종료해라
                        {
                            Console.WriteLine($"{checkList[i]} 실행 확인!");
                            breakFlag = false;
                            break;
                        }
                        else if(settingArr[ii].Contains("Active:"))
                        {
                            //nameCheck = true;
                            breakFlag = true;
                            Console.WriteLine($"{checkList[i]} 실행 재시도중");
                            TerminalCommand.Command($"sudo  service {checkList[i]} start && exit;");
                            Thread.Sleep(8000); //실행후 8초 대기
                            break;
                        }
                        
                        
                    }
                    

                } while (breakFlag);

            }
        }
    }
}
