using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NetSentinel.Models;

public class Department
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Location { get; set; } = String.Empty;

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public List<Employee> Employees { get; set; } = new();

    public Department() { }

    public Department(string name, string location)
    {
        Name = name;
        Location = location;
    }


}
