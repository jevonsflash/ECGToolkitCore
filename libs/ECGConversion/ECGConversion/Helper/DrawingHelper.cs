using SixLabors.Fonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecg.ECGConversion.Helper
{
    public class DrawingHelper
    {

        public static Font GetFont(string family, float size, FontStyle style)
        {
            FontFamily fam = SystemFonts.Get(family);
            var font = new Font(fam, size, style);
            return font;
        }
    }
}
