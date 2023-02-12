﻿using UnityEngine;
using ThunderRoad;

namespace XenobladeRPG
{
    class Bleed : MonoBehaviour
    {
        public Creature creature;
        public CollisionInstance initialDamage;
        CollisionInstance bleedDamage;
        float time = 0;
        float cooldown = 0;
        public void Start()
        {
            if (creature == null)
                creature = GetComponent<Creature>();
            if (initialDamage == null)
            {
                Debug.LogError("Bleed components require an initial collision instance, as it deals 20% of the original damage per tick. Please add a collision instance to the 'initialDamage' variable in the Bleed component and Damage() the creature with that instance.");
                Destroy(this);
            }
            if (XenobladeManager.recordedCollisions.ContainsKey(initialDamage))
            {
                bleedDamage = new CollisionInstance(new DamageStruct(DamageType.Energy, initialDamage.damageStruct.damage * 0.2f));
                bleedDamage.damageStruct.hitRagdollPart = creature.ragdoll.rootPart;
                XenobladeManager.BypassXenobladeDamage(bleedDamage, XenobladeDamageType.Bleed);
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
                bleedDamage = new CollisionInstance(new DamageStruct(DamageType.Energy, collisionInstance.damageStruct.damage * 0.2f));
                bleedDamage.damageStruct.hitRagdollPart = creature.ragdoll.rootPart;
                XenobladeManager.BypassXenobladeDamage(bleedDamage, XenobladeDamageType.Bleed);
                time = Time.time;
                cooldown = Time.time;
                creature.OnDamageEvent -= Creature_OnDamageEvent;
            }
        }

        public void Update()
        {
            if (Time.time - time >= 20 || creature.isKilled || bleedDamage.damageStruct.damage == 0) Destroy(this);
            else if (bleedDamage == null)
            {
                time = Time.time;
                cooldown = Time.time;
            }
            else if (Time.time - cooldown >= 2 || bleedDamage != null)
            {
                creature.Damage(bleedDamage);
                cooldown = Time.time;
            }
        }
    }
}