using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulsAssetPipeline
{
    [Flags]
    public enum SoulsGames
    {
        None = 0,
        DES = 1 << 0,
        DS1 = 1 << 1,
        DS1R = 1 << 2,
        DS2 = 1 << 3,
        DS2SOTFS = 1 << 4,
        DS3 = 1 << 5,
        BB = 1 << 6,
        SDT = 1 << 7,
    }
}
