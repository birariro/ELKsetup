using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            proc.StartInfo.RedirectStandardOutput = true;
            proc.Start();
            Console.WriteLine(proc.StandardOutput.ReadToEnd());


            //코드 출처 https://askubuntu.com/questions/506985/c-opening-the-terminal-process-and-pass-commands
        }
    }
}
