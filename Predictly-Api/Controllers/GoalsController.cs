using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Predictly_Api.Models;

namespace Predictly_Api.Controllers
{
    [Route("goals")]
    [ApiController]
    public class GoalsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GoalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GoalModel>>> GetGoals()
        {
            return await _context.Goals.ToListAsync();
        }

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

        [HttpPost]
        public async Task<ActionResult<GoalModel>> PostGoalModel(GoalModel goalModel)
        {
            _context.Goals.Add(goalModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGoalModel", new { id = goalModel.Id }, goalModel);
        }

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
