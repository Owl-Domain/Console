using System.Text;

namespace OwlDomain.Console.Capabilities.Terminfo;

/// <summary>
/// 	Represents a parser for compiled Terminfo files.
/// </summary>
public static partial class TerminfoParser
{
	#region Nested types
	private enum Format : short
	{
		Legacy = 282, // 0o432
		Extended = 542, // 0o1036
	}
	#endregion

	#region Functions
	/// <summary>Parses the given binary <paramref name="reader"/> as a terminfo value.</summary>
	/// <param name="reader">The binary reader to use to parse the data.</param>
	/// <returns>The parsed terminal capabilities.</returns>
	public static ITerminalCapabilityInfo Parse(BinaryReader reader)
	{
		List<ITerminalCapability> capabilities = [];

		// Note(Nightowl): Read the legacy header;
		Format format = reader.ReadFormat();
		short nameBytes = reader.ReadShort();
		short booleanCount = reader.ReadShort();
		short numberCount = reader.ReadShort();
		short stringCount = reader.ReadShort();
		short stringBytes = reader.ReadShort();

		// Note(Nightowl): Skip name section;
		reader.BaseStream.Position += nameBytes;

		ReadBooleanCapabilities(reader, capabilities, booleanCount);
		reader.AlignEven();
		ReadNumberCapabilities(reader, capabilities, numberCount, format);
		ReadStringCapabilities(reader, capabilities, stringCount, stringBytes);
		ReadExtendedCapabilities(reader, capabilities, format);

		return new TerminalCapabilityInfo(capabilities);
	}
	#endregion

	#region Section helpers
	private static void ReadBooleanCapabilities(BinaryReader reader, List<ITerminalCapability> capabilities, int count)
	{
		if (count > BooleanCapabilities.Count)
			Throw.New.InvalidDataException($"The header indicates that there are {count:n0} boolean capabilities present in the file, but only {BooleanCapabilities.Count:n0} boolean capabilities are known.");

		for (int i = 0; i < count; i++)
		{
			bool? value = reader.ReadBooleanCapability();
			if (value is not true)
				continue;

			CapabilityName name = BooleanCapabilities[i];

			ITerminalCapability capability = TerminalCapability.New(name.Id, name.FriendlyName, value.Value);
			capabilities.Add(capability);
		}
	}
	private static void ReadNumberCapabilities(BinaryReader reader, List<ITerminalCapability> capabilities, int count, Format format)
	{
		if (count > NumberCapabilities.Count)
			Throw.New.InvalidDataException($"The header indicates that there are {count:n0} number capabilities present in the file, but only {NumberCapabilities.Count:n0} number capabilities are known.");

		for (int i = 0; i < count; i++)
		{
			int value = reader.ReadNumberCapability(format);
			if (value is -1 or -2)
				continue;

			CapabilityName name = NumberCapabilities[i];

			ITerminalCapability capability = TerminalCapability.New(name.Id, name.FriendlyName, value);
			capabilities.Add(capability);
		}
	}
	private static void ReadStringCapabilities(BinaryReader reader, List<ITerminalCapability> capabilities, int count, int tableSize)
	{
		if (count > StringCapabilities.Count)
			Throw.New.InvalidDataException($"The header indicates that there are {count:n0} string capabilities present in the file, but only {StringCapabilities.Count:n0} string capabilities are known.");

		Span<short> valueIndecies = count <= 128 ? stackalloc short[count] : new short[count];

		for (int i = 0; i < count; i++)
		{
			short index = reader.ReadShort();
			valueIndecies[i] = index;
		}

		Dictionary<int, string> table = reader.ReadStringTable(tableSize);
		for (int i = 0; i < valueIndecies.Length; i++)
		{
			int valueIndex = valueIndecies[i];
			if (valueIndex is -1 or -2)
				continue;

			CapabilityName name = StringCapabilities[i];

			string value = table[valueIndex];

			ITerminalCapability capability = TerminalCapability.New(name.Id, name.FriendlyName, value);
			capabilities.Add(capability);
		}
	}
	private static void ReadExtendedCapabilities(BinaryReader reader, List<ITerminalCapability> capabilities, Format format)
	{
		if (reader.BaseStream.Position == reader.BaseStream.Length)
			return;

		// Note(Nightowl): Extended header;
		short booleanCount = reader.ReadShort();
		short numberCount = reader.ReadShort();
		short stringCount = reader.ReadShort();
		short tableItemCount = reader.ReadShort();
		short tableSize = reader.ReadShort();
		int nameCount = booleanCount + numberCount + stringCount;

		// Note(Nightowl): Prepare buffers;
		Span<bool?> booleans = booleanCount <= 128 ? stackalloc bool?[booleanCount] : new bool?[booleanCount];
		Span<int> numbers = numberCount <= 128 ? stackalloc int[numberCount] : new int[numberCount];
		Span<short> strings = stringCount <= 128 ? stackalloc short[stringCount] : new short[stringCount];
		Span<short> names = nameCount <= 128 ? stackalloc short[nameCount] : new short[nameCount];

		// Note(Nightowl): Read capabilities values;
		for (int i = 0; i < booleans.Length; i++)
		{
			bool? value = reader.ReadBooleanCapability();
			booleans[i] = value;
		}

		for (int i = 0; i < numbers.Length; i++)
		{
			int value = reader.ReadNumberCapability(format);
			numbers[i] = value;
		}

		int stringWithValueCount = 0;
		for (int i = 0; i < strings.Length; i++)
		{
			short value = reader.ReadShort();
			if (value is not -1 and not -2)
				stringWithValueCount++;

			strings[i] = value;
		}

		for (int i = 0; i < names.Length; i++)
		{
			short value = reader.ReadShort();
			names[i] = value;
		}

		Dictionary<int, string> table = reader.ReadStringTable(tableSize, stringWithValueCount, out int nameOffset);

		// Note(Nightowl): Read boolean capabilities;
		int runningOffset = 0;
		for (int i = 0; i < booleans.Length; i++)
		{
			bool? value = booleans[i];
			if (value is not true)
				continue;

			short nameIndex = names[runningOffset++];
			string name = table[nameOffset + nameIndex];

			ITerminalCapability capability = TerminalCapability.New(name, null, value.Value);
			capabilities.Add(capability);
		}

		// Note(Nightowl): Read number capabilities;
		for (int i = 0; i < numbers.Length; i++)
		{
			int value = numbers[i];
			if (value is -1 or -2)
				continue;

			short nameIndex = names[runningOffset++];
			string name = table[nameOffset + nameIndex];

			ITerminalCapability capability = TerminalCapability.New(name, null, value);
			capabilities.Add(capability);
		}

		// Note(Nightowl): Read string capabilities;
		for (int i = 0; i < strings.Length; i++)
		{
			int valueIndex = strings[i];
			if (valueIndex is -1 or -2)
				continue;


			short nameIndex = names[runningOffset++];
			string name = table[nameOffset + nameIndex];
			string value = table[valueIndex];

			ITerminalCapability capability = TerminalCapability.New(name, null, value);
			capabilities.Add(capability);
		}
	}
	#endregion

	#region Helpers
	private static Dictionary<int, string> ReadStringTable(this BinaryReader reader, int size, int threshold, out int thresholdOffset)
	{
		byte[] stringTable = reader.ReadBytes(size);
		Dictionary<int, string> values = [];
		thresholdOffset = 0;

		for (int i = 0; i < stringTable.Length;)
		{
			int end = Array.IndexOf<byte>(stringTable, 0, i);

			if (end is -1)
				Throw.New.InvalidDataException($"Each string in the table should be null terminated, the string at offset 0x{i:x2} ({i:n0}) was not.");

			string value = Encoding.ASCII.GetString(stringTable, i, end - i);
			values.Add(i, value);

			i = end + 1;

			if (values.Count == threshold)
				thresholdOffset = i;
		}

		return values;
	}
	private static Dictionary<int, string> ReadStringTable(this BinaryReader reader, int size)
	{
		byte[] stringTable = reader.ReadBytes(size);
		Dictionary<int, string> values = [];

		for (int i = 0; i < stringTable.Length;)
		{
			int end = Array.IndexOf<byte>(stringTable, 0, i);

			if (end is -1)
				Throw.New.InvalidDataException($"Each string in the table should be null terminated, the string at offset 0x{i:x2} ({i:n0}) was not.");

			string value = Encoding.ASCII.GetString(stringTable, i, end - i);
			values.Add(i, value);

			i = end + 1;
		}

		return values;
	}
	private static Format ReadFormat(this BinaryReader reader)
	{
		short value = reader.ReadShort();
		Format format = (Format)value;

		if (Enum.IsDefined(typeof(Format), format) is false)
			Throw.New.InvalidDataException($"Unknown terminfo file format ({value}).");

		return format;
	}
	private static short ReadShort(this BinaryReader reader)
	{
		reader.AlignEven();

		byte[] bytes = reader.ReadBytes(2);
		if (BitConverter.IsLittleEndian is false)
			Reverse(bytes);

		short value = BitConverter.ToInt16(bytes);

		return value;
	}
	private static int ReadInt(this BinaryReader reader)
	{
		reader.AlignEven();

		byte[] bytes = reader.ReadBytes(4);

		if (BitConverter.IsLittleEndian is false)
			Reverse(bytes);

		int value = BitConverter.ToInt32(bytes);

		return value;
	}
	private static bool? ReadBooleanCapability(this BinaryReader reader)
	{
		byte value = reader.ReadByte();

		return value switch
		{
			0 => false,
			1 => true,
			254 => null,

			_ => Throw.New.InvalidOperationException<bool>($"Unknown boolean value, expected either 0, 1 or 254 (for -2).")
		};
	}
	private static int ReadNumberCapability(this BinaryReader reader, Format format)
	{
		if (format is Format.Extended)
			return ReadInt(reader);

		return ReadShort(reader);
	}
	private static void AlignEven(this BinaryReader reader)
	{
		long offset = reader.BaseStream.Position;

		if (reader.BaseStream.Position % 2 is 0)
			return;

		byte value = reader.ReadByte();
		if (value is not 0)
			Throw.New.InvalidDataException($"Expected the boundary padding at 0x{offset:x2} (byte {offset:n0}) to be a null byte.");
	}
	private static void Reverse(byte[] values)
	{
		for (int i = 0; i < values.Length / 2; i++)
			(values[i], values[^i]) = (values[^i], values[i]);
	}
	#endregion
}
