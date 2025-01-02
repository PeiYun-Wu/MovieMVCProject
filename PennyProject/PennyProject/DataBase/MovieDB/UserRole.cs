using System;
using System.Collections.Generic;

namespace PennyProject.DataBase.MovieDB;

public partial class UserRole
{
    public string UserId { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; }

    public string UserName { get; set; }

    public int? Age { get; set; }

    public DateTime UpdateDateTime { get; set; }

    public DateTime CreateDateTime { get; set; }
}
