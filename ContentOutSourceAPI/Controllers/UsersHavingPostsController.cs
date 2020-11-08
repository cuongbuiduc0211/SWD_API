using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContentOutSourceAPI.Models;
using ContentOutSourceAPI.DTO;

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

        [HttpPost("requestedPost")]
        public async Task<ActionResult<TblUsersHavingPosts>> AddRequestedPost(RequestedPost requestedPost)
        {
            TblUsersHavingPosts tblUsersHavingPosts = new TblUsersHavingPosts();
            tblUsersHavingPosts.Username = requestedPost.Username;
            tblUsersHavingPosts.PostId = requestedPost.PostId;
            tblUsersHavingPosts.Status = requestedPost.Status;
            List<TblUsersHavingPosts> listAccepted = _context.TblUsersHavingPosts
                .FromSqlRaw("select * from TblUsersHavingPosts where Username = {0} and Status = 'accepted'", tblUsersHavingPosts.Username)
                .ToList<TblUsersHavingPosts>();
            if (listAccepted.Count > 0)
            {
                return BadRequest();
            }
            else
            {
                _context.TblUsersHavingPosts.Add(tblUsersHavingPosts);
            }
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetTblUsersHavingPosts", new { id = tblUsersHavingPosts.Id }, tblUsersHavingPosts);
        }

        [HttpPost("removeRequestedPost")]
        public async Task<ActionResult<TblUsersHavingPosts>> RemoveRequestedPost(RequestedPost requestedPost)
        {
            TblUsersHavingPosts tblUsersHavingPosts = new TblUsersHavingPosts();
            tblUsersHavingPosts.Username = requestedPost.Username;
            tblUsersHavingPosts.PostId = requestedPost.PostId;
            tblUsersHavingPosts.Status = requestedPost.Status;
            List<TblUsersHavingPosts> alreadyRequestedPost = _context.TblUsersHavingPosts
                .FromSqlRaw("select * from TblUsersHavingPosts where Username = {0} and PostId = {1} " +
                "and Status = 'requested'", tblUsersHavingPosts.Username, tblUsersHavingPosts.PostId)
                .ToList<TblUsersHavingPosts>();
            if (alreadyRequestedPost.Count > 0)
            {
                _context.TblUsersHavingPosts.RemoveRange(alreadyRequestedPost);
            }
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetTblUsersHavingPosts", new { id = tblUsersHavingPosts.Id }, tblUsersHavingPosts);

        }

        [HttpPost("requestedPosts")]
        public async Task<ActionResult<List<TblPosts>>> GetRequestedPosts(UsernameDTO usernameDTO)
        {
            List<TblPosts> list = _context.TblPosts
                .FromSqlRaw("select * from tblPosts where Id in " +
                "(select PostId from tblUsersHavingPosts where Username = {0} and Status = 'requested')", usernameDTO.Username)
                .ToList<TblPosts>();
            if (list.Count > 0)
            {
                return list;
            }
            return null;
        }


        [HttpPost("acceptedPosts")]
        public async Task<ActionResult<List<TblPosts>>> GetAcceptedPosts(UsernameDTO usernameDTO)
        {

            List<TblUsersHavingPosts> listRequested = _context.TblUsersHavingPosts
                .FromSqlRaw("select * from tblUsersHavingPosts where Username = {0} and Status = 'requested'", usernameDTO.Username)
                .ToList<TblUsersHavingPosts>();
            List<TblPosts> listAccepted = _context.TblPosts
                .FromSqlRaw("select * from tblPosts where Id in " +
                "(select PostId from tblUsersHavingPosts where Username = {0} and Status = 'accepted')", usernameDTO.Username)
                .ToList<TblPosts>();
            if (listAccepted.Count > 0)
            {
                _context.TblUsersHavingPosts.RemoveRange(listRequested);
                await _context.SaveChangesAsync();
                return listAccepted;

            }
            return null;
        }

        [HttpPost("companyPosts")]
        public async Task<ActionResult<List<TblPosts>>> GetCompanyPosts(UsernameDTO usernameDTO)
        {
            List<TblPosts> listCompanyPosts = new List<TblPosts>();
            List<TblUsersHavingPosts> listCreatedPosts = _context.TblUsersHavingPosts
                .FromSqlRaw("select PostId from tblUsersHavingPosts where Username = {0} and Status = 'created'", usernameDTO.Username)
                .ToList<TblUsersHavingPosts>();
           for (int i = 0; i < listCreatedPosts.Count; i++)
           {
                TblUsersHavingPosts curCreatedPost = listCreatedPosts[i];
                TblPosts companyPost = _context.TblPosts.Find(curCreatedPost);
                listCompanyPosts.Add(companyPost);
           }       
            return listCompanyPosts;
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
