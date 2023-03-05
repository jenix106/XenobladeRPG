using UnityEngine;
using ThunderRoad;

namespace XenobladeRPG
{
    public class Slow : MonoBehaviour
    {
        public Creature creature;
        public BrainModuleMelee melee;
        public float debuffPercent;
        public void Start()
        {
            creature = GetComponent<Creature>();
            melee = creature.brain.instance.GetModule<BrainModuleMelee>(false);
            melee.animationSpeedMultiplier -= debuffPercent;
            XenobladeEvents.InvokeOnDebuffAdded(ref creature, this);
            Destroy(this, 10);
        }
        public void OnDestroy()
        {
            melee.animationSpeedMultiplier += debuffPercent;
            XenobladeEvents.InvokeOnDebuffRemoved(ref creature, this);
        }
    }
}
