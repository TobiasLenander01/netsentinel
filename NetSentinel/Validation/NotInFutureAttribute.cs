using System;
using System.ComponentModel.DataAnnotations;

namespace NetSentinel.Validation;

[AttributeUsage(AttributeTargets.Property)]
public class NotInFutureAttribute : ValidationAttribute
{
    public NotInFutureAttribute() : base("The date cannot be in the future.") { }

    public override bool IsValid(object? value)
    {
        if (value is DateTime dt)
            return dt <= DateTime.UtcNow;
        return true; // let [Required] handle null
    }
}
