using UnityEngine;
using ThunderRoad;

namespace XenobladeRPG
{
    /// <summary>
    /// Slows a creature's attack speed. Default duration is 10 seconds.
    /// </summary>
    public class Slow : MonoBehaviour
    {
        public Creature creature;
        public BrainModuleMelee melee;
        public float debuffPercent;
        public float duration = 10;
        public void Start()
        {
            creature = GetComponent<Creature>();
            melee = creature.brain.instance.GetModule<BrainModuleMelee>(false);
            melee.animationSpeedMultiplier -= debuffPercent;
            XenobladeEvents.InvokeOnDebuffAdded(this, creature, this);
            Destroy(this, duration);
        }
        public void OnDestroy()
        {
            melee.animationSpeedMultiplier += debuffPercent;
            XenobladeEvents.InvokeOnDebuffRemoved(this, creature, this);
        }
    }
}
