using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAZWSaveTool
{
    public class Verifier
    {
        public static bool Verify(byte[] signedData, out byte[] verifiedUnsignedData) 
        {
            verifiedUnsignedData = signedData;
            return true;
        }
    }
}
