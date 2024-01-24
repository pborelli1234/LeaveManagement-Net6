using AutoMapper;
using AutoMapper.QueryableExtensions;
using LeaveManagement.Application.Contracts;
using LeaveManagement.Common.Constants;
using LeaveManagement.Common.Models;
using LeaveManagement.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Application.Repositories
{
    public class LeaveAllocationRepository: GenericRepository<LeaveAllocation>, ILeaveAllocationRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Employee> _userManager;
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly AutoMapper.IConfigurationProvider _configurationProvider;

        public LeaveAllocationRepository(ApplicationDbContext context, 
            UserManager<Employee> userManager, 
            ILeaveTypeRepository leaveTypeRepository, 
            IMapper mapper,
            IEmailSender emailSender,
            AutoMapper.IConfigurationProvider configurationProvider) : base(context)
        {
            _context = context;
            _userManager = userManager;
            _leaveTypeRepository = leaveTypeRepository;
            _mapper = mapper;
            _emailSender = emailSender;
            _configurationProvider = configurationProvider;
        }

        public async Task LeaveAllocation(int leaveTypeId)
        {
            IList<Employee> employees = await _userManager.GetUsersInRoleAsync(Roles.User);
            var period = DateTime.Now.Year;
            var leaveType = await _leaveTypeRepository.GetAsync(leaveTypeId);
            var numberOfDays = 0;
            var employeesWithNewAllocation = new List<Employee>();

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

                employeesWithNewAllocation.Add(employee);
            }

            await AddRangeAsync(allocations);

            foreach (var employee in employees)
            {
                await _emailSender.SendEmailAsync(employee.Email, $"Leave Allocation Posted for {period}", 
                                         $"Your {leaveType.Name} has been posted for the period of {period}. You have been given {leaveType.DefaultDays}.");
            }
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
            //var allocations = await _context.LeaveAllocations
            //                            .Include(q => q.LeaveType)
            //                            .Where(q => q.EmployeeId == employeeId)
            //                            .ToListAsync();

            //Project To gets only the fields in the VM

            var allocations = await _context.LeaveAllocations
                                        .Include(q => q.LeaveType)
                                        .Where(q => q.EmployeeId == employeeId)
                                        .ProjectTo<LeaveAllocationVM>(_configurationProvider)
                                        .ToListAsync();

            var employee = await _userManager.FindByIdAsync(employeeId);

            EmployeeAllocationVM employeeAllocationVM = _mapper.Map<EmployeeAllocationVM>(employee);
            //employeeAllocationVM.LeaveAllocations = _mapper.Map<List<LeaveAllocationVM>>(allocations);

            employeeAllocationVM.LeaveAllocations = allocations;

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

        public async Task<LeaveAllocation?> GetEmployeeAllocation(string employeeId, int leaveTypeId)
        {
            LeaveAllocation allocation = await _context.LeaveAllocations.FirstOrDefaultAsync(q => q.EmployeeId == employeeId && q.LeaveTypeId == leaveTypeId);

            return allocation;
        }
    }
}
