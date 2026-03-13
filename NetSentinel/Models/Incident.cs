using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NetSentinel.Enums;

namespace NetSentinel.Models;

public class Incident
{
    public int Id {get; set;}

    [Required] [MaxLength(200)]
    public string Description {get; set;} = String.Empty;

    [Required]
    public DateTime CreatedAt {get; set;}

    [Required]
    public Priority Priority {get; set;}

    [Required]
    public Status Status {get; set;}

    [Required]
    public Device? Device {get; set;}

    public List<SecurityMeasure> SecurityMeasures { get; set; } = new();

    public Incident() {}

    public Incident(string description)
    {
        Description = description;
    }
}
