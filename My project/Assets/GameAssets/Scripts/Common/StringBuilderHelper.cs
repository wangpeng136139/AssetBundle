using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameAsset
{
    public static class StringBuilderHelper
    {
        private static readonly StringBuilder m_stringBuild = new StringBuilder(2048);

        public static string Concat(params string[] strs)
        {
            m_stringBuild.Clear();
            if (strs != null)
            {
                if(strs.Length == 1)
                {
                    return strs[0];
                }

                foreach (var s in strs)
                {
                    m_stringBuild.Append(s);
                }   
            }
            return m_stringBuild.ToString();
        }
    }
}
