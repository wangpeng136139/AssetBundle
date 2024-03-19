using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameAsset
{
    public class Updater: RootSingletonMono<Updater>
    {
        public void Initialization()
        {

        }
        private void Update()
        {
            Scheduler.OnUpdateAll();
            //下载走的是单独线程，所以不用判断是否繁忙
            DownInfo.OnUpdateAll();
        }
    }
}
