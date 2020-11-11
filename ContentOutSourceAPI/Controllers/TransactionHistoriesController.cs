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
    public class TransactionHistoriesController : ControllerBase
    {
        private readonly ContentOursourceContext _context;

        public TransactionHistoriesController(ContentOursourceContext context)
        {
            _context = context;
        }

        [HttpPost("transaction")]
        public async Task<ActionResult<TransactionHistory>> createTransactionHistory(TransactionHistoryDTO dto)
        {
            TransactionHistory transactionHistory = new TransactionHistory(); //truyền xuống dto có 4 field: postid, giver, receiver, transactiondate
            transactionHistory.PostId = dto.PostId;
            transactionHistory.Giver = dto.Giver;
            transactionHistory.Receiver = dto.Receiver;
            transactionHistory.TransactionDate = DateTime.Now;
            TblUsersHavingPosts usersHavingPosts = _context.TblUsersHavingPosts.FromSqlRaw("select * from TblUsersHavingPosts where " +
                "Username = {0} and PostId = {1}", dto.Receiver, dto.PostId).First(); //tìm bài post của freelancer đã hoàn thành
            usersHavingPosts.Status = "finished"; //set status = finished
            _context.Entry(usersHavingPosts).State = EntityState.Modified;
            TblPosts post = _context.TblPosts.FromSqlRaw("select * from TblPosts where " +
                "Id = {0}", dto.PostId).First(); //tìm bài post trong TblPosts
            post.IsPublic = false; //ko public bài post nữa
            _context.Entry(post).State = EntityState.Modified;
            Int64 postAmount = _context.TblPosts.Find(dto.PostId).Amount; //lấy ra amount của bài post
            transactionHistory.Amount = postAmount; //lưu vào transaction history
            _context.TransactionHistory.Add(transactionHistory); //add transaction dto vào table TransactionHistory
            TblUsers company = _context.TblUsers.Find(dto.Giver); //tìm ra company
            company.Amount -= postAmount; //lấy amount hiện tại của company - amount của bài post đã finished
            _context.Entry(company).State = EntityState.Modified;
            TblUsers freelancer = _context.TblUsers.Find(dto.Receiver); //tìm ra freelancer
            freelancer.Amount += postAmount; //lấy amount hiện tại của freelancer + amount của bài post đã finished
            _context.Entry(freelancer).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return transactionHistory;
        }

        [HttpPost("getTransactions")]
        public async Task<ActionResult<List<TransactionHistoryShowUp>>> GetTransactionHistoryByName(UsernameDTO usernameDTO)
        {
            List<TransactionHistoryShowUp> listResponse = new List<TransactionHistoryShowUp>();
            List<TransactionHistory> listTransaction = _context.TransactionHistory
                .FromSqlRaw("select * from TransactionHistory where Receiver = {0}", usernameDTO.Username).ToList<TransactionHistory>();
            for (int i = 0; i < listTransaction.Count; i++)
            {
                TransactionHistory current = listTransaction[i];
                TransactionHistoryShowUp response = new TransactionHistoryShowUp();
                response.PostTitle = _context.TblPosts.Find(current.PostId).Title;
                response.TransactionDate = current.TransactionDate;
                response.Amount = current.Amount;
                listResponse.Add(response);
            }
            return listResponse;
        }

        // GET: api/TransactionHistories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionHistory>>> GetTransactionHistory()
        {
            return await _context.TransactionHistory.ToListAsync();
        }

        // GET: api/TransactionHistories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionHistory>> GetTransactionHistory(int id)
        {
            var transactionHistory = await _context.TransactionHistory.FindAsync(id);

            if (transactionHistory == null)
            {
                return NotFound();
            }

            return transactionHistory;
        }

        // PUT: api/TransactionHistories/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransactionHistory(int id, TransactionHistory transactionHistory)
        {
            if (id != transactionHistory.Id)
            {
                return BadRequest();
            }

            _context.Entry(transactionHistory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionHistoryExists(id))
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

        // POST: api/TransactionHistories
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<TransactionHistory>> PostTransactionHistory(TransactionHistory transactionHistory)
        {
            _context.TransactionHistory.Add(transactionHistory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTransactionHistory", new { id = transactionHistory.Id }, transactionHistory);
        }

        // DELETE: api/TransactionHistories/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TransactionHistory>> DeleteTransactionHistory(int id)
        {
            var transactionHistory = await _context.TransactionHistory.FindAsync(id);
            if (transactionHistory == null)
            {
                return NotFound();
            }

            _context.TransactionHistory.Remove(transactionHistory);
            await _context.SaveChangesAsync();

            return transactionHistory;
        }

        private bool TransactionHistoryExists(int id)
        {
            return _context.TransactionHistory.Any(e => e.Id == id);
        }
    }
}
