using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFSh_backend.Model
{
    public class File
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Owner { get; set; }
    }
}
