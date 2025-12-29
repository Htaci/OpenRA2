using System.Collections.Generic;
using UnityEngine;

namespace TriLibCore
{
    /// <summary>
    /// Manages the destruction and cleanup of TriLib-loaded assets (e.g., <see cref="Texture2D"/>,
    /// <see cref="Material"/>, <see cref="Mesh"/>) associated with a particular <see cref="GameObject"/>.
    /// This component records references to loaded assets in <see cref="Allocations"/> and
    /// destroys them when the <see cref="GameObject"/> is destroyed.
    /// </summary>
    public class AssetUnloader : MonoBehaviour
    {
        /// <summary>
        /// A list of loaded Unity assets (e.g., meshes, materials, textures) for deallocation.
        /// When this component is destroyed, each allocated asset in this list is also destroyed.
        /// </summary>
        public List<Object> Allocations;

        /// <summary>
        /// The unique identifier for this <see cref="AssetUnloader"/> instance. Setting this property
        /// automatically registers the instance within a global dictionary to track usage counts.
        /// </summary>
        /// <remarks>
        /// Each time the <c>Id</c> is set, <see cref="Register"/> is called, incrementing or initializing
        /// the usage count for that ID. If multiple <see cref="AssetUnloader"/> instances share the same ID,
        /// their references will be counted together until all instances are destroyed.
        /// </remarks>
        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                Register();
            }
        }

        /// <summary>
        /// Custom user data that can be used to store information relevant to the loading process
        /// (e.g., file paths, metadata, or other contextual objects).
        /// </summary>
        public object CustomData;

        /// <summary>
        /// Stores the unique ID for this unloader instance. 
        /// See <see cref="Id"/> for public access.
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private int _id;

        /// <summary>
        /// An integer that increments each time <see cref="GetNextId"/> is called,
        /// used to supply new, unique IDs for <see cref="AssetUnloader"/> instances.
        /// </summary>
        private static int _lastId;

        /// <summary>
        /// A dictionary that tracks how many <see cref="AssetUnloader"/> instances share the same ID.
        /// The key is the unloader ID, and the value is a usage count. 
        /// When the count goes to zero, all assets for that ID are destroyed and removed from tracking.
        /// </summary>
        private static readonly Dictionary<int, int> AssetUnloaders = new Dictionary<int, int>();

        /// <summary>
        /// Returns a fresh allocation identifier, incrementing the internal counter 
        /// to ensure uniqueness for each <see cref="AssetUnloader"/>.
        /// </summary>
        /// <returns>A new, unused allocation identifier.</returns>
        public static int GetNextId()
        {
            return _lastId++;
        }

        /// <summary>
        /// Indicates whether any <see cref="AssetUnloader"/> instances are registered globally.
        /// If <c>true</c>, at least one <see cref="AssetUnloader"/> is currently active in the scene.
        /// </summary>
        public static bool HasRegisteredUnloaders
        {
            get
            {
                return AssetUnloaders.Count > 0;
            }
        }

        /// <summary>
        /// Registers or increments the usage count for this unloader’s <see cref="Id"/>
        /// within the global <see cref="AssetUnloaders"/> dictionary.
        /// </summary>
        private void Register()
        {
            if (!AssetUnloaders.ContainsKey(_id))
            {
                AssetUnloaders[_id] = 0;
            }
            else
            {
                AssetUnloaders[_id]++;
            }
        }

        /// <summary>
        /// Automatically registers this <see cref="AssetUnloader"/> when the containing 
        /// <see cref="GameObject"/> is initialized, incrementing the usage count if needed.
        /// </summary>
        private void Start()
        {
            Register();
        }

        /// <summary>
        /// Invoked when the <see cref="GameObject"/> is destroyed. Decrements the usage count
        /// for the associated <see cref="Id"/> in <see cref="AssetUnloaders"/>. If the usage
        /// count reaches zero, all recorded assets in <see cref="Allocations"/> are destroyed
        /// before removing the ID from tracking.
        /// </summary>
        private void OnDestroy()
        {
            if (AssetUnloaders.TryGetValue(_id, out var value))
            {
                if (--value <= 0)
                {
                    // Destroy all recorded assets
                    for (var i = 0; i < Allocations.Count; i++)
                    {
                        var allocation = Allocations[i];
                        if (allocation != null)
                        {
                            Destroy(allocation);
                        }
                    }
                    AssetUnloaders.Remove(_id);
                }
                else
                {
                    AssetUnloaders[_id] = value;
                }
            }
        }
    }
}
