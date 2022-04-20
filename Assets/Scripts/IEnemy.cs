using System.Threading.Tasks;
using UnityEngine;

namespace FactWorld
{
    public interface IEnemy
    {
        int Damage { get; set; }
        int HP { get; set; }
        int ID { get; set; }
        float ActiveRadius { get; set; }
        float SoundRadius { get; set; }

        Vector3 EnemyOffsetOnHex { get; set; }
        Vector3 Position { get; set; }
        Vector3 LoudestSoundPosition { get; set; }

        float JumpStep { get; set; }

        void NotDieYet();
    }
}
