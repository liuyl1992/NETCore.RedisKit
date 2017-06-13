using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using StackExchange.Redis;
using NETCore.RedisKit.Core.Internal;
using NETCore.RedisKit.Infrastructure.Internal;
using Microsoft.Extensions.Logging;

namespace NETCore.RedisKit.Tests
{
    public class _RedisService_String_Tests
    {
        private readonly IRedisService _RedisService;
        public _RedisService_String_Tests()
        {
            IRedisProvider redisProvider = new RedisProvider(new RedisKitOptions()
            {
                EndPoints = "127.0.0.1:6379"
            });

			ILogger<RedisService> logger = new LoggerFactory().CreateLogger<RedisService>();

			_RedisService = new RedisService(redisProvider, logger);
        }

        [Fact(DisplayName = "设置String值")]
        public void StringSetAsyncTest()
        {
            var test_key = "test_set";
            var setResult = _RedisService.StringSetAsync(test_key, "11111").Result;

            Assert.True(setResult);

            var getValue = _RedisService.StringGetAsync<string>(test_key).Result;
            Assert.NotEmpty(getValue);
            Assert.NotNull(getValue);
            Assert.Equal(getValue, "11111");

            _RedisService.StringSetAsync(test_key, "22222");
            getValue = _RedisService.StringGetAsync<string>(test_key).Result;
            Assert.Equal(getValue, "22222");

            var delResult = _RedisService.StringRemoveAsync(test_key).Result;
            Assert.True(delResult);
        }

        [Fact(DisplayName = "设置String值，并指定过期时间点")]
        public void StringSetAsyncExpireAtTest()
        {
            var test_key = "test_set_expireat";
            var setResult = _RedisService.StringSetAsync(test_key, "11111", DateTime.Now.AddSeconds(5)).Result;
            Assert.True(setResult);

            var getValue = Task<string>.Factory.StartNew(() =>
            {
                Task.Delay(new TimeSpan(0, 0, 6)).Wait();
                return _RedisService.StringGetAsync<string>(test_key).Result;
            }).Result;
            Assert.Null(getValue);
        }


        [Fact(DisplayName = "设置String值,并指定过期时间段")]
        public void StringSetAsyncExpireInTest()
        {
            var test_key = "test_set_expirein";
            var setResult = _RedisService.StringSetAsync(test_key, "11111", new TimeSpan(0, 0, 5)).Result;
            Assert.True(setResult);

            var getValue = Task<string>.Factory.StartNew(() =>
            {
                Task.Delay(new TimeSpan(0, 0, 6)).Wait();
                return _RedisService.StringGetAsync<string>(test_key).Result;
            }).Result;
            Assert.Null(getValue);
        }

        [Fact(DisplayName = "根据单个key获取String值")]
        public void StringGetAsyncTest()
        {
            var test_key = "test_get";

            var getValue = _RedisService.StringGetAsync<string>(test_key).Result;
            Assert.Null(getValue);

            var setResult = _RedisService.StringSetAsync(test_key, "1111").Result;
            Assert.True(setResult);

            getValue = _RedisService.StringGetAsync<string>(test_key).Result;
            Assert.NotNull(getValue);
            Assert.NotEmpty(getValue);
            Assert.Equal(getValue, "1111");

            var delResult = _RedisService.StringRemoveAsync(test_key).Result;
            Assert.True(delResult);
        }

        [Fact(DisplayName = "根据Key集合获取多个值")]
        public void StringGetAsyncRangeTest()
        {
            var test_key = new List<RedisKey>() { "key1", "key2", "key3", "key4" };

            _RedisService.StringSetAsync("key1", "1111");
            _RedisService.StringSetAsync("key2", "2222");
            _RedisService.StringSetAsync("key3", "3333");

            var values = _RedisService.StringGetAsync<string>(test_key).Result.ToList();

            Assert.Equal(values[0], "1111");
            Assert.Equal(values[1], "2222");
            Assert.Equal(values[2], "3333");

            _RedisService.StringRemoveAsync(test_key);
        }

        [Fact(DisplayName = "根据Key移除缓存")]
        public void StringRemoveAsyncTest()
        {
            var test_key = "test_remove";
            var delResult = _RedisService.StringRemoveAsync(test_key).Result;

            Assert.False(delResult);

            _RedisService.StringSetAsync(test_key, "11111");
            delResult = _RedisService.StringRemoveAsync(test_key).Result;
            Assert.True(delResult);
        }

        [Fact(DisplayName ="根据Key集合移除缓存")]
        public void StringRemoveAsyncRangeTest()
        {
            var test_key = new List<RedisKey>() { "key1", "key2", "key3", "key4" };

            _RedisService.StringSetAsync("key1", "1111");
            _RedisService.StringSetAsync("key2", "2222");
            _RedisService.StringSetAsync("key3", "3333");

            var delResult = _RedisService.StringRemoveAsync(test_key).Result;

            Assert.Equal(delResult, 3);
        }
    }
}