namespace Events
{
	public class DeliveryEvent : EventBase
	{
		public int PlayerIndex { get; set; }

		public DeliveryEvent(int playerIndex)
		{
			PlayerIndex = playerIndex;
		}
	}
}
