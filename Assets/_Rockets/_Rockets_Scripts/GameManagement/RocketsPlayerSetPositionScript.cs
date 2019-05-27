using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketsPlayerSetPositionScript : MonoBehaviour
{
    public Vector3 playerSpawn;               //player spawn point
    public Quaternion playerRotation;
    public Vector3 environmentPosition;      //environment spawn

    private void Awake()
    {
        //set environment pos
        transform.position = environmentPosition;

        //set player pos
        Transform player = GameObject.FindWithTag("PlayerParent").transform;

        player.SetParent(gameObject.transform);

        player.position = playerSpawn;
        player.rotation = playerRotation;
    }
}
