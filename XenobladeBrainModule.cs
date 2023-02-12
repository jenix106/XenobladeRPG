using ThunderRoad;
using UnityEngine;

namespace XenobladeRPG
{
    public class XenobladeBrainModule : BrainData.Module
    {
        public int minLevel = 1;
        public int maxLevel = 99;
        public int minLevelVariation = -6;
        public int maxLevelVariation = 6;
        public bool playerRelative = true;
        public bool isUnique = false;
        public string creatureName = "";
        public bool detectionTypeSight = true;
        public bool detectionTypeSound = false;
        public bool forceDetection = false;
        public float criticalRate = 0;
        public float physicalDefense = 0;
        public float etherDefense = 0;
        public DefenseDirection defenseDirection = DefenseDirection.None;
        public int overrideHealth = -1;
        public int overrideStrength = -1;
        public int overrideEther = -1;
        public int overrideAgility = -1;
        public override void Load(Creature creature)
        {
            base.Load(creature);
            if (creature.gameObject.GetComponent<XenobladeStats>() != null) GameObject.Destroy(creature.gameObject.GetComponent<XenobladeStats>());
            creature.gameObject.AddComponent<XenobladeStats>().Setup(minLevel, maxLevel, minLevelVariation, maxLevelVariation, playerRelative, isUnique,
                creatureName, detectionTypeSight, detectionTypeSound, forceDetection, criticalRate, defenseDirection, physicalDefense, etherDefense, overrideHealth, overrideStrength, overrideEther, overrideAgility);
        }
    }
}
