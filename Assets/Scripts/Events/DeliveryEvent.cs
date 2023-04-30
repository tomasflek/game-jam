namespace Events
{
	public class DeliveryEvent : EventBase
	{
		public string PlayerName { get; set; }

		public DeliveryEvent(string playerName)
		{
			PlayerName = playerName;
		}
	}
}
