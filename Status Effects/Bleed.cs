using UnityEngine;
using ThunderRoad;

namespace XenobladeRPG
{
    public class Bleed : MonoBehaviour
    {
        public Creature creature;
        public CollisionInstance initialDamage;
        CollisionInstance bleedDamage;
        public float time = 0;
        float cooldown = 0;
        public float duration = 20;
        public void Start()
        {
            if (creature == null)
                creature = GetComponent<Creature>();
            XenobladeEvents.InvokeOnDebuffAdded(this, creature, this);
            if (initialDamage == null)
            {
                Debug.LogError("Bleed components require an initial collision instance, as it deals 20% of the original damage per tick. Please add a collision instance to the 'initialDamage' variable in the Bleed component and Damage() the creature with that instance.");
                Destroy(this);
            }
            if (XenobladeManager.recordedCollisions.ContainsKey(initialDamage))
            {
                bleedDamage = new CollisionInstance(new DamageStruct(DamageType.Energy, Mathf.Max(initialDamage.damageStruct.damage * 0.2f, 1)));
                bleedDamage.damageStruct.hitRagdollPart = creature.ragdoll.rootPart;
            }
            else
            {
                creature.OnDamageEvent += Creature_OnDamageEvent;
            }
            time = Time.time;
            cooldown = Time.time;
        }

        private void Creature_OnDamageEvent(CollisionInstance collisionInstance)
        {
            if (collisionInstance == initialDamage)
            {
                bleedDamage = new CollisionInstance(new DamageStruct(DamageType.Energy, Mathf.Max(collisionInstance.damageStruct.damage * 0.2f, 1)));
                bleedDamage.damageStruct.hitRagdollPart = creature.ragdoll.rootPart;
                time = Time.time;
                cooldown = Time.time;
                creature.OnDamageEvent -= Creature_OnDamageEvent;
            }
        }

        public void Update()
        {
            if (Time.time - time >= duration || creature.isKilled) Destroy(this);
            else if (bleedDamage == null)
            {
                time = Time.time;
                cooldown = Time.time;
            }
            else if (Time.time - cooldown >= 2 && bleedDamage != null)
            {
                XenobladeManager.Damage(null, creature, bleedDamage, XenobladeDamageType.Bleed);
                cooldown = Time.time;
            }
        }
        public void OnDestroy()
        {
            XenobladeEvents.InvokeOnDebuffRemoved(this, creature, this);
        }
    }
}
