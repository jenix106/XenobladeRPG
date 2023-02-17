using System.Collections;
using ThunderRoad;
using UnityEngine;

namespace XenobladeRPG
{
    public class XenobladeLevelModule : LevelModule
    {
        public static GameObject currentBar;
        public static GameObject playerInfo;
        public static GameObject damage;
        public override IEnumerator OnLoadCoroutine()
        {
            XenobladeManager.LoadFromSave();
            EventManager.onCreatureSpawn += EventManager_onCreatureSpawn;
            EventManager.onCreatureKill += EventManager_onCreatureKill;
            EventManager.onItemSpawn += EventManager_onItemSpawn;
            EventManager.onItemEquip += EventManager_onItemEquip;
            WaveSpawner.OnWaveSpawnerStartRunningEvent.AddListener(call => UpdateHealth());
            Catalog.LoadAssetAsync<GameObject>("Jenix.XenobladeUI", value =>
            {
                currentBar = value;
            }, "XenobladeRPG");
            Catalog.LoadAssetAsync<GameObject>("Jenix.XenobladePlayerUI", value =>
            {
                playerInfo = value;
            }, "XenobladeRPG");
            Catalog.LoadAssetAsync<GameObject>("Jenix.XenobladeDamageUI", value =>
            {
                damage = value;
            }, "XenobladeRPG");
            XenobladePatcher.DoPatching();
            foreach(ItemData data in Catalog.GetDataList<ItemData>())
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
                            itemData = data
                        };
                        data.modules.Add(armor);
                    }
                    if ((data.type == ItemData.Type.Weapon || data.type == ItemData.Type.Shield) && data.GetModule<XenobladeWeaponModule>() == null)
                    {
                        XenobladeWeaponModule weapon = new XenobladeWeaponModule
                        {
                            baseAttackDamage = new Vector2(Mathf.Clamp(8 * XenobladeManager.GetLevel(), 0, 999), Mathf.Clamp(11 * XenobladeManager.GetLevel(), 0, 999)),
                            itemData = data
                        };
                        data.modules.Add(weapon);
                    }
                }
                catch { }
            }
            foreach(DamagerData data in Catalog.GetDataList<DamagerData>())
            {
                try
                {
                    data.playerMinDamage = 0f;
                    data.playerMaxDamage = float.PositiveInfinity;
                }
                catch { }
            }
            foreach(BrainData data in Catalog.GetDataList<BrainData>())
            {
                try
                {
                    if(data.GetModule<XenobladeBrainModule>(false) == null)
                    {
                        XenobladeBrainModule brain = new XenobladeBrainModule();
                        data.modules.Add(brain);
                    }
                }
                catch { }
            }
            if (level?.GetComponent<QuitComponent>() != null)
                level.gameObject.AddComponent<QuitComponent>();
            return base.OnLoadCoroutine();
        }
        public class QuitComponent : MonoBehaviour
        {
            public void OnApplicationQuit()
            {
                XenobladeManager.SaveToJSON();
            }
        }

        private void EventManager_onItemEquip(Item item)
        {
            if(item.data.GetModule<XenobladeWeaponModule>() is XenobladeWeaponModule weaponModule)
            {
                if (item.mainHandler.creature.isPlayer && item.mainHandler.otherHand.grabbedHandle?.item != item) XenobladeManager.SetStatModifier(item, 1, 1, 1, 1, 0, 0, weaponModule.physicalDefense, weaponModule.etherDefense, weaponModule.criticalRate, weaponModule.blockRate);
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
            Player.local.creature.maxHealth += 50 * (XenobladeManager.GetLevel() - 1);
            Player.local.creature.currentHealth = Player.local.creature.maxHealth * healthRatio;
        }
        public override void OnUnload()
        {
            base.OnUnload();
            XenobladeManager.SaveToJSON();
            EventManager.onCreatureSpawn -= EventManager_onCreatureSpawn;
            EventManager.onCreatureKill -= EventManager_onCreatureKill;
            EventManager.onItemSpawn -= EventManager_onItemSpawn;
            EventManager.onItemEquip -= EventManager_onItemEquip;
            WaveSpawner.OnWaveSpawnerStartRunningEvent.RemoveListener(call => UpdateHealth());
        }
        private void EventManager_onCreatureKill(Creature creature, Player player, CollisionInstance collisionInstance, EventTime eventTime)
        {
            if (eventTime == EventTime.OnEnd && !creature.isPlayer)
            {
                if (creature.GetComponent<XenobladeStats>() is XenobladeStats stats)
                {
                    float xpGained = 10;
                    for (int i = 1; i <= stats.GetLevel(); i++)
                    {
                        xpGained += Mathf.Pow(2, (3 + Mathf.FloorToInt(i / 10)));
                    }
                    if (stats.isUnique) xpGained *= 1.5f;
                    if (stats.GetLevel() >= XenobladeManager.GetLevel() + 6) xpGained *= 0.1f;
                    else if (stats.GetLevel() >= XenobladeManager.GetLevel() + 3) xpGained *= 0.5f;
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
                        else xpGained *= 1.5f;
                    }
                    else if (stats.GetLevel() <= XenobladeManager.GetLevel() - 3) xpGained *= 1.25f;
                    XenobladeManager.AddXP(Mathf.RoundToInt(xpGained));
                }
            }
        }

        private void EventManager_onCreatureSpawn(Creature creature)
        {
            if (creature != Player.local.creature)
            {
                /*if (creature.brain.instance.GetModule<XenobladeBrainModule>(false) == null)
                {
                    XenobladeBrainModule obj = new XenobladeBrainModule();
                    creature.brain.instance.modules.Add(obj);
                    obj.Load(creature);
                }*/
                creature.gameObject.AddComponent<XenobladeRemoveBar>().healthBar = Object.Instantiate(currentBar);
            }
            if (creature == Player.local.creature)
            {
                creature.maxHealth += 50 * (XenobladeManager.GetLevel() - 1);
                creature.currentHealth = creature.maxHealth;
                foreach (WristStats wristStat in creature.GetComponentsInChildren<WristStats>())
                {
                    wristStat.gameObject.AddComponent<XenobladePlayerInfo>().info = Object.Instantiate(playerInfo);
                }
                foreach(ContainerData.Content content in creature.container.contents)
                {
                    if(content.itemData.GetModule<XenobladeArmorModule>() is XenobladeArmorModule armorModule)
                    {
                        XenobladeManager.SetStatModifier(armorModule, 1, 1, 1, 1, 1, 0, 0, armorModule.physicalDefense, armorModule.etherDefense, armorModule.weight, 0, 0);
                    }
                }
            }
        }
    }
}
