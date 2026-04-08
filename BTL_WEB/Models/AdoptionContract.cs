using System;
using System.Collections.Generic;

namespace BTL_WEB.Models;

public partial class AdoptionContract
{
    public int ContractId { get; set; }

    public int RequestId { get; set; }

    public int PetId { get; set; }

    public int UserId { get; set; }

    public DateTime SignedDate { get; set; }

    public decimal AdoptionFee { get; set; }

    public string? Terms { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Pet Pet { get; set; } = null!;

    public virtual AdoptionRequest Request { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
