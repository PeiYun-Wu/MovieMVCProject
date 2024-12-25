using System;
using System.Collections.Generic;

namespace PennyProject.DataBase.MovieDB;

public partial class MovieInfo
{
    public string MovieId { get; set; } = null!;

    public string? MovieChinessName { get; set; }

    public string MovieEngName { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImgName { get; set; }

    public string? Director { get; set; }

    public DateTime? ReleaseDateTime { get; set; }

    public DateTime? CreateDateTime { get; set; }

    public byte[]? UpdateDateTime { get; set; }
}
