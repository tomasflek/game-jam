using Events;
using UnityEngine;

namespace GameManagers
{
    public class LevelManager : MonoBehaviour
    {
        private void Awake()
        {
            EventManager.Instance.Register<DeliveryEvent>(OnDelivery);
        }

        private void OnDelivery(DeliveryEvent obj)
        {
            // Next level.
        }
    }
}
