using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad;
using UnityEngine;

namespace XenobladeRPG
{
    /// <summary>
    /// Controls a creature's stats and parameters.
    /// </summary>
    public class XenobladeStats : MonoBehaviour
    {
        Creature creature;
        protected int level;
        protected int baseLevel;
        public int minLevel;
        public int maxLevel;
        public int minLevelVariation;
        public int maxLevelVariation;
        public int overrideHealth;
        public int overrideStrength;
        public int overrideEther;
        public int overrideAgility;
        public int overrideXP;
        public int overrideAP;
        protected int strength;
        protected int baseStrength;
        protected int ether;
        protected int baseEther;
        protected float physicalDefense;
        protected float basePhysicalDefense;
        protected float etherDefense;
        protected float baseEtherDefense;
        protected float baseHealth;
        protected int agility;
        protected int baseAgility;
        protected float criticalRate;
        protected float baseCriticalRate;
        public bool playerRelative;
        public bool isUnique;
        public string creatureName;
        public bool detectionTypeSight;
        public bool detectionTypeSound;
        public bool forceDetection;
        public bool isSleeping;
        public bool isAuraSealed = false;
        public bool isParalyzed = false;
        public bool isArtsSealed = false;
        public bool isBinded = false;
        public bool isPoisoned = false;
        public bool isBurning = false;
        public bool isBleeding = false;
        public bool isShielded = false;
        public bool isArmored = false;
        public bool isEnchanted = false;
        public bool isFreezing = false;
        public bool isLockedOn = false;
        public bool isConfused = false;
        public bool isPierced = false;
        public bool isRegen = false;
        public bool isDamageHealed = false;
        public bool isDebuffResistant = false;
        public bool isReflecting = false;
        public bool isDamageImmune = false;
        public bool isPhysicalArtsPlus = false;
        protected float baseAttackSpeedMultiplier = 1;
        public DefenseDirection defenseDirection;
        public Dictionary<RagdollPart, bool> partDismemberment = new Dictionary<RagdollPart, bool>();
        public bool isDazed = false;
        public bool isToppled = false;
        public bool isBroken = false;
        bool isMage = false;
        bool isLongDistanceMage = false;
        public List<StrengthModifier> strengthModifiers = new List<StrengthModifier>();
        public List<EtherModifier> etherModifiers = new List<EtherModifier>();
        public List<PhysicalDefenseModifier> physicalDefenseModifiers = new List<PhysicalDefenseModifier>();
        public List<EtherDefenseModifier> etherDefenseModifiers = new List<EtherDefenseModifier>();
        public List<AgilityModifier> agilityModifiers = new List<AgilityModifier>();
        public List<CriticalRateModifier> criticalRateModifiers = new List<CriticalRateModifier>();
        public List<HealthModifier> healthModifiers = new List<HealthModifier>();
        protected float healthMultiplier = 1;
        protected float strengthMultiplier = 1;
        protected float etherMultiplier = 1;
        protected float agilityMultiplier = 1f;
        protected float physicalDefenseModifier = 0;
        protected float etherDefenseModifier = 0;
        protected float agilityModifier = 0;
        protected float criticalRateModifier = 0;
        public List<LevelModifier> levelModifiers = new List<LevelModifier>();
        protected int levelModifier = 0;

        public void Start()
        {
            creature = GetComponent<Creature>();
            creature.OnKillEvent += Creature_OnKillEvent;
            if (creature?.brain?.instance?.GetModule<BrainModuleMelee>(false) != null)
                baseAttackSpeedMultiplier = creature.brain.instance.GetModule<BrainModuleMelee>(false).animationSpeedMultiplier;
            baseHealth = creature.maxHealth;
            StartCoroutine(Apply());
        }
        public int GetLevel() => level;
        public int GetStrength() => strength;
        public int GetEther() => ether;
        public float GetPhysicalDefense() => physicalDefense;
        public float GetEtherDefense() => etherDefense;
        public int GetAgility() => agility;
        public float GetCriticalRate() => criticalRate;
        public int GetLevelModifier() => levelModifier;
        public float GetHealthMultiplier() => healthMultiplier;
        public float GetStrengthMultiplier() => strengthMultiplier;
        public float GetEtherMultiplier() => etherMultiplier;
        public float GetAgilityMultiplier() => agilityMultiplier;
        public float GetPhysicalDefenseModifier() => physicalDefenseModifier;
        public float GetEtherDefenseModifier() => etherDefenseModifier;
        public float GetAgilityModifier() => agilityMultiplier;
        public float GetCriticalRateModifier() => criticalRateModifier;
        public int GetBaseLevel() => baseLevel;
        public float GetBaseHealth() => baseHealth;
        public int GetBaseStrength() => baseStrength;
        public int GetBaseEther() => baseEther;
        public float GetBasePhysicalDefense() => basePhysicalDefense;
        public float GetBaseEtherDefense() => baseEtherDefense;
        public int GetBaseAgility() => baseAgility;
        public float GetBaseCriticalRate() => baseCriticalRate;
        public float GetBaseAttackSpeedMultiplier() => baseAttackSpeedMultiplier; 
        public class StrengthModifier
        {
            public object handler;
            public float multiplier;
            public StrengthModifier(
                object handler,
                float multiplier = 1
                )
            {
                this.handler = handler;
                this.multiplier = multiplier;
            }
        }
        public class EtherModifier
        {
            public object handler;
            public float multiplier;
            public EtherModifier(
                object handler,
                float multiplier = 1
                )
            {
                this.handler = handler;
                this.multiplier = multiplier;
            }
        }
        public class PhysicalDefenseModifier
        {
            public object handler;
            public float modifier;
            public PhysicalDefenseModifier(
                object handler,
                float modifier = 0
                )
            {
                this.handler = handler;
                this.modifier = modifier;
            }
        }
        public class EtherDefenseModifier
        {
            public object handler;
            public float modifier;
            public EtherDefenseModifier(
                object handler,
                float modifier = 0
                )
            {
                this.handler = handler;
                this.modifier = modifier;
            }
        }
        public class AgilityModifier
        {
            public object handler;
            public float multiplier;
            public float modifier;
            public AgilityModifier(
                object handler,
                float multiplier = 1,
                float modifier = 0
                )
            {
                this.handler = handler;
                this.multiplier = multiplier;
                this.modifier = modifier;
            }
        }
        public class CriticalRateModifier
        {
            public object handler;
            public float modifier;
            public CriticalRateModifier(
                object handler,
                float modifier = 0
                )
            {
                this.handler = handler;
                this.modifier = modifier;
            }
        }
        public class HealthModifier
        {
            public object handler;
            public float multiplier;
            public HealthModifier(
                object handler,
                float multiplier = 1
                )
            {
                this.handler = handler;
                this.multiplier = multiplier;
            }
        }
        public void SetStrengthModifier(object handler, float multiplier = 1)
        {
            StrengthModifier statModifier = strengthModifiers.FirstOrDefault(p => p.handler == handler);
            if (statModifier != null)
            {
                statModifier.multiplier = multiplier;
            }
            else strengthModifiers.Add(new StrengthModifier(handler, multiplier));
            RefreshStatModifiers();
        }
        public void SetEtherModifier(object handler, float multiplier = 1)
        {
            EtherModifier statModifier = etherModifiers.FirstOrDefault(p => p.handler == handler);
            if (statModifier != null)
            {
                statModifier.multiplier = multiplier;
            }
            else etherModifiers.Add(new EtherModifier(handler, multiplier));
            RefreshStatModifiers();
        }
        public void SetPhysicalDefenseModifier(object handler, float modifier = 0)
        {
            PhysicalDefenseModifier statModifier = physicalDefenseModifiers.FirstOrDefault(p => p.handler == handler);
            if (statModifier != null)
            {
                statModifier.modifier = modifier;
            }
            else physicalDefenseModifiers.Add(new PhysicalDefenseModifier(handler, modifier));
            RefreshStatModifiers();
        }
        public void SetEtherDefenseModifier(object handler, float modifier = 0)
        {
            EtherDefenseModifier statModifier = etherDefenseModifiers.FirstOrDefault(p => p.handler == handler);
            if (statModifier != null)
            {
                statModifier.modifier = modifier;
            }
            else etherDefenseModifiers.Add(new EtherDefenseModifier(handler, modifier));
            RefreshStatModifiers();
        }
        public void SetAgilityModifier(object handler, float multiplier = 1, float modifier = 0)
        {
            AgilityModifier statModifier = agilityModifiers.FirstOrDefault(p => p.handler == handler);
            if (statModifier != null)
            {
                statModifier.multiplier = multiplier;
                statModifier.modifier = modifier;
            }
            else agilityModifiers.Add(new AgilityModifier(handler, multiplier, modifier));
            RefreshStatModifiers();
        }
        public void SetCriticalRateModifier(object handler, float modifier = 0)
        {
            CriticalRateModifier statModifier = criticalRateModifiers.FirstOrDefault(p => p.handler == handler);
            if (statModifier != null)
            {
                statModifier.modifier = modifier;
            }
            else criticalRateModifiers.Add(new CriticalRateModifier(handler, modifier));
            RefreshStatModifiers();
        }
        public void SetHealthModifier(object handler, float multiplier = 1)
        {
            HealthModifier statModifier = healthModifiers.FirstOrDefault(p => p.handler == handler);
            if (statModifier != null)
            {
                statModifier.multiplier = multiplier;
            }
            else healthModifiers.Add(new HealthModifier(handler, multiplier));
            RefreshStatModifiers();
        }
        public void RemoveStrengthModifier(object handler)
        {
            for (int index = 0; index < strengthModifiers.Count; ++index)
            {
                if (strengthModifiers[index].handler == handler)
                    strengthModifiers.RemoveAt(index);
            }
            RefreshStatModifiers();
        }
        public void RemoveEtherModifier(object handler)
        {
            for (int index = 0; index < etherModifiers.Count; ++index)
            {
                if (etherModifiers[index].handler == handler)
                    etherModifiers.RemoveAt(index);
            }
            RefreshStatModifiers();
        }
        public void RemovePhysicalDefenseModifier(object handler)
        {
            for (int index = 0; index < physicalDefenseModifiers.Count; ++index)
            {
                if (physicalDefenseModifiers[index].handler == handler)
                    physicalDefenseModifiers.RemoveAt(index);
            }
            RefreshStatModifiers();
        }
        public void RemoveEtherDefenseModifier(object handler)
        {
            for (int index = 0; index < etherDefenseModifiers.Count; ++index)
            {
                if (etherDefenseModifiers[index].handler == handler)
                    etherDefenseModifiers.RemoveAt(index);
            }
            RefreshStatModifiers();
        }
        public void RemoveAgilityModifier(object handler)
        {
            for (int index = 0; index < agilityModifiers.Count; ++index)
            {
                if (agilityModifiers[index].handler == handler)
                    agilityModifiers.RemoveAt(index);
            }
            RefreshStatModifiers();
        }
        public void RemoveCriticalRateModifier(object handler)
        {
            for (int index = 0; index < criticalRateModifiers.Count; ++index)
            {
                if (criticalRateModifiers[index].handler == handler)
                    criticalRateModifiers.RemoveAt(index);
            }
            RefreshStatModifiers();
        }
        public void RemoveHealthModifier(object handler)
        {
            for (int index = 0; index < healthModifiers.Count; ++index)
            {
                if (healthModifiers[index].handler == handler)
                    healthModifiers.RemoveAt(index);
            }
            RefreshStatModifiers();
        }
        public void ClearStrengthModifiers()
        {
            strengthModifiers.Clear();
            RefreshStatModifiers();
        }
        public void ClearEtherModifiers()
        {
            etherModifiers.Clear();
            RefreshStatModifiers();
        }
        public void ClearPhysicalDefenseModifiers()
        {
            physicalDefenseModifiers.Clear();
            RefreshStatModifiers();
        }
        public void ClearEtherDefenseModifiers()
        {
            etherDefenseModifiers.Clear();
            RefreshStatModifiers();
        }
        public void ClearAgilityModifiers()
        {
            agilityModifiers.Clear();
            RefreshStatModifiers();
        }
        public void ClearCriticalRateModifiers()
        {
            criticalRateModifiers.Clear();
            RefreshStatModifiers();
        }
        public void ClearHealthModifiers()
        {
            healthModifiers.Clear();
            RefreshStatModifiers();
        }
        public void RefreshStatModifiers()
        {
            if (strengthModifiers.Count + etherModifiers.Count + physicalDefenseModifiers.Count + etherDefenseModifiers.Count + agilityModifiers.Count + criticalRateModifiers.Count + healthModifiers.Count == 0)
            {
                strengthMultiplier = 1f;
                etherMultiplier = 1f;
                agilityMultiplier = 1f;
                physicalDefenseModifier = 0;
                etherDefenseModifier = 0;
                agilityModifier = 0f;
                criticalRateModifier = 0f;
                healthMultiplier = 1f;
            }
            else
            {
                float num1 = 1f;
                float num2 = 1f;
                float num3 = 0f;
                float num4 = 0f;
                float num5 = 1f;
                float num6 = 0f;
                float num7 = 0f;
                float num8 = 1f;
                foreach (StrengthModifier modifier in strengthModifiers)
                {
                    num1 *= modifier.multiplier;
                }
                foreach (EtherModifier modifier in etherModifiers)
                {
                    num2 *= modifier.multiplier;
                }
                foreach (PhysicalDefenseModifier modifier in physicalDefenseModifiers)
                {
                    num3 += modifier.modifier;
                }
                foreach (EtherDefenseModifier modifier in etherDefenseModifiers)
                {
                    num4 += modifier.modifier;
                }
                foreach (AgilityModifier modifier in agilityModifiers)
                {
                    num5 *= modifier.multiplier;
                    num6 += modifier.modifier;
                }
                foreach (CriticalRateModifier modifier in criticalRateModifiers)
                {
                    num7 += modifier.modifier;
                }
                foreach (HealthModifier modifier in healthModifiers)
                {
                    num8 *= modifier.multiplier;
                }
                strengthMultiplier = num1;
                etherMultiplier = num2;
                physicalDefenseModifier = num3;
                etherDefenseModifier = num4;
                agilityMultiplier = num5;
                agilityModifier = num6;
                criticalRateModifier = num7;
                healthMultiplier = num8;
            }
            creature.maxHealth = baseHealth * healthMultiplier;
            strength = Mathf.RoundToInt((float)(baseStrength * strengthMultiplier));
            ether = Mathf.RoundToInt((float)(baseEther * etherMultiplier));
            agility = Mathf.RoundToInt((float)((baseAgility + agilityModifier) * agilityMultiplier));
            physicalDefense = basePhysicalDefense + physicalDefenseModifier;
            etherDefense = baseEtherDefense + etherDefenseModifier;
            criticalRate = baseCriticalRate + criticalRateModifier;
        }
        public void ClearStatModifiers()
        {
            strengthModifiers.Clear();
            etherModifiers.Clear();
            physicalDefenseModifiers.Clear();
            etherDefenseModifiers.Clear();
            agilityModifiers.Clear();
            criticalRateModifiers.Clear();
            healthModifiers.Clear();
            RefreshStatModifiers();
        }
        public class LevelModifier
        {
            public object handler;
            public int levelModifier;
            public LevelModifier(
              object handler,
              int levelModifier)
            {
                this.handler = handler;
                this.levelModifier = levelModifier;
            }
        }
        public void SetLevelModifier(object handler, int modifier)
        {
            LevelModifier levelModifier = levelModifiers.FirstOrDefault(p => p.handler == handler);
            if (levelModifier != null)
            {
                levelModifier.levelModifier = modifier;
            }
            else
                levelModifiers.Add(new LevelModifier(handler, modifier));
            RefreshLevelModifiers();
        }
        public void RefreshLevelModifiers()
        {
            if(levelModifiers.Count == 0)
            {
                levelModifier = 0;
            }
            else
            {
                int num1 = 0;
                foreach(LevelModifier levelModifier in levelModifiers)
                {
                    num1 += levelModifier.levelModifier;
                }
                levelModifier = num1;
            }
            level = baseLevel + levelModifier;
            RefreshLevel();
        }
        public void RemoveLevelModifier(object handler)
        {
            for (int index = 0; index < levelModifiers.Count; ++index)
            {
                if (levelModifiers[index].handler == handler)
                    levelModifiers.RemoveAt(index);
            }
            RefreshLevelModifiers();
        }
        public void ClearLevelModifiers()
        {
            levelModifiers.Clear();
            RefreshLevelModifiers();
        }
        public IEnumerator Apply()
        {
            while (!creature.loaded) yield return null;
            foreach (RagdollPart part in creature.ragdoll.parts)
            {
                part.sliceWidth = Mathf.Clamp(0.04f + (0.004f * (XenobladeManager.GetLevel() - level)), 0, 0.08f);
                if (partDismemberment.ContainsKey(part)) partDismemberment.Add(part, part.sliceAllowed);
                if (part.sliceWidth <= 0) part.sliceAllowed = false;
            }
            RefreshLevelModifiers();
            isMage = false;
            isLongDistanceMage = false;
            foreach (ContainerData.Content content in creature.container.contents)
            {
                if (content.itemData.GetModule<ItemModuleSpell>() is ItemModuleSpell module)
                {
                    isMage = true;
                    if ((module.spellData as SpellCastData)?.aiCastMaxDistance > 5) isLongDistanceMage = true;
                }
            }
            if (isLongDistanceMage)
            {
                baseStrength = baseLevel * 0;
                baseEther = baseLevel * 30;
            }
            else if (isMage)
            {
                baseStrength = baseLevel * 15;
                baseEther = baseLevel * 15;
            }
            else
            {
                baseStrength = baseLevel * 30;
                baseEther = baseLevel * 0;
            }
            baseAgility = Mathf.RoundToInt(baseLevel * 1.25f);
            if (overrideStrength > -1) baseStrength = overrideStrength;
            if (overrideEther > -1) baseEther = overrideEther;
            if (overrideAgility > -1) baseAgility = overrideAgility;
            RefreshStatModifiers();
            yield break;
        }
        private void Creature_OnKillEvent(CollisionInstance collisionInstance, EventTime eventTime)
        {
            Reset();
            ClearLevelModifiers();
            ClearStatModifiers();
        }
        public void Reset()
        {
            foreach (RagdollPart part in creature.ragdoll.parts)
            {
                part.sliceWidth = 0.04f;
                if (partDismemberment.ContainsKey(part)) part.sliceAllowed = partDismemberment[part];
            }
            if (creature?.brain?.instance?.GetModule<BrainModuleMelee>() != null)
            baseAttackSpeedMultiplier = creature.brain.instance.GetModule<BrainModuleMelee>(false).animationSpeedMultiplier;
        }

        public void Setup(int min, int max, int minVar, int maxVar, bool relative, bool unique, string name, bool sight, bool sound, bool force, float crit, DefenseDirection direction, float physDef, float ethDef, int health, int str, int eth, int agi, int xp, int ap)
        {
            minLevel = min;
            maxLevel = max;
            minLevelVariation = minVar;
            maxLevelVariation = maxVar;
            playerRelative = relative;
            isUnique = unique;
            creatureName = name;
            if (!playerRelative)
                baseLevel = isUnique ? Math.Max(UnityEngine.Random.Range(minLevel, maxLevel + 1) + UnityEngine.Random.Range(minLevelVariation, maxLevelVariation + 1), 1) : Mathf.Clamp(UnityEngine.Random.Range(minLevel, maxLevel + 1) + UnityEngine.Random.Range(minLevelVariation, maxLevelVariation + 1), 1, 99);
            else
                baseLevel = isUnique ? Math.Max(XenobladeManager.GetLevel() + UnityEngine.Random.Range(minLevelVariation, maxLevelVariation + 1), 1) : Mathf.Clamp(XenobladeManager.GetLevel() + UnityEngine.Random.Range(minLevelVariation, maxLevelVariation + 1), 1, 99);
            detectionTypeSight = sight;
            detectionTypeSound = sound;
            forceDetection = force;
            baseCriticalRate = crit;
            defenseDirection = direction;
            basePhysicalDefense = physDef;
            baseEtherDefense = ethDef;
            overrideHealth = health;
            overrideStrength = str;
            overrideEther = eth;
            overrideAgility = agi;
            overrideXP = xp;
            overrideAP = ap;
        }
        public void RefreshLevel()
        {
            Reset();
            foreach (RagdollPart part in creature.ragdoll.parts)
            {
                part.sliceWidth = Mathf.Clamp(0.04f + (0.004f * (XenobladeManager.GetLevel() - level)), 0, 0.08f);
                if (partDismemberment.ContainsKey(part)) part.sliceAllowed = partDismemberment[part];
                if (part.sliceWidth <= 0) part.sliceAllowed = false;
            }
        }
    }
}
