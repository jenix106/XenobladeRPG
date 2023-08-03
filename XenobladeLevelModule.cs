﻿using System.Collections;
using ThunderRoad;
using UnityEngine;

namespace XenobladeRPG
{
    public class XenobladeLevelModule : ThunderScript
    {
        public static GameObject currentBar;
        public static GameObject playerInfo;
        public static GameObject damage;
        public static GameObject heal;
        public static GameObject statusDamage;
        public static GameObject electricDamage;
        public override void ScriptEnable()
        {
            base.ScriptEnable();
            EventManager.onCreatureSpawn += EventManager_onCreatureSpawn;
            EventManager.onCreatureKill += EventManager_onCreatureKill;
            EventManager.onItemSpawn += EventManager_onItemSpawn;
            EventManager.onItemEquip += EventManager_onItemEquip;
            EventManager.onLevelLoad += EventManager_onLevelLoad;
            WaveSpawner.OnWaveSpawnerStartRunningEvent.AddListener(call => UpdateHealth());
            XenobladePatcher.DoPatching();
        }
        public override void ScriptDisable()
        {
            base.ScriptDisable();
            XenobladeManager.SaveToJSON();
            EventManager.onCreatureSpawn -= EventManager_onCreatureSpawn;
            EventManager.onCreatureKill -= EventManager_onCreatureKill;
            EventManager.onItemSpawn -= EventManager_onItemSpawn;
            EventManager.onItemEquip -= EventManager_onItemEquip;
            EventManager.onLevelLoad -= EventManager_onLevelLoad;
            WaveSpawner.OnWaveSpawnerStartRunningEvent.RemoveListener(call => UpdateHealth());
        }
        private void EventManager_onLevelLoad(LevelData levelData, EventTime eventTime)
        {
            if (currentBar == null)
            {
                Catalog.LoadAssetAsync<GameObject>("Jenix.XenobladeUI", value =>
                {
                    currentBar = value;
                }, "XenobladeRPG");
                Catalog.LoadAssetAsync<GameObject>("Jenix.XenobladeDamageUI", value =>
                {
                    damage = value;
                }, "XenobladeRPG");
                Catalog.LoadAssetAsync<GameObject>("Jenix.XenobladeHealUI", value =>
                {
                    heal = value;
                }, "XenobladeRPG");
                Catalog.LoadAssetAsync<GameObject>("Jenix.XenobladeStatusDamageUI", value =>
                {
                    statusDamage = value;
                }, "XenobladeRPG");
                Catalog.LoadAssetAsync<GameObject>("Jenix.XenobladeElectricDamageUI", value =>
                {
                    electricDamage = value;
                }, "XenobladeRPG");
            }
            if (Player.characterData != null && eventTime == EventTime.OnEnd)
            {
                XenobladeManager.LoadFromSave();
                foreach (ItemData data in Catalog.GetDataList<ItemData>())
                {
                    try
                    {
                        if (data.type == ItemData.Type.Wardrobe && data.GetModule<XenobladeArmorModule>() == null)
                        {
                            XenobladeArmorModule armor = new XenobladeArmorModule
                            {
                                physicalDefense = 50 * data.tier,
                                etherDefense = 50 * data.tier,
                                weight = 1 * data.tier,
                                itemData = data,
                            };
                            data.modules.Add(armor);
                        }
                        if ((data.type == ItemData.Type.Weapon || data.type == ItemData.Type.Shield) && data.GetModule<XenobladeWeaponModule>() == null)
                        {
                            XenobladeWeaponModule weapon = new XenobladeWeaponModule
                            {
                                itemData = data,
                                isAutoGenerated = true
                            };
                            data.modules.Add(weapon);
                        }
                    }
                    catch { }
                }
                foreach (DamagerData data in Catalog.GetDataList<DamagerData>())
                {
                    try
                    {
                        data.playerMinDamage = 0f;
                        data.playerMaxDamage = float.PositiveInfinity;
                    }
                    catch { }
                }
                foreach (BrainData data in Catalog.GetDataList<BrainData>())
                {
                    try
                    {
                        if (data.GetModule<XenobladeBrainModule>(false) == null && data.id != "Player")
                        {
                            XenobladeBrainModule brain = new XenobladeBrainModule();
                            data.modules.Add(brain);
                        }
                    }
                    catch { }
                }
            }
        }
        private void EventManager_onItemEquip(Item item)
        {
            if(item.data.GetModule<XenobladeWeaponModule>() is XenobladeWeaponModule weaponModule)
            {
                if (item.mainHandler.creature.isPlayer && item.mainHandler.otherHand.grabbedHandle?.item != item)
                {
                    XenobladeManager.SetPhysicalDefenseModifier(item, 1, weaponModule.physicalDefense);
                    XenobladeManager.SetEtherDefenseModifier(item, 1, weaponModule.etherDefense);
                    XenobladeManager.SetCriticalRateModifier(item, weaponModule.criticalRate);
                    XenobladeManager.SetBlockRateModifier(item, weaponModule.blockRate);
                }
            }
        }
        private void EventManager_onItemSpawn(Item item)
        {
            if (item.data.GetModule<XenobladeWeaponModule>() == null && item.GetComponent<ItemMagicProjectile>() is ItemMagicProjectile component)
            {
                if(component.item?.lastHandler != null && component.item?.lastHandler?.grabbedHandle?.item?.data?.GetModule<XenobladeWeaponModule>() is XenobladeWeaponModule staff)
                {
                    XenobladeWeaponModule spell = new XenobladeWeaponModule
                    {
                        attackDamageRange = staff.attackDamageRange,
                        itemData = component.item.data
                    };
                    item.data.modules.Add(spell);
                }
            }
        }
        public void UpdateHealth()
        {
            float healthRatio = Player.local.creature.currentHealth / Player.local.creature.maxHealth;
            Player.local.creature.maxHealth += 100 * (XenobladeManager.GetLevel() - 1);
            Player.local.creature.currentHealth = Player.local.creature.maxHealth * healthRatio;
        }
        private void EventManager_onCreatureKill(Creature creature, Player player, CollisionInstance collisionInstance, EventTime eventTime)
        {
            if (eventTime == EventTime.OnEnd && !creature.isPlayer)
            {
                if (creature.GetComponent<XenobladeStats>() is XenobladeStats stats)
                {
                    float xpGained = 10;
                    float apGained = 10;
                    float spGained = 10;
                    for (int i = 1; i <= stats.GetLevel(); i++)
                    {
                        xpGained += Mathf.Pow(2, (3 + Mathf.FloorToInt(i / 15)));
                        apGained += 3;
                    }
                    if (stats.isUnique)
                    {
                        xpGained *= 2f;
                        apGained *= 2f;
                    }
                    if (stats.overrideXP > -1) xpGained = stats.overrideXP;
                    if (stats.overrideAP > -1) apGained = stats.overrideAP;
                    if (stats.GetLevel() >= XenobladeManager.GetLevel() + 6)
                    {
                        xpGained *= 0.1f;
                        apGained *= 0.1f;
                        spGained = 1;
                    }
                    else if (stats.GetLevel() >= XenobladeManager.GetLevel() + 3)
                    {
                        xpGained *= 0.5f;
                        apGained *= 0.5f;
                        spGained = 5;
                    }
                    else if (stats.GetLevel() <= XenobladeManager.GetLevel() - 6)
                    {
                        if (stats.GetLevel() > XenobladeManager.GetLevel() + 21)
                        {
                            xpGained = 74;
                            for (int i = 1; i <= XenobladeManager.GetLevel(); i++)
                            {
                                xpGained += Mathf.Pow(2, (3 + Mathf.FloorToInt(i / 10)));
                            }
                        }
                        else
                        {
                            xpGained *= 1.5f;
                        }
                        apGained *= 1.5f;
                        spGained = 20;
                    }
                    else if (stats.GetLevel() <= XenobladeManager.GetLevel() - 3)
                    {
                        xpGained *= 1.25f;
                        apGained *= 1.25f;
                        spGained = 15;
                    }
                    XenobladeManager.AddXP(Mathf.RoundToInt(xpGained));
                    XenobladeManager.AddAP(Mathf.RoundToInt(apGained));
                    XenobladeManager.AddSP(Mathf.RoundToInt(spGained));
                }
            }
        }
        private void EventManager_onCreatureSpawn(Creature creature)
        {
            if (creature != Player.local.creature)
            {
                creature.gameObject.AddComponent<XenobladeCreatureInfo>().healthBar = Object.Instantiate(currentBar);
            }
            if (creature == Player.local.creature)
            {
                creature.maxHealth += 100 * (XenobladeManager.GetLevel() - 1);
                creature.currentHealth = creature.maxHealth;
                Catalog.LoadAssetAsync<GameObject>("Jenix.XenobladePlayerUI", value =>
                {
                    playerInfo = value;
                    foreach (WristStats wristStat in creature.GetComponentsInChildren<WristStats>())
                    {
                        wristStat.gameObject.AddComponent<XenobladePlayerInfo>().info = Object.Instantiate(playerInfo);
                    }
                }, "XenobladeRPG");
                foreach(ContainerData.Content content in creature.container.contents)
                {
                    if(content.itemData.GetModule<XenobladeArmorModule>() is XenobladeArmorModule armorModule)
                    {
                        XenobladeManager.SetPhysicalDefenseModifier(armorModule, 1, armorModule.physicalDefense);
                        XenobladeManager.SetEtherDefenseModifier(armorModule, 1, armorModule.etherDefense);
                        XenobladeManager.SetAgilityModifier(armorModule, 1, armorModule.weight);
                    }
                }
            }
        }
    }
}
