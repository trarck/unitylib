using System;

namespace YH.Hash
{
	public class MurmurHash2
	{
		const UInt64 A_M = 0xc6a4a7935bd1e995;
		const int A_R = 47;

		const UInt32 B_M = 0x5bd1e995;
		const int B_R = 24;
		public static UInt64 MurmurHash64A(byte[] data, UInt64 seed)
		{
			if (data == null)
			{
				return seed;
			}

			int len = data.Length;
			UInt64 h = seed ^ ((uint)len * A_M);


			int index = 0;
			while (index+8<=len)
			{
				UInt64 k = BitConverter.ToUInt64(data,index);

				k *= A_M;
				k ^= k >> A_R;
				k *= A_M;

				h ^= k;
				h *= A_M;

				index += 8;
			}

			//int remain = (len & 7) - 1;
			//if (remain > 0)
			//{
			//	for (; remain >= 0; --remain)
			//	{
			//		h ^= (UInt64)data[index + remain] << (remain*8);
			//	}

			//	h *= A_M;
			//}
			switch (len & 7)// == ( (len-index) & 7 ) == ((len % 8) & 7)
			{
				case 7:
					h ^= (UInt64)data[index + 6] << 48;
					h ^= (UInt64)data[index + 5] << 40;
					h ^= (UInt64)data[index + 4] << 32;
					h ^= (UInt64)data[index + 3] << 24;
					h ^= (UInt64)data[index + 2] << 16;
					h ^= (UInt64)data[index + 1] << 8;
					h ^= (UInt64)data[index];
					h *= A_M;
					break;
				case 6:
					h ^= (UInt64)data[index + 5] << 40;
					h ^= (UInt64)data[index + 4] << 32;
					h ^= (UInt64)data[index + 3] << 24;
					h ^= (UInt64)data[index + 2] << 16;
					h ^= (UInt64)data[index + 1] << 8;
					h ^= (UInt64)data[index];
					h *= A_M;
					break;
				case 5:
					h ^= (UInt64)data[index + 4] << 32;
					h ^= (UInt64)data[index + 3] << 24;
					h ^= (UInt64)data[index + 2] << 16;
					h ^= (UInt64)data[index + 1] << 8;
					h ^= (UInt64)data[index];
					h *= A_M;
					break;
				case 4:
					h ^= (UInt64)data[index + 3] << 24;
					h ^= (UInt64)data[index + 2] << 16;
					h ^= (UInt64)data[index + 1] << 8;
					h ^= (UInt64)data[index];
					h *= A_M;
					break;
				case 3:
					h ^= (UInt64)data[index + 2] << 16;
					h ^= (UInt64)data[index + 1] << 8;
					h ^= (UInt64)data[index];
					h *= A_M;
					break;
				case 2:
					h ^= (UInt64)data[index + 1] << 8;
					h ^= (UInt64)data[index];
					h *= A_M;
					break;
				case 1:
					h ^= (UInt64)data[index];
					h *= A_M;
					break;
			};

			//if ((len & 7)!=0)
			//{
			//	h *= A_M;
			//}

			h ^= h >> A_R;
			h *= A_M;
			h ^= h >> A_R;

			return h;
		}

		public static UInt64 MurmurHash64B(byte[] data, UInt64 seed)
		{
			if (data == null)
			{
				return seed;
			}

			int len = data.Length;
			UInt32 h1 = ((UInt32)seed) ^ (UInt32)len;
			UInt32 h2 = ((UInt32)seed) >> 32;

			int index = 0;
			while (len >= index + 8)
			{
				UInt32 k1 = BitConverter.ToUInt32(data, index);
				k1 *= B_M;
				k1 ^= k1 >> B_R;
				k1 *= B_M;
				h1 *= B_M;
				h1 ^= k1;

				index += 4;

				UInt32 k2 = BitConverter.ToUInt32(data, index);
				k2 *= B_M;
				k2 ^= k2 >> B_R;
				k2 *= B_M;
				h2 *= B_M;
				h2 ^= k2;

				index += 4;
			}

			if (len >= index + 4)
			{
				UInt32 k1 = BitConverter.ToUInt32(data, index);
				k1 *= B_M;
				k1 ^= k1 >> B_R;
				k1 *= B_M;
				h1 *= B_M;
				h1 ^= k1;
				index += 4;
			}

			int remain = len - index;
			if (remain > 0)
			{
				//for (remain = remain - 1; remain >= 0; --remain)
				//{
				//	h2 ^= (UInt32)data[index + remain] << (remain * 8);
				//}

				switch (remain)
				{
					case 3:
						h2 ^= (UInt32)data[index + 2] << 16;
						h2 ^= (UInt32)data[index + 1] << 8;
						h2 ^= (UInt32)data[index];
						break;
					case 2:
						h2 ^= (UInt32)data[index + 1] << 8;
						h2 ^= (UInt32)data[index];
						break;
					case 1:
						h2 ^= (UInt32)data[index];
						break;
				};
				h2 *= B_M;
			}

			h1 ^= h2 >> 18;
			h1 *= B_M;
			h2 ^= h1 >> 22;
			h2 *= B_M;
			h1 ^= h2 >> 17;
			h1 *= B_M;
			h2 ^= h1 >> 19;
			h2 *= B_M;

			UInt64 h = h1;

			h = (h << 32) | h2;

			return h;
		}

		/// <summary>
		/// 获取字符串的hash值
		/// 不是大小端安全的，依赖运行平台。
		/// 主要是因为在把字符串buff转成uint时，直接使用指针转换。
		/// </summary>
		/// <param name="key"></param>
		/// <param name="seed"></param>
		/// <returns></returns>
		public static unsafe UInt64 MurmurHash64B(string key, UInt64 seed)
		{
			if (string.IsNullOrEmpty(key))
			{
				return seed;
			}

			int len = key.Length * 2;
			UInt32 h1 = ((UInt32)seed) ^ (UInt32)len;
			UInt32 h2 = ((UInt32)seed) >> 32;
	
			fixed (char* pStr = key)
			{
				UInt32* pData = (UInt32*)pStr;

				while (len >= 8)
				{
					UInt32 k1 = *pData++;
					k1 *= B_M;
					k1 ^= k1 >> B_R;
					k1 *= B_M;
					h1 *= B_M;
					h1 ^= k1;

					UInt32 k2 = *pData++;
					k2 *= B_M;
					k2 ^= k2 >> B_R;
					k2 *= B_M;
					h2 *= B_M;
					h2 ^= k2;

					len -= 8;
				}

				if (len >= 4)
				{
					UInt32 k1 = *pData++;
					k1 *= B_M;
					k1 ^= k1 >> B_R;
					k1 *= B_M;
					h1 *= B_M;
					h1 ^= k1;

					len -= 4;
				}

				if (len > 0)
				{
					byte* pRemain = (byte*)pData;
					//for (len = len - 1; len >= 0; --len)
					//{
					//	h2 ^= (UInt32)(*(pRemain + len)) << (len * 8);
					//}

					switch (len)
					{
						case 3:
							h2 ^= (UInt32)(*(pRemain + 2)) << 16;
							h2 ^= (UInt32)(*(pRemain + 1)) << 8;
							h2 ^= (UInt32)(*pRemain);
							h2 *= B_M;
							break;
						case 2:
							h2 ^= (UInt32)(*(pRemain + 1)) << 8;
							h2 ^= (UInt32)(*pRemain);
							h2 *= B_M;
							break;
						case 1:
							h2 ^= (UInt32)(*pRemain);
							h2 *= B_M;
							break;
					};
				}
			}

			h1 ^= h2 >> 18;
			h1 *= B_M;
			h2 ^= h1 >> 22;
			h2 *= B_M;
			h1 ^= h2 >> 17;
			h1 *= B_M;
			h2 ^= h1 >> 19;
			h2 *= B_M;

			UInt64 h = h1;

			h = (h << 32) | h2;

			return h;
		}

		/// <summary>
		/// 计算字符串hash。
		/// 使用小端方式转化字符串buff到uint。
		/// 跳过utf16中的0字符。目前只对ascii码的字符串有效。
		/// </summary>
		/// <param name="key"></param>
		/// <param name="seed"></param>
		/// <returns></returns>
		public static unsafe UInt64 MurmurHash64BLE(string key, UInt64 seed)
		{
			if (string.IsNullOrEmpty(key))
			{
				return seed;
			}

			int len = key.Length;
			UInt32 h1 = ((UInt32)seed) ^ (UInt32)len;
			UInt32 h2 = ((UInt32)seed) >> 32;

			fixed (char* pStr = key)
			{
				byte* pData = (byte*)pStr;

				while (len >= 8)
				{

					UInt32 k1 = ((UInt32)(*pData)) | ((UInt32)(*(pData + 2))) | ((UInt32)(*(pData + 4))) | ((UInt32)(*(pData + 6)));
					pData += 8;

					k1 *= B_M;
					k1 ^= k1 >> B_R;
					k1 *= B_M;
					h1 *= B_M;
					h1 ^= k1;
					

					UInt32 k2 = ((UInt32)(*pData)) | ((UInt32)(*(pData + 2))) | ((UInt32)(*(pData + 4))) | ((UInt32)(*(pData + 6)));
					pData += 8;

					k2 *= B_M;
					k2 ^= k2 >> B_R;
					k2 *= B_M;
					h2 *= B_M;
					h2 ^= k2;

					len -= 8;
				}

				if (len >= 4)
				{
					UInt32 k1 = ((UInt32)(*pData)) | ((UInt32)(*(pData + 2))) | ((UInt32)(*(pData + 4))) | ((UInt32)(*(pData + 6)));
					pData += 8;

					k1 *= B_M;
					k1 ^= k1 >> B_R;
					k1 *= B_M;
					h1 *= B_M;
					h1 ^= k1;

					len -= 4;
				}

				if (len > 0)
				{
					//for (len = len - 1; len >= 0; --len)
					//{
					//	h2 ^= (UInt32)(*(pData + len)) << (len * 8);
					//}

					switch (len)
					{
						case 3:
							h2 ^= (UInt32)(*(pData + 4)) << 16;
							h2 ^= (UInt32)(*(pData + 2)) << 8;
							h2 ^= (UInt32)(*pData);
							break;
						case 2:
							h2 ^= (UInt32)(*(pData + 2)) << 8;
							h2 ^= (UInt32)(*pData);
							break;
						case 1:
							h2 ^= (UInt32)(*pData);
							break;
					};
					h2 *= B_M;
				}
			}

			h1 ^= h2 >> 18;
			h1 *= B_M;
			h2 ^= h1 >> 22;
			h2 *= B_M;
			h1 ^= h2 >> 17;
			h1 *= B_M;
			h2 ^= h1 >> 19;
			h2 *= B_M;

			UInt64 h = h1;

			h = (h << 32) | h2;

			return h;
		}

		public static unsafe UInt64 MurmurHash64BBE(string key, UInt64 seed)
		{
			if (string.IsNullOrEmpty(key))
			{
				return seed;
			}
			int len = key.Length * 2;
			UInt32 h1 = ((UInt32)seed) ^ (UInt32)len;
			UInt32 h2 = ((UInt32)seed) >> 32;

			fixed (char* pStr = key)
			{
				byte* pData = (byte*)pStr;

				while (len >= 8)
				{
					UInt32 k1 = ReadUInt32BE(pData);
					k1 *= B_M;
					k1 ^= k1 >> B_R;
					k1 *= B_M;
					h1 *= B_M;
					h1 ^= k1;

					UInt32 k2 = ReadUInt32BE(pData);
					k2 *= B_M;
					k2 ^= k2 >> B_R;
					k2 *= B_M;
					h2 *= B_M;
					h2 ^= k2;

					len -= 8;
				}

				if (len >= 4)
				{
					UInt32 k1 = ReadUInt32BE(pData);
					k1 *= B_M;
					k1 ^= k1 >> B_R;
					k1 *= B_M;
					h1 *= B_M;
					h1 ^= k1;

					len -= 4;
				}

				if (len > 0)
				{
					//for (len = len - 1; len >= 0; --len)
					//{
					//	h2 ^= (UInt32)(*(pData + len)) << (len * 8);
					//}
					switch (len)
					{
						case 3:
							h2 ^= (UInt32)(*(pData + 2)) << 16;
							h2 ^= (UInt32)(*(pData + 1)) << 8;
							h2 ^= (UInt32)(*pData);
							break;
						case 2:
							h2 ^= (UInt32)(*(pData + 1)) << 8;
							h2 ^= (UInt32)(*pData);
							break;
						case 1:
							h2 ^= (UInt32)(*pData);
							break;
					};
					h2 *= B_M;
				}
			}

			h1 ^= h2 >> 18;
			h1 *= B_M;
			h2 ^= h1 >> 22;
			h2 *= B_M;
			h1 ^= h2 >> 17;
			h1 *= B_M;
			h2 ^= h1 >> 19;
			h2 *= B_M;

			UInt64 h = h1;

			h = (h << 32) | h2;

			return h;
		}

		/// <summary>
		/// c#字符串是按utf16存储的，对于只包含英文的字符串，可以跳过0。相当于计算减半。
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		private static unsafe UInt32 ReadUInt32LE(byte* p)
		{
			UInt32 a = *p;
			a |= (UInt32)(*(p+2)) << 8;
			a |= (UInt32)(*(p+4)) << 16;
			a |= (UInt32)(*(p+6)) << 24;
			p += 8;
			return a;
		}

		private static unsafe UInt32 ReadUInt32BE(byte* p)
		{
			UInt32 a = *p++;
			a |= a << 8 + *p++;
			a |= a << 8 + *p++;
			a |= a << 8 + *p++;
			return a;
		}
	}
}
