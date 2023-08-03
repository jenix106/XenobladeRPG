using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using ThunderRoad;

namespace XenobladeRPG
{
    class XenobladeMenu : MenuModule
    {
        public Button leftShoulder;
        public Button rightShoulder;
        public Button leftHip;
        public Button rightHip;
        public Button chest;
        public Button helm;
        public Button pants;
        public Button boots;
        public Button leftHand;
        public Button rightHand;
        public Button leftHeld;
        public Button rightHeld;
        public Button shoulderArmor;
        public RectTransform item;
        public RectTransform player;
        public RectTransform weapon;
        public RectTransform armor;
        public Text itemName;
        public Text itemTier;
        public Text itemDescription;
        public Text weaponAttack;
        public Text weaponPhysDef;
        public Text weaponEthDef;
        public Text weaponCrit;
        public Text weaponBlock;
        public Text armorPhysDef;
        public Text armorEthDef;
        public Text armorWeight;
        public Text playerLevel;
        public Text playerXP;
        public Text playerHealth;
        public Text playerAttack;
        public Text playerStrength;
        public Text playerEther;
        public Text playerPhysDef;
        public Text playerEthDef;
        public Text playerAgility;
        public Text playerCrit;
        public Text playerBlock;
        public Item leftHeldItem;
        public Item rightHeldItem;
        public Item leftShoulderItem;
        public Item rightShoulderItem;
        public Item leftHipItem;
        public Item rightHipItem;
        public ContainerData.Content chestContent;
        public ContainerData.Content shoulderContent;
        public ContainerData.Content helmContent;
        public ContainerData.Content pantsContent;
        public ContainerData.Content bootsContent;
        public ContainerData.Content leftHandContent;
        public ContainerData.Content rightHandContent;
        public UIMenu menu;
        public Button pressedButton;
        public override void Init(MenuData menuData, UIMenu menu)
        {
            base.Init(menuData, menu);
            this.menu = menu;
            leftShoulder = menu.GetCustomReference<Button>("LeftShoulder");
            rightShoulder = menu.GetCustomReference<Button>("RightShoulder");
            leftHip = menu.GetCustomReference<Button>("LeftHip");
            rightHip = menu.GetCustomReference<Button>("RightHip");
            chest = menu.GetCustomReference<Button>("Chest");
            helm = menu.GetCustomReference<Button>("Helm");
            pants = menu.GetCustomReference<Button>("Pants");
            boots = menu.GetCustomReference<Button>("Boots");
            leftHand = menu.GetCustomReference<Button>("LeftHand");
            rightHand = menu.GetCustomReference<Button>("RightHand");
            leftHeld = menu.GetCustomReference<Button>("LeftHeld");
            rightHeld = menu.GetCustomReference<Button>("RightHeld");
            shoulderArmor = menu.GetCustomReference<Button>("ShoulderArmor");
            item = menu.GetCustomReference<RectTransform>("Item");
            player = menu.GetCustomReference<RectTransform>("Player");
            weapon = menu.GetCustomReference<RectTransform>("Weapon");
            armor = menu.GetCustomReference<RectTransform>("Armor");
            itemName = menu.GetCustomReference<Text>("ItemName");
            itemTier = menu.GetCustomReference<Text>("ItemTier");
            itemDescription = menu.GetCustomReference<Text>("ItemDescription");
            weaponAttack = menu.GetCustomReference<Text>("WeaponAttack");
            weaponPhysDef = menu.GetCustomReference<Text>("WeaponPhysDef");
            weaponEthDef = menu.GetCustomReference<Text>("WeaponEthDef");
            weaponCrit = menu.GetCustomReference<Text>("WeaponCrit");
            weaponBlock = menu.GetCustomReference<Text>("WeaponBlock");
            armorPhysDef = menu.GetCustomReference<Text>("ArmorPhysDef");
            armorEthDef = menu.GetCustomReference<Text>("ArmorEthDef");
            armorWeight = menu.GetCustomReference<Text>("ArmorWeight");
            playerLevel = menu.GetCustomReference<Text>("PlayerLevel");
            playerXP = menu.GetCustomReference<Text>("PlayerXP");
            playerHealth = menu.GetCustomReference<Text>("PlayerHealth");
            playerAttack = menu.GetCustomReference<Text>("PlayerAttack");
            playerStrength = menu.GetCustomReference<Text>("PlayerStrength");
            playerEther = menu.GetCustomReference<Text>("PlayerEther");
            playerPhysDef = menu.GetCustomReference<Text>("PlayerPhysDef");
            playerEthDef = menu.GetCustomReference<Text>("PlayerEthDef");
            playerAgility = menu.GetCustomReference<Text>("PlayerAgility");
            playerCrit = menu.GetCustomReference<Text>("PlayerCrit");
            playerBlock = menu.GetCustomReference<Text>("PlayerBlock");
            leftShoulder.onClick.AddListener(delegate { ButtonClick(leftShoulder, Holder.DrawSlot.BackLeft); });
            rightShoulder.onClick.AddListener(delegate { ButtonClick(rightShoulder, Holder.DrawSlot.BackRight); });
            leftHip.onClick.AddListener(delegate { ButtonClick(leftHip, Holder.DrawSlot.HipsLeft); });
            rightHip.onClick.AddListener(delegate { ButtonClick(rightHip, Holder.DrawSlot.HipsRight); });
            chest.onClick.AddListener(delegate { ButtonClick(chest, Holder.DrawSlot.None, chestContent); });
            helm.onClick.AddListener(delegate { ButtonClick(helm, Holder.DrawSlot.None, helmContent); });
            pants.onClick.AddListener(delegate { ButtonClick(pants, Holder.DrawSlot.None, pantsContent); });
            boots.onClick.AddListener(delegate { ButtonClick(boots, Holder.DrawSlot.None, bootsContent); });
            leftHand.onClick.AddListener(delegate { ButtonClick(leftHand, Holder.DrawSlot.None, leftHandContent); });
            rightHand.onClick.AddListener(delegate { ButtonClick(rightHand, Holder.DrawSlot.None, rightHandContent); });
            leftHeld.onClick.AddListener(delegate { ButtonClick(leftHeld, Holder.DrawSlot.None, null, Player.local.handLeft.ragdollHand); });
            rightHeld.onClick.AddListener(delegate { ButtonClick(rightHeld, Holder.DrawSlot.None, null, Player.local.handRight.ragdollHand); });
            shoulderArmor.onClick.AddListener(delegate { ButtonClick(shoulderArmor, Holder.DrawSlot.None, shoulderContent); });
        }
        public IEnumerator Update(bool isShown)
        {
            while (isShown)
            {
                UpdateValues();
                yield return null;
            }
        }
        public override void OnShow(bool show)
        {
            base.OnShow(show);
            Level.master.StartCoroutine(Update(show));
        }
        public void ButtonClick(Button button, Holder.DrawSlot drawSlot = Holder.DrawSlot.None, ContainerData.Content itemArmor = null, RagdollHand hand = null)
        {
            UpdateValues();
            item.gameObject.SetActive(false);
            weapon.gameObject.SetActive(false);
            armor.gameObject.SetActive(false);
            player.gameObject.SetActive(true);
            Item itemWeapon = null;
            if (drawSlot != Holder.DrawSlot.None)
                itemWeapon = Player.local?.creature?.equipment?.GetHolder(drawSlot)?.items.FirstOrDefault();
            else if (hand != null)
                itemWeapon = hand?.grabbedHandle?.item;
            if (button == pressedButton)
            {
                pressedButton = null;
                return;
            }
            if (itemWeapon != null && itemWeapon.data.GetModule<XenobladeWeaponModule>() is XenobladeWeaponModule weaponModule)
            {
                item.gameObject.SetActive(true);
                weapon.gameObject.SetActive(true);
                player.gameObject.SetActive(false);
                weaponAttack.text = "Attack: " + weaponModule.attackDamageRange.x + "-" + weaponModule.attackDamageRange.y;
                weaponPhysDef.text = "Physical Def: " + weaponModule.physicalDefense;
                weaponEthDef.text = "Ether Def: " + weaponModule.etherDefense;
                weaponCrit.text = "Critical Rate: +" + weaponModule.criticalRate * 100 + "%";
                weaponBlock.text = "Block Rate: +" + weaponModule.blockRate * 100 + "%";
                itemName.text = itemWeapon.data.displayName;
                itemTier.text = "Tier " + itemWeapon.data.tier;
                itemDescription.text = itemWeapon.data.description;
            }
            else if (itemArmor != null && itemArmor.itemData.GetModule<XenobladeArmorModule>() is XenobladeArmorModule armorModule)
            {
                item.gameObject.SetActive(true);
                armor.gameObject.SetActive(true);
                player.gameObject.SetActive(false);
                armorPhysDef.text = "Physical Def: " + armorModule.physicalDefense;
                armorEthDef.text = "Ether Def: " + armorModule.etherDefense;
                armorWeight.text = "Weight: " + armorModule.weight;
                itemName.text = itemArmor.itemData.displayName;
                itemTier.text = "Tier " + itemArmor.itemData.tier;
                itemDescription.text = itemArmor.itemData.description;
            }
            pressedButton = button;
        }
        public void UpdateValues()
        {
            leftHeldItem = Player.local?.handLeft?.ragdollHand?.grabbedHandle?.item;
            rightHeldItem = Player.local?.handRight?.ragdollHand?.grabbedHandle?.item;
            leftShoulderItem = Player.local?.creature?.equipment?.GetHolder(Holder.DrawSlot.BackLeft).items.FirstOrDefault();
            rightShoulderItem = Player.local?.creature?.equipment?.GetHolder(Holder.DrawSlot.BackRight).items.FirstOrDefault();
            leftHipItem = Player.local?.creature?.equipment?.GetHolder(Holder.DrawSlot.HipsLeft).items.FirstOrDefault();
            rightHipItem = Player.local?.creature?.equipment?.GetHolder(Holder.DrawSlot.HipsRight).items.FirstOrDefault();
            chestContent = Player.local?.creature?.equipment?.GetWornContent("Torso", 1);
            helmContent = Player.local?.creature?.equipment?.GetWornContent("Head", 0);
            pantsContent = Player.local?.creature?.equipment?.GetWornContent("Legs", 2);
            bootsContent = Player.local?.creature?.equipment?.GetWornContent("Feet", 0);
            leftHandContent = Player.local?.creature?.equipment?.GetWornContent("HandLeft", 0);
            rightHandContent = Player.local?.creature?.equipment?.GetWornContent("HandRight", 0);
            shoulderContent = Player.local?.creature?.equipment?.GetWornContent("Torso", 0);
            leftShoulderItem?.data?.LoadIconAsync(false, texture =>
            {
                leftShoulder.GetComponent<RawImage>().texture = texture;
            });
            if (leftShoulderItem?.data == null) leftShoulder.GetComponent<RawImage>().texture = null;
            rightShoulderItem?.data?.LoadIconAsync(false, texture =>
            {
                rightShoulder.GetComponent<RawImage>().texture = texture;
            });
            if (rightShoulderItem?.data == null) rightShoulder.GetComponent<RawImage>().texture = null;
            leftHipItem?.data?.LoadIconAsync(false, texture =>
            {
                leftHip.GetComponent<RawImage>().texture = texture;
            });
            if (leftHipItem?.data == null) leftHip.GetComponent<RawImage>().texture = null;
            rightHipItem?.data?.LoadIconAsync(false, texture =>
            {
                rightHip.GetComponent<RawImage>().texture = texture;
            });
            if (rightHipItem?.data == null) rightHip.GetComponent<RawImage>().texture = null;
            chestContent?.itemData?.LoadIconAsync(false, texture =>
            {
                chest.GetComponent<RawImage>().texture = texture;
            });
            if (chestContent?.itemData == null) chest.GetComponent<RawImage>().texture = null;
            helmContent?.itemData?.LoadIconAsync(false, texture =>
            {
                helm.GetComponent<RawImage>().texture = texture;
            });
            if (helmContent?.itemData == null) helm.GetComponent<RawImage>().texture = null;
            pantsContent?.itemData?.LoadIconAsync(false, texture =>
            {
                pants.GetComponent<RawImage>().texture = texture;
            });
            if (pantsContent?.itemData == null) pants.GetComponent<RawImage>().texture = null;
            bootsContent?.itemData?.LoadIconAsync(false, texture =>
            {
                boots.GetComponent<RawImage>().texture = texture;
            });
            if (bootsContent?.itemData == null) boots.GetComponent<RawImage>().texture = null;
            leftHandContent?.itemData?.LoadIconAsync(false, texture =>
            {
                leftHand.GetComponent<RawImage>().texture = texture;
            });
            if (leftHandContent?.itemData == null) leftHand.GetComponent<RawImage>().texture = null;
            rightHandContent?.itemData?.LoadIconAsync(false, texture =>
            {
                rightHand.GetComponent<RawImage>().texture = texture;
            });
            if (rightHandContent?.itemData == null) rightHand.GetComponent<RawImage>().texture = null;
            leftHeldItem?.data?.LoadIconAsync(false, texture =>
            {
                leftHeld.GetComponent<RawImage>().texture = texture;
            });
            if (leftHeldItem?.data == null) leftHeld.GetComponent<RawImage>().texture = null;
            rightHeldItem?.data?.LoadIconAsync(false, texture =>
            {
                rightHeld.GetComponent<RawImage>().texture = texture;
            });
            if (rightHeldItem?.data == null) rightHeld.GetComponent<RawImage>().texture = null;
            shoulderContent?.itemData?.LoadIconAsync(false, texture =>
            {
                shoulderArmor.GetComponent<RawImage>().texture = texture;
            });
            if (shoulderContent?.itemData == null) shoulderArmor.GetComponent<RawImage>().texture = null;
            foreach (CustomReference reference in menu.customReferences)
            {
                if (reference?.transform?.gameObject?.GetComponent<RawImage>() is RawImage image)
                {
                    image.gameObject.SetActive(image.texture != null);
                    if(reference.transform.gameObject.GetComponent<Button>() is Button button && pressedButton == button && image.texture == null)
                    {
                        item.gameObject.SetActive(false);
                        weapon.gameObject.SetActive(false);
                        armor.gameObject.SetActive(false);
                        player.gameObject.SetActive(true);
                        pressedButton = null;
                    }
                }
            }
            playerLevel.text = "LV " + XenobladeManager.GetLevel();
            playerXP.text = "EXP " + XenobladeManager.GetCurrentXP();
            playerHealth.text = Player.local.creature.currentHealth + "/" + Player.local.creature.maxHealth;
            if (rightHeldItem?.data?.GetModule<XenobladeWeaponModule>() is XenobladeWeaponModule moduleRight && leftHeldItem?.data?.GetModule<XenobladeWeaponModule>() is XenobladeWeaponModule moduleLeft)
                playerAttack.text = ((moduleRight.attackDamageRange.x <= moduleLeft.attackDamageRange.x ? moduleRight.attackDamageRange.x : moduleLeft.attackDamageRange.x) + XenobladeManager.GetStrength()) + "-" +
                        ((moduleRight.attackDamageRange.y >= moduleLeft.attackDamageRange.y ? moduleRight.attackDamageRange.y : moduleLeft.attackDamageRange.y) + XenobladeManager.GetStrength());
            else if (rightHeldItem?.data?.GetModule<XenobladeWeaponModule>() is XenobladeWeaponModule moduleRight2)
                playerAttack.text = (moduleRight2.attackDamageRange.x + XenobladeManager.GetStrength()) + "-" + (moduleRight2.attackDamageRange.y + XenobladeManager.GetStrength());
            else if (leftHeldItem?.data?.GetModule<XenobladeWeaponModule>() is XenobladeWeaponModule moduleLeft2)
                playerAttack.text = (moduleLeft2.attackDamageRange.x + XenobladeManager.GetStrength()) + "-" + (moduleLeft2.attackDamageRange.y + XenobladeManager.GetStrength());
            else playerAttack.text = XenobladeManager.GetStrength() + "-" + XenobladeManager.GetStrength();
            playerStrength.text = XenobladeManager.GetStrength().ToString();
            playerEther.text = XenobladeManager.GetEther().ToString();
            playerPhysDef.text = XenobladeManager.GetPhysicalDefense().ToString();
            playerEthDef.text = XenobladeManager.GetEtherDefense().ToString();
            playerAgility.text = XenobladeManager.GetAgility().ToString();
            playerCrit.text = XenobladeManager.GetCriticalRate() * 100 + "%";
            playerBlock.text = XenobladeManager.GetBlockRate() * 100 + "%";
        }
    }
}
