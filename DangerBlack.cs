using System.Collections;
using ThunderRoad;
using UnityEngine;

namespace XenobladeRPG
{
    public class DangerBlack : MonoBehaviour
    {
        Creature creature;
        BrainModuleMelee melee;
        BrainModuleDodge dodge;
        BrainModuleEquipment equipment;
        BrainModuleHitReaction hitReaction;
        BrainModuleDefense defense;
        BrainModuleBow bow;
        BrainModuleCast cast;
        public void Start()
        {
            creature = GetComponent<Creature>();
            //creature.GetComponent<XenobladeStats>().baseAttackSpeedMultiplier = 0.5f;
            melee = creature.brain.instance.GetModule<BrainModuleMelee>(false);
            dodge = creature.brain.instance.GetModule<BrainModuleDodge>(false);
            equipment = creature.brain.instance.GetModule<BrainModuleEquipment>(false);
            hitReaction = creature.brain.instance.GetModule<BrainModuleHitReaction>(false);
            defense = creature.brain.instance.GetModule<BrainModuleDefense>(false);
            bow = creature.brain.instance.GetModule<BrainModuleBow>(false);
            cast = creature.brain.instance.GetModule<BrainModuleCast>(false);
            StartCoroutine(Apply());
        }
        public IEnumerator Apply()
        {
            while (!creature.loaded) yield return null;
            if (melee != null && melee.meleeEnabled)
            {
                melee.animationSpeedMultiplier = 0.5f;
                melee.armSpringMultiplier = 1f;
                melee.armMaxForceMultiplier = 5f;
                melee.meleeMax = 1;
                melee.minMaxTimeBetweenAttack.x = 3f;
                melee.minMaxTimeBetweenAttack.y = 4f;
            }
            if (dodge != null && dodge.enabled)
            {
                dodge.dodgeChance = 0f;
                dodge.dodgeSpeed = 0f;
                dodge.dodgeWhenGrabbed = false;
                dodge.dodgeWhenWeaponGrabbed = false;
            }
            if (equipment != null)
            {
                equipment.allowArmGrabDisarm = true;
                equipment.allowArmStabDisarm = true;
                equipment.grabDisarmPushLevel = 0;
                equipment.handHitDisarmPushLevel = 0;
            }
            if (hitReaction != null)
            {
                hitReaction.recoverySpeed = 0.5f;
                hitReaction.parryRecoilCooldown = 3f;
                foreach (BrainModuleHitReaction.PushBehaviour push in hitReaction.pushMagicBehaviors)
                {
                    push.duringAttack = BrainModuleHitReaction.PushBehaviour.Effect.StaggerFull;
                    push.duringAttackJump = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringCombat = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringIdle = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringStagger = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringStaggerFull = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                }
                foreach (BrainModuleHitReaction.PushBehaviour push in hitReaction.pushHitBehaviors)
                {
                    push.duringAttack = BrainModuleHitReaction.PushBehaviour.Effect.StaggerFull;
                    push.duringAttackJump = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringCombat = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringIdle = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringStagger = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringStaggerFull = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                }
                foreach (BrainModuleHitReaction.PushBehaviour push in hitReaction.pushGrabThrowBehaviors)
                {
                    push.duringAttack = BrainModuleHitReaction.PushBehaviour.Effect.StaggerFull;
                    push.duringAttackJump = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringCombat = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringIdle = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringStagger = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                    push.duringStaggerFull = BrainModuleHitReaction.PushBehaviour.Effect.Destabilize;
                }
            }
            if (defense != null && defense.enabled)
            {
                defense.armSpringMultiplier = 0.5f;
                defense.armMaxForceMultiplier = 5f;
            }
            if (bow != null)
            {
                bow.arrowDrawDelay = 1f;
                bow.arrowNockDelay = 2f;
                bow.bowDrawDelay = 2f;
                bow.aimMoveSpeed = 5;
                bow.turnSpeed = 0.5f;
                bow.minMaxTimeToAttackFromAim.y = 6;
                bow.minMaxTimeBetweenAttack = new Vector2(6, 12);
            }
            if (cast != null)
            {
                cast.castMinMaxDelay = new Vector2(6, 7);
                cast.chargeDurationMultiplier = 1.5f;
                cast.spreadCone = new Vector2(0.9f, 0.9f);
            }
            yield break;
        }
    }
}
