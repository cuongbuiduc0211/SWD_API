using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContentOutSourceAPI.Models;
using ContentOutSourceAPI.Validators;
using ContentOutSourceAPI.DTO;

namespace ContentOutSourceAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ContentOursourceContext _context;
        private readonly UsersValidator validator = new UsersValidator();
        public UsersController(ContentOursourceContext context)
        {
            _context = context;
        }

        [HttpGet("totalPostAndUser")]
        public async Task<ActionResult<NumberDTO>> GetTotalPostAndUser()
        {
            NumberDTO numberDTO = new NumberDTO();
            numberDTO.totalPost = _context.TblPosts.Count();
            numberDTO.totalFreelancer = _context.TblUsers
                .FromSqlRaw("select Username from TblUsers where RoleId = 2").Count();
            numberDTO.totalCompany = _context.TblUsers
                .FromSqlRaw("select Username from TblUsers where RoleId = 3").Count();
            return numberDTO;
        }


        [HttpGet("listFreelancer")]
        public async Task<ActionResult<List<TblUsers>>> ListFreelancer() {
            List<TblUsers> listFreelancer = _context.TblUsers
                .FromSqlRaw("select * from TblUsers where RoleId = 2").ToList<TblUsers>();
            
            if (listFreelancer.Count > 0)
            {
                return listFreelancer;
            }

            return BadRequest();
        }

        [HttpGet("listCompany")]
        public async Task<ActionResult<List<TblUsers>>> listCompany()
        {
            List<TblUsers> listCompany = _context.TblUsers
                .FromSqlRaw("select * from TblUsers where RoleId = 3").ToList<TblUsers>();

            if (listCompany.Count > 0)
            {
                return listCompany;
            }

            return BadRequest();
        }

        

        [HttpPost("loginAdmin")]
        public async Task<ActionResult<TblUsers>> CheckLoginAdmin (TblUsers admin)
        {
            TblUsers userEntity = _context.TblUsers.Find(admin.Username);
            if(userEntity != null)
            {
                if(userEntity.RoleId == 1)
                {
                    if (userEntity.Password.Equals(admin.Password))
                    {
                        return userEntity;
                    }
                }
            }
            return Unauthorized();
        }

        [HttpPost("loginUser")]
        public async Task<ActionResult<TblUsers>> CheckLoginUser (string username, string fullname, string avatar)
        {
            TblUsers userEntity = _context.TblUsers.Find(username);
            Console.WriteLine("kk");
            if (userEntity != null)
            {
                if (userEntity.RoleId == 2)
                {
                    return userEntity;
                }
            }
            else
            {
                TblUsers dto = new TblUsers();
                dto.Username = username;
                dto.Password = "1";
                dto.RoleId = 2;
                dto.Fullname = fullname;
                dto.Rating = 0;
                dto.Avatar = avatar;
                dto.Status = "active";
                _context.TblUsers.Add(dto);
                await _context.SaveChangesAsync();
                return dto;
            }
            return Unauthorized();
        }


        //GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TblUsers>>> GetTblUsers()
        {
            return await _context.TblUsers.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TblUsers>> GetTblUsers(string id)
        {
            var tblUsers = await _context.TblUsers.FindAsync(id);

            if (tblUsers == null)
            {
                return NotFound();
            }

            return tblUsers;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTblUsers(string id, TblUsers tblUsers)
        {
            if (id != tblUsers.Username)
            {
                return BadRequest();
            }

            _context.Entry(tblUsers).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TblUsersExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.

       

        [HttpPost]
        public async Task<ActionResult<TblUsers>> PostTblUsers(TblUsers tblUsers)
        {
            try
            {
                validator.ValidateUserFields(tblUsers);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }

            _context.TblUsers.Add(tblUsers);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TblUsersExists(tblUsers.Username))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTblUsers", new { id = tblUsers.Username }, tblUsers);
        }

        [HttpPost("findMatchingWriter")]
        public async Task<ActionResult<TblUsers>> FindMacthingWriter(String customerUsername, String copywriterUsername)
        {
            List<TblUsers> listWriters = new List<TblUsers>();
            List<UsersHavingKeywords> listKeywordsOfCustomer = _context.UsersHavingKeywords
                .FromSqlRaw("select Username, KeywordId from UsersHavingKeywords where Username = {0}", customerUsername)
                .ToList<UsersHavingKeywords>();
            List<UsersHavingKeywords> listKeywordsOfCopywriter = _context.UsersHavingKeywords
                .FromSqlRaw("select Username, KeywordId from UsersHavingKeywords where Username = {0}", copywriterUsername)
                .ToList<UsersHavingKeywords>();
            for (int i = 0; i < listKeywordsOfCustomer.Count; i++)
            {
                for (int j = 0; j < listKeywordsOfCopywriter.Count; j++)
                {
                    if (listKeywordsOfCustomer[i].KeywordId == listKeywordsOfCopywriter[j].KeywordId)
                    {
                        listWriters.Add(_context.TblUsers.Find(listKeywordsOfCopywriter[j].Username));
                    }
                }
            }
            if (listWriters.Count > 0)
            {
                return listWriters[0];
            }
            return BadRequest();
        }

        [HttpPost("findMatchingWriters")]
        public async Task<ActionResult<List<TblUsers>>> FindMacthingWriters(String customerUsername)
        {
            ISet<TblUsers> listWriters = new HashSet<TblUsers>();
            List<UsersHavingKeywords> listKeywordsOfCustomer = _context.UsersHavingKeywords
                .FromSqlRaw("select Username, KeywordId from UsersHavingKeywords where Username = {0}", customerUsername)
                .ToList<UsersHavingKeywords>();
            List<UsersHavingKeywords> listKeywordsOfCopywriter = _context.UsersHavingKeywords
                .FromSqlRaw("select Username, KeywordId from UsersHavingKeywords where Username != {0}", customerUsername)
                .ToList<UsersHavingKeywords>();
            for (int i = 0; i < listKeywordsOfCustomer.Count; i++)
            {
                for (int j = 0; j < listKeywordsOfCopywriter.Count; j++)
                {
                    if (listKeywordsOfCustomer[i].KeywordId == listKeywordsOfCopywriter[j].KeywordId)
                    {
                        listWriters.Add(_context.TblUsers.Find(listKeywordsOfCopywriter[j].Username));
                    }
                }
            }
            if (listWriters.Count > 0)
            {
                return listWriters.ToList();
            }
            return BadRequest();
        }

        [HttpPost("findMatchingWriterByPostKeywords")]
        public async Task<ActionResult<List<TblUsers>>> FindMacthingWriterByPostKeywords(int postId, String copywriterUsername)
        {
            List<TblUsers> listWriters = new List<TblUsers>();
            List<TblPostsHavingKeywords> listKeywordsOfPost = _context.TblPostsHavingKeywords
              //  .FromSqlRaw("select KeywordId from TblPostsHavingKeywords where PostId = {0}", postId)
              .Where(p => p.PostId == postId)
              .Select(p => p)
                .ToList<TblPostsHavingKeywords>();
            List<UsersHavingKeywords> listKeywordsOfCopywriter = _context.UsersHavingKeywords
               // .FromSqlRaw("select KeywordId from UsersHavingKeywords where Username = {0}", copywriterUsername)
               .Where(u => u.Username == copywriterUsername)
               .Select(u => u)
                .ToList<UsersHavingKeywords>();

            for (int i = 0; i < listKeywordsOfPost.Count; i++)
            {
                for (int j = 0; j < listKeywordsOfCopywriter.Count; j++)
                {
                    if (listKeywordsOfPost[i].KeywordId == listKeywordsOfCopywriter[j].KeywordId)
                    {
                        listWriters.Add(_context.TblUsers.Find(listKeywordsOfCopywriter[j].Username));
                    }
                }
            }
            if (listWriters.Count > 0)
            {
                return listWriters;
            }
            return BadRequest();
        }

        [HttpPost("findMatchingWriterBysPostKeywords")]
        public async Task<ActionResult<List<TblUsers>>> FindMacthingWritersByPostKeywords(int postId, String customerName)
        {
            ISet<TblUsers> listWriters = new HashSet<TblUsers>();
            List<TblPostsHavingKeywords> listKeywordsOfPost = _context.TblPostsHavingKeywords
              //  .FromSqlRaw("select KeywordId from TblPostsHavingKeywords where PostId = {0}", postId)
              .Where(p => p.PostId == postId)
              .Select(p => p)
                .ToList<TblPostsHavingKeywords>();
            List<UsersHavingKeywords> listKeywordsOfCopywriter = _context.UsersHavingKeywords
               // .FromSqlRaw("select KeywordId from UsersHavingKeywords where Username = {0}", copywriterUsername)
               .Where(u => u.Username != customerName)
               .Select(u => u)
                .ToList<UsersHavingKeywords>();

            for (int i = 0; i < listKeywordsOfPost.Count; i++)
            {
                for (int j = 0; j < listKeywordsOfCopywriter.Count; j++)
                {
                    if (listKeywordsOfPost[i].KeywordId == listKeywordsOfCopywriter[j].KeywordId)
                    {
                        listWriters.Add(_context.TblUsers.Find(listKeywordsOfCopywriter[j].Username));
                    }
                }
            }
            if (listWriters.Count > 0)
            {
                return listWriters.ToList();
            }
            return BadRequest();
        }
        // DELETE: api/Users/5

        [HttpDelete("{id}")]
        public async Task<ActionResult<TblUsers>> DeleteTblUsers(string id)
        {
            var tblUsers = await _context.TblUsers.FindAsync(id);
            if (tblUsers == null)
            {
                return NotFound();
            }

            _context.TblUsers.Remove(tblUsers);
            await _context.SaveChangesAsync();

            return tblUsers;
        }

        private bool TblUsersExists(string id)
        {
            return _context.TblUsers.Any(e => e.Username == id);
        }
    }
}
