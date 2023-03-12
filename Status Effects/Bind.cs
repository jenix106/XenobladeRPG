using UnityEngine;
using ThunderRoad;

namespace XenobladeRPG
{
    public class Bind : MonoBehaviour
    {
        public Creature creature;
        public float duration = 10;
        public bool allowMove;
        public void Start()
        {
            creature = GetComponent<Creature>();
            creature.currentLocomotion.SetSpeedModifier(this, 0, 0, 0, 0, 0);
            allowMove = creature.currentLocomotion.allowMove;
            creature.currentLocomotion.allowMove = false;
            XenobladeEvents.InvokeOnDebuffAdded(this, creature, this);
            Destroy(this, duration);
        }
        public void OnDestroy()
        {
            creature.currentLocomotion.RemoveSpeedModifier(this);
            creature.currentLocomotion.allowMove = allowMove;
            XenobladeEvents.InvokeOnDebuffRemoved(this, creature, this);
        }
    }
}
