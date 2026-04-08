using System;
using System.Collections.Generic;

namespace BTL_WEB.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int? AppointmentId { get; set; }

    public int? ContractId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public DateTime PaymentDate { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public string? TransactionCode { get; set; }

    public virtual Appointment? Appointment { get; set; }

    public virtual AdoptionContract? Contract { get; set; }
}
