using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;

namespace NetSentinel.Models;

public class Employee
{
    public int Id { get; set;}
    
    [Required] [MaxLength(50)]
    public string EmployeeNumber {get; set;} = string.Empty;
    
    [Required] [MaxLength(50)]
    public string Name {get; set;} = string.Empty;

    public List<Device> Devices {get; set;} = new();

    [Required] [MaxLength(50)]
    public string Email {get; set;} = string.Empty;

    [Required]
    public Department? Department {get; set;}

    public Employee(){}

    public Employee(string employeeNumber, string name, string email, Department? department)
    {
        EmployeeNumber = employeeNumber;
        Name = name;
        Email = email;
        Department = department;
    }



}
