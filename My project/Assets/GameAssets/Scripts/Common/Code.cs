using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameAsset
{
    public static  class Code
    {
        /// <summary>
        /// 成功
        /// </summary>
        public const int Sucess = 0;
        /// <summary>
        /// 下载MapResVersion失败
        /// </summary>
        public const int FailDown_MapResVersion = -1;
        /// <summary>
        /// 下载MapRes失败
        /// </summary>
        public const int FailDown_MapRes = -2;
        /// <summary>
        /// 加载MapRes失败
        /// </summary>
        public const int FailLoad_MapRes = -3;
        /// <summary>
        /// 加载MapResVersion失败
        /// </summary>
        public const int FailLoad_MapResVersion = -4;
        /// <summary>
        /// 解析MapResVersion失败
        /// </summary>
        public const int FailLoad_MapResVersionExcepition = -5;
        /// <summary>
        /// MapResVersion返回值错误
        /// </summary>
        public const int FailLoad_ReturnMapResVersionExcepition = -9;
        /// <summary>
        /// 解析MapRes失败
        /// </summary>
        public const int FailLoad_MapResExcepition = -6;
        /// <summary>
        /// 解析MapResVersion失败
        /// </summary>
        public const int FailLoad_MapResVersionWriteExcepition = -7;
        /// <summary>
        /// 解析MapRes失败
        /// </summary>
        public const int FailLoad_MapResWriteExcepition = -8;
        /// <summary>
        /// 初始化Map失败
        /// </summary>
        public const int FailInit_Map = -9999;
        /// <summary>
        /// 未知错误
        /// </summary>
        public const int Fail_Unknow = -10000;

    }
}
