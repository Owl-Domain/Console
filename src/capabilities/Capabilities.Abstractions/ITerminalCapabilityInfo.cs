namespace OwlDomain.Console.Capabilities;

/// <summary>
/// 	Represents information about a terminal's capabilities.
/// </summary>
public interface ITerminalCapabilityInfo : IReadOnlyCollection<ITerminalCapability>, IReadOnlyDictionary<string, ITerminalCapability>
{
	#region Methods
	/// <summary>Exposes the enumerator, which supports a simple iteration over a collection of a specified type.</summary>
	/// <returns>An enumerator that can be used to iterate through the collection.</returns>
	new IEnumerator<ITerminalCapability> GetEnumerator();
	#endregion
}