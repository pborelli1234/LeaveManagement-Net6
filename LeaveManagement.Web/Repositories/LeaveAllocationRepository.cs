using AutoMapper;
using LeaveManagement.Web.Constants;
using LeaveManagement.Web.Contracts;
using LeaveManagement.Web.Data;
using LeaveManagement.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace LeaveManagement.Web.Repositories
{
    public class LeaveAllocationRepository: GenericRepository<LeaveAllocation>, ILeaveAllocationRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Employee> _userManager;
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private readonly IMapper _mapper;

        public LeaveAllocationRepository(ApplicationDbContext context, 
            UserManager<Employee> userManager, ILeaveTypeRepository leaveTypeRepository, IMapper mapper) : base(context)
        {
            _context = context;
            _userManager = userManager;
            _leaveTypeRepository = leaveTypeRepository;
            _mapper = mapper;
        }

        public async Task LeaveAllocation(int leaveTypeId)
        {
            IList<Employee> employees = await _userManager.GetUsersInRoleAsync(Roles.User);
            var period = DateTime.Now.Year;
            LeaveType? leaveType = await _leaveTypeRepository.GetAsync(leaveTypeId);
            var numberOfDays = 0;

            if (leaveType != null)
                numberOfDays = leaveType.DefaultDays;

            List<LeaveAllocation> allocations = new List<LeaveAllocation>();

            foreach (Employee employee in employees)
            {
                if (await AllocationExists(employee.Id, period, numberOfDays))
                    continue;

                allocations.Add(new LeaveAllocation
                {
                    EmployeeId = employee.Id,
                    LeaveTypeId = leaveTypeId,
                    Period = period,
                    NumberOfDays = numberOfDays
                });

                //await AddAsync(allocation);
            }

            await AddRangeAsync(allocations);
        }

        public async Task<bool> AllocationExists(string employeeId, int leaveTypeId, int period)
        {
            var allocationExists = await _context.LeaveAllocations.AnyAsync(q => q.EmployeeId == employeeId 
                                                        && q.LeaveTypeId == leaveTypeId 
                                                        && q.Period == period);

            return allocationExists;
        }

        public async Task<EmployeeAllocationVM> GetEmployeeAllocations(string employeeId)
        {
            var allocations = await _context.LeaveAllocations
                                        .Include(q => q.LeaveType)
                                        .Where(q => q.EmployeeId == employeeId)
                                        .ToListAsync();

            var employee = await _userManager.FindByIdAsync(employeeId);

            EmployeeAllocationVM employeeAllocationVM = _mapper.Map<EmployeeAllocationVM>(employee);
            employeeAllocationVM.LeaveAllocations = _mapper.Map<List<LeaveAllocationVM>>(allocations);

            return employeeAllocationVM;
        }

        public async Task<LeaveAllocationEditVM> GetEmployeeAllocationDetails(int id)
        {
            var allocation = await _context.LeaveAllocations
                                        .Include(q => q.LeaveType)
                                        .FirstOrDefaultAsync(q => q.Id == id);

            if (allocation == null)
            {
                return null;
            }

            var employee = await _userManager.FindByIdAsync(allocation.EmployeeId);

            var model = _mapper.Map<LeaveAllocationEditVM>(allocation);
            model.Employee = _mapper.Map<EmployeeListVM>(await _userManager.FindByIdAsync(allocation.EmployeeId));

            return model;
        }

        public async Task<bool> UpdateEmployeeAllocation(LeaveAllocationEditVM leaveAllocationEditVM)
        {
            var leaveAllocation = await GetAsync(leaveAllocationEditVM.Id);

            if (leaveAllocation == null)
            {
                return false;
            }

            leaveAllocation.Period = leaveAllocationEditVM.Period;
            leaveAllocation.NumberOfDays = leaveAllocationEditVM.NumberOfDays;

            await UpdateAsync(leaveAllocation);

            return true;
        }
    }
}
