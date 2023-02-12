using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ThunderRoad;

namespace XenobladeRPG
{
    class Chill : MonoBehaviour
    {
        public Creature creature;
        public CollisionInstance initialDamage;
        CollisionInstance chillDamage;
        float time = 0;
        float cooldown = 0;
        public void Start()
        {
            if (creature == null)
                creature = GetComponent<Creature>();
            if (initialDamage == null)
            {
                Debug.LogError("Chill components require an initial collision instance, as it deals 60% of the original damage per tick. Please add a collision instance to the 'initialDamage' variable in the Chill component and Damage() the creature with that instance.");
                Destroy(this);
            }
            if (XenobladeManager.recordedCollisions.ContainsKey(initialDamage))
            {
                chillDamage = new CollisionInstance(new DamageStruct(DamageType.Energy, initialDamage.damageStruct.damage * 0.6f));
                chillDamage.damageStruct.hitRagdollPart = creature.ragdoll.rootPart;
                XenobladeManager.BypassXenobladeDamage(chillDamage, XenobladeDamageType.Chill);
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
                chillDamage = new CollisionInstance(new DamageStruct(DamageType.Energy, collisionInstance.damageStruct.damage * 0.6f));
                chillDamage.damageStruct.hitRagdollPart = creature.ragdoll.rootPart;
                XenobladeManager.BypassXenobladeDamage(chillDamage, XenobladeDamageType.Chill);
                time = Time.time;
                cooldown = Time.time;
                creature.OnDamageEvent -= Creature_OnDamageEvent;
            }
        }

        public void Update()
        {
            if (Time.time - time >= 10 || creature.isKilled || chillDamage.damageStruct.damage == 0) Destroy(this);
            else if (chillDamage == null)
            {
                time = Time.time;
                cooldown = Time.time;
            }
            else if (Time.time - cooldown >= 2 || chillDamage != null)
            {
                creature.Damage(chillDamage);
                cooldown = Time.time;
            }
        }
    }
}
