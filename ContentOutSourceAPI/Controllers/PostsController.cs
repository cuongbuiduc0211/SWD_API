using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContentOutSourceAPI.Models;
using ContentOutSourceAPI.DTO;
using System.Collections;

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

        [HttpGet("postsbymonth")]
        public async Task<ActionResult<PostByMonth>> NumberPostByMonth(string month)
        {
            //List<TblPosts> posts = _context.TblPosts.FromSqlRaw("SELECT count(p.Id) AS numberpost FROM tblPosts p " +
            //    "WHERE(Month(p.CreatedDate) = {0}", month).ToList<TblPosts>();
            List<TblPosts> posts = _context.TblPosts.FromSqlRaw("select * from tblPosts where CreatedDate = {0}", month).ToList<TblPosts>();
            PostByMonth postByMonth = new PostByMonth();
            postByMonth.numberPosts = posts.Count;
            postByMonth.month = month;      
            return postByMonth;
        }

        [HttpGet("testFunction/{id}")]
        public IActionResult testFunction(int id)
        {

            return Ok(findListKeyByPostId(id));
        }

        [HttpPost("WriterPost")]
        public async Task<ActionResult<List<PostAndKeyword>>> GetWriterPost([FromBody]UsernameDTO usernameDTO)
        {
            List<TblPosts> listNotHaving = getListPostNotInHavingPosts(usernameDTO.Username);
            List<TblPosts> listResponse = new List<TblPosts>();
            List<PostAndKeyword> listPostAndKeywordResponse = new List<PostAndKeyword>();

            for (int i = 0; i < listNotHaving.Count; i++)
            {
                TblPosts currentPost = listNotHaving[i];
                if (currentPost.PostType.Equals("Writer"))
                {
                    listResponse.Add(currentPost);

                    PostAndKeyword postAndKeyword = new PostAndKeyword();
                    postAndKeyword.Id = currentPost.Id;
                    postAndKeyword.Title = currentPost.Title;
                    postAndKeyword.Description = currentPost.Description;
                    postAndKeyword.CharacterLimit = currentPost.CharacterLimit;
                    postAndKeyword.Amount = currentPost.Amount;
                    postAndKeyword.PostType = currentPost.PostType;
                    postAndKeyword.RelatedDocument = currentPost.RelatedDocument;
                    postAndKeyword.IsPublic = currentPost.IsPublic;
                    postAndKeyword.CreatedDate = currentPost.CreatedDate;
                    postAndKeyword.Status = currentPost.Status;
                    postAndKeyword.listKeywords = findListKeyByPostId(currentPost.Id);

                    listPostAndKeywordResponse.Add(postAndKeyword);

                }
            }

            return listPostAndKeywordResponse;

        }


        //Nhận vào PostID và trả về 1 list các bài Keyword theo PostID
        private List<string> findListKeyByPostId(int id)
        {
            List<TblPostsHavingKeywords> listHavingKeyword = _context.TblPostsHavingKeywords.ToList<TblPostsHavingKeywords>();
            List<TblKeywords> listTblKeywords = _context.TblKeywords.ToList<TblKeywords>();

            List<int?> listKeywordID = new List<int?>();
            List<string> listKeywordResponse = new List<string>();

            // Lấy ra 1 list KeywordID
            for(int i = 0; i < listHavingKeyword.Count; i++)
            {
                TblPostsHavingKeywords current = listHavingKeyword[i];

                if(id == current.PostId)
                {
                    listKeywordID.Add(current.KeywordId);
                }
            }

            // Chạy vòng for để tìm ra List Keyword
            if(listKeywordID != null)
            {
                for(int i = 0; i < listKeywordID.Count; i++)
                {
                    int? currentListID = listKeywordID[i];
                    
                    for(int j = 0; j < listTblKeywords.Count; j++)
                    {
                        TblKeywords currentTblKeyword = listTblKeywords[j];
                        if(currentTblKeyword.Id == currentListID)
                        {
                            listKeywordResponse.Add(currentTblKeyword.Name);
                        }
                    }
                }
            }

            return listKeywordResponse;
        }


        //Hàm này trả về list Post không có trong tblUserHavingPosts
        private List<TblPosts> getListPostNotInHavingPosts(string username)
        {
            List<TblUsersHavingPosts> listHavingPosts = _context.TblUsersHavingPosts
               .FromSqlRaw("select * from TblUsersHavingPosts")
               .ToList<TblUsersHavingPosts>();
            List<TblPosts> listPost = _context.TblPosts
                .FromSqlRaw("select * from TblPosts")
                .ToList<TblPosts>();

            List<int> listPostId = new List<int>();
            List<TblPosts> listPostResponse = listPost;

            //Lấy ra listPostId
            for (int i = 0; i < listHavingPosts.Count; i++)
            {
                TblUsersHavingPosts current = listHavingPosts[i];
                if (username.Equals(current.Username))
                {
                    listPostId.Add(current.PostId);
                }
            }


            for (int i = 0; i < listPostId.Count; i++)
            {
                int currentId = listPostId[i];
                for (int j = 0; j < listPostResponse.Count; j++)
                {
                    TblPosts currentPost = listPostResponse[j];
                    if (currentId == currentPost.Id)
                    {
                        int index = findIndexByIdPodst(currentPost.Id, listPostResponse);
                        if (index != -1)
                        {
                            listPostResponse.RemoveAt(index);
                            break;
                        }

                    }
                }

            }



            return listPostResponse;
        }


        //Hàm này trả về index trong 1 list
        private int findIndexByIdPodst(int id, List<TblPosts> listPostResponse)
        {


            for (int i = 0; i < listPostResponse.Count; i++)
            {
                TblPosts currentPost = listPostResponse[i];

                if (currentPost.Id == id)
                {
                    return i;
                }

            }
            return -1;
        }


        [HttpPost("TranslatePost")]
        public async Task<ActionResult<List<PostAndKeyword>>> GetTranslatePost([FromBody] UsernameDTO usernameDTO)
        {
            List<TblPosts> listNotHaving = getListPostNotInHavingPosts(usernameDTO.Username);
            List<TblPosts> listResponse = new List<TblPosts>();
            List<PostAndKeyword> listPostAndKeywordResponse = new List<PostAndKeyword>();

            for (int i = 0; i < listNotHaving.Count; i++)
            {
                TblPosts currentPost = listNotHaving[i];
                if (currentPost.PostType.Equals("Translate"))
                {
                    listResponse.Add(currentPost);

                    PostAndKeyword postAndKeyword = new PostAndKeyword();
                    postAndKeyword.Id = currentPost.Id;
                    postAndKeyword.Title = currentPost.Title;
                    postAndKeyword.Description = currentPost.Description;
                    postAndKeyword.CharacterLimit = currentPost.CharacterLimit;
                    postAndKeyword.Amount = currentPost.Amount;
                    postAndKeyword.PostType = currentPost.PostType;
                    postAndKeyword.RelatedDocument = currentPost.RelatedDocument;
                    postAndKeyword.IsPublic = currentPost.IsPublic;
                    postAndKeyword.CreatedDate = currentPost.CreatedDate;
                    postAndKeyword.Status = currentPost.Status;
                    postAndKeyword.listKeywords = findListKeyByPostId(currentPost.Id);

                    listPostAndKeywordResponse.Add(postAndKeyword);

                }
            }

            return listPostAndKeywordResponse;

        }

        [HttpPost("DesignPost")]
        public async Task<ActionResult<List<PostAndKeyword>>> GetDesignPost([FromBody] UsernameDTO usernameDTO)
        {
            List<TblPosts> listNotHaving = getListPostNotInHavingPosts(usernameDTO.Username);
            List<TblPosts> listResponse = new List<TblPosts>();
            List<PostAndKeyword> listPostAndKeywordResponse = new List<PostAndKeyword>();

            for (int i = 0; i < listNotHaving.Count; i++)
            {
                TblPosts currentPost = listNotHaving[i];
                if (currentPost.PostType.Equals("Design"))
                {
                    listResponse.Add(currentPost);

                    PostAndKeyword postAndKeyword = new PostAndKeyword();
                    postAndKeyword.Id = currentPost.Id;
                    postAndKeyword.Title = currentPost.Title;
                    postAndKeyword.Description = currentPost.Description;
                    postAndKeyword.CharacterLimit = currentPost.CharacterLimit;
                    postAndKeyword.Amount = currentPost.Amount;
                    postAndKeyword.PostType = currentPost.PostType;
                    postAndKeyword.RelatedDocument = currentPost.RelatedDocument;
                    postAndKeyword.IsPublic = currentPost.IsPublic;
                    postAndKeyword.CreatedDate = currentPost.CreatedDate;
                    postAndKeyword.Status = currentPost.Status;
                    postAndKeyword.listKeywords = findListKeyByPostId(currentPost.Id);

                    listPostAndKeywordResponse.Add(postAndKeyword);

                }
            }

            return listPostAndKeywordResponse;

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
