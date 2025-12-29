#pragma warning disable 649
using TriLibCore.Extensions;
using TriLibCore.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace TriLibCore.Samples
{
    /// <summary>
    /// Demonstrates how to load a 3D model using an OS file picker and apply a custom material mapping for Autodesk Interactive materials.
    /// This sample creates the <see cref="AssetLoaderOptions"/> if necessary, configures them using 
    /// <see cref="AutodeskInteractiveMaterialsHelper"/>, and then launches the file picker via 
    /// <see cref="AssetLoaderFilePicker"/>.
    /// </summary>
    public class AutodeskInteractiveMaterialsSample : MonoBehaviour
    {
        /// <summary>
        /// Holds a reference to the last loaded model GameObject.
        /// </summary>
        private GameObject _loadedGameObject;

        /// <summary>
        /// Button that triggers the model load action.
        /// </summary>
        [SerializeField]
        private Button _loadModelButton;

        /// <summary>
        /// Text element used to indicate the current loading progress.
        /// </summary>
        [SerializeField]
        private Text _progressText;

        /// <summary>
        /// Cached <see cref="AssetLoaderOptions"/> instance used for configuring the asset loader.
        /// </summary>
        private AssetLoaderOptions _assetLoaderOptions;

        /// <summary>
        /// Creates the <see cref="AssetLoaderOptions"/> (if not already available) and displays the OS file-picker 
        /// to select a model file for loading. The method also configures the asset loader to use the Autodesk Interactive 
        /// material mapper.
        /// </summary>
        /// <remarks>
        /// You can create custom <see cref="AssetLoaderOptions"/> by right-clicking in the Assets window and selecting 
        /// "TriLib->Create->AssetLoaderOptions->Pre-Built AssetLoaderOptions".
        /// </remarks>
        public void LoadModel()
        {
            if (_assetLoaderOptions == null)
            {
                _assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(false, true);
            }
            // Configure the AssetLoaderOptions to use the Autodesk Interactive material mapper.
            AutodeskInteractiveMaterialsHelper.SetupStatic(ref _assetLoaderOptions);

            // Create an instance of the file picker asset loader and show the file dialog.
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
        /// Callback executed when the model loading begins.
        /// </summary>
        /// <param name="filesSelected">
        /// A boolean indicating whether any file was selected.
        /// </param>
        private void OnBeginLoad(bool filesSelected)
        {
            _loadModelButton.interactable = !filesSelected;
            _progressText.enabled = filesSelected;
        }

        /// <summary>
        /// Callback executed if an error occurs during the model loading process.
        /// </summary>
        /// <param name="obj">
        /// A contextualized error object containing the original exception and additional context.
        /// </param>
        private void OnError(IContextualizedError obj)
        {
            Debug.LogError($"An error occurred while loading your model: {obj.GetInnerException()}");
        }

        /// <summary>
        /// Callback executed to update the loading progress UI.
        /// </summary>
        /// <param name="assetLoaderContext">
        /// The context used to load the model, containing progress information.
        /// </param>
        /// <param name="progress">
        /// A float (from 0 to 1) indicating the current progress percentage.
        /// </param>
        private void OnProgress(AssetLoaderContext assetLoaderContext, float progress)
        {
            _progressText.text = $"Progress: {progress:P}";
        }

        /// <summary>
        /// Callback executed when the model and its associated resources (e.g., textures, materials) 
        /// have been fully loaded.
        /// </summary>
        /// <remarks>
        /// The fully loaded GameObject is referenced by <see cref="AssetLoaderContext.RootGameObject"/>.
        /// </remarks>
        /// <param name="assetLoaderContext">
        /// The context used for the model loading process.
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
        /// Callback executed when the model's meshes and hierarchy have been loaded.
        /// </summary>
        /// <remarks>
        /// The loaded GameObject is available in <see cref="AssetLoaderContext.RootGameObject"/>.
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
