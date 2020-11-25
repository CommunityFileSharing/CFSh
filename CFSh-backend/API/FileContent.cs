using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFSh_backend.API
{
    public class FileContent
    {
        public string Content { get; set; } // base64 file
        public string Name { get; set; }
        public int Owner { get; set; }
    }
}
