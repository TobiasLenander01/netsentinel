using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NetSentinel.Models;

public class Device
{
    public int Id {get; set;}

    [Required]
    [MaxLength(100)]
    public string Name {get; set;} = string.Empty;

    [Required]
    [MaxLength(15)]
    public string IPAddress {get; set;} = string.Empty;

    [MaxLength(17)]
    [Required]
    public string SerialNumber {get; set;} = string.Empty;

    public List<Incident> Incidents {get; set;} = new();

    public bool HasIncidents => Incidents.Count > 0;

    [Required]
    public Employee? Owner {get; set;}
}
