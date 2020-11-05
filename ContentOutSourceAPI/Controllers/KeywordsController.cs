using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContentOutSourceAPI.Models;

namespace ContentOutSourceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeywordsController : ControllerBase
    {
        private readonly ContentOursourceContext _context;

        public KeywordsController(ContentOursourceContext context)
        {
            _context = context;
        }

        // GET: api/Keywords
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TblKeywords>>> GetTblKeywords()
        {
            return await _context.TblKeywords.ToListAsync();
        }

        // GET: api/Keywords/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TblKeywords>> GetTblKeywords(int id)
        {
            var tblKeywords = await _context.TblKeywords.FindAsync(id);

            if (tblKeywords == null)
            {
                return NotFound();
            }

            return tblKeywords;
        }

        // PUT: api/Keywords/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTblKeywords(int id, TblKeywords tblKeywords)
        {
            if (id != tblKeywords.Id)
            {
                return BadRequest();
            }

            _context.Entry(tblKeywords).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TblKeywordsExists(id))
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

        // POST: api/Keywords
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<TblKeywords>> PostTblKeywords(TblKeywords tblKeywords)
        {
            _context.TblKeywords.Add(tblKeywords);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TblKeywordsExists(tblKeywords.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTblKeywords", new { id = tblKeywords.Id }, tblKeywords);
        }

        // DELETE: api/Keywords/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TblKeywords>> DeleteTblKeywords(int id)
        {
            var tblKeywords = await _context.TblKeywords.FindAsync(id);
            if (tblKeywords == null)
            {
                return NotFound();
            }

            _context.TblKeywords.Remove(tblKeywords);
            await _context.SaveChangesAsync();

            return tblKeywords;
        }

        private bool TblKeywordsExists(int id)
        {
            return _context.TblKeywords.Any(e => e.Id == id);
        }
    }
}
