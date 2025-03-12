using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectPrn222.Models;

public partial class ProductPrice
{
    [Key]
    public int Id { get; set; }
    public int ProductId { get; set; }

    public float Price { get; set; }

    public DateTime UpdateDate { get; set; } = DateTime.Now;

    public virtual Product Product { get; set; } = null!;
}
