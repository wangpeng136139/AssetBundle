using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameAsset
{
    public interface ISchedulerCharacter
    {
        public bool OnUpdate();

        public bool OnCompelte();

        public int Priority { get; }
    }
}
