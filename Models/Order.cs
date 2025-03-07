using System;
using System.Collections.Generic;

namespace ProjectPrn222.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public float TotalAmount { get; set; }

    public int Status { get; set; }

    public int PaymentMethod { get; set; }

    public string? Notes { get; set; }

    public string? ShippingAddress { get; set; }

    public string? VourcherId { get; set; }

    public float DiscountAmount { get; set; }

    public float FinalTotal { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual PaymentMethod PaymentMethodNavigation { get; set; } = null!;

    public virtual ApplicationUser User { get; set; } = null!;

    public virtual Vourcher? Vourcher { get; set; }
}
