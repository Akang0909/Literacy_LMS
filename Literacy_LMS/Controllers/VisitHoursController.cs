using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Literacy_LMS.Data;
using Literacy_LMS.Models;
using Microsoft.EntityFrameworkCore;

namespace Literacy_LMS.Controllers
{
    public class VisitHoursController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VisitHoursController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Display all visit hours
        public async Task<IActionResult> Index()
        {
            var visitHours = await _context.VisitHours.FirstOrDefaultAsync();
            return View("~/Views/Library/VisitHours.cshtml", visitHours);
        }



        // GET: VisitHours/Create
        public IActionResult Create()
        {
            return View("~/Views/Library/VisitHours/Create.cshtml");
        }

        // POST: VisitHours/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OpeningTime,ClosingTime")] VisitHours visitHour)
        {
            if (ModelState.IsValid)
            {
                _context.Add(visitHour);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Library/VisitHours/Create.cshtml", visitHour);
        }

        // GET: VisitHours/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var visitHour = await _context.VisitHours.FindAsync(id);
            if (visitHour == null) return NotFound();

            return View("~/Views/Library/VisitHours/Edit.cshtml", visitHour);
        }

        // POST: VisitHours/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OpeningTime,ClosingTime")] VisitHours visitHour)
        {
            if (id != visitHour.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(visitHour);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Library/VisitHours/Edit.cshtml", visitHour);
        }

        // GET: VisitHours/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var visitHour = await _context.VisitHours.FindAsync(id);
            if (visitHour == null) return NotFound();

            return View("~/Views/Library/VisitHours/Delete.cshtml", visitHour);
        }

        // POST: VisitHours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var visitHour = await _context.VisitHours.FindAsync(id);
            if (visitHour != null)
            {
                _context.VisitHours.Remove(visitHour);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(VisitHours model)
        {
            if (ModelState.IsValid)
            {
                var existingRecord = _context.VisitHours.FirstOrDefault();

                if (existingRecord != null)
                {
                    // Update existing record
                    existingRecord.OpeningTime = model.OpeningTime;
                    existingRecord.ClosingTime = model.ClosingTime;
                }
                else
                {
                    // Insert new record if none exists
                    _context.VisitHours.Add(model);
                }

                _context.SaveChanges(); // Save changes to DB
                return RedirectToAction("Index"); // Redirect back to the page
            }

            return View("Index", model);
        }
    }
}
