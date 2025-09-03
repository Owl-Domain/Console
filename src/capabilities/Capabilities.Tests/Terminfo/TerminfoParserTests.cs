namespace OwlDomain.Console.Capabilities.Tests.Terminfo;

[TestClass]
public sealed class TerminfoParserTests
{
	#region Tests
	[DynamicData(nameof(GetValidTerminfoFiles), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetTerminfoFileName))]
	[TestMethod]
	public void Parse_WithValidFile_NoExceptionIsThrown(string path)
	{
		// Arrange
		using FileStream stream = File.OpenRead(path);
		using BinaryReader reader = new(stream);

		// Act
		void Act() => _ = TerminfoParser.Parse(reader);

		// Assert
		Assert.That
			.DoesNotThrowAnyException(Act)
			.AreEqual(stream.Position, stream.Length);
	}
	#endregion

	#region Helpers
	public static string GetTerminfoFileName(MethodInfo _, object[] data)
	{
		Debug.Assert(data.Length is 1);
		string path = (string)data[0];
		string name = Path.GetFileName(path);

		return $"/{name[0]}/{name}";
	}
	private static IEnumerable<object?[]> GetValidTerminfoFiles()
	{
		const string relativeDirectory = @"../../../Terminfo/valid_files/";
		string directory = Path.GetFullPath(relativeDirectory);

		foreach (string file in Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories))
			yield return [file];
	}
	#endregion
}
