﻿using UnityEngine;
using ThunderRoad;

namespace XenobladeRPG
{
    class Blaze : MonoBehaviour
    {
        public Creature creature;
        public CollisionInstance initialDamage;
        CollisionInstance blazeDamage;
        float time = 0;
        float cooldown = 0;
        public void Start()
        {
            if (creature == null)
                creature = GetComponent<Creature>();
            if (initialDamage == null)
            {
                Debug.LogError("Blaze components require an initial collision instance, as it deals 40% of the original damage per tick. Please add a collision instance to the 'initialDamage' variable in the Blaze component and Damage() the creature with that instance.");
                Destroy(this);
            }
            if (XenobladeManager.recordedCollisions.ContainsKey(initialDamage))
            {
                blazeDamage = new CollisionInstance(new DamageStruct(DamageType.Energy, initialDamage.damageStruct.damage * 0.4f));
                blazeDamage.damageStruct.hitRagdollPart = creature.ragdoll.rootPart;
                XenobladeManager.BypassXenobladeDamage(blazeDamage, XenobladeDamageType.Blaze);
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
                blazeDamage = new CollisionInstance(new DamageStruct(DamageType.Energy, collisionInstance.damageStruct.damage * 0.4f));
                blazeDamage.damageStruct.hitRagdollPart = creature.ragdoll.rootPart;
                XenobladeManager.BypassXenobladeDamage(blazeDamage, XenobladeDamageType.Blaze);
                time = Time.time;
                cooldown = Time.time;
                creature.OnDamageEvent -= Creature_OnDamageEvent;
            }
        }

        public void Update()
        {
            if (Time.time - time >= 20 || creature.isKilled || blazeDamage.damageStruct.damage == 0) Destroy(this);
            else if (blazeDamage == null)
            {
                time = Time.time;
                cooldown = Time.time;
            }
            else if (Time.time - cooldown >= 2 || blazeDamage != null)
            {
                creature.Damage(blazeDamage);
                cooldown = Time.time;
            }
        }
    }
}