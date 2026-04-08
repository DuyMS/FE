using System;
using System.Collections.Generic;

namespace BTL_WEB.Models;

public partial class PetImage
{
    public int ImageId { get; set; }

    public int PetId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool IsPrimary { get; set; }

    public DateTime UploadedAt { get; set; }

    public virtual Pet Pet { get; set; } = null!;
}
