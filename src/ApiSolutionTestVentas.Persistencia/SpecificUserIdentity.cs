using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ApiSolutionTestVentas.Persistencia;

public class SpecificUserIdentity : IdentityUser
{
    [StringLength(150)]
    public string FirstName { get; set; } = default!;

    [StringLength(150)]
    public string LastName { get; set; } = default!;

    public int Age { get; set; }

    public DocumentTypeEnum DocumentType { get; set; }

    [StringLength(20)]
    public string DocumentNumber { get; set; } = default!;
}

public enum DocumentTypeEnum : short
{
    Dni,
    Passport,
    Cel,
    Others
}