namespace OwlDomain.Console.Capabilities;

/// <summary>
/// 	Contains general helper functions related to the <see cref="TerminalCapability{T}"/>.
/// </summary>
public static class TerminalCapability
{
	#region Functions
	/// <summary>Creates a new instance of the <see cref="TerminalCapability{T}"/>.</summary>
	/// <typeparam name="T">The type of the capability's value.</typeparam>
	/// <param name="id">The id for the capability.</param>
	/// <param name="friendlyName">
	/// 	The user friendly name for the capability, if <see langword="null"/>
	/// 	then the given <paramref name="id"/> will be used.
	/// </param>
	/// <param name="value">The value for the capability.</param>
	/// <exception cref="ArgumentException">
	/// 	Thrown if either the given <paramref name="id"/> or <paramref name="friendlyName"/>
	/// 	(if not <see langword="null"/>) are empty, or only consist of white-space characters.
	/// </exception>
	/// <returns>The created terminal capability.</returns>
	public static TerminalCapability<T> New<T>(string id, string? friendlyName, T value) => new(id, friendlyName, value);
	#endregion
}

/// <summary>
/// 	Represents information about a single terminal capability.
/// </summary>
/// <typeparam name="T">The type of the capability's value.</typeparam>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public class TerminalCapability<T> : ITerminalCapability<T>
{
	#region Properties
	/// <inheritdoc/>
	public string Id { get; }

	/// <inheritdoc/>
	public string FriendlyName { get; }

	/// <inheritdoc/>
	public T Value { get; }
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="TerminalCapability{T}"/>.</summary>
	/// <param name="id">The id for the capability.</param>
	/// <param name="friendlyName">
	/// 	The user friendly name for the capability, if <see langword="null"/>
	/// 	then the given <paramref name="id"/> will be used.
	/// </param>
	/// <param name="value">The value for the capability.</param>
	/// <exception cref="ArgumentException">
	/// 	Thrown if either the given <paramref name="id"/> or <paramref name="friendlyName"/>
	/// 	(if not <see langword="null"/>) are empty, or only consist of white-space characters.
	/// </exception>
	public TerminalCapability(string id, string? friendlyName, T value)
	{
		id.ThrowIfNullOrEmptyOrWhitespace(nameof(id));
		friendlyName.ThrowIfEmptyOrWhitespace(nameof(friendlyName));

		Id = id;
		FriendlyName = friendlyName ?? id;
		Value = value;
	}
	#endregion

	#region Helpers
	[ExcludeFromCodeCoverage]
	private string DebuggerDisplay()
	{
		const string typeName = nameof(TerminalCapability<T>);
		const string friendlyNameName = nameof(FriendlyName);
		const string valueName = nameof(Value);

		return $"{typeName} {{ {friendlyNameName} = ({FriendlyName}), {valueName} = ({Value}) }}";
	}
	#endregion
}
