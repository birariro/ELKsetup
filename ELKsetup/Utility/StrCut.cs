using System;
using System.Collections.Generic;
using System.Text;

namespace ELKsetup.Utility
{
    class StrCut
    {
        public static string StrChange(string Original, string strStart, string strEnd, bool flag)
        {
            
            int indexStart = 0;
            StringBuilder resultStr = new StringBuilder();
            if (strStart != null)
            {
                indexStart = Original.IndexOf(strStart);
                if (indexStart == -1) throw new Exception("Parameter(strStart) Check it out"); //no strStart
                indexStart += strStart.Length;
            }
            else indexStart = 0;


            if (strEnd != null)
            {
                int indexEnd = Original.IndexOf(strEnd, indexStart) - indexStart;
                if (indexEnd < 0) throw new Exception("Parameter(strEnd) Check it out"); //no strEnd
                Original = Original.Substring(indexStart, indexEnd);
            }
            else Original = Original.Substring(indexStart);

            if (flag == false) return Original;

            for (int i = 0; i < Original.Length; i++)
            {
                int htmlChar = Convert.ToInt32(Original[i]);
                if (htmlChar >= 48 && htmlChar <= 57 || htmlChar == 46) resultStr.Append(Original[i]);
                else if (resultStr.Length != 0 && (htmlChar < 48 || htmlChar > 57)) break;
            }
            if (resultStr.Length == 0) throw new Exception("All Check it out"); //no Value
            return resultStr.ToString();
        }

    }
}
