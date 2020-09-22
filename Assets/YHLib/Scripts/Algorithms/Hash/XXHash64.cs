using System;
using System.IO;

namespace YH.Hash
{
	public class XXHash64
	{
		public struct XXHState
		{
			public ulong total_len;
			public ulong seed;
			public ulong v1;
			public ulong v2;
			public ulong v3;
			public ulong v4;
			public int memsize;
			public byte[] memory;
		};

		const ulong PRIME64_1 = 11400714785074694791UL,
					PRIME64_2 = 14029467366897019727UL,
					PRIME64_3 = 1609587929392839161UL,
					PRIME64_4 = 9650029242287828579UL,
					PRIME64_5 = 2870177450012600261UL;

		protected XXHState _state;
		public XXHash64()
		{

		}

		public static ulong CalculateHash(byte[] buf, int len = -1, ulong seed = 0)
		{
			ulong h64;
			int index = 0;
			if (len == -1)
			{
				len = buf.Length;
			}


			if (len >= 32)
			{
				int limit = len - 32;
				ulong v1 = seed + PRIME64_1 + PRIME64_2;
				ulong v2 = seed + PRIME64_2;
				ulong v3 = seed;
				ulong v4 = seed - PRIME64_1;

				do
				{
					v1 = CalcSubHash(v1, buf, index);
					index += 8;
					v2 = CalcSubHash(v2, buf, index);
					index += 8;
					v3 = CalcSubHash(v3, buf, index);
					index += 8;
					v4 = CalcSubHash(v4, buf, index);
					index += 8;
				} while (index <= limit);

				h64 = RotateLeft(v1, 1) + RotateLeft(v2, 7) + RotateLeft(v3, 12) + RotateLeft(v4, 18);

				v1 *= PRIME64_2;
				v1 = RotateLeft(v1, 31);
				v1 *= PRIME64_1;
				h64 ^= v1;
				h64 = h64 * PRIME64_1 + PRIME64_4;

				v2 *= PRIME64_2;
				v2 = RotateLeft(v2, 31);
				v2 *= PRIME64_1;
				h64 ^= v2;
				h64 = h64 * PRIME64_1 + PRIME64_4;

				v3 *= PRIME64_2;
				v3 = RotateLeft(v3, 31);
				v3 *= PRIME64_1;
				h64 ^= v3;
				h64 = h64 * PRIME64_1 + PRIME64_4;

				v4 *= PRIME64_2;
				v4 = RotateLeft(v4, 31);
				v4 *= PRIME64_1;
				h64 ^= v4;
				h64 = h64 * PRIME64_1 + PRIME64_4;
			}
			else
			{
				h64 = seed + PRIME64_5;
			}

			h64 += (uint)len;

			while (index <= len - 8)
			{
				ulong k1 = BitConverter.ToUInt64(buf, index);
				k1 *= PRIME64_2;
				k1 = RotateLeft(k1, 31);
				k1 *= PRIME64_1;
				h64 ^= k1;
				h64 = RotateLeft(h64, 27) * PRIME64_1 + PRIME64_4;

				index += 8;
			}

			if (index <= len - 4)
			{
				h64 ^= (ulong)(BitConverter.ToUInt32(buf, index)) * PRIME64_1;
				h64 = RotateLeft(h64, 23) * PRIME64_2 + PRIME64_3;
				index += 4;
			}

			while (index < len)
			{
				h64 += buf[index] * PRIME64_5;
				h64 = RotateLeft(h64, 11) * PRIME64_1;
				index++;
			}

			h64 ^= h64 >> 33;
			h64 *= PRIME64_2;
			h64 ^= h64 >> 29;
			h64 *= PRIME64_3;
			h64 ^= h64 >> 32;

			return h64;
		}

		public static ulong CalculateHash(Stream stream, long len = -1, uint seed = 0)
		{
			ulong h64;
			var index = 0;

			if (!stream.CanRead || !stream.CanSeek)
				throw new InvalidOperationException("Stream has to be seekable and readable");

			if (len == -1)
			{
				len = stream.Length;
			}

			var streamPosition = stream.Position;
			stream.Seek(0, SeekOrigin.Begin);

			var buffer = new byte[32];
			if (len >= 32)
			{
				var limit = len - 32;
				ulong v1 = seed + PRIME64_1 + PRIME64_2;
				ulong v2 = seed + PRIME64_2;
				ulong v3 = seed;
				ulong v4 = seed - PRIME64_1;

				do
				{
					var loopIndex = 0;
					stream.Read(buffer, 0, buffer.Length);

					v1 = CalcSubHash(v1, buffer, loopIndex);
					loopIndex += 8;
					v2 = CalcSubHash(v2, buffer, loopIndex);
					loopIndex += 8;
					v3 = CalcSubHash(v3, buffer, loopIndex);
					loopIndex += 8;
					v4 = CalcSubHash(v4, buffer, loopIndex);
					loopIndex += 8;

					index += loopIndex;
				} while (index <= limit);

				h64 = RotateLeft(v1, 1) + RotateLeft(v2, 7) + RotateLeft(v3, 12) + RotateLeft(v4, 18);

				v1 *= PRIME64_2;
				v1 = RotateLeft(v1, 31);
				v1 *= PRIME64_1;
				h64 ^= v1;
				h64 = h64 * PRIME64_1 + PRIME64_4;

				v2 *= PRIME64_2;
				v2 = RotateLeft(v2, 31);
				v2 *= PRIME64_1;
				h64 ^= v2;
				h64 = h64 * PRIME64_1 + PRIME64_4;

				v3 *= PRIME64_2;
				v3 = RotateLeft(v3, 31);
				v3 *= PRIME64_1;
				h64 ^= v3;
				h64 = h64 * PRIME64_1 + PRIME64_4;

				v4 *= PRIME64_2;
				v4 = RotateLeft(v4, 31);
				v4 *= PRIME64_1;
				h64 ^= v4;
				h64 = h64 * PRIME64_1 + PRIME64_4;
			}
			else
			{
				h64 = seed + PRIME64_5;
			}

			h64 += (ulong)len;

			buffer = new byte[8];
			while (index <= len - 8)
			{
				stream.Read(buffer, 0, buffer.Length);
				ulong k1 = BitConverter.ToUInt64(buffer, 0);
				k1 *= PRIME64_2;
				k1 = RotateLeft(k1, 31);
				k1 *= PRIME64_1;
				h64 ^= k1;
				h64 = RotateLeft(h64, 27) * PRIME64_1 + PRIME64_4;

				index += 8;
			}

			buffer = new byte[4];
			if (index <= len - 4)
			{
				stream.Read(buffer, 0, buffer.Length);
				h64 ^= (ulong)(BitConverter.ToUInt32(buffer, index)) * PRIME64_1;
				h64 = RotateLeft(h64, 23) * PRIME64_2 + PRIME64_3;

				index += 4;
			}


			buffer = new byte[1];
			while (index < len)
			{
				stream.Read(buffer, 0, buffer.Length);
				h64 += buffer[0] * PRIME64_5;
				h64 = RotateLeft(h64, 11) * PRIME64_1;
				index++;
			}

			stream.Seek(streamPosition, SeekOrigin.Begin);

			h64 ^= h64 >> 33;
			h64 *= PRIME64_2;
			h64 ^= h64 >> 29;
			h64 *= PRIME64_3;
			h64 ^= h64 >> 32;

			return h64;
		}

		public void Init(ulong seed = 0)
		{
			_state.seed = seed;
			_state.v1 = seed + PRIME64_1 + PRIME64_2;
			_state.v2 = seed + PRIME64_2;
			_state.v3 = seed + 0;
			_state.v4 = seed - PRIME64_1;
			_state.total_len = 0;
			_state.memsize = 0;
			_state.memory = new byte[16];
		}

		public bool Update(byte[] input, int len)
		{
			int index = 0;

			_state.total_len += (uint)len;

			if (_state.memsize + len < 32)
			{
				Array.Copy(input, 0, _state.memory, _state.memsize, len);
				_state.memsize += len;

				return true;
			}

			if (_state.memsize > 0)
			{
				Array.Copy(input, 0, _state.memory, _state.memsize, 32 - _state.memsize);

				_state.v1 = CalcSubHash(_state.v1, _state.memory, index);
				index += 8;
				_state.v2 = CalcSubHash(_state.v2, _state.memory, index);
				index += 8;
				_state.v3 = CalcSubHash(_state.v3, _state.memory, index);
				index += 8;
				_state.v4 = CalcSubHash(_state.v4, _state.memory, index);
				index += 8;

				index = 0;
				_state.memsize = 0;
			}

			if (index <= len - 32)
			{
				int limit = len - 32;
				ulong v1 = _state.v1;
				ulong v2 = _state.v2;
				ulong v3 = _state.v3;
				ulong v4 = _state.v4;

				do
				{
					v1 = CalcSubHash(v1, input, index);
					index += 8;
					v2 = CalcSubHash(v2, input, index);
					index += 8;
					v3 = CalcSubHash(v3, input, index);
					index += 8;
					v4 = CalcSubHash(v4, input, index);
					index += 8;
				} while (index <= limit);

				_state.v1 = v1;
				_state.v2 = v2;
				_state.v3 = v3;
				_state.v4 = v4;
			}

			if (index < len)
			{
				Array.Copy(input, index, _state.memory, 0, len - index);
				_state.memsize = len - index;
			}
			return true;
		}

		public ulong Digest()
		{
			ulong h64;
			int index = 0;
			if (_state.total_len >= 32)
			{
				h64 = RotateLeft(_state.v1, 1) + RotateLeft(_state.v2, 7) + RotateLeft(_state.v3, 12) + RotateLeft(_state.v4, 18);

				_state.v1 *= PRIME64_2;
				_state.v1 = RotateLeft(_state.v1, 31);
				_state.v1 *= PRIME64_1;
				h64 ^= _state.v1;
				h64 = h64 * PRIME64_1 + PRIME64_4;

				_state.v2 *= PRIME64_2;
				_state.v2 = RotateLeft(_state.v2, 31);
				_state.v2 *= PRIME64_1;
				h64 ^= _state.v2;
				h64 = h64 * PRIME64_1 + PRIME64_4;

				_state.v3 *= PRIME64_2;
				_state.v3 = RotateLeft(_state.v3, 31);
				_state.v3 *= PRIME64_1;
				h64 ^= _state.v3;
				h64 = h64 * PRIME64_1 + PRIME64_4;

				_state.v4 *= PRIME64_2;
				_state.v4 = RotateLeft(_state.v4, 31);
				_state.v4 *= PRIME64_1;
				h64 ^= _state.v4;
				h64 = h64 * PRIME64_1 + PRIME64_4;
			}
			else
			{
				h64 = _state.seed + PRIME64_5;
			}

			h64 += (ulong)_state.total_len;

			while (index <= _state.memsize - 8)
			{
				ulong k1 = BitConverter.ToUInt64(_state.memory, index);
				k1 *= PRIME64_2;
				k1 = RotateLeft(k1, 31);
				k1 *= PRIME64_1;
				h64 ^= k1;
				h64 = RotateLeft(h64, 27) * PRIME64_1 + PRIME64_4;

				index += 8;
			}

			if (index <= _state.memsize - 4)
			{
				h64 ^= (ulong)(BitConverter.ToUInt32(_state.memory, index)) * PRIME64_1;
				h64 = RotateLeft(h64, 23) * PRIME64_2 + PRIME64_3;
				index += 4;
			}

			while (index < _state.memsize)
			{
				h64 += _state.memory[index] * PRIME64_5;
				h64 = RotateLeft(h64, 11) * PRIME64_1;
				index++;
			}

			h64 ^= h64 >> 15;
			h64 *= PRIME64_2;
			h64 ^= h64 >> 13;
			h64 *= PRIME64_3;
			h64 ^= h64 >> 16;

			return h64;
		}
		private static ulong CalcSubHash(ulong value, byte[] buf, int index)
		{
			ulong read_value = BitConverter.ToUInt64(buf, index);
			value += read_value * PRIME64_2;
			value = RotateLeft(value, 31);
			value *= PRIME64_1;
			return value;
		}

		private static ulong RotateLeft(ulong value, int count)
		{
			return (value << count) | (value >> (64 - count));
		}
	}
}