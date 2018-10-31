using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BountyChecker
{
    public class Connection
    {
        public bool Result { get; set; }
        public string Address { get; set; }

        public Connection(bool result, string address)
        {
            Result = result;
            Address = address;
        }
    }
}
