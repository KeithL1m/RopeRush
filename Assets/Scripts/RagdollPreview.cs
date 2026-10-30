using UnityEngine;
using UnityEngine.InputSystem;

namespace SyncRush
{
    /// <summary>
    /// TEMP preview for the ragdoll look (CharacterTest scene only, delete with that scene). After a short delay
    /// one Y Bot goes limp where it stands and another is launched like a pendulum hit. Space replays.
    /// </summary>
    public class RagdollPreview : MonoBehaviour
    {
        [SerializeField] private RagdollRig _limp;
        [SerializeField] private RagdollRig _launched;

        [Tooltip("Velocity given to the launched one. Default = the current pendulum knockback (20 sideways + 7 up).")]
        [SerializeField] private Vector3 _launchVelocity = new(20f, 7f, 0f);

        [SerializeField] private float _delay = 1f;

        private float _timer;
        private bool _fired;

        private void Start() => Replay();

        private void Replay()
        {
            _limp.SetRagdolled(false);
            _launched.SetRagdolled(false);
            _timer = _delay;
            _fired = false;
        }

        private void Update()
        {
            if (Keyboard.current?.spaceKey.wasPressedThisFrame == true) Replay();
            if (_fired || (_timer -= Time.deltaTime) > 0f) return;

            _fired = true;
            _limp.SetRagdolled(true);
            _launched.SetRagdolled(true);
            _launched.Launch(_launchVelocity);
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 700, 25), "RAGDOLL PREVIEW: left goes limp, right is launched like a pendulum hit. SPACE = replay");
            GUI.Label(new Rect(10, 30, 700, 25), "Tune on 'Ragdoll Preview - Launched' > RagdollRig > Launch comedy (Play Mode edits reset on stop: note the values)");
        }
    }
}
