using UnityEngine;

//Реагирует на выход игрока из некоторой сферы-зоны вокруг купюры - пространства, где игрок не улетает слишком далеко от купюры
namespace Planes
{
    public class SphereZone : MonoBehaviour
    {
        //если игрок покидает сферу-зону
        private void OnTriggerExit(Collider other)
        {
            RedSphere sphere = other.GetComponentInChildren<RedSphere>(); //если триггер среагировал на игрока, то у него будет данный компонент
            if (sphere != null) //соответственно ссылка не будет null
            {
                sphere.inSphere = false; //устанавливаем, что игрок покинул сферу-зону
                sphere.disabling = false; //прекращаем убирать красный шар вокруг игрока, если это в данный момент происходит
                sphere.EnableSphere(); //запускаем показ красного шара вокруг игрока
            }
        }

        //если игрок возвращается в сферу
        private void OnTriggerEnter(Collider other)
        {
            RedSphere sphere = other.GetComponentInChildren<RedSphere>(); //если триггер среагировал на игрока, то у него будет данный компонент
            if (sphere != null) //соответственно ссылка не будет null
            {
                sphere.inSphere = true; //устанавливаем, что игрок находится в зоне-сфере
                sphere.DisableSphere(); //убираем предупреждающую сферу вокруг игрока (ограничения на то, что игрок должен не опускаться ниже купюры, включены в эту функцию)
            }
        }
    }
}