using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ECGViewer.Extensions
{

    public static class ItemCollectionExtensions
    {
        public static void AddRange(this ItemCollection source, IEnumerable<Control> controls)
        {
            foreach (var control in controls)
            {
                source.Add(control);
            }
        }




    }
}
