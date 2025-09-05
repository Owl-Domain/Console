namespace OwlDomain.Console.Capabilities;

/// <summary>
/// 	Represents information about a terminal's capabilities.
/// </summary>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public sealed class TerminalCapabilityInfo : ITerminalCapabilityInfo
{
	#region Fields
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly Dictionary<string, ITerminalCapability> _capabilities = [];
	#endregion

	#region Properties
	/// <inheritdoc/>
	public int Count => _capabilities.Count;

	/// <inheritdoc/>
	public IEnumerable<string> Keys => _capabilities.Keys;

	/// <inheritdoc/>
	public IEnumerable<ITerminalCapability> Values => _capabilities.Values;
	#endregion

	#region Indexers
	/// <inheritdoc/>
	public ITerminalCapability this[string key] => _capabilities[key];
	#endregion

	#region Constructors
	/// <summary>Creates an empty instance of the <see cref="TerminalCapabilityInfo"/>.</summary>
	public TerminalCapabilityInfo() { }

	/// <summary>Creates a new instance of the <see cref="TerminalCapabilityInfo"/> initialised with the given <paramref name="capabilities"/>.</summary>
	/// <param name="capabilities">The capabilities to initialise the created <see cref="TerminalCapabilityInfo"/> instance with.</param>
	public TerminalCapabilityInfo(params IEnumerable<ITerminalCapability> capabilities)
	{
		foreach (ITerminalCapability cap in capabilities)
			_capabilities.Add(cap.Id, cap);
	}
	#endregion

	#region Methods
	/// <inheritdoc/>
	public bool ContainsKey(string key) => _capabilities.ContainsKey(key);

	/// <inheritdoc/>
	public bool TryGetValue(string key, out ITerminalCapability value) => _capabilities.TryGetValue(key, out value);

	/// <inheritdoc/>
	public IEnumerator<ITerminalCapability> GetEnumerator() => _capabilities.Values.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_capabilities.Values).GetEnumerator();
	IEnumerator<KeyValuePair<string, ITerminalCapability>> IEnumerable<KeyValuePair<string, ITerminalCapability>>.GetEnumerator() => _capabilities.GetEnumerator();
	#endregion

	#region Helpers
	[ExcludeFromCodeCoverage]
	private string DebuggerDisplay()
	{
		const string typeName = nameof(TerminalCapabilityInfo);
		const string countName = nameof(Count);

		return $"{typeName} {{ {countName} = ({Count:n0}) }}";
	}
	#endregion
}
