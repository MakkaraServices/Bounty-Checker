using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BountyChecker
{
    public class Connection
    {
        public enum ConnectionSpec
        {
            Ethereum,
            BitcoinInputInput,
            BitcoinInputOutput
        }
        public bool Result { get; set; }
        public string Address { get; set; }

        public ConnectionSpec ConnectionType { get; set; }

        public string ConnectionDescription
        {

             get
            {
                switch (ConnectionType)
                {
                    case ConnectionSpec.Ethereum:
                        return "Ethereum";
                    case ConnectionSpec.BitcoinInputInput:
                        return "BitcoinInputInput";                        
                    case ConnectionSpec.BitcoinInputOutput:
                        return "BitcoinInputOutput";
                    default:
                        return "Wtf?";
                }
            }
        }  

        public Connection(bool result, string address)
        {
            Result = result;
            Address = address;
            ConnectionType = ConnectionSpec.Ethereum;
        }

        public Connection(bool result, string address, ConnectionSpec connectionType)
        {
            Result = result;
            Address = address;
            ConnectionType = connectionType;
        }
    }
}
