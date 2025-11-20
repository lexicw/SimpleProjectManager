using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleProjectManager.Module.Controllers
{
    public class ColorToInt32Converter : ValueConverter<Color?, int?>
    {
        public ColorToInt32Converter()
            : base(
                c => c.HasValue ? c.Value.ToArgb() : (int?)null,
                v => v.HasValue ? Color.FromArgb(v.Value) : (Color?)null
            )
        { }
    }
}
