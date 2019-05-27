using UnityEngine;

namespace Planes
{
    public class IslandCollider : MonoBehaviour
    {
        int _shootable;
        private void Awake()
        {
            _shootable = LayerMask.NameToLayer("Shootable");
        }
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("IslandTrigger");
            if (other.gameObject.layer == _shootable)
            {
                CharacterPrint obj = other.gameObject.GetComponent<CharacterPrint>();
                if (obj != null)
                {
                    if (obj.isPlayer)
                    {
                        obj.TakeDamage(CharacterPrint.CRUSH);
                        Debug.Log("Crush!");
                    }
                    else
                    {
                        Debug.Log("Enemy Crush");
                        obj.TakeDamage(obj.health / 10);
                    }
                }
                else
                {
                    Debug.Log("Script not found");
                }
            }
        }
    }
}