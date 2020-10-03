using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ELKsetup
{
    public class Setting
    {
        static bool flag = true;
        public static void ELKSetting(string IP)
        {

            Setting_Elasticsearch(IP);
            Setting_Kibana(IP);
            Setting_Filebeat();

            Console.WriteLine("ELK Setting End ");

        }


        private static void Setting_Elasticsearch(string IP)
        {
            string[] elasticsearchOrigin = File.ReadAllLines(@"/etc/elasticsearch/elasticsearch.yml");
            int elasticLength = elasticsearchOrigin.Length ; 
            string[] elasticsearch = new string[elasticLength];
            for (int i = 0; i < elasticLength; i++)
            {
                if (elasticsearchOrigin[i].Contains("network.host")) elasticsearch[i] = "network.host : \"0.0.0.0\"";
                else if (elasticsearchOrigin[i].Contains("cluster.initial_master_nodes")) elasticsearch[i] = "cluster.initial_master_nodes : [\"" + IP + "\"]";
                else elasticsearch[i] = elasticsearchOrigin[i];
            }
            using (StreamWriter outputFile = new StreamWriter(@"/etc/elasticsearch/elasticsearch.yml"))
            {
                foreach (string line in elasticsearch) outputFile.WriteLine(line);
            }
        }
        private static void Setting_Kibana(string IP)
        {

            string[] KibanaOrigin = File.ReadAllLines(@"/etc/kibana/kibana.yml");
            string[] Kibana = new string[KibanaOrigin.Length];
            for (int i = 0; i < KibanaOrigin.Length; i++)
            {
                if (KibanaOrigin[i].Contains("server.port")) Kibana[i] = "server.port : 5601";                
                else if (KibanaOrigin[i].Contains("server.host")) Kibana[i] = "server.host : \"0.0.0.0\"";
                else if (KibanaOrigin[i].Contains("elasticsearch.hosts")) Kibana[i] = "elasticsearch.hosts: [\"http://"+IP+":9200\"]";

                else Kibana[i] = KibanaOrigin[i];
                

            }
            using (StreamWriter outputFile = new StreamWriter(@"/etc/kibana/kibana.yml"))
            {
                foreach (string line in Kibana)
                {
                    outputFile.WriteLine(line);
                }
            }
        }
        private static void Setting_Filebeat()
        {
            string[] FilebeatAddLog = new string[] { "/var/log/*.log\n", "/var/log/syslog\n", "/var/log/apache2.log" }; 
            //Filebeat에서 수집할 로그 들.

            string[] FilebeatOrigin = File.ReadAllLines(@"/etc/filebeat/filebeat.yml");
            string[] Filebeat = new string[FilebeatOrigin.Length];
            for (int i = 0; i < FilebeatOrigin.Length; i++)
            {
                if (i>5&&FilebeatOrigin[i-1].Contains("Change to true to enable this input configuration")) Filebeat[i] = "  enabled: true"; //enabled가 여러개라서 제한을 둔다.
                else if (FilebeatOrigin[i].Contains("/var/log/*.log"))
                {
                    string addLog = "";
                    for(byte ii = 0; ii< FilebeatAddLog.Length; ii++)
                    {
                        addLog += $"    - {FilebeatAddLog[ii]}";
                    }
                    Filebeat[i] = addLog;
                }
                else Filebeat[i] = FilebeatOrigin[i];

            }
            using (StreamWriter outputFile = new StreamWriter(@"/etc/filebeat/filebeat.yml"))
            {
                foreach (string line in Filebeat)
                {
                    outputFile.WriteLine(line);
                }
            }
        }



        public static void XpackSetting()
        {
            Setting_Elasticsearch_Xpack();
        }
        private static void Setting_Elasticsearch_Xpack()
        {
            bool check = true; //세팅이 2번 동작하지않도록 확인 작업
            string[] elasticsearchOrigin = File.ReadAllLines(@"/etc/elasticsearch/elasticsearch.yml");
            for(int i = elasticsearchOrigin.Length-10; i< elasticsearchOrigin.Length;i++)
            {
                if(elasticsearchOrigin[i].Equals("xpack.security.enabled: true") || elasticsearchOrigin[i].Equals("xpack.security.transport.ssl.enabled: true"))
                {
                    check = false; //세팅값이있다면 다시 세팅하지마라
                }
            }
            if(check) //세팅한적이없다면 세팅작업
            {
                using (StreamWriter outputFile = new StreamWriter(@"/etc/elasticsearch/elasticsearch.yml", true))
                {
                    outputFile.WriteLine("xpack.security.enabled: true\nxpack.security.transport.ssl.enabled: true");
                }
                flag = false;
                TerminalCommand.Command($"gnome-terminal -- bash -ic 'cd $HOME; " +
                $"service elasticsearch restart &&" +
                $"service kibana restart &&" +
                $"exit;" +
                $"bash'");
            }

            bool settingFlag = false;
            TerminalCommand.Command($"gnome-terminal -- bash -ic 'cd $HOME; " +
                $"echo 1 > /etc/setting.txt && chmod 777 /etc/setting.txt &&" +
                $"exit;bash'");

            do //패스워드 명령어는 ELK가 실행되고 어느정도 시간차가 필요하다. 무한루프로 게속 시도한다.
            {
                TerminalCommand.Command($"sudo /usr/share/elasticsearch/bin/elasticsearch-setup-passwords interactive -N > /etc/setting.txt ; exit");
                Thread.Sleep(1000);
                string[] settingArr = File.ReadAllLines(@"/etc/setting.txt");
                Console.WriteLine(settingArr.Length);
                for (int i = 0; i < settingArr.Length; i++)
                {
                    Console.WriteLine(settingArr[i]);
                    if (settingArr[i].Contains("passwords for")) //패스워드 세팅이 나온상황이다. 아닌경우 elasticsearch running?
                    {
                        Console.WriteLine("ok");
                        settingFlag = true; //세팅값이있다면 다시 세팅하지마라
                        break;
                    }
                }
                Task.Delay(3000);
                Console.WriteLine("설정 재시도중");
            } while (settingFlag != true);

            if(settingFlag==true)
            {
                TerminalCommand.Command($"gnome-terminal -- bash -ic 'cd $HOME; " +
               $"sudo /usr/share/elasticsearch/bin/elasticsearch-setup-passwords interactive;" +
               $"bash'");
            }
            
            


        }
    }
}
