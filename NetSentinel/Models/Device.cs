using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NetSentinel.Models;

public class Device
{
    public int Id {get; set;}

    [MaxLength(100)]
    public string Name {get; set;} = string.Empty;

    [MaxLength(15)]
    public string IPAddress {get; set;} = string.Empty;

    [MaxLength(17)]
    [Required]
    public string SerialNumber {get; set;} = string.Empty;

    public List<Incident> Incidents {get; set;} = new();

    [Required]
    public Employee? Owner {get; set;}
}
