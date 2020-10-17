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
    [Route("api/roles")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly ContentOursourceContext _context;

        public RolesController(ContentOursourceContext context)
        {
            _context = context;
        }

        // GET: api/Roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TblRoles>>> GetTblRoles()
        {
            return await _context.TblRoles.ToListAsync();
        }

        // GET: api/Roles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TblRoles>> GetTblRoles(int id)
        {
            var tblRoles = await _context.TblRoles.FindAsync(id);

            if (tblRoles == null)
            {
                return NotFound();
            }

            return tblRoles;
        }

        // PUT: api/Roles/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTblRoles(int id, TblRoles tblRoles)
        {
            if (id != tblRoles.Id)
            {
                return BadRequest();
            }

            _context.Entry(tblRoles).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TblRolesExists(id))
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

        // POST: api/Roles
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<TblRoles>> PostTblRoles(TblRoles tblRoles)
        {
            _context.TblRoles.Add(tblRoles);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TblRolesExists(tblRoles.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTblRoles", new { id = tblRoles.Id }, tblRoles);
        }

        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TblRoles>> DeleteTblRoles(int id)
        {
            var tblRoles = await _context.TblRoles.FindAsync(id);
            if (tblRoles == null)
            {
                return NotFound();
            }

            _context.TblRoles.Remove(tblRoles);
            await _context.SaveChangesAsync();

            return tblRoles;
        }

        private bool TblRolesExists(int id)
        {
            return _context.TblRoles.Any(e => e.Id == id);
        }
    }
}
