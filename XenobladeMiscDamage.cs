﻿using ThunderRoad;
using UnityEngine;
using TMPro;

namespace XenobladeRPG
{
    /// <summary>
    /// Controls the damage indicator text when you damage a creature with any damage type other than Physical/Ether.
    /// </summary>
    public class XenobladeMiscDamage : MonoBehaviour
    {
        public CollisionInstance instance;
        public Creature creature;
        public TMP_Text damage;
        public void Start()
        {
            damage = transform.Find("Damage").GetComponent<TMP_Text>();
            if (instance.contactPoint != Vector3.zero)
                transform.position = instance.contactPoint;
            else if (instance.damageStruct.hitRagdollPart != null)
            {
                transform.position = instance.damageStruct.hitRagdollPart.transform.position;
            }
            else
            {
                transform.position = creature.ragdoll.targetPart.transform.position;
            }
            transform.position += new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f));
            damage.text = ((int)instance.damageStruct.damage).ToString();
            Destroy(gameObject, 1f);
        }
        public void Update()
        {
            if (Spectator.local?.cam != null && Spectator.local.state != Spectator.State.Disabled)
                transform.rotation = Quaternion.LookRotation(-(Spectator.local.cam.transform.position - transform.position).normalized);
            else if (Player.local?.head?.cam != null)
                transform.rotation = Quaternion.LookRotation(-(Player.local.head.cam.transform.position - transform.position).normalized);
            transform.position += (Vector3.up * 0.5f) * Time.deltaTime;
            Color tempDmg = damage.color;
            tempDmg.a -= Time.deltaTime;
            damage.color = tempDmg;
        }
    }
}
