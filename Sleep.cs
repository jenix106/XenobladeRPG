using UnityEngine;
using ThunderRoad;

namespace XenobladeRPG
{
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
            XenobladeEvents.InvokeOnDebuffAdded(ref creature, this);
        }

        private void Creature_OnDamageEvent(CollisionInstance collisionInstance)
        {
            if(collisionInstance.damageStruct.damage > 0)
            {
                Destroy(this);
            }
        }
        public void OnDestroy()
        {
            creature.OnDamageEvent -= Creature_OnDamageEvent;
            creature.ragdoll.SetState(Ragdoll.State.Destabilized);
            creature.brain.RemoveNoStandUpModifier(this);
            XenobladeEvents.InvokeOnDebuffRemoved(ref creature, this);
        }
    }
}
