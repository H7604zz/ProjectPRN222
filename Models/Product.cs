using System;
using System.Collections.Generic;

namespace ProjectPrn222.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? Image { get; set; }

    public int Quanity { get; set; }

    public int CategoryId { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    public virtual ICollection<ProductPrice> ProductPrices { get; set; } = new List<ProductPrice>();

}
