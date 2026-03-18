using System;
using System.ComponentModel.DataAnnotations;

namespace NetSentinel.Models;

public class SecurityMeasure
{
    public int Id { get; set; }

    [MaxLength(400)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(200)] [Required]
    public string Result { get; set; } = string.Empty;

    [Required]
    public Incident? Incident { get; set; }

    public Employee? PerformedBy { get; set; }

    [Required]
    public DateTime PerformedAt { get; set; }

    public SecurityMeasure()
    {
        
    }

    public SecurityMeasure(string description, string result, Incident incident, Employee performedBy, DateTime performedAt)
    {
        Description = description;
        Result = result;
        Incident = incident;
        PerformedBy = performedBy;
        PerformedAt = performedAt;
    }
}
