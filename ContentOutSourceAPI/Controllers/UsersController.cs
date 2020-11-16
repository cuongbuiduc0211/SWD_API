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

        [HttpPost("userBeans")]
        public async Task<ActionResult<int>> GetUserBeans(UsernameDTO usernameDTO)
        {
            int amount = (int) _context.TblUsers.Find(usernameDTO.Username).Amount;
            return amount;
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
        public async Task<ActionResult<List<TblUsers>>> ListFreelancer()
        {
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
        public async Task<ActionResult<TblUsers>> CheckLoginAdmin(TblUsers admin)
        {
            TblUsers userEntity = _context.TblUsers.Find(admin.Username);
            if (userEntity != null)
            {
                if (userEntity.RoleId == 1)
                {
                    if (userEntity.Password.Equals(admin.Password))
                    {
                        return userEntity;
                    }
                }
            }
            return Unauthorized();
        }

        [HttpPut("status")]
        public async Task<ActionResult<TblUsers>> setStatus(UsernameDTO usernameDTO)
        {
            TblUsers userEntity = _context.TblUsers
                .FromSqlRaw("select * from tblUsers where Username = {0}", usernameDTO.Username).First();
            if (userEntity != null)
            {
                if (userEntity.Status.Equals("active"))
                {
                    userEntity.Status = "banned";
                    _context.Entry(userEntity).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else if (userEntity.Status.Equals("banned"))
                {
                    userEntity.Status = "active";
                    _context.Entry(userEntity).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                return NotFound();
            }
            return userEntity;
            
        }

        [HttpPost("loginUser")]
        public async Task<ActionResult<TblUsers>> CheckLoginUser([FromBody]LoginUserDTO loginUserDTO)
        {
            TblUsers userEntity = _context.TblUsers.Find(loginUserDTO.Username);
            if (userEntity != null)
            {
                if (userEntity.RoleId == 2 && userEntity.Status.Equals("active"))
                {
                    return userEntity;
                }
                else if (userEntity.RoleId == 2 && userEntity.Status.Equals("banned"))
                {
                    return NotFound();
                }
            }
            else
            {
                TblUsers dto = new TblUsers();
                dto.Username = loginUserDTO.Username;
                dto.Password = "1";
                dto.RoleId = 2;
                dto.Fullname = loginUserDTO.Fullname;
                dto.Rating = 0;
                dto.Avatar = loginUserDTO.Avatar;
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
