using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform
{
    public class Utils
    {
        public static string DataFile(string i_file) =>
            System.IO.Path.Combine("C:/projects/Platform/data", i_file);
        
        public static void Swap(ref float i_first, ref float i_second)
        {
            float tmp = i_first;
            i_first = i_second;
            i_second = tmp;
        }
    }
}
