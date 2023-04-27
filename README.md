# Linux PreOOMKiller


When the container oom occurs in kubernetes, the system kernel uses SIGKILL to kill the process, causing the .net crashdump to fail to be created. This library regularly reads the memory usage ratio of the container process, and gracefully terminates the process before the container oom, so that the crashdump will be generated normally.




## custom options

```cs
        // ms
public int CkeckInterval = 500;
public string MaxMemoryFilePath = "/sys/fs/cgroup/memory/memory.stat";
public string UsedMemoryFilePath = "/sys/fs/cgroup/memory/memory.usage_in_bytes";
public decimal Percent = 95;
```

By default, the memory usage is checked every 500ms. If the usage exceeds 95% of the limit, the process will exit. At this time, as long as the carsh dump is configured, the dump file can be created normally. Note that it is best to lower COMPlus_GCHeapHardLimitPercent or DOTNET_GCHeapHardLimitPercent at the same time.
