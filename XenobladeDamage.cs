using ThunderRoad;
using UnityEngine;
using TMPro;

namespace XenobladeRPG
{
    /// <summary>
    /// Controls the damage indicator text when you damage a creature with Physical/Ether damage.
    /// </summary>
    public class XenobladeDamage : MonoBehaviour
    {
        public CollisionInstance instance;
        public Creature creature;
        public TMP_Text critical;
        public TMP_Text damageResist;
        public bool isResisted = false;
        public bool isCritical = false;
        public bool isBlocked = false;
        public bool isDefended = false;
        public bool isMissed = false;
        public void Start()
        {
            critical = transform.Find("Critical").GetComponent<TMP_Text>();
            critical.gameObject.SetActive(isCritical && !isMissed);
            damageResist = transform.Find("DamageResist").GetComponent<TMP_Text>();
            if (isResisted || isMissed) isDefended = false;
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
            if (isDefended)
                damageResist.text += ((int)instance.damageStruct.damage).ToString() + (isBlocked ? " Block" : "");
            else if (!isMissed) damageResist.text = isResisted ? "Resist" : ((int)instance.damageStruct.damage).ToString() + (isBlocked ? " Block" : "");
            else damageResist.text = "Miss";
            Destroy(gameObject, 1f);
        }
        public void Update()
        {
            if (Spectator.local?.cam != null && Spectator.local.state != Spectator.State.Disabled)
                transform.rotation = Quaternion.LookRotation(-(Spectator.local.cam.transform.position - transform.position).normalized);
            else if (Player.local?.head?.cam != null)
                transform.rotation = Quaternion.LookRotation(-(Player.local.head.cam.transform.position - transform.position).normalized);
            transform.position += (Vector3.up * 0.5f) * Time.deltaTime;
            Color tempCrit = critical.color;
            tempCrit.a -= Time.deltaTime * (50 / 255);
            critical.color = tempCrit;
            Color tempDmg = damageResist.color;
            tempDmg.a -= Time.deltaTime;
            damageResist.color = tempDmg;
        }
    }
}
