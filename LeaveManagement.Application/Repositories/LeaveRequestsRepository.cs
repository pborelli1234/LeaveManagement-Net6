using AutoMapper;
using LeaveManagement.Application.Contracts;
using LeaveManagement.Common.Models;
using LeaveManagement.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Application.Repositories
{
    public class LeaveRequestsRepository : GenericRepository<LeaveRequest>, ILeaveRequestsRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILeaveAllocationRepository _leaveAllocationRepository;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<Employee> _userManager;

        public LeaveRequestsRepository(ApplicationDbContext context, 
            IMapper mapper, 
            IHttpContextAccessor httpContextAccessor, 
            ILeaveAllocationRepository leaveAllocationRepository,
            IEmailSender emailSender,
            UserManager<Employee> userManager) : base(context) 
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _leaveAllocationRepository = leaveAllocationRepository;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        public async Task CancelLeaveRequest(int leaveRequestId)
        {
            LeaveRequest? leaveRequest = await GetAsync(leaveRequestId);

            if (leaveRequest != null)
            {
                leaveRequest.Cancelled = true;

                await UpdateAsync(leaveRequest);

                var emailBody = $"Your leave request from {leaveRequest.StartDate} to {leaveRequest.EndDate} has been Cancelled Successfully.";

                var employee = await _userManager.FindByIdAsync(leaveRequest.RequestingEmployeeId);
                await _emailSender.SendEmailAsync(employee.Email, $"Leave Request Cancelled", emailBody);
            }
        }

        public async Task ChangeApprovalStatus(int leaveRequestId, bool approved)
        {
            var leaveRequest = await GetAsync(leaveRequestId);

            leaveRequest.Approved = approved;

            if (approved)
            {
                var allocation = await _leaveAllocationRepository.GetEmployeeAllocation(leaveRequest.RequestingEmployeeId, leaveRequest.LeaveTypeId);

                int daysRequested = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays;

                allocation.NumberOfDays = daysRequested;

                await _leaveAllocationRepository.UpdateAsync(allocation);
            }

            await UpdateAsync(leaveRequest);

            var employee = await _userManager.FindByIdAsync(leaveRequest.RequestingEmployeeId);
            var approvalStatus = approved ? "Approved" : "Declined";
            var emailBody = $"Your leave request from {leaveRequest.StartDate} to {leaveRequest.EndDate} has been {approvalStatus}.";

            await _emailSender.SendEmailAsync(employee.Email, $"Leave Request {approvalStatus}", emailBody);
        }

        public async Task<bool> CreateLeaveRequest(LeaveRequestCreateVM leaveRequestCreateVM)
        {
            var employee = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User);

            var leaveAllocation = await _leaveAllocationRepository.GetEmployeeAllocation(employee.Id, leaveRequestCreateVM.LeaveTypeId);

            if (leaveAllocation == null)
            {
                return false;
            }

            int daysRequested = (int)(leaveRequestCreateVM.EndDate.Value - leaveRequestCreateVM.StartDate.Value).TotalDays;

            if (daysRequested > leaveAllocation.NumberOfDays)
            {
                return false;
            }

            var leaveRequest = _mapper.Map<LeaveRequest>(leaveRequestCreateVM);
            leaveRequest.DateRequested = DateTime.Now;
            leaveRequest.DateCreated = DateTime.Now;
            leaveRequest.DateModified = DateTime.Now;
            leaveRequest.RequestingEmployeeId = employee.Id;

            await AddAsync(leaveRequest);

            var emailBody = "Your leave request from " + leaveRequest.StartDate + " to " + leaveRequest.EndDate + " has been submitted for approval.";

            await _emailSender.SendEmailAsync(employee.Email, "Leave Request Submitted Successfully", emailBody);

            return true;
        }

        public async Task<AdminLeaveRequestViewVM> GetAdminLeaveRequestList()
        {
            var leaveRequests = await _context.LeaveRequests.Include(q => q.LeaveType).ToListAsync();

            var model = new AdminLeaveRequestViewVM
            {
                TotalRequests = leaveRequests.Count,
                ApprovedRequests = leaveRequests.Count(q => q.Approved == true),
                PendingRequests = leaveRequests.Count(q => q.Approved == null),
                RejectedRequests = leaveRequests.Count(q => q.Approved == false),
                LeaveRequests = _mapper.Map<List<LeaveRequestVM>>(leaveRequests)
            };

            foreach (var leaveRequest in model.LeaveRequests)
            {
                leaveRequest.Employee =_mapper.Map<EmployeeListVM>(await _userManager.FindByIdAsync(leaveRequest.RequestingEmployeeId));
            }

            return model;
        }

        public async Task<List<LeaveRequest>> GetAllAsync(string employeeId)
        {
            var leaveRequests = await _context.LeaveRequests.Include(q => q.LeaveType).Where(q => q.RequestingEmployeeId == employeeId).ToListAsync(); 

            return leaveRequests;
        }

        public async Task<LeaveRequestVM?> GetLeaveRequestAsync(int? id)
        {
            var leaveRequest = await _context.LeaveRequests.Include(q => q.LeaveType).FirstOrDefaultAsync(q => q.Id == id);

            if (leaveRequest == null)
            {
                return null;
            }

            var model = _mapper.Map<LeaveRequestVM>(leaveRequest);
            model.Employee = _mapper.Map<EmployeeListVM>(await _userManager.FindByIdAsync(leaveRequest?.RequestingEmployeeId));

            return model;
        }

        public async Task<EmployeeLeaveRequestViewVM> GetMyLeaveDetails()
        {
            var employee = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User);
            var allocations = (await _leaveAllocationRepository.GetEmployeeAllocations(employee.Id)).LeaveAllocations;
            var requests = _mapper.Map<List<LeaveRequestVM>>(await GetAllAsync(employee.Id));

            var model = new EmployeeLeaveRequestViewVM(allocations, requests);

            return model;
        }

    }
}
