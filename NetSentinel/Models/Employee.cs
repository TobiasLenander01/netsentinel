using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;

namespace NetSentinel.Models;

public class Employee
{
    public int Id { get; set;}
    
    [Required] [MaxLength(15)]
    public string EmployeeNo {get; set;} = String.Empty;
    
    [Required] [MaxLength(15)]
    public string Name {get; set;} = String.Empty;

    public List<Device> Devices {get; set;} = new();

    [Required]
    public Department Department {get; set;}

    public Employee(){}

    public Employee(string employeeNo, string name)
    {
        EmployeeNo = employeeNo;
        Name = name;
    }



}
