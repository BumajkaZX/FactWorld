using UnityEngine.Events;
using Cinemachine;

namespace FactWorld
{
    public static class GameEventManager
    {
       
        public static  bool WhichTurn { get => turn;  private set { } }
        public static float NoiseChanse { get => _noiseChance; set => _noiseChance = value; }

        private static bool turn = false;

        private static float _noiseChance;

        public static UnityEvent Turn = new UnityEvent();

        public static MainController MainController;

        public static CinemachineVirtualCamera Camera;

        public static void InverseTurn()
        {
            turn = !turn;
        }
        public static void TurnStart()
        {
            Turn.Invoke();
        }

    }
}

