using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace PreOOMKiller
{
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// 默认最大内存内存百分比95%，每0.5秒检查一次，建议 将 COMPlus_GCHeapHardLimitPercent 设置为70%
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPreOOMKiller(this IServiceCollection services, Action<PreOOMKillerOptions> config)
        {
            services.AddHostedService<PreOOMKillerHostedService>();
            services.Configure(config);
            return services;
        }
        /// <summary>
        /// 默认最大内存内存百分比95%，每0.5秒检查一次，建议 将 COMPlus_GCHeapHardLimitPercent 设置为70%
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPreOOMKiller(this IServiceCollection services)
        {
            Action<PreOOMKillerOptions> config = con => { };
            services.AddHostedService<PreOOMKillerHostedService>();
            services.Configure(config);
            return services;
        }
    }
}
