using System;
using System.Linq;
using System.Reflection;
using SyncRush;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Builds the ragdoll (GDD §5.2) on a humanoid model with Unity's own Ragdoll Wizard (UnityEditor.RagdollBuilder,
/// called by reflection since it's internal), then switches it all off: kinematic bodies, disabled colliders,
/// on the PlayerRagdoll layer, plus a RagdollRig to turn it on at runtime. Safe to re-run: the old ragdoll is
/// removed first. Menu: Sync Rush/Rebuild Player Ragdoll (rebuilds PlayerPrefab's Y Bot).
/// </summary>
public static class BuildPlayerRagdoll
{
    public const string PlayerPrefabPath = "Assets/Prefabs/PlayerPrefab.prefab";
    public const string RagdollLayerName = "PlayerRagdoll";

    // Total ragdoll mass. Only affects how the body reacts to its own joints (hazards are kinematic).
    private const float TotalMass = 20f;

    [MenuItem("Sync Rush/Rebuild Player Ragdoll")]
    public static void RebuildPlayerPrefab()
    {
        var root = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);
        try
        {
            var animator = root.transform.Find("Model/YBot").GetComponent<Animator>();
            Debug.Log("[BuildPlayerRagdoll] " + Build(animator));
            PrefabUtility.SaveAsPrefabAsset(root, PlayerPrefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    /// <summary>Builds (or rebuilds) the ragdoll on a humanoid Animator's skeleton. Returns a summary.</summary>
    public static string Build(Animator animator)
    {
        int layer = EnsureLayer(RagdollLayerName);
        Transform hips = animator.GetBoneTransform(HumanBodyBones.Hips);
        RemoveRagdoll(hips);

        var builderType = AppDomain.CurrentDomain.GetAssemblies()
            .Select(a => a.GetType("UnityEditor.RagdollBuilder")).FirstOrDefault(t => t != null)
            ?? throw new Exception("UnityEditor.RagdollBuilder not found (Unity version changed?)");
        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        object builder = Activator.CreateInstance(builderType, true);
        builderType.GetField("animator", flags).SetValue(builder, animator);
        builderType.GetMethod("AutoAssignTransforms", flags).Invoke(builder, null);
        builderType.GetField("totalMass", flags).SetValue(builder, TotalMass);
        string problem = (string)builderType.GetMethod("CheckConsistency", flags).Invoke(builder, null);
        if (!string.IsNullOrEmpty(problem)) throw new Exception("Ragdoll Wizard: " + problem);
        builderType.GetMethod("CalculateAxes", flags).Invoke(builder, null);
        builderType.GetMethod("OnWizardCreate", flags).Invoke(builder, null);

        // Everything off until RagdollRig turns it on.
        var bodies = hips.GetComponentsInChildren<Rigidbody>(true);
        foreach (var body in bodies)
        {
            body.gameObject.layer = layer;
            body.isKinematic = true;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            // Speculative works for kinematic and dynamic bodies; fast hits on a 5 m body tunnel otherwise.
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            body.solverIterations = 12;          // default 6: big bodies + joints jitter at the default
            body.solverVelocityIterations = 4;
            foreach (var c in body.GetComponents<Collider>()) c.enabled = false;
        }

        if (animator.GetComponent<RagdollRig>() == null) animator.gameObject.AddComponent<RagdollRig>();
        return $"ragdoll on '{animator.name}': {bodies.Length} bodies, {hips.GetComponentsInChildren<CharacterJoint>(true).Length} joints, layer {layer}";
    }

    private static void RemoveRagdoll(Transform hips)
    {
        foreach (var j in hips.GetComponentsInChildren<CharacterJoint>(true)) UnityEngine.Object.DestroyImmediate(j);
        foreach (var c in hips.GetComponentsInChildren<Collider>(true)) UnityEngine.Object.DestroyImmediate(c);
        foreach (var b in hips.GetComponentsInChildren<Rigidbody>(true)) UnityEngine.Object.DestroyImmediate(b);
    }

    /// <summary>Returns the layer's index, creating it in the first free user slot if needed.</summary>
    private static int EnsureLayer(string name)
    {
        int existing = LayerMask.NameToLayer(name);
        if (existing >= 0) return existing;

        var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        var layers = tagManager.FindProperty("layers");
        for (int i = 8; i < layers.arraySize; i++)
        {
            var slot = layers.GetArrayElementAtIndex(i);
            if (!string.IsNullOrEmpty(slot.stringValue)) continue;
            slot.stringValue = name;
            tagManager.ApplyModifiedPropertiesWithoutUndo();
            return i;
        }
        throw new Exception("No free layer slot for " + name);
    }
}
