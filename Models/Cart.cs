using System;
using System.Collections.Generic;

namespace ProjectPrn222.Models;

public partial class Cart
{
    public int CartId { get; set; }

    public string UserId { get; set; } = null!;

    public int ProductId { get; set; }

    public int QuantityInCart { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual ApplicationUser User { get; set; } = null!;
}
