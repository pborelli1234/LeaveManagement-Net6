using AutoMapper;
using LeaveManagement.Web.Constants;
using LeaveManagement.Web.Contracts;
using LeaveManagement.Web.Data;
using LeaveManagement.Web.Models;
using LeaveManagement.Web.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.Web.Controllers
{
    public class EmployeesController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly UserManager<Employee> _userManager;
        private readonly IMapper _mapper;
        private readonly ILeaveAllocationRepository _leaveAllocationRepository;
        private readonly ILeaveTypeRepository _leaveTypeRepository;

        public EmployeesController(UserManager<Employee> userManager, IMapper mapper, 
                                    ILeaveAllocationRepository leaveAllocationRepository, 
                                    ILeaveTypeRepository leaveTypeRepository)
        {
            _userManager = userManager;
            _mapper = mapper;
            _leaveAllocationRepository = leaveAllocationRepository;
            _leaveTypeRepository = leaveTypeRepository;
        }

        // GET: EmployeesController
        public async Task<IActionResult> Index()
        {
            var employees = await _userManager.GetUsersInRoleAsync(Roles.User);
            var model = _mapper.Map<List<EmployeeListVM>>(employees.OrderBy(e => e.Firstname).ThenBy(e => e.Lastname));

            return View(model);
        }

        // GET: EmployeesController/ViewAllocations/employeeId
        public async Task<IActionResult> ViewAllocations(string id)
        {
            var employeeAllocationVM = await _leaveAllocationRepository.GetEmployeeAllocations(id);

            return View(employeeAllocationVM);
        }

        // GET: EmployeesController/Edit/5
        public async Task<IActionResult> EditAllocation(int id)
        {
            var model = await _leaveAllocationRepository.GetEmployeeAllocationDetails(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // POST: EmployeesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAllocation(int id, LeaveAllocationEditVM leaveAllocationEditVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isUpdated = await _leaveAllocationRepository.UpdateEmployeeAllocation(leaveAllocationEditVM);

                    if (isUpdated)
                    {
                        return RedirectToAction(nameof(ViewAllocations), new { id = leaveAllocationEditVM.EmployeeId });
                    }                    
                }                
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error has occured. Please try again later.");
            }

            leaveAllocationEditVM.Employee = _mapper.Map<EmployeeListVM>(await _userManager.FindByIdAsync(leaveAllocationEditVM.EmployeeId));
            leaveAllocationEditVM.LeaveType = _mapper.Map<LeaveTypeViewModel>(await _leaveTypeRepository.GetAsync(leaveAllocationEditVM.LeaveTypeId));

            return View(leaveAllocationEditVM);
        }
    }
}
