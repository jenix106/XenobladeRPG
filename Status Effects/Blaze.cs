using UnityEngine;
using ThunderRoad;

namespace XenobladeRPG
{
    /// <summary>
    /// Deals Blaze damage every 2 seconds for 40% of the initial damage. Default duration is 20 seconds.
    /// </summary>
    public class Blaze : MonoBehaviour
    {
        public Creature creature;
        public CollisionInstance initialDamage;
        CollisionInstance blazeDamage;
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
                Debug.LogError("Blaze components require an initial collision instance, as it deals 40% of the original damage per tick. Please add a collision instance to the 'initialDamage' variable in the Blaze component and Damage() the creature with that instance.");
                Destroy(this);
            }
            if (XenobladeManager.recordedCollisions.ContainsKey(initialDamage))
            {
                blazeDamage = new CollisionInstance(new DamageStruct(DamageType.Energy, Mathf.Max(initialDamage.damageStruct.damage * 0.4f, 1)));
                blazeDamage.damageStruct.hitRagdollPart = creature.ragdoll.rootPart;
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
                blazeDamage = new CollisionInstance(new DamageStruct(DamageType.Energy, Mathf.Max(collisionInstance.damageStruct.damage * 0.4f, 1)));
                blazeDamage.damageStruct.hitRagdollPart = creature.ragdoll.rootPart;
                time = Time.time;
                cooldown = Time.time;
                creature.OnDamageEvent -= Creature_OnDamageEvent;
            }
        }

        public void Update()
        {
            if (Time.time - time >= duration || creature.isKilled) Destroy(this);
            else if (blazeDamage == null)
            {
                time = Time.time;
                cooldown = Time.time;
            }
            else if (Time.time - cooldown >= 2 && blazeDamage != null)
            {
                XenobladeManager.Damage(null, creature, blazeDamage, XenobladeDamageType.Blaze);
                cooldown = Time.time;
            }
        }
        public void OnDestroy()
        {
            XenobladeEvents.InvokeOnDebuffRemoved(this, creature, this);
        }
    }
}
