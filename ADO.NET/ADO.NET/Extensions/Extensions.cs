using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO.NET.Extensions
{
    public static class Extensions
    {
        public static bool IsValidChoice(this char symbol)
        {
            if (symbol.ToString().ToLower().Equals("y") || symbol.ToString().ToLower().Equals("n"))
                return true;

            return false;
        }
    }
}
