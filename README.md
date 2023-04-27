# PreOOMKiller


When the container oom occurs in kubernetes, the system kernel uses SIGKILL to kill the process, causing the .net crashdump to fail to be created. This library regularly reads the memory usage ratio of the container process, and gracefully terminates the process before the container oom, so that the crashdump will be generated normally
