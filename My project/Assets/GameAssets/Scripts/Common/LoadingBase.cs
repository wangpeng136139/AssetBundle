using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameAsset
{
    public  abstract class LoadingBase : ISchedulerCharacter
    {
        public virtual LoadedState loadedState { get; protected set; } = LoadedState.None;

        public bool IsDone { get { return loadedState == LoadedState.Error || loadedState == LoadedState.Compelete; } }

        public bool IsError => loadedState == LoadedState.Error;

        public bool IsLoading => loadedState == LoadedState.Loading;

        public abstract void Load(bool Immediate);

        public abstract bool OnUpdate();

        public abstract bool OnCompelte();

        public virtual int Priority { get; } = 1;  


        protected void AddQueue(bool IsForceUpdate) 
        {
            Scheduler.Add(this, IsForceUpdate);
        }
    }
}
