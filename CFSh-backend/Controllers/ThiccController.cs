using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CFSh_backend.Helpers;
using CFSh_backend.Model;
using Microsoft.AspNetCore.Authorization;

namespace CFSh_backend.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ThiccController : ControllerBase
    {
        private readonly DataContext _context;

        public ThiccController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Server
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ThiccClient>>> GetThiccClient()
        {
            return await _context.ThiccClient.ToListAsync();
        }

        // GET: api/Server/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ThiccClient>> GetThiccClient(int id)
        {
            var thiccClient = await _context.ThiccClient.FindAsync(id);

            if (thiccClient == null)
            {
                return NotFound();
            }

            return thiccClient;
        }

        // PUT: api/Server/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutThiccClient(int id, ThiccClient thiccClient)
        {
            if (id != thiccClient.Id)
            {
                return BadRequest();
            }

            _context.Entry(thiccClient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ThiccClientExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Thicc
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<ThiccClient>> PostThiccClient(ThiccClient thiccClient)
        {
            System.Diagnostics.Debug.WriteLine("POST reqest with " + thiccClient.Id);
            _context.ThiccClient.Add(thiccClient);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetThiccClient", new { id = thiccClient.Id }, thiccClient);
        }

        // DELETE: api/Server/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ThiccClient>> DeleteThiccClient(int id)
        {
            var thiccClient = await _context.ThiccClient.FindAsync(id);
            if (thiccClient == null)
            {
                return NotFound();
            }

            _context.ThiccClient.Remove(thiccClient);
            await _context.SaveChangesAsync();

            return thiccClient;
        }

        private bool ThiccClientExists(int id)
        {
            return _context.ThiccClient.Any(e => e.Id == id);
        }
    }
}
