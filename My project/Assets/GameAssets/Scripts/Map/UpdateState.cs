using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  GameAsset
{
    public enum UpdateState
    {
        UpdateState_None,
        UpdateState_OnReady,
        UpdateState_OnCheck,
        UpdateState_OnDowning,
        UpdateState_OnSuccess,
        UpdateState_OnFail,
    }
}
