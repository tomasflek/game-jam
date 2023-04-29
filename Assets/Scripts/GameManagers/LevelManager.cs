using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _players;

    private void Awake()
    {
        EventManager.Instance.Register<DeliveryEvent>(OnDelivery);
    }

    private void OnDelivery(DeliveryEvent obj)
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
