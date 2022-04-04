using UnityEngine.Events;

namespace FactWorld
{
    public class GameEventManager
    {
        public static UnityEvent Turn = new UnityEvent();

        public static MainController _mainController;

        private static bool turn = true;
        public static bool WhichTurn { get { return turn; } private set { } }
        public static void TurnStart()
        {
            turn = !turn;
            Turn.Invoke();
        }

    }
}

