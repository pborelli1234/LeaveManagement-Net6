using AutoMapper;
using LeaveManagement.Web.Data;
using LeaveManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Web.Controllers
{
    public class LeaveTypesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public LeaveTypesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: LeaveTypes
        public async Task<IActionResult> Index()
        {
            //List<LeaveType> leaveTypes = await _context.LeaveTypes.ToListAsync();

            var leaveTypes = _mapper.Map<List<LeaveTypeViewModel>>(await _context.LeaveTypes.ToListAsync());

            return View(leaveTypes);
        }

        // GET: LeaveTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.LeaveTypes == null)
            {
                return NotFound();
            }

            LeaveType? leaveType = await _context.LeaveTypes.FirstOrDefaultAsync(m => m.Id == id);
            LeaveTypeViewModel leaveTypeVM = _mapper.Map<LeaveTypeViewModel>(leaveType);

            if (leaveType == null)
            {
                return NotFound();
            }

            return View(leaveTypeVM);
        }

        // GET: LeaveTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LeaveTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveTypeViewModel leaveTypeVM)
        {
            if (ModelState.IsValid)
            {
                //var leaveType = _context.LeaveTypes.FirstOrDefault(m => m.Id == leaveTypeVM.Id);

                var leaveType = _mapper.Map<LeaveType>(leaveTypeVM);

                leaveType.DateCreated = DateTime.Now;
                leaveType.DateModified = DateTime.Now;

                _context.Add(leaveType);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(leaveTypeVM);
        }

        // GET: LeaveTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.LeaveTypes == null)
            {
                return NotFound();
            }

            var leaveType = await _context.LeaveTypes.FindAsync(id);
            var leaveTypeVM = _mapper.Map<LeaveTypeViewModel>(leaveType);

            if (leaveType == null)
            {
                return NotFound();
            }

            return View(leaveTypeVM);
        }

        // POST: LeaveTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LeaveTypeViewModel leaveTypeVM)
        {
            if (id != leaveTypeVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    leaveTypeVM.DateModified = DateTime.Now;
                    var leaveType = _mapper.Map<LeaveType>(leaveTypeVM);

                    _context.Update(leaveType);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    //DbUpdateConcurrencyException checks that 2 people are not editing the field at the same time
                    if (!LeaveTypeExists(leaveTypeVM.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw new Exception("Unable to edit record as it is already been edited at the same time.");
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(leaveTypeVM);
        }

        // GET: LeaveTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.LeaveTypes == null)
            {
                return NotFound();
            }

            var leaveType = await _context.LeaveTypes.FirstOrDefaultAsync(m => m.Id == id);

            if (leaveType == null)
            {
                return NotFound();
            }

            return View(leaveType);
        }

        // POST: LeaveTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.LeaveTypes == null)
            {
                return Problem("Entity set 'ApplicationDbContext.LeaveTypes'  is null.");
            }
            var leaveType = await _context.LeaveTypes.FindAsync(id);

            if (leaveType != null)
            {
                _context.LeaveTypes.Remove(leaveType);
            }
            
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool LeaveTypeExists(int id)
        {
          return (_context.LeaveTypes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
