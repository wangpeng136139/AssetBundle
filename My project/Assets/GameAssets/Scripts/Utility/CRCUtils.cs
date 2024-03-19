using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameAsset
{
    public static class CRCUtils
    {
        public static uint ComputeCRC32(Stream stream)
        {
            var crc32 = new CRC32();
            return crc32.Compute(stream);
        }

        public static uint ComputeCRC32(string filename,bool isRealPath = false)
        {
            if (!PathUtils.Exist(filename, isRealPath, out var fullPath)) return 0;

            using (var stream = FileUtils.OpenRead(filename))
            {
                return ComputeCRC32(stream);
            }
        }
    }
}
