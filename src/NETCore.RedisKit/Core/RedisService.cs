using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NETCore.RedisKit.Loging;
using NETCore.RedisKit.Infrastructure;
using NETCore.RedisKit.Shared;
using Newtonsoft.Json;
using StackExchange.Redis;
using NETCore.RedisKit.Services;

namespace NETCore.RedisKit
{
    public partial class RedisService:IRedisService
    {
        private readonly IRedisProvider _RedisProvider;
        private readonly IRedisLogger _Logger;
        private readonly ISerializeService _SerializeService;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="redisProvider"></param>
        /// <param name="logger"></param>
        /// <param name="objSerialize"></param>
        public RedisService(IRedisProvider redisProvider, IRedisLogger logger, ISerializeService objSerialize)
        {
            _RedisProvider = redisProvider;
            _Logger = logger;
            _SerializeService = objSerialize;
        }

        #region Sync

        #region Basic

        /// <summary>
        /// 自增（将值加1）
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/></param>
        /// <returns>返回自增之后的值</returns>
        public long Increment(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            _Logger.LogInformation("Increment key={key} with value=1", key);
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.StringIncrement(key, 1, flags);
        }

        /// <summary>
        /// 在原有值上加操作(long)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">待加值（long）</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/></param>
        /// <returns>返回加后结果</returns>
        public long Increment(RedisKey key, long value, CommandFlags flags = CommandFlags.None)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));

            _Logger.LogInformation("Increment {key}=with long valu={value}", key, value);
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.StringIncrement(key, value, flags);

        }

        /// <summary>
        /// 在原有值上加操作(double)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">待加值（double）</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/></param>
        /// <returns></returns>
        public double Increment(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            _Logger.LogInformation("Increment key={key} with double value={value}", key, value);

            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.StringIncrement(key, value, flags);
        }

        /// <summary>
        /// 自减（将值减1）
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/></param>
        /// <returns></returns>
        public long Decrement(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            _Logger.LogInformation("Decrement key={key} with value=1", key);
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.StringDecrement(key, 1, flags);
        }

        /// <summary>
        /// 在原有值上减操作(long)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">待减值</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/></param>
        /// <returns>返回减后结果</returns>
        public long Decrement(RedisKey key, long value, CommandFlags flags = CommandFlags.None)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            _Logger.LogInformation("Decrement key={key} with long value={value}", key, value);
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.StringDecrement(key, value, flags);
        }

        /// <summary>
        /// 在原有值上减操作(double)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">待减值</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/></param>
        /// <returns>返回减后结果</returns>
        public double Decrement(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            _Logger.LogInformation("Decrement key={key} with double value={value}", key, value);
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.StringDecrement(key, value, flags);
        }

        /// <summary>
        /// 重命名一个Key，值不变
        /// </summary>
        /// <param name="oldKey">旧键</param>
        /// <param name="newKey">新键</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/></param>
        /// <returns></returns>
        public bool KeyRename(RedisKey oldKey, RedisKey newKey, CommandFlags flags = CommandFlags.None)
        {
            Guard.ArgumentNotNullOrEmpty(oldKey, nameof(oldKey));
            Guard.ArgumentNotNullOrEmpty((newKey), nameof(newKey));

            _Logger.LogInformation("Rename oldKey={oldkey} to newKey={newKey}.Condition: flags={flags}", oldKey, newKey, flags);
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            if (oldKey.Equals(newKey))
            {
                _Logger.LogWarning("oldKey={oldKey} equals newKey={newKey}", oldKey, newKey);
                return false;
            }

            if (!db.KeyExists(oldKey, flags))
            {
                _Logger.LogWarning("oldKey={oldKey} don't exist. Condition:flags={flags}", oldKey, flags);
                return false;
            }
            return db.KeyRename(oldKey, newKey, When.Always, flags);
        }

        /// <summary>
        /// 获取Key对应Redis的类型
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/></param>
        /// <returns></returns>
        public RedisType KeyType(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            _Logger.LogInformation("Get reids key type with key={key}.Condition:flags={flags}", key, flags);
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.KeyType(key, flags);
        }

        /// <summary>
        /// 判断Key是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/></param>
        /// <returns></returns>
        public bool KeyExists(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            _Logger.LogInformation("Check reids key exist with key={key}.Condition:flags={flags}", key, flags);
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.KeyExists(key, flags);
        }

        #endregion

        #region Item
        /// <summary>
        /// Item Set操作（包括新增（key不存在）/更新(如果key已存在)）
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        /// <param name="when">操作前置条件<see cref="When"/></param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns>true 成功 false 失败</returns>
        public bool ItemSet<T>(RedisKey key, T val, When when = When.Always, CommandFlags flags = CommandFlags.DemandMaster)
        {
            return ItemSetInner(key, val, when, flags);
        }

        [Obsolete("The method is out of date and recommended use 'ItemSet'")]
        public bool StringSet<T>(RedisKey key, T val, When when = When.Always, CommandFlags flags = CommandFlags.DemandMaster)
        {
            return ItemSetInner(key, val, when, flags);
        }

        private bool ItemSetInner<T>(RedisKey key, T val, When when = When.Always, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            RedisValue value = DataSerialize(val);
            _Logger.LogInformation("StringSet Key={key} with value={value}. Condition: When={when},Flags={flags} ", key, value, when, flags);
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.StringSet(key, value, null, when, flags);

        }

        /// <summary>
        /// Item Set操作（包括新增/更新）,同时可以设置过期时间点
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        /// <param name="expiresAt">过期时间点</param>
        /// <param name="when">操作前置条件<see cref="When"/></param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns>true 成功 false 失败</returns>
        public bool ItemSet<T>(RedisKey key, T val, DateTime expiresAt, When when = When.Always, CommandFlags flags = CommandFlags.DemandMaster)
        {
            return ItemSetInner(key, val, expiresAt, when, flags);
        }

        [Obsolete("The method is out of date and recommended use 'ItemSet'")]
        public bool StringSet<T>(RedisKey key, T val, DateTime expiresAt, When when = When.Always, CommandFlags flags = CommandFlags.DemandMaster)
        {
            return ItemSetInner(key, val, expiresAt, when, flags);
        }

        private bool ItemSetInner<T>(RedisKey key, T val, DateTime expiresAt, When when = When.Always, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            RedisValue value = DataSerialize(val);
            _Logger.LogInformation("StringSet Key={key} with value={value} expiresAt={expiresAt}. Condition: When={when},Flags={flags} ", key, value, expiresAt, when, flags);
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            var timeSpan = expiresAt.Subtract(DateTime.Now);
            return db.StringSet(key, value, timeSpan, when, flags);
        }

        /// <summary>
        /// Item Set操作（包括新增/更新）,同时可以设置过期时间段
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        /// <param name="expiresIn">过期时间段</param>
        /// <param name="when">操作前置条件<see cref="When"/></param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns>true 成功 false 失败</returns>
        public bool ItemSet<T>(RedisKey key, T val, TimeSpan expiresIn, When when = When.Always, CommandFlags flags = CommandFlags.DemandMaster)
        {
            return ItemSetInner(key, val, expiresIn, when, flags);
        }

        [Obsolete("The method is out of date and recommended use 'ItemSet'")]
        public bool StringSet<T>(RedisKey key, T val, TimeSpan expiresIn, When when = When.Always, CommandFlags flags = CommandFlags.DemandMaster)
        {
            return ItemSetInner(key, val, expiresIn, when, flags);
        }

        private bool ItemSetInner<T>(RedisKey key, T val, TimeSpan expiresIn, When when = When.Always, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            RedisValue value = DataSerialize(val);
            _Logger.LogInformation("StringSet Key={key} with value={value} expiresAt={expiresIn}. Condition: When={when},Flags={flags} ", key, value, expiresIn, when, flags);

            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.StringSet(key, value, expiresIn, when, flags);
        }

        /// <summary>
        /// Item Get 操作
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为PreferSlave</param>
        /// <returns>如果key存在，找到对应Value,如果不存在，返回默认值.</returns>
        public T ItemGet<T>(RedisKey key, CommandFlags flags = CommandFlags.PreferSlave)
        {
            return ItemGetInner<T>(key, flags);
        }

        [Obsolete("The method is out of date and recommended use 'ItemGet'")]
        public T StringGet<T>(RedisKey key, CommandFlags flags = CommandFlags.PreferSlave)
        {
            return ItemGetInner<T>(key, flags);
        }

        private T ItemGetInner<T>(RedisKey key, CommandFlags flags = CommandFlags.PreferSlave)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            RedisValue value = db.StringGet(key, flags);

            _Logger.LogInformation("StringGet value={value} by key={key}. Condition:Flags={flags} ", value, key, flags);

            if (value.IsNullOrEmpty)
            {
                var result = default(T);
                _Logger.LogWarning("StringGet default value={value} by key={key}. Condition:Flags={flags} ", result, key, flags);

                return result;
            }
            return DataDserialize<T>(value);
        }


        /// <summary>
        /// Item Get 操作（获取多条）
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="keys">键集合</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为PreferSlave</param>
        /// <returns></returns>
        public IEnumerable<T> ItemGet<T>(IEnumerable<RedisKey> keys, CommandFlags flags = CommandFlags.PreferSlave)
        {
            return ItemGetInner<T>(keys, flags);
        }

        [Obsolete("The method is out of date and recommended use 'ItemGet'")]
        public IEnumerable<T> StringGet<T>(IEnumerable<RedisKey> keys, CommandFlags flags = CommandFlags.PreferSlave)
        {
            return ItemGetInner<T>(keys, flags);
        }
        private IEnumerable<T> ItemGetInner<T>(IEnumerable<RedisKey> keys, CommandFlags flags = CommandFlags.PreferSlave)
        {
            Guard.ArgumentNotNullOrEmpty(keys, nameof(keys));
           
            var result = new List<T>();
            if (keys != null && keys.Any())
            {
                ConnectionMultiplexer redis = _RedisProvider.Redis;
                IDatabase db = redis.GetDatabase();
                RedisValue[] values = db.StringGet(keys.ToArray(), flags);
                if (values != null && values.Length > 0)
                {
                    values.ForEach(x =>
                    {
                        if (!x.IsNullOrEmpty)
                        {
                            result.Add(DataDserialize<T>(x));
                        }
                    });

                    _Logger.LogInformation("StringGet keys={keys} values={values}.Conditions: flags={flags}", keys, DataSerialize(values), flags);
                }
                else
                {
                    _Logger.LogWarning("StringGet keys={keys} values is null or empty.Conditions: flags={flags}", keys, flags);
                }
            }
            else
            {
                _Logger.LogWarning("StringGet with keys={keys} is null or empty.Conditions: flags={flags}", keys, flags);
            }
            return result;
        }


        /// <summary>
        /// Item remove 操作
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns>True if the key was removed. else false</returns>
        public bool ItemRemove(RedisKey key, CommandFlags flags = CommandFlags.DemandMaster)
        {
            return ItemRemoveInnner(key, flags);
        }

        [Obsolete("The method is out of date and recommended use 'ItemRemove'")]
        public bool StringRemove(RedisKey key, CommandFlags flags = CommandFlags.DemandMaster)
        {
            return ItemRemoveInnner(key, flags);
        }


        private bool ItemRemoveInnner(RedisKey key, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            _Logger.LogInformation("StringRemove value with key={key},Coditions: flags={flags}", key, flags);
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.KeyDelete(key, flags);
        }

        /// <summary>
        /// Item remove 操作
        /// </summary>
        /// <param name="keys">键集合</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns></returns>
        public long ItemRemove(IEnumerable<RedisKey> keys, CommandFlags flags = CommandFlags.DemandMaster)
        {
            return ItemRemoveInnner(keys, flags);
        }

        [Obsolete("The method is out of date and recommended use 'ItemRemove'")]
        public long StringRemove(IEnumerable<RedisKey> keys, CommandFlags flags = CommandFlags.DemandMaster)
        {
            return ItemRemoveInnner(keys, flags);
        }

        private long ItemRemoveInnner(IEnumerable<RedisKey> keys, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(keys, nameof(keys));
            _Logger.LogInformation("StringRemove multi values with keys={keys},Coditions: flags={flags}", keys, flags);
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            var keyArray = keys.ToArray();
            return db.KeyDelete(keyArray, flags);
        }

        #endregion

        #region List

        /// <summary>
        /// List Insert Left 操作，将val插入pivot位置的左边
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="val">待插入值</param>
        /// <param name="pivot">参考值</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns>返回插入左侧成功后List的长度 或 -1 表示pivot未找到.</returns>
        public long ListInsertLeft<T>(RedisKey key, T val, T pivot, CommandFlags flags = CommandFlags.DemandMaster)
        {
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            RedisValue value = DataSerialize(val);
            RedisValue pivotValue = DataSerialize(pivot);

            return db.ListInsertBefore(key, pivotValue, value, flags);
        }

        /// <summary>
        /// List Insert Right 操作，，将val插入pivot位置的右边
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="val">待插入值</param>
        /// <param name="pivot">参考值</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns> 返回插入右侧成功后List的长度 或 -1 表示pivot未找到.</returns>
        public long ListInsertRight<T>(RedisKey key, T val, T pivot, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            RedisValue value = DataSerialize(val);
            RedisValue pivotValue = DataSerialize(pivot);

            return db.ListInsertAfter(key, pivotValue, value, flags);
        }

        /// <summary>
        /// List Left Push  操作
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        /// <param name="when">操作前置条件<see cref="When"/></param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns>Push操作之后，List的长度</returns>
        public long ListLeftPush<T>(RedisKey key, T val, When when = When.Always, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            RedisValue value = DataSerialize(val);
            return db.ListLeftPush(key, value, when, flags);
        }

        /// <summary>
        /// List Left Push 操作(添加多条)
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="vals">值集合</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns></returns>
        public long ListLeftPushRanage<T>(RedisKey key, IEnumerable<T> vals, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            if (vals == null || !vals.Any())
            {
                return 0;
            }
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            RedisValue[] values = new RedisValue[vals.Count()];
            var i = 0;
            vals.ForEach(x =>
            {
                values[i] = DataSerialize(x);
                i++;
            });

            return db.ListLeftPush(key, values, flags);
        }

        /// <summary>
        /// List Right Push 操作
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        /// <param name="when">操作前置条件<see cref="When"/></param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns></returns>
        public long ListRightPush<T>(RedisKey key, T val, When when = When.Always, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            RedisValue value = DataSerialize(val);
            return db.ListRightPush(key, value, when, flags);
        }

        /// <summary>
        /// List Right Push 操作(添加多条)
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="vals">值集合</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns></returns>
        public long ListRightPushRanage<T>(RedisKey key, IEnumerable<T> vals, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            Guard.ArgumentNotNull(vals, nameof(vals));
            if (vals == null || !vals.Any())
            {
                return 0;
            }
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            RedisValue[] values = new RedisValue[vals.Count()];
            var i = 0;
            vals.ForEach(x =>
            {
                values[i] = DataSerialize(x);
                i++;
            });
            return db.ListRightPush(key, values, flags);
        }

        /// <summary>
        /// List Left Pop 操作
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns></returns>
        public T ListLeftPop<T>(RedisKey key, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            RedisValue value = db.ListLeftPop(key, flags);
            if (!value.IsNullOrEmpty)
            {
                return DataDserialize<T>(value);
            }
            return default(T);
        }

        /// <summary>
        /// List Right Pop 操作
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns></returns>
        public T ListRightPop<T>(RedisKey key, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            RedisValue value = db.ListRightPop(key, flags);
            if (!value.IsNullOrEmpty)
            {
                return DataDserialize<T>(value);
            }
            return default(T);
        }

        /// <summary>
        /// List Remove 操作
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="vals">值集合</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns>the number of removed elements</returns>
        public long ListRemove<T>(RedisKey key, T val, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            RedisValue value = DataSerialize(val);
            return db.ListRemove(key, value, 0, flags);
        }

        /// <summary>
        /// List Remove All 操作
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns></returns>
        public bool ListRemoveAll(RedisKey key, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.KeyDelete(key, flags);
        }

        /// <summary>
        /// List Count 操作
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为PreferSlave</param>
        /// <returns></returns>
        public long ListCount(RedisKey key, CommandFlags flags = CommandFlags.PreferSlave)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.ListLength(key, flags);
        }

        /// <summary>
        /// List Get By Index 操作 (Index 0表示左侧第一个,-1表示右侧第一个)
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="index">索引</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为PreferSlave</param>
        /// <returns></returns>
        public T ListGetByIndex<T>(RedisKey key, long index, CommandFlags flags = CommandFlags.PreferSlave)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            RedisValue value = db.ListGetByIndex(key, index, flags);
            if (!value.IsNullOrEmpty)
            {
                return DataDserialize<T>(value);
            }
            return default(T);
        }

        /// <summary>
        /// List Get All 操作(注意：从左往右取值)
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为PreferSlave</param>
        /// <returns></returns>
        public IEnumerable<T> ListGetAll<T>(RedisKey key, CommandFlags flags = CommandFlags.PreferSlave)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            var values = db.ListRange(key, 0, -1, flags);
            var result = new List<T>();
            if (values.Any())
            {
                values.ToList().ForEach(x =>
                {
                    if (!x.IsNullOrEmpty)
                    {
                        result.Add(DataDserialize<T>(x));
                    }
                });
            }
            return result;
        }

        /// <summary>
        /// List Get Range 操作(注意：从左往右取值)
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="startIndex">开始索引 从0开始</param>
        /// <param name="stopIndex">结束索引 -1表示结尾</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为PreferSlave</param>
        /// <returns></returns>
        public IEnumerable<T> ListGetRange<T>(RedisKey key, long startIndex, long stopIndex, CommandFlags flags = CommandFlags.PreferSlave)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            var values = db.ListRangeAsync(key, startIndex, stopIndex, flags).Result;
            var result = new List<T>();
            if (values.Any())
            {
                values.ToList().ForEach(x =>
                {
                    if (!x.IsNullOrEmpty)
                    {
                        result.Add(DataDserialize<T>(x));
                    }
                });
            }
            return result;
        }

        /// <summary>
        /// List Expire At 操作
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="expireAt">DateTime失效点：到达该时间点，立即失效</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns></returns>
        public bool ListExpireAt(RedisKey key, DateTime expireAt, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.KeyExpire(key, expireAt, flags);
        }

        /// <summary>
        /// 设置List缓存过期
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="expiresIn">TimeSpan失效点：经过该时间段，立即失效</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns></returns>
        public bool ListExpireIn(RedisKey key, TimeSpan expiresIn, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.KeyExpire(key, expiresIn, flags);
        }

        #endregion

        #region Set

        /// <summary>
        /// Set Add 操作
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns>如果值不存在，则添加到集合，返回True否则返回False</returns>
        public bool SetAdd<T>(RedisKey key, T val, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            RedisValue value = DataSerialize(val);
            return db.SetAdd(key, value, flags);
        }

        /// <summary>
        /// Set Add 操作(添加多条)
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="vals">值集合</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns>添加值到集合，如果存在重复值，则不添加，返回添加的总数</returns>
        public long SetAddRanage<T>(RedisKey key, IEnumerable<T> vals, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            if (vals == null || !vals.Any())
            {
                return 0;
            }
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            RedisValue[] values = new RedisValue[vals.Count()];
            var i = 0;
            vals.ForEach(x =>
            {
                values[i] = DataSerialize(x);
                i++;
            });

            return db.SetAdd(key, values, flags);
        }

        /// <summary>
        /// Set Remove 操作
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns>如果值从Set集合中移除返回True，否则返回False</returns>
        public bool SetRemove<T>(RedisKey key, T val, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            RedisValue value = DataSerialize(val);
            return db.SetRemove(key, value, flags);
        }

        /// <summary>
        /// Set Remove 操作
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="vals">值集合</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns></returns>
        public long SetRemoveRange<T>(RedisKey key, IEnumerable<T> vals, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            if (vals == null || !vals.Any())
            {
                return 0;
            }
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            RedisValue[] values = new RedisValue[vals.Count()];
            var i = 0;
            vals.ForEach(x =>
            {
                values[i] = DataSerialize(x);
                i++;
            });
            return db.SetRemove(key, values, flags);
        }

        /// <summary>
        /// Set Remove All 操作
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns> True if the key was removed.</returns>
        public bool SetRemoveAll(RedisKey key, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.KeyDelete(key, flags);
        }

        /// <summary>
        /// Set Combine 操作(可以求多个集合并集/交集/差集)
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="keys">多个集合的key<see cref="IEnumerable{T}"/></param>
        /// <param name="operation">合并类型<see cref="SetOperation"/></param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns></returns>
        public IEnumerable<T> SetCombine<T>(IEnumerable<RedisKey> keys, SetOperation operation, CommandFlags flags = CommandFlags.PreferSlave)
        {
            Guard.ArgumentNotNullOrEmpty(keys, nameof(keys));
            var result = new List<T>();
            if (keys == null || !keys.Any())
            {
                return result;
            }

            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            var values = db.SetCombine(operation, keys.ToArray(), flags);
            if (values != null && values.Any())
            {
                values.ToList().ForEach(x =>
                {
                    if (!x.IsNullOrEmpty)
                    {
                        result.Add(DataDserialize<T>(x));
                    }
                });
            }
            return result;
        }

        /// <summary>
        /// Set Combine 操作(可以求2个集合并集/交集/差集)
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="firstKey">第一个Set的Key</param>
        /// <param name="sencondKey">第二个Set的Key</param>
        /// <param name="operation">合并类型<see cref="SetOperation"/></param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns>list with members of the resulting set.</returns>
        public IEnumerable<T> SetCombine<T>(RedisKey firstKey, RedisKey sencondKey, SetOperation operation, CommandFlags flags = CommandFlags.PreferSlave)
        {
            Guard.ArgumentNotNullOrEmpty(firstKey, nameof(firstKey));
            Guard.ArgumentNotNullOrEmpty(sencondKey, nameof(sencondKey));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            RedisValue[] values = db.SetCombine(operation, firstKey, sencondKey, flags);
            List<T> result = new List<T>();
            if (values != null && values.Any())
            {
                values.ToList().ForEach(x =>
                {
                    if (!x.IsNullOrEmpty)
                    {
                        result.Add(DataDserialize<T>(x));
                    }
                });
            }
            return result;
        }

        /// <summary>
        /// Set Combine And Store In StoreKey Set 操作
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="storeKey">新集合Id</param>
        /// <param name="soureKeys">多个集合的key</param>
        /// <param name="operation">合并类型<see cref="SetOperation"/></param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns></returns>
        public long SetCombineStore<T>(RedisKey storeKey, IEnumerable<RedisKey> soureKeys, SetOperation operation, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(storeKey, nameof(storeKey));
            if (soureKeys == null || !soureKeys.Any())
            {
                return 0;
            }
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.SetCombineAndStore(operation, storeKey, soureKeys.ToArray(), flags);
        }

        /// <summary>
        /// Set Combine And Store In StoreKey Set 操作
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="storeKey">新集合Id</param>
        /// <param name="firstKey">第一个集合Key</param>
        /// <param name="secondKey">第二个集合Key</param>
        /// <param name="operation">合并类型<see cref="SetOperation"/></param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns></returns>
        public long SetCombineStore<T>(RedisKey storeKey, RedisKey firstKey, RedisKey secondKey, SetOperation operation, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(storeKey, nameof(storeKey));
            Guard.ArgumentNotNullOrEmpty(firstKey, nameof(firstKey));
            Guard.ArgumentNotNullOrEmpty(secondKey, nameof(secondKey));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.SetCombineAndStore(operation, storeKey, firstKey, secondKey, flags);
        }

        /// <summary>
        /// Set Move 操作（将元素从soure移动到destination）
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="sourceKey">数据源集合</param>
        /// <param name="destinationKey">待添加集合</param>
        /// <param name="val">待移动元素</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns></returns>
        public bool SetMove<T>(RedisKey sourceKey, RedisKey destinationKey, T val, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(sourceKey, nameof(sourceKey));
            Guard.ArgumentNotNullOrEmpty(destinationKey, nameof(destinationKey));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            RedisValue value = DataSerialize(val);
            return db.SetMove(sourceKey, destinationKey, value, flags);
        }
        /// <summary>
        /// Set Exists 操作
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为PreferSlave</param>
        /// <returns></returns>
        public bool SetExists<T>(RedisKey key, T val, CommandFlags flags = CommandFlags.PreferSlave)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            RedisValue value = DataSerialize(val);
            return db.SetContains(key, value, flags);
        }

        /// <summary>
        ///  Set Count 操作
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为PreferSlave</param>
        /// <returns></returns>
        public long SetCount(RedisKey key, CommandFlags flags = CommandFlags.PreferSlave)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.SetLength(key, flags);
        }

        /// <summary>
        /// Set Get All 操作
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为PreferSlave</param>
        /// <returns></returns>
        public IEnumerable<T> SetGetAll<T>(RedisKey key, CommandFlags flags = CommandFlags.PreferSlave)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            RedisValue[] values = db.SetMembers(key);
            var result = new List<T>();

            if (values != null && values.Any())
            {
                values.ToList().ForEach(x =>
                {
                    if (!x.IsNullOrEmpty)
                    {
                        result.Add(DataDserialize<T>(x));
                    }
                });
            }
            return result;
        }

        /// <summary>
        /// Set Expire At 操作
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="expireAt">DateTime失效点：到达该时间点，立即失效</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns></returns>
        public bool SetExpireAt(RedisKey key, DateTime expireAt, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.KeyExpire(key, expireAt, flags);
        }
        /// <summary>
        /// Set Expire In 操作
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="expireIn">TimeSpan失效点：经过该时间段，立即失效</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns></returns>
        public bool SetExpireIn(RedisKey key, TimeSpan expireIn, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.KeyExpire(key, expireIn, flags);
        }
        #endregion

        #region SortedSet

        /// <summary>
        /// SortedSet Add 操作
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        /// <param name="score">优先级</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns></returns>
        public bool SortedSetAdd<T>(RedisKey key, T val, double score, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            RedisValue value = DataSerialize(val);
            return db.SortedSetAdd(key, value, score, flags);
        }
        /// <summary>
        /// SortedSet Add 操作（多条）
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="vals">待添加值集合<see cref="SortedSetEntry"/></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public long SortedSetAdd(RedisKey key, IEnumerable<SortedSetEntry> vals, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            if (vals == null || !vals.Any())
            {
                return 0;
            }
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            SortedSetEntry[] values = vals.ToArray();
            return db.SortedSetAdd(key, values, flags);
        }
        /// <summary>
        /// SortedSet Increment Score 操作
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        /// <param name="score">优先级</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns>Incremented score</returns>
        public double SortedSetIncrementScore<T>(RedisKey key, T val, double score, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            RedisValue value = DataSerialize(val);
            return db.SortedSetIncrement(key, value, score, flags);
        }

        /// <summary>
        /// SortedSet Decrement Score 操作
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        /// <param name="score">优先级</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns>Decremented score</returns>
        public double SortedSetDecrementScore<T>(RedisKey key, T val, double score, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            RedisValue value = DataSerialize(val);
            return db.SortedSetDecrement(key, value, score, flags);
        }

        /// <summary>
        /// Sorted Remove 操作
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns></returns>
        public bool SortedSetRemove<T>(RedisKey key, T val, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            RedisValue value = DataSerialize(val);
            return db.SortedSetRemove(key, value, flags);
        }
        /// <summary>
        /// Sorted Remove 操作(删除多条)
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="vals">值</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns></returns>
        public long SortedSetRemoveRanage<T>(RedisKey key, IEnumerable<T> vals, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            if (vals == null || !vals.Any())
            {
                return 0;
            }
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            RedisValue[] values = new RedisValue[vals.Count()];
            var i = 0;
            vals.ForEach(x =>
            {
                values[i] = DataSerialize(x);
                i++;
            });

            return db.SortedSetRemove(key, values, flags);
        }
        /// <summary>
        /// Sorted Remove 操作(根据索引区间删除,索引值按Score由小到大排序)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="startIndex">开始索引，0表示第一项</param>
        /// <param name="stopIndex">结束索引，-1标识倒数第一项</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns></returns>
        public long SortedSetRemove(RedisKey key, long startIndex, long stopIndex, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.SortedSetRemoveRangeByRank(key, startIndex, stopIndex, flags);
        }
        /// <summary>
        /// Sorted Remove 操作(根据Score区间删除，同时根据exclue<see cref="Exclude"/>排除删除项)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="startScore">开始Score</param>
        /// <param name="stopScore">结束Score</param>
        /// <param name="exclue">排除项<see cref="Exclude"/></param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns>the number of elements removed.</returns>
        public long SortedSetRemove(RedisKey key, double startScore, double stopScore, Exclude exclue, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.SortedSetRemoveRangeByScore(key, startScore, stopScore, exclue, flags);
        }

        /// <summary>
        /// Sorted Remove All 操作(删除全部)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns>thre reuslt of all sorted set removed</returns>
        public bool SortedSetRemoveAll(RedisKey key, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.KeyDelete(key, flags);
        }

        /// <summary>
        /// Sorted Set Trim 操作
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="size">保留条数</param>
        /// <param name="order">根据order<see cref="Order"/>来保留指定区间，如保留前100名，保留后100名</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns>移除元素数量</returns>
        public long SortedSetTrim(RedisKey key, long size, Order order = Order.Descending, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();

            if (order == Order.Ascending)
            {
                return db.SortedSetRemoveRangeByRank(key, size, -1, flags);
            }
            else
            {
                return db.SortedSetRemoveRangeByRank(key, 0, -size - 1, flags);
            }
        }

        /// <summary>
        /// Sorted Set Count 操作
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns></returns>
        public long SortedSetCount(RedisKey key, CommandFlags flags = CommandFlags.PreferSlave)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.SortedSetLength(key, double.NegativeInfinity, double.PositiveInfinity, Exclude.None, flags);

        }

        /// <summary>
        /// Sorted Set Exists 操作
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为PreferSlave</param>
        /// <returns></returns>
        public bool SortedSetExists<T>(RedisKey key, T val, CommandFlags flags = CommandFlags.PreferSlave)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            RedisValue value = DataSerialize(val);
            return db.SortedSetScore(key, value, flags) != null;

        }

        /// <summary>
        /// SortedSet Pop Min Score Element 操作
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为PreferSlave</param>
        /// <returns></returns>
        public T SortedSetGetMinByScore<T>(RedisKey key, CommandFlags flags = CommandFlags.PreferSlave)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            var values = db.SortedSetRangeByRank(key, 0, 1, Order.Ascending, flags);

            if (values != null && values.Any())
            {
                return DataDserialize<T>(values.First());
            }
            return default(T);
        }

        /// <summary>
        /// SortedSet Pop Max Score Element 操作
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为PreferSlave</param>
        /// <returns></returns>
        public T SortedSetGetMaxByScore<T>(RedisKey key, CommandFlags flags = CommandFlags.PreferSlave)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            var values = db.SortedSetRangeByRank(key, 0, 1, Order.Descending, flags);

            if (values != null && values.Any())
            {
                return DataDserialize<T>(values.First());
            }
            return default(T);
        }

        /// <summary>
        /// Sorted Set Get Page List 操作
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="order">排序规则<see cref="Order"/></param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为PreferSlave</param>
        /// <returns></returns>
        public IEnumerable<T> SortedSetGetPageList<T>(RedisKey key, int page, int pageSize, Order order = Order.Ascending, CommandFlags flags = CommandFlags.PreferSlave)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            Guard.ArgumentMinValue(page, 1, nameof(page));
            Guard.ArgumentMinValue(pageSize, 1, nameof(pageSize));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            RedisValue[] values = db.SortedSetRangeByRank(key, (page - 1) * pageSize, page * pageSize - 1, order, flags);
            List<T> result = new List<T>();
            if (values != null && values.Length > 0)
            {
                foreach (var value in values)
                {
                    if (!value.IsNullOrEmpty)
                    {
                        var data = DataDserialize<T>(value);
                        result.Add(data);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Sorted Set Get Page List 操作(根据分数区间)
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="startScore">开始值</param>
        /// <param name="stopScore">停止值</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="order">排序规则<see cref="Order"/></param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为PreferSlave</param>
        /// <param name="exclude">排除规则<see cref="Exclude"/>,默认为None</param>
        /// <returns></returns>
        public IEnumerable<T> SortedSetGetPageList<T>(RedisKey key, double startScore, double stopScore, int page, int pageSize, Order order = Order.Ascending, CommandFlags flags = CommandFlags.PreferSlave, Exclude exclude = Exclude.None)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            Guard.ArgumentMinValue(page, 1, nameof(page));
            Guard.ArgumentMinValue(pageSize, 1, nameof(pageSize));

            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            var values = db.SortedSetRangeByScore(key, startScore, stopScore, exclude, order, (page - 1) * pageSize, page * pageSize - 1, flags);
            var result = new List<T>();
            if (values != null && values.Length > 0)
            {
                foreach (var value in values)
                {
                    if (!value.IsNullOrEmpty)
                    {
                        var data = DataDserialize<T>(value);
                        result.Add(data);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Sorted Set Get Page List With Score 操作
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="order">排序规则<see cref="Order"/></param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为PreferSlave</param>
        /// <returns></returns>
        public SortedSetEntry[] SortedSetGetPageListWithScore(RedisKey key, int page, int pageSize, Order order = Order.Ascending, CommandFlags flags = CommandFlags.PreferSlave)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            Guard.ArgumentMinValue(page, 1, nameof(page));
            Guard.ArgumentMinValue(pageSize, 1, nameof(pageSize));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.SortedSetRangeByRankWithScores(key, (page - 1) * pageSize, page * pageSize - 1, order, flags);
        }
        /// <summary>
        /// Sorted Set Get Page List With Score 操作(根据分数区间)
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="startScore">开始值</param>
        /// <param name="stopScore">停止值</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="order">排序规则<see cref="Order"/></param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为PreferSlave</param>
        /// <param name="exclude">排除规则<see cref="Exclude"/>,默认为None</param>
        /// <returns></returns>
        public SortedSetEntry[] SortedSetGetPageListWithScore(RedisKey key, double startScore, double stopScore, int page, int pageSize, Order order = Order.Ascending, CommandFlags flags = CommandFlags.PreferSlave, Exclude exclude = Exclude.None)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            Guard.ArgumentMinValue(page, 1, nameof(page));
            Guard.ArgumentMinValue(pageSize, 1, nameof(pageSize));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.SortedSetRangeByScoreWithScores(key, startScore, stopScore, exclude, order, (page - 1) * pageSize, page * pageSize - 1, flags);
        }

        /// <summary>
        /// SortedSet Get All 操作
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="order">排序规则<see cref="Order"/></param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为PreferSlave</param>
        /// <returns></returns>
        public IEnumerable<T> SortedSetGetAll<T>(RedisKey key, Order order = Order.Ascending, CommandFlags flags = CommandFlags.PreferSlave)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            var values = db.SortedSetRangeByRank(key, 0, -1, order, flags);
            var result = new List<T>();
            foreach (var value in values)
            {
                if (!value.IsNullOrEmpty)
                {
                    var data = DataDserialize<T>(value);
                    result.Add(data);
                }
            }
            return result;
        }

        /// <summary>
        /// Sorted Set Combine And Store 操作
        /// </summary>
        /// <param name="storeKey">存储SortedKey</param>
        /// <param name="combineKeys">待合并的Key集合<see cref="Array"/></param>
        /// <param name="operation">合并类型<see cref="SetOperation"/></param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns></returns>
        public long SortedSetCombineAndStore(RedisKey storeKey, RedisKey[] combineKeys, SetOperation setOperation, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(storeKey, nameof(storeKey));
            if (combineKeys == null || !combineKeys.Any())
            {
                return 0;
            }

            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.SortedSetCombineAndStore(setOperation, storeKey, combineKeys, null, Aggregate.Sum, flags);
        }
        /// <summary>
        /// Sorted Set Combine And Store 操作
        /// </summary>
        /// <param name="storeKey">存储SortedKey</param>
        /// <param name="combineKeys">待合并的Key集合<see cref="IEnumerable{T}"/></param>
        /// <param name="operation">合并类型<see cref="SetOperation"/></param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns></returns>
        public long SortedSetCombineAndStore(RedisKey storeKey, IEnumerable<RedisKey> combineKeys, SetOperation setOperation, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(storeKey, nameof(storeKey));
            if (combineKeys == null || !combineKeys.Any())
            {
                return 0;
            }
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            var keys = combineKeys.ToArray();
            return db.SortedSetCombineAndStore(setOperation, storeKey, keys, null, Aggregate.Sum, flags);
        }
        /// <summary>
        ///  Sorted Set Expire At DeteTime 操作
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="expiresAt">DateTime失效点：到达该时间点，立即失效</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns></returns>
        public bool SortedSetExpireAt(RedisKey key, DateTime expiresAt, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.KeyExpire(key, expiresAt, flags);
        }

        /// <summary>
        /// Sorted Set Expire In TimeSpan 操作
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="expiresIn">TimeSpan失效点：经过该时间段，立即失效</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns></returns>
        public bool SortedSetExpireIn(RedisKey key, TimeSpan expiresIn, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.KeyExpire(key, expiresIn, flags);
        }
        #endregion

        #region Hash

        /// <summary>
        /// Hash Set 操作（新增/更新）
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="hashField">hash项Id</param>
        /// <param name="val">值</param>
        /// <param name="when">依据value的执行条件<see cref="When"/></param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns></returns>
        public bool HashSet<T>(RedisKey key, RedisValue hashField, T val, When when = When.Always, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            Guard.ArgumentNotNullOrEmpty(hashField, nameof(hashField));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            var value = DataSerialize(val);
            return db.HashSet(key, hashField, value, when, flags);
        }

        /// <summary>
        /// Hash Set 操作（新增/更新多条）
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="values">值集合<see cref="HashEntry"/></param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns></returns>
        public void HashSetRange(RedisKey key, IEnumerable<HashEntry> hashFields, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            Guard.ArgumentNotNullOrEmpty(hashFields, nameof(hashFields));
            if (hashFields == null || !hashFields.Any())
            {
                return;
            }
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            db.HashSet(key, hashFields.ToArray(), flags);
        }

        /// <summary>
        /// Hash Remove 操作
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashField">hash项</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为PreferSlave</param>
        /// <returns></returns>
        public bool HashRemove(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            Guard.ArgumentNotNullOrEmpty(hashField, nameof(hashField));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.HashDelete(key, hashField, flags);
        }
        /// <summary>
        /// Hash Remove 操作(删除多条)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashField">hash项集合<see cref="Array"/></param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为PreferSlave</param>
        /// <returns></returns>
        public long HashRemove(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            if (hashFields == null || !hashFields.Any())
            {
                return 0;
            }
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.HashDelete(key, hashFields, flags);
        }

        /// <summary>
        /// Hash Remove 操作(删除多条)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashField">hash项集合<see cref="IEnumerable{T}"/></param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为PreferSlave</param>
        /// <returns></returns>
        public long HashRemove(RedisKey key, IEnumerable<RedisValue> hashFields, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            if (hashFields == null || !hashFields.Any())
            {
                return 0;
            }
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.HashDelete(key, hashFields.ToArray(), flags);
        }

        /// <summary>
        /// Hash Remove All 操作(删除全部)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为PreferSlave</param>
        /// <returns></returns>
        public bool HashRemoveAll(RedisKey key, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.KeyDelete(key, flags);
        }

        /// <summary>
        /// Hash Exists 操作
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashField">hash项</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为PreferSlave</param>
        /// <returns></returns>
        public bool HashExists(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.PreferSlave)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            Guard.ArgumentNotNullOrEmpty(hashField, nameof(hashField));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.HashExists(key, hashField, flags);

        }
        /// <summary>
        /// Hash Count 操作
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为PreferSlave</param>
        /// <returns></returns>
        public long HashCount(RedisKey key, CommandFlags flags = CommandFlags.PreferSlave)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.HashLength(key, flags);
        }

        /// <summary>
        /// Hash Get 操作
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="hashField">hash项</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为PreferSlave</param>
        /// <returns></returns>
        public T HashGet<T>(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.PreferSlave)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            Guard.ArgumentNotNullOrEmpty(hashField, nameof(hashField));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            var value = db.HashGet(key, hashField, flags);
            if (!value.IsNullOrEmpty)
            {
                return DataDserialize<T>(value);
            }
            return default(T);
        }

        /// <summary>
        /// Hash Get 操作
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="hashFields">hash项集合</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为PreferSlave</param>
        /// <returns></returns>
        public IEnumerable<T> HashGet<T>(RedisKey key, IEnumerable<RedisValue> hashFields, CommandFlags flags = CommandFlags.PreferSlave)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            Guard.ArgumentNotNullOrEmpty(hashFields, nameof(hashFields));
            var result = new List<T>();
            if (hashFields != null && hashFields.Any())
            {
                ConnectionMultiplexer redis = _RedisProvider.Redis;
                IDatabase db = redis.GetDatabase();
                var values = db.HashGet(key, hashFields.ToArray(), flags);

                if (values != null && values.Length > 0)
                {
                    values.ForEach(x =>
                    {
                        if (!x.IsNullOrEmpty)
                        {
                            result.Add(DataDserialize<T>(x));
                        }
                    });
                }
            }
            return result;
        }

        /// <summary>
        /// Hash Get All 操作
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为PreferSlave</param>
        /// <returns></returns>
        public IEnumerable<T> HashGetAll<T>(RedisKey key, CommandFlags flags = CommandFlags.PreferSlave)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            var result = new List<T>();
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            var values = db.HashValues(key, flags);

            if (values != null && values.Length > 0)
            {
                values.ForEach(x =>
                {
                    if (!x.IsNullOrEmpty)
                    {
                        result.Add(DataDserialize<T>(x));
                    }
                });
            }
            return result;
        }

        /// <summary>
        /// Hash Get All 操作 (返回HashEntry)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为PreferSlave</param>
        /// <returns></returns>
        public HashEntry[] HashGetAll(RedisKey key, CommandFlags flags = CommandFlags.PreferSlave)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.HashGetAll(key, flags);
        }

        /// <summary>
        ///  Hahs Expire At DeteTime 操作
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="expiresAt">DateTime失效点：到达该时间点，立即失效</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns></returns>
        public bool HashExpireAt(RedisKey key, DateTime expiresAt, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.KeyExpire(key, expiresAt, flags);
        }

        /// <summary>
        /// Hash Expire In TimeSpan 操作
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="expiresIn">TimeSpan失效点：经过该时间段，立即失效</param>
        /// <param name="flags">操作标识<see cref="CommandFlags"/>,默认为DemandMaster</param>
        /// <returns></returns>
        public bool HashExpireIn(RedisKey key, TimeSpan expiresIn, CommandFlags flags = CommandFlags.DemandMaster)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            ConnectionMultiplexer redis = _RedisProvider.Redis;
            IDatabase db = redis.GetDatabase();
            return db.KeyExpire(key, expiresIn, flags);
        }
        #endregion

        #endregion

        #region private
        /// <summary>
        /// JSON序列化
        /// </summary>
        /// <typeparam name="T">泛型类</typeparam>
        /// <param name="value">序列化对象</param>
        /// <returns>序列化之后的json字符串</returns>
        private string DataSerialize<T>(T val)
        {
            if (val == null)
            {
                return string.Empty;
            }
            return _SerializeService.ObjectSerialize(val);
        }

        /// <summary>
        /// JSON反序列化
        /// </summary>
        /// <typeparam name="T">泛型类</typeparam>
        /// <param name="value">json字符串</param>
        /// <returns>反序列化之后的对象</returns>
        private T DataDserialize<T>(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return default(T);
            }
            return _SerializeService.ObjectDserialize<T>(val);
        }
        #endregion

    }
}
