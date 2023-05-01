using Character;
using Events;
using GameManagers;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class PickupController : MonoBehaviour
{
    private bool _pickedUp;
    private BoxCollider _collider;
    [SerializeField]
    private float rotationSpeed;

    private IPrefab _player;


	// Start is called before the first frame update
	private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_pickedUp)
        {
            transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !_pickedUp)
        {
            _player = other.GetComponent<IPrefab>();
            _pickedUp = true;

            GameManager.Instance.Pickup(other.transform);
            int prefabInd = other.GetComponent<IPrefab>().PrefabInt;
			AudioManager.Instance.PlayCharacterSound(prefabInd, "PackagePickup");
        }
        else if (other.gameObject.CompareTag("Home"))
        {
            EventManager.Instance.SendEvent(new DeliveryEvent(_player.PlayerIndex));  
        }
    }
}
