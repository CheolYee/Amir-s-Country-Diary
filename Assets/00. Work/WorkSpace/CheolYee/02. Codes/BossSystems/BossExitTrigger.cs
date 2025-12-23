using _00._Work.Resources._02._Codes;
using _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems;
using _00._Work.WorkSpace.CheolYee._02._Codes.FSMSystem;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.BossSystems
{
    public class BossExitTrigger : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            var boss = other.GetComponentInParent<Boss>();
            if (boss == null) return;

            if (boss.CurrentState == BossStates.EXIT)
            {
                SoundManager.Instance?.PlayBgm(BgmId.Normal);
                Bus<CameraGlobalSetPresenceEvent>.Raise(new CameraGlobalSetPresenceEvent(true, false));
                Destroy(boss.gameObject);
            }
        }
    }
}