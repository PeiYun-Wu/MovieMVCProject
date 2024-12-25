﻿using System;
using System.Collections.Generic;

namespace PennyProject.DataBase.MovieDB;

public partial class UserFavorite
{
    public string MemberId { get; set; } = null!;

    public string MovieId { get; set; } = null!;

    public string? MovieName { get; set; }

    public DateTime? CreateDateTime { get; set; }

    public DateTime? UpdateDateTime { get; set; }
}
