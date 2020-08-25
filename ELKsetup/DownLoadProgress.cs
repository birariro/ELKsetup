using System;
using System.Collections.Generic;
using System.Text;

namespace ELKsetup
{
    class DownLoadProgress
    {

        const char _block = '#';
       
        public string WriteProgressBar(int percent)
        {
            string result = "";
            result+="[";
            var p = (int)((percent / 10f) + .5f);
            for (var i = 0; i < 10; ++i)
            {
                if (i >= p)
                    result += ' ';
                else
                    result += _block;
            }
            result += ($"] {percent}%");
            return result;
        }
    }
}
