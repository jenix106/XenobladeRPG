using UnityEngine;
using ThunderRoad;

namespace XenobladeRPG
{
    public class Poison : MonoBehaviour
    {
        public Creature creature;
        public CollisionInstance initialDamage;
        CollisionInstance poisonDamage;
        public float time = 0;
        float cooldown = 0;
        public float duration = 30;
        public void Start()
        {
            if (creature == null)
                creature = GetComponent<Creature>();
            XenobladeEvents.InvokeOnDebuffAdded(this, creature, this);
            if (initialDamage == null)
            {
                Debug.LogError("Poison components require an initial collision instance, as it deals 100% of the original damage per tick. Please add a collision instance to the 'initialDamage' variable in the Poison component and Damage() the creature with that instance.");
                Destroy(this);
            }
            if (XenobladeManager.recordedCollisions.ContainsKey(initialDamage))
            {
                poisonDamage = new CollisionInstance(new DamageStruct(DamageType.Energy, Mathf.Max(initialDamage.damageStruct.damage, 1)));
                poisonDamage.damageStruct.hitRagdollPart = creature.ragdoll.rootPart;
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
                poisonDamage = new CollisionInstance(new DamageStruct(DamageType.Energy, Mathf.Max(collisionInstance.damageStruct.damage, 1)));
                poisonDamage.damageStruct.hitRagdollPart = creature.ragdoll.rootPart;
                time = Time.time;
                cooldown = Time.time;
                creature.OnDamageEvent -= Creature_OnDamageEvent;
            }
        }

        public void Update()
        {
            if (Time.time - time >= duration || creature.isKilled) Destroy(this);
            else if (poisonDamage == null)
            {
                time = Time.time;
                cooldown = Time.time;
            }
            else if (Time.time - cooldown >= 2 && poisonDamage != null)
            {
                XenobladeManager.Damage(null, creature, poisonDamage, XenobladeDamageType.Poison);
                cooldown = Time.time;
            }
        }
        public void OnDestroy()
        {
            XenobladeEvents.InvokeOnDebuffRemoved(this, creature, this);
        }
    }
}
