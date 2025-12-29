using System.Collections;
using UnityEngine;

namespace TriLibCore
{
    /// <summary>
    /// Provides a singleton <see cref="GameObject"/> and <see cref="MonoBehaviour"/> in the scene 
    /// specifically for running coroutines. This helper is particularly useful for static classes 
    /// or non-<see cref="MonoBehaviour"/> objects that require coroutine-driven logic without 
    /// needing to create their own <see cref="GameObject"/>.
    /// </summary>
    public class CoroutineHelper : MonoBehaviour
    {
        private static CoroutineHelper _instance;

        /// <summary>
        /// Retrieves the singleton instance of the <see cref="CoroutineHelper"/>. 
        /// If no instance exists, one is automatically created in the scene with 
        /// the name "Coroutine Helper".
        /// </summary>
        /// <remarks>
        /// The created <see cref="GameObject"/> is marked with <see cref="HideFlags.DontSave"/>, 
        /// which generally prevents it from persisting in serialized scenes.
        /// </remarks>
        public static CoroutineHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("Coroutine Helper").AddComponent<CoroutineHelper>();
                    _instance.hideFlags = HideFlags.DontSave;
                }
                return _instance;
            }
        }

        /// <summary>
        /// Synchronously executes the provided <see cref="IEnumerator"/> until it completes. 
        /// Unlike <see cref="MonoBehaviour.StartCoroutine"/>, this method does not run 
        /// asynchronously; it blocks until the iterator finishes.
        /// </summary>
        /// <param name="enumerator">The <see cref="IEnumerator"/> to execute step by step.</param>
        /// <remarks>
        /// Because this method runs synchronously, it can be used in cases where you need 
        /// to process an iterator without yielding control back to Unity’s main loop.
        /// Note that any calls to <c>yield return</c> in the enumerator won’t cause 
        /// asynchronous delays.
        /// </remarks>
        public static void RunMethod(IEnumerator enumerator)
        {
            while (enumerator.MoveNext())
            {
                // Empty loop body, as the enumerator steps through until completion.
            }
        }

        /// <summary>
        /// When the <see cref="GameObject"/> hosting this <see cref="CoroutineHelper"/> is destroyed,
        /// the singleton reference is reset. Future calls to <see cref="Instance"/> 
        /// will recreate the helper as needed.
        /// </summary>
        private void OnDestroy()
        {
            _instance = null;
        }
    }
}
