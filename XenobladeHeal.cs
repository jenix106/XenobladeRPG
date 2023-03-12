using ThunderRoad;
using UnityEngine;
using TMPro;

namespace XenobladeRPG
{
    /// <summary>
    /// Controls the heal indicator text when you heal a creature.
    /// </summary>
    public class XenobladeHeal : MonoBehaviour
    {
        public Creature creature;
        public TMP_Text heal;
        public float amount;
        public void Start()
        {
            heal = transform.Find("Heal").GetComponent<TMP_Text>();
            transform.position = creature.ragdoll.targetPart.transform.position;
            transform.position += new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f));
            heal.text = amount.ToString();
            Destroy(gameObject, 1f);
        }
        public void Update()
        {
            if (Spectator.local?.cam != null && Spectator.local.state != Spectator.State.Disabled)
                transform.rotation = Quaternion.LookRotation(-(Spectator.local.cam.transform.position - transform.position).normalized);
            else if (Player.local?.head?.cam != null)
                transform.rotation = Quaternion.LookRotation(-(Player.local.head.cam.transform.position - transform.position).normalized);
            transform.position += (Vector3.up * 0.5f) * Time.deltaTime;
            Color tempHeal = heal.color;
            tempHeal.a -= Time.deltaTime;
            heal.color = tempHeal;
        }
    }
}
