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
    [Route("api/posts")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly ContentOursourceContext _context;
        

        public PostsController(ContentOursourceContext context)
        {
            _context = context;
        }

        [HttpPost("WriterPost")]
        public async Task<ActionResult<List<TblPosts>>> GetWriterPost([FromBody]UsernameDTO usernameDTO)
        {
            List<TblUsersHavingPosts> list = _context.TblUsersHavingPosts
                .FromSqlRaw("select * from TblUsersHavingPosts")
                .ToList<TblUsersHavingPosts>();
            List<TblPosts> listPost = _context.TblPosts
                .FromSqlRaw("select * from TblPosts")
                .ToList<TblPosts>();

            List<int> listPostId = new List<int>();
            List<TblPosts> listPostResponse = new List<TblPosts>();

            for (int i = 0; i < list.Count; i++)
            {
                TblUsersHavingPosts current = list[i];
                if (usernameDTO.Username.Equals(current.Username))
                {

                    listPostId.Add(current.PostId);


                }
            }

           

            for (int i = 0; i < listPostId.Count; i++)
            {
                int currentId = listPostId[i];
                for (int j = 0; j < listPost.Count; j++)
                {
                    TblPosts currentPost = listPost[j];
                    if (currentPost.Id != currentId)
                    {
                        listPostResponse.Add(currentPost);
                    }

                }
            }
            return listPostResponse;


               

            }


        [HttpGet("TranslatePost")]
        public async Task<ActionResult<List<TblPosts>>> GetTranslatePost()
        {
            List<TblPosts> translatePostList = _context.TblPosts
                .FromSqlRaw("select * from TblPosts where PostType = 'Translate'").ToList<TblPosts>();

            if (translatePostList.Count > 0)
            {
                return translatePostList;
            }

            return BadRequest();
        }

        [HttpGet("DesignPost")]
        public async Task<ActionResult<List<TblPosts>>> GetDesignPost()
        {
            List<TblPosts> designPostList = _context.TblPosts
                .FromSqlRaw("select * from TblPosts where PostType = 'Design'").ToList<TblPosts>();

            if (designPostList.Count > 0)
            {
                return designPostList;
            }

            return BadRequest();
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
