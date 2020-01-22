using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace DradonBotSharp.Utils
{
	public static class NBTParser
	{


		public static object Parse(string encoded)
		{
			using GZipStream stream = new GZipStream(new MemoryStream(Convert.FromBase64String(encoded)), CompressionMode.Decompress);
			if (!new NBTReader(stream).TryParseTag(out var tag))
			{
				throw new InvalidDataException("Invalid NBT data");
			}
			return tag;
		}

		private unsafe class NBTReader : BinaryReader
		{
			public NBTReader(Stream stream) : base(stream) { }

			public T ParseBE<T>(Func<byte[], int, T> byteConverter) where T : unmanaged
			{
				byte[] bytes = ReadBytes(sizeof(T));
				if (BitConverter.IsLittleEndian)
				{
					bytes = bytes.Reverse().ToArray();
				}
				return byteConverter(bytes, 0);
			}

			public T[] ParseArray<T>(Func<T> elementProvider)
			{
				T[] array = new T[ParseInt()];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = elementProvider();
				}
				return array;
			}

			public bool TryParseTag(out KeyValuePair<string, object> tag)
			{
				sbyte elementId = ParseSByte();
				if (elementId == 0)
				{
					tag = default;
					return false;
				}
				tag = new KeyValuePair<string, object>(ParseString(), ParseNBT(elementId));
				return true;
			}

			public object ParseNBT(sbyte tagId)
            {
                switch (tagId)
                {
                    case 0:
                        return null;
                    case 1:
                        return ParseSByte();
                    case 2:
                        return ParseShort();
                    case 3:
                        return ParseInt();
                    case 4:
                        return ParseLong();
                    case 5:
                        return ParseFloat();
                    case 6:
                        return ParseDouble();
                    case 7:
                        return ParseSByteArray();
                    case 8:
                        return ParseString();
                    case 9:
                        return ParseList();
                    case 10:
                        return ParseCompound();
                    case 11:
                        return ParseIntArray();
                    case 12:
                        return ParseLongArray();
                    default:
                        throw new NotSupportedException($"TagID {tagId} is not supported");
                }
            }

            public sbyte ParseSByte() => ReadSByte();
			public short ParseShort() => ParseBE(BitConverter.ToInt16);
			public int ParseInt() => ParseBE(BitConverter.ToInt32);
			public long ParseLong() => ParseBE(BitConverter.ToInt64);
			public float ParseFloat() => ParseBE(BitConverter.ToSingle);
			public double ParseDouble() => ParseBE(BitConverter.ToDouble);
			public sbyte[] ParseSByteArray() => ParseArray(ParseSByte);
			public string ParseString() => Encoding.UTF8.GetString(ReadBytes(ParseBE(BitConverter.ToInt16)));
			public IList<object> ParseList()
			{
				sbyte elementId = ParseSByte();
				return ParseArray(() => ParseNBT(elementId));
			}
			public IDictionary<string, object> ParseCompound()
			{
				IDictionary<string, object> compound = new Dictionary<string, object>();
				while (true)
				{
					if (!TryParseTag(out var tag)) { break; }
					compound.Add(tag);
				}
				return compound;
			}
			public int[] ParseIntArray() => ParseArray(ParseInt);
			public long[] ParseLongArray() => ParseArray(ParseLong);
		}
	}
}
