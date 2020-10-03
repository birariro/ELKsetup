using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ELKsetup
{
    public class TerminalCommand
    {
        public static void Command(string command)
        {
            
            Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "/bin/bash";
            proc.StartInfo.Arguments = "-c \" " + command + " \"";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true; //데이터 가져오기 안됨 ㅡㅡ
            proc.Start();

            string result = proc.StandardOutput.ReadToEnd();
            Console.WriteLine(result);
           
            
        }
    }
}
