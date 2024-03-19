using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameAsset
{
    interface ILoadCharacter
    {
        LoadedState loadedState { get;} 

        bool IsDone { get; }

        bool IsError { get; }

        void Load(bool Immediate);
    }
}
