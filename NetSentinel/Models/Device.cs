using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NetSentinel.Models;

public class Device
{
    public int Id {get; set;}

    [MaxLength(100)]
    public string Name {get; set;} = string.Empty;

    [MaxLength(100)]
    public string IPAddress {get; set;} = string.Empty;

    [MaxLength(100)]
    [Required]
    public string SerialNumber {get; set;} = string.Empty;

    public List<Incident> Incidents {get; set;} = new();

    public bool HasIncidents => Incidents.Count > 0;

    public Employee? Owner {get; set;}

    public Device() {}

    public Device(string name, string ipAddress, string serialNumber, Employee owner)
    {
        Name = name;
        IPAddress = ipAddress;
        SerialNumber = serialNumber;
        Owner = owner;
    }
}
