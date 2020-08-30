using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace ELKsetup
{
    public class Setting
    {
        public static void ELKSetting()
        {
           
            Console.WriteLine("Server IP : ");
            string IP = Console.ReadLine();

            Setting_Elasticsearch(IP);
            Setting_Kibana(IP);
            Setting_Filebeat();

        }


        private static void Setting_Elasticsearch(string IP)
        {
            string[] elasticsearchOrigin = File.ReadAllLines(@"/etc/elasticsearch/elasticsearch.yml");
            int elasticLength = elasticsearchOrigin.Length ; 
            string[] elasticsearch = new string[elasticLength];
            for (int i = 0; i < elasticLength; i++)
            {
                if (elasticsearchOrigin[i].Contains("network.host")) elasticsearch[i] = "network.host : [\"localhost\",\"" + IP + "\"]";
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
                else if (KibanaOrigin[i].Contains("server.host")) Kibana[i] = "server.host : "+IP;
                else if (KibanaOrigin[i].Contains("elasticsearch.hosts")) Kibana[i] = "elasticsearch.hosts: [\"http://localhost:9200\"]";

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
            string[] FilebeatAddLog = new string[] { "/var/log/*.log", "/var/log/syslog", "/var/log.apache2.log" }; 
            //Filebeat에서 수집할 로그 들.

            string[] FilebeatOrigin = File.ReadAllLines(@"/etc/filebeat/filebeat.yml");
            string[] Filebeat = new string[FilebeatOrigin.Length];
            for (int i = 0; i < FilebeatOrigin.Length; i++)
            {
                if (FilebeatOrigin[i].Contains("enabled: false")) Filebeat[i] = "enabled: true";
                else if (FilebeatOrigin[i].Contains("/var/log/*.log"))
                {
                    string addLog = "";
                    for(byte ii = 0; ii< FilebeatAddLog.Length; ii++)
                    {
                        addLog += $"    - {FilebeatAddLog[ii]}\n";
                    }
                    Filebeat[i] = addLog;
                    Console.WriteLine(Filebeat[i]);
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
    }
}
