﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeaveManagement.Common.Models
{
    public class LeaveRequestCreateVM : IValidatableObject 
    {
        [Required]
        [Display(Name = "Start Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Required]
        [Display(Name = "Leave Type")]
        public int LeaveTypeId { get; set; }

        public SelectList? LeaveTypes { get; set; }

        [Display(Name = "Request Comments")]
        //[StringLength(1000, ErrorMessage = "The {0} cannot exceed {1} characters.")]
        public string? RequestComments { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate > EndDate)
            {
                yield return new ValidationResult("The Start Date must be before the End Date.",
                    new[] { nameof(StartDate), nameof(EndDate) });
            }

            if (RequestComments?.Length > 250)
            {
                yield return new ValidationResult("Comments are to long.", new[] { nameof(RequestComments) });
            }
        }
    }
}
