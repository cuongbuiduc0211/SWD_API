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
    public class UsersHavingPostsController : ControllerBase
    {
        private readonly ContentOursourceContext _context;

        public UsersHavingPostsController(ContentOursourceContext context)
        {
            _context = context;
        }

        [HttpGet("requestedPost")]
        public async Task<ActionResult<List<TblUsersHavingPosts>>> GetRequestedPost()
        {
            List<TblUsersHavingPosts> list = _context.TblUsersHavingPosts
                .FromSqlRaw("select p.* from tblPosts p, tblUsersHavingPosts u " +
                "where p.Id = u.PostId and u.Status = 'requested'")
                .ToList<TblUsersHavingPosts>();

            if (list.Count > 0)
            {
                return list;
            }

            return BadRequest();
        }

        [HttpGet("acceptedPost")]
        public async Task<ActionResult<List<TblUsersHavingPosts>>> GetAcceptedPost()
        {
            List<TblUsersHavingPosts> list = _context.TblUsersHavingPosts
                .FromSqlRaw("select p.* from tblPosts p, tblUsersHavingPosts u " +
                "where p.Id = u.PostId and u.Status = 'accepted'")
                .ToList<TblUsersHavingPosts>();

            if (list.Count > 0)
            {
                return list;
            }

            return BadRequest();
        }
        // GET: api/UsersHavingPosts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TblUsersHavingPosts>>> GetTblUsersHavingPosts()
        {
            return await _context.TblUsersHavingPosts.ToListAsync();
        }

        // GET: api/UsersHavingPosts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TblUsersHavingPosts>> GetTblUsersHavingPosts(int id)
        {
            var tblUsersHavingPosts = await _context.TblUsersHavingPosts.FindAsync(id);

            if (tblUsersHavingPosts == null)
            {
                return NotFound();
            }

            return tblUsersHavingPosts;
        }

        // PUT: api/UsersHavingPosts/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTblUsersHavingPosts(int id, TblUsersHavingPosts tblUsersHavingPosts)
        {
            if (id != tblUsersHavingPosts.Id)
            {
                return BadRequest();
            }

            _context.Entry(tblUsersHavingPosts).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TblUsersHavingPostsExists(id))
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

        // POST: api/UsersHavingPosts
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<TblUsersHavingPosts>> PostTblUsersHavingPosts(TblUsersHavingPosts tblUsersHavingPosts)
        {
            _context.TblUsersHavingPosts.Add(tblUsersHavingPosts);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TblUsersHavingPostsExists(tblUsersHavingPosts.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTblUsersHavingPosts", new { id = tblUsersHavingPosts.Id }, tblUsersHavingPosts);
        }

        // DELETE: api/UsersHavingPosts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TblUsersHavingPosts>> DeleteTblUsersHavingPosts(int id)
        {
            var tblUsersHavingPosts = await _context.TblUsersHavingPosts.FindAsync(id);
            if (tblUsersHavingPosts == null)
            {
                return NotFound();
            }

            _context.TblUsersHavingPosts.Remove(tblUsersHavingPosts);
            await _context.SaveChangesAsync();

            return tblUsersHavingPosts;
        }

        private bool TblUsersHavingPostsExists(int id)
        {
            return _context.TblUsersHavingPosts.Any(e => e.Id == id);
        }
    }
}
