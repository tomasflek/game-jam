using Character;
using Events;
using GameManagers;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class PickupController : MonoBehaviour
{
    private bool _pickedUp;
    private BoxCollider _collider;
    [SerializeField]
    private float rotationSpeed;

    private PlayerController _player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

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
            _player = other.GetComponent<PlayerController>();
            _pickedUp = true;

            GameManager.Instance.Pickup(other.transform);
        }
        else if (other.gameObject.CompareTag("Home"))
        {
            EventManager.Instance.SendEvent(new DeliveryEvent(_player.Name));
        }
    }
}
