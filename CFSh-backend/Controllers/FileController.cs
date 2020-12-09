using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFSh_backend.API;
using CFSh_backend.Helpers;
using CFSh_backend.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CFSh_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly DataContext _context;

        //static IEnumerable<string> Split(string str, int chunkSize)
        //{
        //    List<string> ret = new List<string>();
        //    int index = 0;
        //    int step = str.Length / chunkSize;
        //    while (index < str.Length)
        //    {
        //        if (str.Length - index < step)
        //        {
        //            ret.Add(str.Substring(index));
        //        }
        //        else
        //        {
        //            ret.Add(str.Substring(index, index + step));
        //            index += step;
        //        }

        //    }
        //    return ret;
        //    //return Enumerable.Range(0, str.Length / chunkSize)
        //    //    .Select(i => str.Substring(i * chunkSize, chunkSize));
        //}

        static IEnumerable<string> Split(string str, int maxChunkSize)
        {
            for (int i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
        }

        public FileController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FileContent>> DownloadFile(int Id)
        {
            File file = await _context.Files.FindAsync(Id);
            Dictionary<int, List<int>> shards = new Dictionary<int, List<int>>();
            foreach(FileShard shard in (from fs in _context.FileShards where fs.FileId == file.Id select fs).ToList())
            {
                if (!shards.ContainsKey(shard.Id))
                {
                    shards[shard.Id] = new List<int>();
                }
                shards[shard.Id].Add(shard.ThiccClient);
            }
            SortedDictionary<int, string> data = new SortedDictionary<int, string>();
            foreach (int Key in shards.Keys)
            {
                string shardData = null;
                foreach (int thiccClient in shards[Key])
                {
                    ThiccClient client = _context.ThiccClient.Find(thiccClient);
                    try
                    {
                        shardData = TcpHelper.FromThiccClient(client.IP, client.Port, Key);
                        break;
                    }
                    catch (System.IO.FileNotFoundException) { 
                    }
                }
                if (shardData == null) return NotFound();
                data[Key] = shardData;
            }
            FileContent ret = new FileContent {
                Owner = file.Owner,
                Name = file.Name,
                Content = ""
            };
            foreach (int shardN in data.Keys)
            {
                ret.Content += data[shardN];
            }
            return ret;
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadFile(FileContent fileContent)
        {
            // file.Content / x
            File file = new File
            {
                Owner = fileContent.Owner,
                Name = fileContent.Name
            };
            _context.Files.Add(file);
            await _context.SaveChangesAsync();

            var rnd = new Random();

            foreach (string chunk in Split(fileContent.Content, (fileContent.Content.Length + 3) / 3))
            {
                System.Diagnostics.Debug.WriteLine("Processing string chunk, size: " + chunk.Length);
                int replicas = 0;
                int replicaExpected = 1;
                foreach (ThiccClient client in (from t in _context.ThiccClient where t.FreeSpace > chunk.Length select t).ToList().OrderBy(a => rnd.Next()))
                {
                    //try
                    //{
                    System.Diagnostics.Debug.WriteLine("Selected ThiccClient " + client.Id);
                        FileShard fileShard = new FileShard
                        {
                            FileId = file.Id,
                        };
                        _context.FileShards.Add(fileShard);
                        await _context.SaveChangesAsync();
                    //System.Diagnostics.Debug.WriteLine("Sending data...");
                        TcpHelper.ToThiccClient(client.IP, client.Port, chunk, fileShard.Id);
                    //System.Diagnostics.Debug.WriteLine("Data sent");
                        fileShard.ThiccClient = client.Id;
                        await _context.SaveChangesAsync();
                        replicas += 1;
                    //System.Diagnostics.Debug.WriteLine("Replication is " + replicas);
                        if (replicas >= replicaExpected) break;
                    //}
                    //catch (Exception e)
                    //{
                    //    Console.WriteLine(e.Message);
                    //    throw e;
                    //}
                    
                }
                if (replicas < replicaExpected)
                {
                    throw new Exception("Replication error: " + replicas + " vs " + replicaExpected);
                }
            }
            return Ok();
        }
    }
}
