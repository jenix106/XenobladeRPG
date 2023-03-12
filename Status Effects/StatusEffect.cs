using UnityEngine;
using ThunderRoad;

namespace XenobladeRPG
{
    public class StatusEffect : MonoBehaviour
    {
        public Creature creature;
        public float damage;
        public float damageMultiplier;
        public float duration;
        public XenobladeDamageType damageType = XenobladeDamageType.Unknown;
        CollisionInstance statusEffectDamage;
        public float time = 0;
        float cooldown = 0;
        StatusEffect other;
        public void Start()
        {
            if (creature == null)
                creature = GetComponent<Creature>();
            XenobladeEvents.InvokeOnDebuffAdded(this, creature, this);
            statusEffectDamage = new CollisionInstance(new DamageStruct(DamageType.Energy, damage * damageMultiplier));
            statusEffectDamage.damageStruct.hitRagdollPart = creature.ragdoll.rootPart;
            time = Time.time;
            cooldown = Time.time;
            foreach(StatusEffect status in creature.GetComponents<StatusEffect>())
            {
                if(status != null && status.damageType == damageType && status != this)
                {
                    other = status;
                }
            }
            Destroy(other);
        }

        public void Update()
        {
            if (Time.time - time >= duration || creature.isKilled || statusEffectDamage.damageStruct.damage == 0) Destroy(this);
            else if (Time.time - cooldown >= 2 && statusEffectDamage != null)
            {
                XenobladeManager.Damage(null, creature, statusEffectDamage, damageType);
                cooldown = Time.time;
            }
        }
        public void OnDestroy()
        {
            XenobladeEvents.InvokeOnDebuffRemoved(this, creature, this);
        }
    }
}
