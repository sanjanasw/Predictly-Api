using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Predictly_Api.Models;

namespace Predictly_Api.Controllers
{
    [Route("goal")]
    [ApiController]
    public class GoalController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GoalController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Goal
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GoalModel>>> GetGoals()
        {
            return await _context.Goals.ToListAsync();
        }

        // GET: api/Goal/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GoalModel>> GetGoalModel(int id)
        {
            var goalModel = await _context.Goals.FindAsync(id);

            if (goalModel == null)
            {
                return NotFound();
            }

            return goalModel;
        }

        // PUT: api/Goal/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGoalModel(int id, GoalModel goalModel)
        {
            if (id != goalModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(goalModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GoalModelExists(id))
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

        // POST: api/Goal
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GoalModel>> PostGoalModel(GoalModel goalModel)
        {
            _context.Goals.Add(goalModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGoalModel", new { id = goalModel.Id }, goalModel);
        }

        // DELETE: api/Goal/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGoalModel(int id)
        {
            var goalModel = await _context.Goals.FindAsync(id);
            if (goalModel == null)
            {
                return NotFound();
            }

            _context.Goals.Remove(goalModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GoalModelExists(int id)
        {
            return _context.Goals.Any(e => e.Id == id);
        }
    }
}
