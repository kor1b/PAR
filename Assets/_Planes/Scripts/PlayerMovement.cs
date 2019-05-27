using UnityEngine;

namespace Planes
{
    public class PlayerMovement : MonoBehaviour
    {
        [Tooltip("Скорость, с которой игрок постоянно движется вперед")]
        public float speed = 10f;
        [Tooltip("Скорость поворота")]
        public float rotationSpeed = 1f;

        private Transform _player; //позиция игрока
        private Transform _nose; //нос - точка, которая находится спереди самолета, и к которой он будет "стремиться"
        private FixedJoystick _joystick; //джойстик - управление
        private Rigidbody _playerRigidbody; //для остановки посторонних вращений
        //данные из джойстика
        private float horizontalMove;
        private float verticalMove;

        CharacterPrint _characterPrint;
        void Awake()
        {
            _player = GetComponent<Transform>();
            _joystick = GameObject.FindWithTag("Joystick").GetComponent<FixedJoystick>();
            _nose = GameObject.FindWithTag("Nose").GetComponent<Transform>();
            _playerRigidbody = GetComponent<Rigidbody>();
            _characterPrint = GetComponent<CharacterPrint>();
        }

        void FixedUpdate()
        {
            if (_characterPrint.takeoffMode || !GameManager.gameIsGoing)
            {
                return;
            }
            _playerRigidbody.angularVelocity = new Vector3(0, 0, 0); //останавливаем посторонние вращения
            //постоянное движение вперед
            _player.position = Vector3.MoveTowards(_player.position, _nose.position, speed * Time.deltaTime);

            //выясняем, куда потянут джойстик
            horizontalMove = _joystick.Horizontal;
            verticalMove = _joystick.Vertical;

            if (horizontalMove < -0.2f || horizontalMove > 0.2f) //если джойстик потянут горизонтально на некоторую значительную величину
            {
                _player.Rotate(0, horizontalMove * rotationSpeed, 0); //вращаем игрока; вместе с ним вращается "нос", в сторону которого он летит
            }

            if (verticalMove < -0.2f || verticalMove > 0.2f) //если джойстик потянут вертикально на некоторую значительную величину
            {
                _player.Rotate(-verticalMove * rotationSpeed, 0, 0); //вращаем игрока; вместе с ним вращается "нос", в сторону которого он летит
            }
        }
    }
}