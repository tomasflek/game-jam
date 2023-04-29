namespace Events
{
	/// <summary>
	/// Base class for custom events.
	/// </summary>
	public abstract class EventBase
	{
		/// <summary>
		/// Indicates that this event was processed and should not be passed to other receivers.
		/// </summary>
		public bool StopPropagation = false;
	}
}
