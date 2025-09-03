namespace OwlDomain.Console.Capabilities;

/// <summary>
/// 	Represents information about a single terminal capability.
/// </summary>
public interface ITerminalCapability
{
	#region Properties
	/// <summary>A unique ID used to identify the capability.</summary>
	string Id { get; }

	/// <summary>A user friendly name of the capability.</summary>
	/// <remarks>If a friendly name doesn't exist then the <see cref="Id"/> should be used instead.</remarks>
	string? FriendlyName { get; }

	/// <summary>The value of the capability.</summary>
	object? Value { get; }
	#endregion
}

/// <summary>
/// 	Represents information about a single terminal capability.
/// </summary>
/// <typeparam name="T">The type of the capability's value.</typeparam>
public interface ITerminalCapability<out T> : ITerminalCapability
{
	#region Properties
	/// <summary>The value of the capability.</summary>
	new T Value { get; }
	object? ITerminalCapability.Value => Value;
	#endregion
}