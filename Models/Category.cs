using System;
using System.Collections.Generic;

namespace ProjectPrn222.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;
    public bool IsActive { get; set; }
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
