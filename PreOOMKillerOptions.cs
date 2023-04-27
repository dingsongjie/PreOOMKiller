using System;
using System.Collections.Generic;
using System.Text;

namespace PreOOMKiller
{
    public class PreOOMKillerOptions
    {
        /// <summary>
        /// 检查间隔，单位毫秒
        /// </summary>
        public int CkeckInterval = 500;
        public string MaxMemoryFilePath = "/sys/fs/cgroup/memory/memory.stat";
        public string UsedMemoryFilePath = "/sys/fs/cgroup/memory/memory.usage_in_bytes";
        public decimal Percent = 95;
    }
}
