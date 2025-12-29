#pragma warning disable 649
using TriLibCore.Extensions;
using TriLibCore.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace TriLibCore.Samples
{
    /// <summary>
    /// Demonstrates how to load a 3D model using an OS file picker and apply a custom material mapping for glTF2 materials.
    /// This sample creates the <see cref="AssetLoaderOptions"/> if necessary, configures them using 
    /// <see cref="GltfMaterialsHelper"/>, and then launches the file picker via 
    /// <see cref="AssetLoaderFilePicker"/>.
    /// </summary>
    public class GltfMaterialsSample : MonoBehaviour
    {
        /// <summary>
        /// The most recently loaded model GameObject.
        /// </summary>
        private GameObject _loadedGameObject;

        /// <summary>
        /// Button used to trigger the model loading process.
        /// </summary>
        [SerializeField]
        private Button _loadModelButton;

        /// <summary>
        /// UI text element used to display the loading progress.
        /// </summary>
        [SerializeField]
        private Text _progressText;

        /// <summary>
        /// Cached instance of <see cref="AssetLoaderOptions"/> used to configure the model loading process.
        /// </summary>
        private AssetLoaderOptions _assetLoaderOptions;

        /// <summary>
        /// Creates the <see cref="AssetLoaderOptions"/> if necessary, configures them to use the glTF2 material mapper,
        /// and then opens the OS file-picker for the user to select a model file.
        /// </summary>
        /// <remarks>
        /// You can create custom AssetLoaderOptions by right-clicking in the Assets Explorer and selecting 
        /// "TriLib->Create->AssetLoaderOptions->Pre-Built AssetLoaderOptions".
        /// </remarks>
        public void LoadModel()
        {
            if (_assetLoaderOptions == null)
            {
                _assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(false, true);
            }
            // Configure the options to use the glTF2 material mapper.
            GltfMaterialsHelper.SetupStatic(ref _assetLoaderOptions);

            // Create a file picker loader and open the file selection dialog.
            var assetLoaderFilePicker = AssetLoaderFilePicker.Create();
            assetLoaderFilePicker.LoadModelFromFilePickerAsync(
                "Select a Model file",
                OnLoad,
                OnMaterialsLoad,
                OnProgress,
                OnBeginLoad,
                OnError,
                null,
                _assetLoaderOptions);
        }

        /// <summary>
        /// Called when the model loading process begins.
        /// This callback updates the UI to reflect that a file selection has been made and loading has started.
        /// </summary>
        /// <param name="filesSelected">
        /// <c>true</c> if a file has been selected; otherwise, <c>false</c>.
        /// </param>
        private void OnBeginLoad(bool filesSelected)
        {
            _loadModelButton.interactable = !filesSelected;
            _progressText.enabled = filesSelected;
        }

        /// <summary>
        /// Callback invoked when an error occurs during model loading.
        /// Logs the error to the console.
        /// </summary>
        /// <param name="obj">
        /// The contextualized error, which contains details about the original exception and loading context.
        /// </param>
        private void OnError(IContextualizedError obj)
        {
            Debug.LogError($"An error occurred while loading your model: {obj.GetInnerException()}");
        }

        /// <summary>
        /// Callback invoked to update the UI with the current loading progress.
        /// </summary>
        /// <param name="assetLoaderContext">
        /// The context used to load the model, which contains progress information.
        /// </param>
        /// <param name="progress">
        /// A float value between 0 and 1 representing the current loading progress.
        /// </param>
        private void OnProgress(AssetLoaderContext assetLoaderContext, float progress)
        {
            _progressText.text = $"Progress: {progress:P}";
        }

        /// <summary>
        /// Callback invoked when the model, including its textures and materials, has been fully loaded.
        /// </summary>
        /// <remarks>
        /// The fully loaded model GameObject is available via the <see cref="AssetLoaderContext.RootGameObject"/> property.
        /// </remarks>
        /// <param name="assetLoaderContext">
        /// The context used to load the model.
        /// </param>
        private void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
        {
            if (assetLoaderContext.RootGameObject != null)
            {
                Debug.Log("Model fully loaded.");
            }
            else
            {
                Debug.Log("Model could not be loaded.");
            }
            _loadModelButton.interactable = true;
            _progressText.enabled = false;
        }

        /// <summary>
        /// Callback invoked when the model's meshes and hierarchy have been loaded.
        /// This callback updates the sample by replacing any previously loaded model and adjusting
        /// the main camera to frame the new model.
        /// </summary>
        /// <remarks>
        /// The loaded model is stored in <see cref="_loadedGameObject"/> and is accessible via 
        /// the <see cref="AssetLoaderContext.RootGameObject"/> property.
        /// </remarks>
        /// <param name="assetLoaderContext">
        /// The context that holds the loaded model data.
        /// </param>
        private void OnLoad(AssetLoaderContext assetLoaderContext)
        {
            if (_loadedGameObject != null)
            {
                Destroy(_loadedGameObject);
            }
            _loadedGameObject = assetLoaderContext.RootGameObject;
            if (_loadedGameObject != null && Camera.main != null)
            {
                // Adjust the main camera to fit the bounds of the loaded model.
                Camera.main.FitToBounds(assetLoaderContext.RootGameObject, 2f);
            }
        }
    }
}
