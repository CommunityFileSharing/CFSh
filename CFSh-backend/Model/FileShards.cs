using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFSh_backend.Model
{
    public class FileShards
    {
        public int Id { get; set; }
        public string FileId { get; set; }
        public int ThiccClient { get; set; }
        public int ShardId { get; set; }
    }
}
