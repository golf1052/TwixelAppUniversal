using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwixelAPI;

namespace TwixelAppUniversal
{
    public class AppConstants
    {
        public static Twixel Twixel;

        public enum StreamQuality
        {
            Source,
            High,
            Medium,
            Low,
            Mobile,
            Chunked
        }
    }
}
