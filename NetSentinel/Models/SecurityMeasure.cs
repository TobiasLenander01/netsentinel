using System;
using System.ComponentModel.DataAnnotations;
using NetSentinel.Enums;

namespace NetSentinel.Models;

public class SecurityMeasure
{
    public int Id { get; set; }

    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Result { get; set; } = string.Empty;

    [Required]
    public Incident? Incident { get; set; }

    [Required]
    public Employee? PerformedBy { get; set; }

    public SecurityMeasure()
    {
        
    }

    public SecurityMeasure(string description, string result)
    {
        Description = description;
        Result = result;

    }
}
