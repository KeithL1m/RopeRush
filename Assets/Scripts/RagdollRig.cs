using UnityEngine;

namespace SyncRush
{
    /// <summary>
    /// Switches a humanoid model between animated and ragdoll (GDD §5.2: kinematic until a big hit, then a
    /// short ragdoll). Sits on the model next to its Animator; the bones' Rigidbodies, Colliders and
    /// CharacterJoints come from Assets/Editor/BuildPlayerRagdoll.cs ("Sync Rush/Rebuild Player Ragdoll").
    ///
    /// Purely local physics: deciding *when* to ragdoll and networking it is the caller's job.
    /// While animated, every bone body is kinematic with its collider off, so the ragdoll costs nothing
    /// and can't touch anything.
    /// </summary>
    public class RagdollRig : MonoBehaviour
    {
        [Tooltip("Gravity the ragdoll falls with, in m/s². Matches SyncRushPlayerController's 20; Unity's own " +
                 "9.81 made a 5 m ragdoll fall in slow motion. The difference is added as extra force.")]
        [SerializeField] private float _gravity = 20f;

        [Header("Launch comedy (Launch only; SetVelocity is the plain version)")]
        [Tooltip("Hit velocity multiplier for a ragdoll launch. Above 1 = ragdolled hits fly further than plain knockback.")]
        [SerializeField] private float _launchBoost = 1.25f;

        [Tooltip("Extra upward speed (m/s) on a ragdoll launch, so it pops up before tumbling away.")]
        [SerializeField] private float _extraLift = 4f;

        [Tooltip("Head-over-heels spin of the whole body in the hit direction, in radians/s (6.3 = one flip per second).")]
        [SerializeField] private float _tumble = 12f;

        [Tooltip("Random extra speed (m/s) per limb, so arms and legs flail instead of flying as one piece.")]
        [SerializeField] private float _limbFlail = 8f;

        [Tooltip("Random spin per limb, in radians/s.")]
        [SerializeField] private float _limbSpin = 15f;

        // Unity caps spin at 7 rad/s by default, which silently mutes the tumble and flail.
        private const float MaxAngularVelocity = 50f;

        private Animator _animator;
        private Rigidbody[] _bodies;
        private Collider[] _colliders;

        public bool IsRagdolled { get; private set; }

        /// <summary>The ragdoll's root body (hips): follow this while ragdolled.</summary>
        public Rigidbody Hips { get; private set; }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _bodies = GetComponentsInChildren<Rigidbody>(true);
            var colliders = new System.Collections.Generic.List<Collider>();
            foreach (var body in _bodies)
            {
                body.maxAngularVelocity = MaxAngularVelocity;
                colliders.AddRange(body.GetComponents<Collider>());
                // The root body is the one without a joint (the builder joints every other bone to a parent).
                if (body.GetComponent<CharacterJoint>() == null) Hips = body;
            }
            _colliders = colliders.ToArray();
            SetRagdolled(false);
        }

        /// <summary>Ragdoll on: animation stops and the bones simulate. Off: back to kinematic + animated.</summary>
        public void SetRagdolled(bool ragdolled)
        {
            IsRagdolled = ragdolled;
            foreach (var body in _bodies)
            {
                if (!ragdolled && !body.isKinematic)
                {
                    body.linearVelocity = Vector3.zero;
                    body.angularVelocity = Vector3.zero;
                }
                body.isKinematic = !ragdolled;
            }
            foreach (var c in _colliders) c.enabled = ragdolled;
            if (_animator != null) _animator.enabled = !ragdolled;
        }

        /// <summary>Sets every bone moving at <paramref name="velocity"/>, so the whole body flies with a hit.</summary>
        public void SetVelocity(Vector3 velocity)
        {
            foreach (var body in _bodies)
                if (!body.isKinematic) body.linearVelocity = velocity;
        }

        /// <summary>
        /// Ragdoll launch from a hit: the hit velocity (boosted, with an extra pop up), a head-over-heels tumble
        /// in the hit direction, and random flailing per limb. The randomness is cosmetic; each peer may differ.
        /// </summary>
        public void Launch(Vector3 hitVelocity)
        {
            Vector3 velocity = hitVelocity * _launchBoost + Vector3.up * _extraLift;
            Vector3 flat = Vector3.ProjectOnPlane(hitVelocity, Vector3.up);
            // Spin axis: sideways to the hit, so the body flips forward over the direction it's flying.
            Vector3 tumbleAxis = flat.sqrMagnitude > 0.01f ? Vector3.Cross(Vector3.up, flat.normalized) : Random.onUnitSphere;

            foreach (var body in _bodies)
            {
                if (body.isKinematic) continue;
                body.linearVelocity = velocity;
                body.angularVelocity = tumbleAxis * _tumble;
                if (body == Hips) continue; // limbs only: the hips carry the body's flight path
                body.linearVelocity += Random.insideUnitSphere * _limbFlail;
                body.angularVelocity += Random.insideUnitSphere * _limbSpin;
            }
        }

        private void FixedUpdate()
        {
            if (!IsRagdolled) return;
            float extra = _gravity + Physics.gravity.y; // Physics.gravity.y is negative
            if (extra <= 0f) return;
            foreach (var body in _bodies)
                body.AddForce(Vector3.down * extra, ForceMode.Acceleration);
        }
    }
}
