using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PashayanLab1.Data;
using PashayanLab1.Models;
using Microsoft.AspNetCore.Authorization;

namespace PashayanLab1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly DataContext _context;

        public TransactionsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
          if (_context.Transactions == null) 

          {
              return NotFound();
          }
            var transactions = from Transactions in _context.Transactions
                               join Users in _context.Users
                               on Transactions.UserID equals Users.Id
                               select new
                               {
                                   Users.Username,
                                   Transactions.Date,
                                   Transactions.Cost,
                                   Transactions.PayMethod,

                               };
            return Ok(transactions);

        }

        // GET: api/Transactions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
          if (_context.Transactions == null)
          {
              return NotFound();
          }
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }

            return transaction;
        }

        // PUT: api/Transactions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransaction(int id, Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return BadRequest();
            }

            _context.Entry(transaction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
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

        // POST: api/Transactions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Transaction>> PostTransaction(Transaction transaction)
        {
          if (_context.Transactions == null)
          {
              return Problem("Entity set 'DataContext.Transactions'  is null.");
          }
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTransaction", new { id = transaction.Id }, transaction);
        }

        // DELETE: api/Transactions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            if (_context.Transactions == null)
            {
                return NotFound();
            }
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TransactionExists(int id)
        {
            return (_context.Transactions?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpPost("BuyCarById"), Authorize (Roles = "Admin,Noob")]
        public async Task<ActionResult<string>> BuyCar(int _CarID, string Method)
        {
            int _UserID = GetUserID(User.Identity.Name);

            Transaction Сonnection = new Transaction();


            if (CarExists(_CarID))
            {
                _context.Transactions.Add(Сonnection);
                int UserBalance = GetUserBalance(User.Identity.Name);
                int CarPrice = GetCarPrice(_CarID);
                if ((UserBalance >= CarPrice))
                {
                    int NewBalance = UserBalance - CarPrice;
                    Сonnection.Cost = CarPrice;
                    Сonnection.PayMethod = Method;
                    Сonnection.Date = DateTime.Now;
                    Сonnection.UserID = _UserID;
                    Сonnection.CarID = _CarID;
                    SetNewUserBalance(_UserID, NewBalance);
                    await _context.SaveChangesAsync();
                    return Ok(Сonnection);
                }

                else
                    return BadRequest("You haven't enough money to buy this car");
            }
            else return BadRequest("There are no cars with this id");
        }

        [HttpGet("ShowCarsByUserID"), Authorize(Roles = "Admin,Noob")]
        public async Task<IQueryable> _ShowCars(int _UserID)
        {
            var query = from Cars in _context.Cars
                        join Transactions in _context.Transactions
                        on Cars.Id equals Transactions.CarID
                        where Transactions.UserID == _UserID
                        select new
                        {
                            Cars.Brand,
                            Cars.Model,
                            Transactions.Cost,
                            Transactions.PayMethod,
                            Transactions.Date
                        };
            return query;
        }

        [HttpGet("ShowBoughtCars"), Authorize(Roles = "Admin,Noob")]
        public async Task<IQueryable> _ShowBoughtCars()
        {
            var query = from Transactions in _context.Transactions
                        join Users in _context.Users
                        on Transactions.UserID equals Users.Id
                        join Cars in _context.Cars
                        on Transactions.CarID equals Cars.Id
                        where Users.Id == Transactions.UserID && Cars.Id == Transactions.CarID
                        select new
                        {
                            Users.Username,
                            Cars.Model,
                            Transactions.Cost,
                            Transactions.PayMethod,
                            Transactions.Date
                        };
            return query;
        }




        [HttpGet("ShowCarsByModel"), Authorize(Roles = "Admin,Noob")]
        public async Task<IQueryable> _ShowCarsByModel(string _Brand)
        {
            var query = from Cars in _context.Cars
                            //join Transactions in _context.Transactions
                            //on Cars.Id equals Transactions.CarID
                        where Cars.Brand == _Brand
                        select new
                        {
                            Cars.Brand,
                            Cars.Model,
                            Cars.Cost,
                            Cars.Mileage,
                            Cars.Warranty
                        };
            return query;
        }




        private int GetUserID(string name)
        {
            IQueryable<int> query = from Users in _context.Users where Users.Username == name select Users.Id;
            int id = query.FirstOrDefault();
            return id;
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }

        private int GetUserBalance(string name)
        {
            IQueryable<int> query = from Users in _context.Users where Users.Username == name select Users.Balance;
            int _Balance = query.FirstOrDefault();
            return _Balance;
        }

        private int GetCarPrice(int CarID)
        {
            IQueryable<int> query = from Cars in _context.Cars where Cars.Id == CarID select Cars.Cost;
            int _Price = query.FirstOrDefault();
            return _Price;
        }
        private void SetNewUserBalance(int _UserID, int _NewBalance)
        {
            var query = from Users in _context.Users where Users.Id == _UserID select Users;
            foreach (User user in query)
            {
                user.Balance = _NewBalance;
            }
        }
    }

    }
