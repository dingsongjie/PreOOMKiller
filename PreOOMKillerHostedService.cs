using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace PreOOMKiller
{
    public class PreOOMKillerHostedService : BackgroundService
    {
        private readonly ILogger<PreOOMKillerHostedService> _logger;
        private readonly IOptions<PreOOMKillerOptions> _options;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private long _maxMemoryInBytes;


        public PreOOMKillerHostedService(ILogger<PreOOMKillerHostedService> logger, IOptions<PreOOMKillerOptions> options,IHostApplicationLifetime app)
        {
            _options = options;
            _logger = logger;
            _applicationLifetime = app;
        }
        public Task Init(PreOOMKillerOptions options, CancellationToken cancellationToken)
        {
            try
            {
                string statContent = File.ReadAllText(options.MaxMemoryFilePath);
                long hierarchicalMemoryLimit = 0;
                string[] statLines = statContent.Split('\n');
                foreach (var line in statLines)
                {
                    if (line.StartsWith("hierarchical_memory_limit"))
                    {
                        var parts = line.Split(' ');
                        if (parts.Length == 2 && long.TryParse(parts[1], out hierarchicalMemoryLimit))
                        {
                            break;
                        }
                    }
                }
                _maxMemoryInBytes = hierarchicalMemoryLimit;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get hierarchical_memory_limit error");
                throw new Exception("Get hierarchical_memory_limit error", ex);
            }

            _logger.LogDebug($"MaxMemoryInBytes is  {_maxMemoryInBytes.ToString()}.");
            return Task.CompletedTask;
        }
        public Task Check(PreOOMKillerOptions options, CancellationToken cancellationToken)
        {
            string usageContent = File.ReadAllText(options.UsedMemoryFilePath);
            // 解析 memory.usage_in_bytes 字段的值
            long memoryUsageInBytes = 0;
            if (long.TryParse(usageContent, out memoryUsageInBytes))
            {
                // 比较 hierarchical_memory_limit 和 memory.usage_in_bytes 的大小
                if (memoryUsageInBytes > _maxMemoryInBytes * options.Percent / 100)
                {
                    var message = $"Memory usage exceeds estimated limit : {options.Percent}";
                    _logger.LogWarning(message);
                    Environment.ExitCode = 0;
                    _applicationLifetime.StopApplication();
                }
            }
            return Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _logger.LogWarning("PreOOMKiller only works in linux");
                return;
            }
            await Init(_options.Value, stoppingToken);

            _logger.LogInformation("PreOOMKiller started");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug($"PreOOMKiller beginning Check.");

                await Check(_options.Value, stoppingToken);

                _logger.LogDebug($"PreOOMKiller  Check end.");

                await Task.Delay(_options.Value.CkeckInterval, stoppingToken);
            }
        }
    }
}
