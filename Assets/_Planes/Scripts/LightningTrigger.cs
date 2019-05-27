using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planes
{
    public class LightningTrigger : Lightning
    {
        CapsuleCollider _thisCollider;

        private void Awake()
        {
            _thisCollider = GetComponent<CapsuleCollider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Hit smth");
            CharacterPrint obj = other.GetComponentInParent<CharacterPrint>();
            if (obj != null)
            {
                if (obj.isPlayer)
                {
                    Debug.Log("Player hitted");
                    obj.TakeDamage(obj.health / 2); //отнимаем здоровье
                    _thisCollider.enabled = false;
                    hitted = true;
                }
            }
        }
    }
}