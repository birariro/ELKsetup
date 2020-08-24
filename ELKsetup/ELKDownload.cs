using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ELKsetup
{
    class ELKDownload
    {
        public int[] AllDownLoadStatus = new int[4];
        public string[] AllDownLoadName = new string[] {"elasticsearch","logstash","kibana","filebeat"};
        public int selectStatus { get; set; }
        public async Task DownLoad()
        {
           
            ELKWebDownLoad("https://artifacts.elastic.co/downloads/elasticsearch/elasticsearch-7.1.0-amd64.deb", @"D:\ProjectPath\ELK\DownloadTest\elasticsearch" , 0);
            ELKWebDownLoad("https://artifacts.elastic.co/downloads/logstash/logstash-7.1.0.deb", @"D:\ProjectPath\ELK\DownloadTest\logstash",1);
            ELKWebDownLoad("https://artifacts.elastic.co/downloads/kibana/kibana-7.1.0-amd64.deb", @"D:\ProjectPath\ELK\DownloadTest\kibana",2);
            ELKWebDownLoad("https://artifacts.elastic.co/downloads/beats/filebeat/filebeat-7.1.0-amd64.deb", @"D:\ProjectPath\ELK\DownloadTest\filebeat",3);
            

            Console.ReadLine();
            
           
        }

        private async Task<bool> ELKWebDownLoad(string url ,string path , int selectValue)
        {
            selectStatus = selectValue; // Bug
            string name = url.Split("/")[4];
            Uri uri = new Uri(url);
            Console.WriteLine("DownLoad Start : "+name);
            try
            {
                
                WebClient webClient = new WebClient();
                webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                webClient.DownloadFileAsync(uri, path);
                webClient.Dispose();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            int thisStatus = e.ProgressPercentage;
            
            AllDownLoadStatus[selectStatus] = thisStatus;

            Console.WriteLine($"{AllDownLoadName[selectStatus]} :  {AllDownLoadStatus[selectStatus]} %");
        }

       

    }
}
