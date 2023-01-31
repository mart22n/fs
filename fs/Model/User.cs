using System;
using System.Collections.Generic;

namespace fs.Model;

public partial class User
{
    public int Id { get; set; }

    public string Fullname { get; set; } = null!;

    public string Nickname { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;
}
