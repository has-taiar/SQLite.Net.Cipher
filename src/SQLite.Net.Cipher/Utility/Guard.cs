using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite.Net.Cipher.Utility
{
    internal class Guard
    {
        public static void CheckForNull(object input, string errorMessage)
        {
            if (input == null)
                throw new ArgumentNullException(errorMessage);
        }
    }
}
