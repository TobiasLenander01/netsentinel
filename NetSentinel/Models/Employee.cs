using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;

namespace NetSentinel.Models;

public class Employee
{
    public int Id { get; set;}
    
    [Required] [MaxLength(100)]
    public string EmployeeNo {get; set;} = String.Empty;
    
    [Required] [MaxLength(100)]
    public string Name {get; set;} = String.Empty;

    public List<Device> Devices {get; set;} = new();

    [Required] [MaxLength(100)]
    public string Email {get; set;} = String.Empty;

    [Required]
    public Department? Department {get; set;}

    public Employee(){}

    public Employee(string employeeNo, string name, string email, Department? department)
    {
        EmployeeNo = employeeNo;
        Name = name;
        Email = email;
        Department = department;
    }



}
