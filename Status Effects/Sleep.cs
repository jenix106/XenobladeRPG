using UnityEngine;
using ThunderRoad;

namespace XenobladeRPG
{
    /// <summary>
    /// Puts a creature to sleep. Physical/ether attacks always crit against sleeping creatures.
    /// </summary>
    public class Sleep : MonoBehaviour
    {
        public Creature creature;
        public void Start()
        {
            creature = GetComponent<Creature>();
            creature.OnDamageEvent += Creature_OnDamageEvent;
            creature.ragdoll.SetState(Ragdoll.State.Destabilized);
            creature.ragdoll.SetState(Ragdoll.State.Inert);
            creature.brain.AddNoStandUpModifier(this);
            XenobladeEvents.InvokeOnDebuffAdded(this, creature, this);
        }

        private void Creature_OnDamageEvent(CollisionInstance collisionInstance, EventTime eventTime)
        {
            if(collisionInstance.damageStruct.damage > 0 && eventTime == EventTime.OnEnd)
            {
                Destroy(this);
            }
        }
        public void OnDestroy()
        {
            creature.OnDamageEvent -= Creature_OnDamageEvent;
            creature.ragdoll.SetState(Ragdoll.State.Destabilized);
            creature.brain.RemoveNoStandUpModifier(this);
            XenobladeEvents.InvokeOnDebuffRemoved(this, creature, this);
        }
    }
}
