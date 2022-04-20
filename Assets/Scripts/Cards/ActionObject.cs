using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactWorld
{
    [CreateAssetMenu(menuName = "Card/Action Card")]
    public class ActionObject : ScriptableObject, IObject
    {
        public GameObject Card { get => card; set => card = value; }
        public bool IsAttackGain { get => isAttackGain; set => isAttackGain = value; }
        public int Damage { get => damage; set => damage = value; }
        public float NoiseRadius { get => noiseRadius; set => noiseRadius = value; }
        public float CardPickAttackGainChance { get => cardPickAttackGainChance; set => cardPickAttackGainChance = value; }
        public int HPCost { get => hpCost; set => hpCost = value; }
        public int Duration { get => duration; set => duration = value; }
        public bool IsHide { get => isHide; set => isHide = value; }
        public float HideChance { get => hideChance; set => hideChance = value; }
        public float CardPickHideChance { get => cardPickHideChance; set => cardPickHideChance = value; }
        public bool IsDialogue { get => isDialogue; set => isDialogue = value; }
        public float Eloquence { get => eloquence; set => eloquence = value; }
        public float VoiceNoise { get => voiceNoise; set => voiceNoise = value; }
        public float CardPickDialogueChance { get => cardPickDialogueChance; set => cardPickDialogueChance = value; }
        public bool IsFear { get => isFear; set => isFear = value; }
        public int StepDecrease { get => stepDecrease; set => stepDecrease = value; }
        public bool IsParanoia { get => isParanoia; set => isParanoia = value; }
        public int Ghosts { get => ghosts; set => ghosts = value; }
        public float GhostsSpawnRadius { get => ghostsSpawnRadius; set => ghostsSpawnRadius = value; }
        public int MaxCards { get => maxCards; set => maxCards = value; }
        

        [SerializeField] private GameObject card;

        [Space(20)]

        [SerializeField] private bool isAttackGain;
        [SerializeField] private int damage;
        [SerializeField] private float noiseRadius;
        [SerializeField] private float cardPickAttackGainChance;
        [SerializeField] private int hpCost;
        [SerializeField] private int duration;

        [Space(20)]

        [SerializeField] private bool isHide;
        [SerializeField] private float hideChance;
        [SerializeField] private float cardPickHideChance;

        [Space(20)]

        [SerializeField] private bool isDialogue;
        [SerializeField] private float eloquence;
        [SerializeField] private float voiceNoise;
        [SerializeField] private float cardPickDialogueChance;

        [Space(20)]

        [SerializeField] private bool isFear;
        [SerializeField] private int stepDecrease;

        [Space(20)]

        [SerializeField] private bool isParanoia;
        [SerializeField] private int ghosts;
        [SerializeField] private float ghostsSpawnRadius;

        [Space(50)]

        [SerializeField] private int maxCards;
        public void Use()
        {
        
        }
    }
}
