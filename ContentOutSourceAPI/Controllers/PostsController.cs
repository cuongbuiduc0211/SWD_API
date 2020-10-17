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
    [Route("api/posts")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly ContentOursourceContext _context;

        public PostsController(ContentOursourceContext context)
        {
            _context = context;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TblPosts>>> GetTblPosts()
        {
            return await _context.TblPosts.ToListAsync();
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TblPosts>> GetTblPosts(int id)
        {
            var tblPosts = await _context.TblPosts.FindAsync(id);

            if (tblPosts == null)
            {
                return NotFound();
            }

            return tblPosts;
        }

        // PUT: api/Posts/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTblPosts(int id, TblPosts tblPosts)
        {
            if (id != tblPosts.Id)
            {
                return BadRequest();
            }

            _context.Entry(tblPosts).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TblPostsExists(id))
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

        // POST: api/Posts
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<TblPosts>> PostTblPosts(TblPosts tblPosts)
        {
            _context.TblPosts.Add(tblPosts);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TblPostsExists(tblPosts.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTblPosts", new { id = tblPosts.Id }, tblPosts);
        }

        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TblPosts>> DeleteTblPosts(int id)
        {
            var tblPosts = await _context.TblPosts.FindAsync(id);
            if (tblPosts == null)
            {
                return NotFound();
            }

            _context.TblPosts.Remove(tblPosts);
            await _context.SaveChangesAsync();

            return tblPosts;
        }

        private bool TblPostsExists(int id)
        {
            return _context.TblPosts.Any(e => e.Id == id);
        }
    }
}
