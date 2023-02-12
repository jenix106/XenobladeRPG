using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad;
using UnityEngine;

namespace XenobladeRPG
{
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
        protected float baseAttackSpeedMultiplier;
        public DefenseDirection defenseDirection;
        public Dictionary<RagdollPart, bool> partDismemberment = new Dictionary<RagdollPart, bool>();
        public bool isDazed = false;
        public bool isToppled = false;
        public bool isBroken = false;
        bool isMage = false;
        bool isLongDistanceMage = false;
        public List<StatModifier> statModifiers = new List<StatModifier>();
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
        public class StatModifier
        {
            public object handler;
            public float healthMultiplier;
            public float strengthMultiplier;
            public float etherMultiplier;
            public float agilityMultiplier;
            public float physicalDefenseModifier;
            public float etherDefenseModifier;
            public float agilityModifier;
            public float criticalRateModifier;
            public StatModifier(
              object handler,
              float healthMultiplier,
              float strengthMultiplier,
              float etherMultiplier,
              float agilityMultiplier,
              float physicalDefenseModifier,
              float etherDefenseModifier,
              float agilityModifier,
              float criticalRateModifier)
            {
                this.handler = handler;
                this.healthMultiplier = healthMultiplier;
                this.strengthMultiplier = strengthMultiplier;
                this.etherMultiplier = etherMultiplier;
                this.agilityMultiplier = agilityMultiplier;
                this.physicalDefenseModifier = physicalDefenseModifier;
                this.etherDefenseModifier = etherDefenseModifier;
                this.agilityModifier = agilityModifier;
                this.criticalRateModifier = criticalRateModifier;
            }
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
        public void SetStatModifier(
            object handler,
            float healthMultiplier = 1,
            float strengthMultiplier = 1,
            float etherMultiplier = 1,
            float agilityMultiplier = 1,
            float physicalDefenseModifier = 0,
            float etherDefenseModifier = 0,
            float agilityModifier = 0,
            float criticalRateModifier = 0)
        {
            StatModifier statModifier = statModifiers.FirstOrDefault(p => p.handler == handler);
            if (statModifier != null)
            {
                statModifier.healthMultiplier = healthMultiplier;
                statModifier.strengthMultiplier = strengthMultiplier;
                statModifier.etherMultiplier = etherMultiplier;
                statModifier.agilityMultiplier = agilityMultiplier;
                statModifier.physicalDefenseModifier = physicalDefenseModifier;
                statModifier.etherDefenseModifier = etherDefenseModifier;
                statModifier.agilityModifier = agilityModifier;
                statModifier.criticalRateModifier = criticalRateModifier;
            }
            else
                statModifiers.Add(new StatModifier(handler, healthMultiplier, strengthMultiplier, etherMultiplier, agilityMultiplier, physicalDefenseModifier, etherDefenseModifier, agilityModifier, criticalRateModifier));
            RefreshStatModifiers();
        }
        public void RefreshStatModifiers()
        {
            if (statModifiers.Count == 0)
            {
                healthMultiplier = 1f;
                strengthMultiplier = 1f;
                etherMultiplier = 1f;
                agilityMultiplier = 1f;
                physicalDefenseModifier = 0f;
                etherDefenseModifier = 0f;
                agilityModifier = 0f;
                criticalRateModifier = 0f;
            }
            else
            {
                float num1 = 1f;
                float num2 = 1f;
                float num3 = 1f;
                float num4 = 0f;
                float num5 = 0f;
                float num6 = 0f;
                float num7 = 1f;
                float num8 = 0f;
                foreach (StatModifier statModifier in statModifiers)
                {
                    num1 *= statModifier.healthMultiplier;
                    num2 *= statModifier.strengthMultiplier;
                    num3 *= statModifier.etherMultiplier;
                    num4 += statModifier.physicalDefenseModifier;
                    num5 += statModifier.etherDefenseModifier;
                    num6 += statModifier.criticalRateModifier;
                    num7 *= statModifier.agilityMultiplier;
                    num8 += statModifier.agilityModifier;
                }
                healthMultiplier = num1;
                strengthMultiplier = num2;
                etherMultiplier = num3;
                physicalDefenseModifier = num4;
                etherDefenseModifier = num5;
                criticalRateModifier = num6;
                agilityMultiplier = num7;
                agilityModifier = num8;
            }
            creature.maxHealth = baseHealth * healthMultiplier;
            strength = Mathf.RoundToInt((float)(baseStrength * strengthMultiplier));
            ether = Mathf.RoundToInt((float)(baseEther * etherMultiplier));
            agility = Mathf.RoundToInt((float)((baseAgility + agilityModifier) * agilityMultiplier));
            physicalDefense = basePhysicalDefense + physicalDefenseModifier;
            etherDefense = baseEtherDefense + etherDefenseModifier;
            criticalRate = baseCriticalRate + criticalRateModifier;
        }
        public void RemoveStatModifier(object handler)
        {
            for (int index = 0; index < statModifiers.Count; ++index)
            {
                if (statModifiers[index].handler == handler)
                    statModifiers.RemoveAt(index);
            }
            RefreshStatModifiers();
        }
        public void ClearStatModifiers()
        {
            statModifiers.Clear();
            RefreshStatModifiers();
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
            /*if (level >= XenobladeManager.GetLevel() + 6) creature.gameObject.AddComponent<DangerRed>();
            else if (level >= XenobladeManager.GetLevel() + 3) creature.gameObject.AddComponent<DangerYellow>();
            else if (level <= XenobladeManager.GetLevel() - 6) creature.gameObject.AddComponent<DangerBlack>();
            else if (level <= XenobladeManager.GetLevel() - 3) creature.gameObject.AddComponent<DangerBlue>();*/
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
            /*if (creature.gameObject.GetComponent<DangerRed>() != null) Destroy(creature.gameObject.GetComponent<DangerRed>());
            if (creature.gameObject.GetComponent<DangerYellow>() != null) Destroy(creature.gameObject.GetComponent<DangerYellow>());
            if (creature.gameObject.GetComponent<DangerBlack>() != null) Destroy(creature.gameObject.GetComponent<DangerBlack>());
            if (creature.gameObject.GetComponent<DangerBlue>() != null) Destroy(creature.gameObject.GetComponent<DangerBlue>());*/
            foreach (RagdollPart part in creature.ragdoll.parts)
            {
                part.sliceWidth = 0.04f;
                if (partDismemberment.ContainsKey(part)) part.sliceAllowed = partDismemberment[part];
            }
            baseAttackSpeedMultiplier = creature.brain.instance.GetModule<BrainModuleMelee>(false).animationSpeedMultiplier;
        }

        public void Setup(int min, int max, int minVar, int maxVar, bool relative, bool unique, string name, bool sight, bool sound, bool force, float crit, DefenseDirection direction, float physDef, float ethDef, int health, int str, int eth, int agi)
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
        }
        public void RefreshLevel()
        {
            Reset();
            /*if (level >= XenobladeManager.GetLevel() + 6) creature.gameObject.AddComponent<DangerRed>();
            else if (level >= XenobladeManager.GetLevel() + 3) creature.gameObject.AddComponent<DangerYellow>();
            else if (level <= XenobladeManager.GetLevel() - 6) creature.gameObject.AddComponent<DangerBlack>();
            else if (level <= XenobladeManager.GetLevel() - 3) creature.gameObject.AddComponent<DangerBlue>();*/
            foreach (RagdollPart part in creature.ragdoll.parts)
            {
                part.sliceWidth = Mathf.Clamp(0.04f + (0.004f * (XenobladeManager.GetLevel() - level)), 0, 0.08f);
                if (partDismemberment.ContainsKey(part)) part.sliceAllowed = partDismemberment[part];
                if (part.sliceWidth <= 0) part.sliceAllowed = false;
            }
        }
    }
}
