using UnityEngine.Events;

namespace FactWorld
{
    public class GameEventManager
    {
       
        public static  bool WhichTurn { get => turn;  private set { } }
        public static float NoiseChanse { get => _noiseChance; set => _noiseChance = value; }

        private static bool turn = true;

        private static float _noiseChance;

        public static UnityEvent Turn = new UnityEvent();

        public static MainController MainController;

        public static void TurnStart()
        {
            turn = !turn;
            Turn.Invoke();
        }

    }
}

