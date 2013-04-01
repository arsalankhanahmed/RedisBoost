#region Apache Licence, Version 2.0
/*
 Copyright 2013 Andrey Bulygin.

 Licensed under the Apache License, Version 2.0 (the "License"); 
 you may not use this file except in compliance with the License. 
 You may obtain a copy of the License at 

		http://www.apache.org/licenses/LICENSE-2.0

 Unless required by applicable law or agreed to in writing, software 
 distributed under the License is distributed on an "AS IS" BASIS, 
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
 See the License for the specific language governing permissions 
 and limitations under the License.
 */
#endregion

using System;
using System.Threading.Tasks;
using NBoosters.RedisBoost.Core;
using NBoosters.RedisBoost.Misk;

namespace NBoosters.RedisBoost
{
	public partial class RedisClient
	{
		public Task<Bulk> GetAsync(string key)
		{
			return BulkResponseCommand(RedisConstants.Get, key.ToBytes());
		}

		public Task<string> SetAsync<T>(string key, T value)
		{
			return SetAsync(key, Serialize(value));
		}

		public Task<string> SetAsync(string key, byte[] value)
		{
			return StatusResponseCommand(RedisConstants.Set, key.ToBytes(), value);
		}

		public Task<long> AppendAsync<T>(string key, T value)
		{
			return AppendAsync(key, Serialize(value));
		}

		public Task<long> AppendAsync(string key, byte[] value)
		{
			return IntegerResponseCommand(RedisConstants.Append, key.ToBytes(), value);
		}

		public Task<long> BitCountAsync(string key)
		{
			return IntegerResponseCommand(RedisConstants.BitCount);
		}

		public Task<long> BitCountAsync(string key, int start, int end)
		{
			return IntegerResponseCommand(RedisConstants.BitCount, start.ToBytes(), end.ToBytes());
		}

		public Task<long> DecrAsync(string key)
		{
			return IntegerResponseCommand(RedisConstants.Decr, key.ToBytes());
		}

		public Task<long> IncrAsync(string key)
		{
			return IntegerResponseCommand(RedisConstants.Incr, key.ToBytes());
		}

		public Task<long> DecrByAsync(string key, int decrement)
		{
			return IntegerResponseCommand(RedisConstants.DecrBy, key.ToBytes(), decrement.ToBytes());
		}

		public Task<long> IncrByAsync(string key, int increment)
		{
			return IntegerResponseCommand(RedisConstants.IncrBy, key.ToBytes(), increment.ToBytes());
		}

		public Task<Bulk> IncrByFloatAsync(string key, double increment)
		{
			return BulkResponseCommand(RedisConstants.IncrByFloat, key.ToBytes(), increment.ToBytes());
		}

		public Task<T> GetRangeAsync<T>(string key, int start, int end)
		{
			return GetRangeAsync(key, start, end).ContinueWithIfNoError(t => Deserialize<T>(t.Result));
		}

		public Task<Bulk> GetRangeAsync(string key, int start, int end)
		{
			return BulkResponseCommand(RedisConstants.GetRange, key.ToBytes(), start.ToBytes(), end.ToBytes());
		}

		public Task<long> SetRangeAsync<T>(string key, int offset, T value)
		{
			return SetRangeAsync(key, offset, Serialize(value));
		}

		public Task<long> SetRangeAsync(string key, int offset, byte[] value)
		{
			return IntegerResponseCommand(RedisConstants.SetRange, key.ToBytes(), offset.ToBytes(), value);
		}

		public Task<long> StrLenAsync(string key)
		{
			return IntegerResponseCommand(RedisConstants.StrLen, key.ToBytes());
		}

		public Task<Bulk> GetSetAsync<T>(string key, T value)
		{
			return GetSetAsync(key, Serialize(value));
		}

		public Task<Bulk> GetSetAsync(string key, byte[] value)
		{
			return BulkResponseCommand(RedisConstants.GetSet, key.ToBytes(), value);
		}

		public Task<MultiBulk> MGetAsync(params string[] keys)
		{
			var request = ComposeRequest(RedisConstants.MGet, keys);
			return MultiBulkResponseCommand(request);
		}

		public Task<string> MSetAsync(params MSetArgs[] args)
		{
			var request = ComposeMSetRequest(RedisConstants.MSet, args);
			return StatusResponseCommand(request);
		}

		public Task<long> MSetNxAsync(params MSetArgs[] args)
		{
			var request = ComposeMSetRequest(RedisConstants.MSetNx, args);
			return IntegerResponseCommand(request);
		}

		public Task<string> SetExAsync<T>(string key, int seconds, T value)
		{
			return SetExAsync(key, seconds, Serialize(value));
		}

		public Task<string> SetExAsync(string key, int seconds, byte[] value)
		{
			return StatusResponseCommand(RedisConstants.SetEx, key.ToBytes(), seconds.ToBytes(), value);
		}

		public Task<string> PSetExAsync<T>(string key, int milliseconds, T value)
		{
			return PSetExAsync(key, milliseconds, Serialize(value));
		}

		public Task<string> PSetExAsync(string key, int milliseconds, byte[] value)
		{
			return StatusResponseCommand(RedisConstants.PSetEx, key.ToBytes(), milliseconds.ToBytes(), value);
		}

		public Task<long> SetNxAsync<T>(string key, T value)
		{
			return SetNxAsync(key, Serialize(value));
		}

		public Task<long> SetNxAsync(string key, byte[] value)
		{
			return IntegerResponseCommand(RedisConstants.SetNx, key.ToBytes(), value);
		}

		public Task<long> BitOpAsync(BitOpType bitOp, string destKey, params string[] keys)
		{
			var request = ComposeRequest(RedisConstants.BitOp, bitOp.ToBytes(), destKey.ToBytes(), keys);
			return IntegerResponseCommand(request);
		}

		public Task<long> GetBitAsync(string key, long offset)
		{
			return IntegerResponseCommand(RedisConstants.GetBit, key.ToBytes(), offset.ToBytes());
		}

		public Task<long> SetBitAsync(string key, long offset, int value)
		{
			return IntegerResponseCommand(RedisConstants.SetBit, key.ToBytes(), offset.ToBytes(), value.ToBytes());
		}

		#region request composing
		private byte[][] ComposeMSetRequest(byte[] command, MSetArgs[] args)
		{
			if (args.Length == 0)
				throw new ArgumentException("Invalid argument items count", "args");

			var request = new byte[args.Length * 2 + 1][];
			request[0] = command;
			for (int i = 0; i < args.Length; i++)
			{
				var arg = args[i];
				request[i * 2 + 1] = arg.KeyOrField.ToBytes();
				request[i * 2 + 2] = arg.IsArray ? (byte[])arg.Value : Serialize(arg.Value);
			}
			return request;
		}
		#endregion
	}
}
