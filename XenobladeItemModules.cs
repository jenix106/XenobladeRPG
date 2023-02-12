using UnityEngine;
using ThunderRoad;

namespace XenobladeRPG
{
    public class XenobladeArmorModule : ItemModuleApparel
    {
        public int physicalDefense = 0;
        public int etherDefense = 0;
        public int weight = 0;
        public override void OnEquip(Creature creature, ApparelModuleType equippedOn, ItemModuleWardrobe.CreatureWardrobe wardrobeData)
        {
            base.OnEquip(creature, equippedOn, wardrobeData);
            if (creature?.player != null)
            {
                XenobladeManager.SetStatModifier(this, 1, 1, 1, 1, 1, 0, 0, physicalDefense, etherDefense, weight, 0, 0);
            }
        }
        public override void OnUnequip(Creature creature, ApparelModuleType equippedOn, ItemModuleWardrobe.CreatureWardrobe wardrobeData)
        {
            base.OnUnequip(creature, equippedOn, wardrobeData);
            if (creature?.player != null)
            {
                XenobladeManager.RemoveStatModifier(this);
            }
        }
    }
    public class XenobladeWeaponModule : ItemModule
    {
        public bool isGrowthWeapon = false;
        public Vector2 attackDamageRange = new Vector2(0, 0);
        public Vector3 baseAttackDamage = new Vector3(0, 0);
        public int physicalDefense = 0;
        public int etherDefense = 0;
        public float criticalRate = 0;
        public float blockRate = 0;
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.OnGrabEvent += Item_OnGrabEvent;
            item.OnUngrabEvent += Item_OnUngrabEvent;
            if (!isGrowthWeapon)
            {
                attackDamageRange.x = Mathf.Clamp(baseAttackDamage.x, 0, 999);
                attackDamageRange.y = Mathf.Clamp(baseAttackDamage.y, 0, 999);
            }
            else if (isGrowthWeapon)
            {
                attackDamageRange.x = Mathf.Clamp(baseAttackDamage.x * XenobladeManager.GetLevel(), 0, 999);
                attackDamageRange.y = Mathf.Clamp(baseAttackDamage.y * XenobladeManager.GetLevel(), 0, 999);
            }
        }
        public void RefreshLevel()
        {
            if (isGrowthWeapon)
            {
                attackDamageRange.x = Mathf.Clamp(baseAttackDamage.x * XenobladeManager.GetLevel(), 0, 999);
                attackDamageRange.y = Mathf.Clamp(baseAttackDamage.y * XenobladeManager.GetLevel(), 0, 999);
            }
        }

        private void Item_OnUngrabEvent(Handle handle, RagdollHand ragdollHand, bool throwing)
        {
            if (ragdollHand.creature.isPlayer && ragdollHand.otherHand.grabbedHandle?.item != item) XenobladeManager.RemoveStatModifier(item);
        }

        private void Item_OnGrabEvent(Handle handle, RagdollHand ragdollHand)
        {
            if (ragdollHand.creature.isPlayer && ragdollHand.otherHand.grabbedHandle?.item != item) XenobladeManager.SetStatModifier(item, 1, 1, 1, 1, 1, 0, 0, physicalDefense, etherDefense, 0, criticalRate, blockRate);
        }
    }
}
