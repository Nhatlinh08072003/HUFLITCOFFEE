
using System;
using System.Collections.Generic;

namespace HUFLITCOFFEE.Models.Main;
public class DetailViewModel
{
    public Product? MainProduct { get; set; }
    public List<Product>? RelatedProducts { get; set; }
}