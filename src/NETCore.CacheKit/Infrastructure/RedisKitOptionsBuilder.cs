using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NETCore.RedisKit.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using NETCore.RedisKit.Infrastructure.Internal;
using NETCore.RedisKit.Core.Internal;
using StackExchange.Redis;
using System.Linq;

namespace NETCore.RedisKit.Infrastructure
{
    public class RedisKitOptionsBuilder : IRedisKitOptionsBuilder
    {
        /// <summary>
        /// Gets the service collection in which the interception based services are added.
        /// </summary>
        public IServiceCollection serviceCollection { get; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="services">service collection</param>
        public RedisKitOptionsBuilder(IServiceCollection services)
        {
            this.serviceCollection = services;
        }

        /// <summary>
        /// get redis options and add ConnectionMultiplexer to sercice collection
        /// </summary>
        /// <param name="options">redis options</param>
        /// <param name="lifetime"><see cref="ServiceLifetime"/></param>
        /// <returns></returns>
        public IRedisKitOptionsBuilder UseRedis(IRedisKitOptions options, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            Guard.ArgumentNotNull(options, nameof(options));

            AddProviderService(options);
               
            serviceCollection.TryAdd(new ServiceDescriptor(typeof(IRedisService), typeof(RedisService), lifetime));
            return this;
        }

        /// <summary>
        /// add core service 
        /// </summary>
        /// <param name="options"></param>
        private void AddProviderService(IRedisKitOptions options)
        {
            RedisProvider provider = new RedisProvider(options);
            serviceCollection.TryAddSingleton<IRedisProvider>(provider);
        }
    }
}