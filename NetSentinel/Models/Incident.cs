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

    public Priority Priority {get; set;}

    public Status Status {get; set;}

    public Device Device {get; set;}

    public List<SecurityMeasure> SecurityMeasures = new();

    public Incident() {}

    public Incident(string description)
    {
        Description = description;
    }
}
