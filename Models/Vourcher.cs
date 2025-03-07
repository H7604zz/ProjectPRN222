using System;
using System.Collections.Generic;

namespace ProjectPrn222.Models;

public partial class Vourcher
{
    public string VourcherId { get; set; } = null!;

    public string Code { get; set; } = null!;

    public int Discount { get; set; }

    public DateOnly CreatedAt { get; set; }

    public DateOnly ExpiryDate { get; set; }

    public decimal MinOrderValue { get; set; }

    public decimal? MaxDiscountAmount { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
