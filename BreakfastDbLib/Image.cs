using System;
using System.Collections.Generic;

namespace BreakfastDbLib;

public partial class Image
{
    public int ImageId { get; set; }

    public string Name { get; set; } = null!;

    public byte[] ImageData { get; set; } = null!;

    public virtual ICollection<Item> Items { get; } = new List<Item>();
}
