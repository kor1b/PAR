using UnityEngine;

//перемещает камеру следом за игроком, нужен только для тестировки без купюры
namespace Planes
{
    public class CameraFollowDEL : MonoBehaviour
    {
        public Transform player;
        public Vector3 offset;

        void Update()
        {
            if (player != null)
            {
                transform.position = player.position + offset;
            }
        }
    }
}