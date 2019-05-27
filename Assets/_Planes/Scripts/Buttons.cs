using UnityEngine;
using UnityEngine.UI;

//скрипт сохраняет действие всех кнопок при перезагрузке сцены
namespace Planes {
    public class Buttons : MonoBehaviour
    {
        void Awake()
        {
            Button button = GetComponent<Button>();
            button.onClick.AddListener(TaskOnClick);
        }

        void TaskOnClick()
        {
             GameManager.instance.PauseButtonPressed();
        }
    }
}