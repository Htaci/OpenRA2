#pragma warning disable 168, 618
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TriLibCore.Mappers;
using TriLibCore.Utils;
using UnityEngine;
using FileMode = System.IO.FileMode;
using HumanDescription = UnityEngine.HumanDescription;
using Object = UnityEngine.Object;
using System.Collections;
using System.Runtime;
using TriLibCore.Extensions;
using TriLibCore.General;
using TriLibCore.Interfaces;
using TriLibCore.Materials;
using TriLibCore.Textures;
#if TRILIB_DRACO
using TriLibCore.Gltf.Reader;
using TriLibCore.Gltf.Draco;
using TriLibCore.Collections;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace TriLibCore
{
    /// <summary>
    /// Provides static methods for loading 3D models and related assets into Unity. This class includes
    /// functionality for reading various file types or streams, creating mesh and material data, setting
    /// up animations, configuring LODs, and more.
    /// </summary>
    public static partial class AssetLoader
    {
        /// <summary>
        /// The namespace used internally by TriLib Mappers.
        /// </summary>
        private const string TriLibMappersNamespace = "TriLibCore.Mappers";

        /// <summary>
        /// The default message displayed when validating Asset Loader Options.
        /// You can disable these validations in the 'Edit -> Project Settings -> TriLib' menu.
        /// </summary>
        private const string ValidationMessage = "You can disable these validations in the 'Edit->Project Settings->TriLib' menu.";

#if UNITY_EDITOR
        /// <summary>
        /// Attempts to load a <see cref="ScriptableObject"/> asset of the specified <paramref name="type"/> from the Unity project.
        /// If it cannot be found, the method creates a new instance of that <see cref="ScriptableObject"/> and saves it
        /// as an asset at a generated path under the TriLib mappers folder.
        /// </summary>
        /// <param name="type">
        /// The name of the <see cref="ScriptableObject"/> class (e.g., "ByBonesRootBoneMapper"). 
        /// The file name of the asset is derived from this parameter.
        /// </param>
        /// <param name="namespace">
        /// The namespace in which the <paramref name="type"/> resides. 
        /// This is used to fully qualify the class when creating a new instance.
        /// </param>
        /// <param name="subFolder">
        /// The subfolder name under the TriLib mappers directory where the asset should be located or created.
        /// </param>
        /// <returns>
        /// A reference to the loaded or newly created <see cref="ScriptableObject"/>. 
        /// Returns <c>null</c> if the creation fails (e.g., if the type could not be found).
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown if the TriLib mappers placeholder file cannot be located, indicating 
        /// an issue with the TriLib package import.
        /// </exception>
        private static Object LoadOrCreateScriptableObject(string type, string @namespace, string subFolder)
        {
            string mappersFilePath;
            var triLibMapperAssets = AssetDatabase.FindAssets("TriLibMappersPlaceholder");
            if (triLibMapperAssets.Length > 0)
            {
                mappersFilePath = AssetDatabase.GUIDToAssetPath(triLibMapperAssets[0]);
            }
            else
            {
                throw new Exception("Could not find \"TriLibMappersPlaceholder\" file. Please re-import the TriLib package.");
            }

            var mappersDirectory = $"{FileUtils.GetFileDirectory(mappersFilePath)}";
            var assetDirectory = $"{mappersDirectory}/{subFolder}";

            // Ensure the target folder exists; create it if necessary.
            if (!AssetDatabase.IsValidFolder(assetDirectory))
            {
                AssetDatabase.CreateFolder(mappersDirectory, subFolder);
            }

            var assetPath = $"{assetDirectory}/{type}.asset";
            var scriptableObject = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));

            // If the asset doesn't exist, create a new ScriptableObject and save it.
            if (scriptableObject == null)
            {
                scriptableObject = CreateScriptableObjectSafe(type, @namespace);
                if (scriptableObject != null)
                {
                    AssetDatabase.CreateAsset(scriptableObject, assetPath);
                }
            }

            return scriptableObject;
        }
#endif

        /// <summary>
        /// Creates an <see cref="AssetLoaderOptions"/> instance with TriLib's default settings and
        /// registers mappers such as <see cref="ByBonesRootBoneMapper"/> and all valid <see cref="MaterialMapper"/> instances.
        ///
        /// If <paramref name="generateAssets"/> is true (in the Unity Editor), the scriptable objects
        /// will be created as assets; otherwise, they will be created in memory only.
        /// </summary>
        /// <param name="generateAssets">If <c>true</c>, newly created ScriptableObject instances will be saved as assets.</param>
        /// <param name="supressWarning">
        /// If <c>true</c>, suppresses the warning about creating a new <see cref="AssetLoaderOptions"/> instance.
        /// Pass <c>true</c> if you are caching the instance to avoid repeated warnings.
        /// </param>
        /// <returns>
        /// A new <see cref="AssetLoaderOptions"/> object initialized with default TriLib settings and
        /// the available mappers.
        /// </returns>
        public static AssetLoaderOptions CreateDefaultLoaderOptions(bool generateAssets = false, bool supressWarning = false)
        {
            if (!supressWarning)
            {
                Debug.LogWarning("TriLib: You are creating a new AssetLoaderOptions instance. If you are caching this instance and don't want this message to be displayed again, pass `true` to the `supressWarning` parameter of `CreateDefaultLoaderOptions` call.");
            }
            var assetLoaderOptions = ScriptableObject.CreateInstance<AssetLoaderOptions>();
            ByBonesRootBoneMapper byBonesRootBoneMapper;
#if UNITY_EDITOR
            if (generateAssets)
            {
                byBonesRootBoneMapper = (ByBonesRootBoneMapper)LoadOrCreateScriptableObject("ByBonesRootBoneMapper", TriLibMappersNamespace, "RootBone");
            }
            else
            {
                byBonesRootBoneMapper = ScriptableObject.CreateInstance<ByBonesRootBoneMapper>();
            }
#else
    byBonesRootBoneMapper = ScriptableObject.CreateInstance<ByBonesRootBoneMapper>();
#endif
            byBonesRootBoneMapper.name = "ByBonesRootBoneMapper";
            assetLoaderOptions.RootBoneMapper = byBonesRootBoneMapper;

            var materialMappers = new List<MaterialMapper>();
            for (var i = 0; i < MaterialMapper.RegisteredMappers.Count; i++)
            {
                var materialMapperName = MaterialMapper.RegisteredMappers[i];
                var materialMapperNamespace = MaterialMapper.RegisteredMapperNamespaces[i];
                if (materialMapperName == null)
                {
                    continue;
                }

                MaterialMapper materialMapper;
                try
                {
#if UNITY_EDITOR
                    if (generateAssets)
                    {
                        materialMapper = LoadOrCreateScriptableObject(materialMapperName, materialMapperNamespace, "Material") as MaterialMapper;
                    }
                    else
                    {
                        materialMapper = LoadOrCreateScriptableObjectSafe(materialMapperName, materialMapperNamespace, "Mappers/Material", true) as MaterialMapper;
                    }
#else
            materialMapper = LoadOrCreateScriptableObjectSafe(materialMapperName, materialMapperNamespace, "Mappers/Material", true) as MaterialMapper;
#endif
                }
                catch
                {
                    materialMapper = null;
                }

                if (materialMapper is object)
                {
                    materialMapper.name = materialMapperName;
                    if (materialMapper.IsCompatible(null))
                    {
                        materialMappers.Add(materialMapper);
                    }
                    else
                    {
#if UNITY_EDITOR
                        var assetPath = AssetDatabase.GetAssetPath(materialMapper);
                        if (assetPath == null)
                        {
                            Object.DestroyImmediate(materialMapper);
                        }
#else
                Object.Destroy(materialMapper);
#endif
                    }
                }
            }

            if (materialMappers.Count == 0)
            {
                Debug.LogWarning("TriLib could not find any suitable MaterialMapper in the project.");
            }
            else
            {
                assetLoaderOptions.MaterialMappers = materialMappers.ToArray();
            }
            return assetLoaderOptions;
        }

        /// <summary>
        /// Determines and returns the name of a compatible <see cref="MaterialMapper"/> implementation
        /// based on the currently active render pipeline (HDRP, URP, or built-in/Standard).
        /// </summary>
        /// <returns>
        /// The name of the <see cref="MaterialMapper"/> best suited for the active render pipeline.
        /// If HDRP is active, returns <c>HDRPMaterialMapper</c>. If URP is active, returns
        /// <c>UniversalRPMaterialMapper</c>. Otherwise, returns <c>StandardMaterialMapper</c>.
        /// </returns>
        public static string GetCompatibleMaterialMapperName()
        {
            string materialMapper;
            if (GraphicsSettingsUtils.IsUsingHDRPPipeline)
            {
                materialMapper = "HDRPMaterialMapper";
            }
            else if (GraphicsSettingsUtils.IsUsingUniversalPipeline)
            {
                materialMapper = "UniversalRPMaterialMapper";
            }
            else
            {
                materialMapper = "StandardMaterialMapper";
            }
            return materialMapper;
        }

        /// <summary>
        /// Retrieves the project-wide selected <see cref="MaterialMapper"/> based on TriLib settings.
        /// If a matching mapper is found and <paramref name="instantiate"/> is <c>true</c>, this method
        /// will create a new instance of that mapper.
        /// </summary>
        /// <param name="instantiate">
        /// If <c>true</c>, creates a new instance of the selected <see cref="MaterialMapper"/>. 
        /// If <c>false</c>, returns the prefab/reference directly.
        /// </param>
        /// <returns>
        /// The selected <see cref="MaterialMapper"/>, or <c>null</c> if no mapper is selected in the TriLib settings.
        /// </returns>
        public static MaterialMapper GetSelectedMaterialMapper(bool instantiate)
        {
            for (var i = 0; i < MaterialMapper.RegisteredMappers.Count; i++)
            {
                var materialMapperName = MaterialMapper.RegisteredMappers[i];
                var materialMapperNamespace = MaterialMapper.RegisteredMapperNamespaces[i];
                if (TriLibSettings.GetBool(materialMapperName))
                {
                    return LoadOrCreateScriptableObjectSafe(materialMapperName, materialMapperNamespace, "Mappers/Materials", instantiate) as MaterialMapper;
                }
            }
            return null;
        }

        /// <summary>
        /// Configures the specified <see cref="AssetLoaderOptions"/> for optimal loading performance.
        /// This method attempts to load the Autodesk Interactive and glTF2 material helpers (if they exist),
        /// sets default references (e.g., blend shape mapper), and tweaks various settings for faster loading.
        /// </summary>
        /// <param name="assetLoaderOptions">
        /// A reference to the <see cref="AssetLoaderOptions"/> instance to configure with the fastest possible settings.
        /// </param>
        public static void LoadFastestSettings(ref AssetLoaderOptions assetLoaderOptions)
        {
            // Looks for the GLTF and Autodesk Interactive material helpers and use them to setup additional material mappers, if found.
            var autodeskInteractiveMaterialsHelper = Resources.Load<MaterialsHelper>("MaterialHelpers/AutodeskInteractiveMaterialsHelper");
            if (autodeskInteractiveMaterialsHelper != null)
            {
                autodeskInteractiveMaterialsHelper.Setup(ref assetLoaderOptions);
            }
            else
            {
                Debug.LogWarning("TriLib could not find the AutodeskInteractive Material Mapper. The default TriLib Material Mappers will be deprecated soon.");
            }

            var gltfMaterialsHelper = Resources.Load<MaterialsHelper>("MaterialHelpers/GltfMaterialsHelper");
            if (gltfMaterialsHelper != null)
            {
                gltfMaterialsHelper.Setup(ref assetLoaderOptions);
            }
            else
            {
                Debug.LogWarning("TriLib could not find the glTF2 Material Mapper. The default TriLib Material Mappers will be deprecated soon.");
            }

            // Looks for the BlendShapePlayerMapper and use it as the blend shape mapper, if found.
            var blendShapePlayerMapper = Resources.Load<BlendShapeMapper>("Mappers/BlendShapePlayerMapper");
            if (blendShapePlayerMapper != null)
            {
                assetLoaderOptions.BlendShapeMapper = blendShapePlayerMapper;
            }

            // Enable warnings for better debugging and use faster native calculation/loading methods:
            assetLoaderOptions.ShowLoadingWarnings = true;
            assetLoaderOptions.UseUnityNativeNormalCalculator = true;
            assetLoaderOptions.UseUnityNativeTextureLoader = true;

            // Disable mesh optimization for faster loading:
            assetLoaderOptions.OptimizeMeshes = false;

            // Additional relevant texture settings:
            assetLoaderOptions.GetCompatibleTextureFormat = false;
            assetLoaderOptions.EnforceAlphaChannelTextures = false;
        }

        /// <summary>
        /// Loads a model from the specified <paramref name="path"/> asynchronously, returning an
        /// <see cref="AssetLoaderContext"/> that contains information about the loading process and
        /// the resulting <see cref="GameObject"/> hierarchy.
        ///
        /// This method creates or uses an existing <see cref="AssetLoaderOptions"/> if none is provided.
        /// It attempts to load the model in a background thread where possible (depending on platform/thread settings),
        /// and calls back on the Unity main thread (unless otherwise noted).
        /// </summary>
        /// <param name="path">The absolute or relative file path to the model.</param>
        /// <param name="onLoad">
        /// Callback invoked on the main thread once the model is loaded (but materials may still be pending). 
        /// Use this to perform any setup/initialization right after the base model data is available.
        /// </param>
        /// <param name="onMaterialsLoad">
        /// Callback invoked on the main thread after the model’s materials (textures, shaders, etc.) have finished loading.
        /// This is the final stage of the loading process.
        /// </param>
        /// <param name="onProgress">
        /// Callback invoked (potentially on a background thread) whenever the loading progress changes.
        /// The float parameter represents a value from 0 to 1 indicating progress completion.
        /// </param>
        /// <param name="onError">
        /// Callback invoked on the main thread if an error occurs at any point during the loading process.
        /// </param>
        /// <param name="wrapperGameObject">
        /// An optional parent <see cref="GameObject"/> under which the newly loaded model <see cref="GameObject"/>
        /// will be placed. If <c>null</c>, the model will be loaded at the root level.
        /// </param>
        /// <param name="assetLoaderOptions">
        /// The <see cref="AssetLoaderOptions"/> controlling how the model is loaded. If <c>null</c>,
        /// this method will create and use a default set of loading options.
        /// </param>
        /// <param name="customContextData">
        /// Any custom data you want to pass along to the <see cref="AssetLoaderContext"/> for use within the loading pipeline.
        /// </param>
        /// <param name="haltTask">
        /// If <c>true</c>, loading tasks are created but not started immediately. This can be used to queue or chain multiple tasks.
        /// </param>
        /// <param name="onPreLoad">
        /// Callback invoked on a background thread before Unity objects are created. Use this for any setup tasks
        /// that need to run ahead of object instantiation.
        /// </param>
        /// <param name="isZipFile">Indicates whether the specified <paramref name="path"/> points to a Zip file containing model data.</param>
        /// <returns>
        /// The <see cref="AssetLoaderContext"/> that tracks all loading information, including references to the
        /// loaded <see cref="GameObject"/> hierarchy and any associated data or errors.
        /// </returns>
        public static AssetLoaderContext LoadModelFromFile(
            string path,
            Action<AssetLoaderContext> onLoad = null,
            Action<AssetLoaderContext> onMaterialsLoad = null,
            Action<AssetLoaderContext, float> onProgress = null,
            Action<IContextualizedError> onError = null,
            GameObject wrapperGameObject = null,
            AssetLoaderOptions assetLoaderOptions = null,
            object customContextData = null,
            bool haltTask = false,
            Action<AssetLoaderContext> onPreLoad = null,
            bool isZipFile = false)
        {
            var assetLoaderContext = new AssetLoaderContext
            {
                Options = assetLoaderOptions ? assetLoaderOptions : CreateDefaultLoaderOptions(),
                Filename = path,
                BasePath = FileUtils.GetFileDirectory(path),
                WrapperGameObject = wrapperGameObject,
                OnMaterialsLoad = onMaterialsLoad,
                OnLoad = onLoad,
                OnProgress = onProgress,
                HandleError = HandleError,
                OnError = onError,
                OnPreLoad = onPreLoad,
                CustomData = customContextData,
                HaltTasks = haltTask,
#if (UNITY_WEBGL && !TRILIB_ENABLE_WEBGL_THREADS) || (UNITY_WSA && !TRILIB_ENABLE_UWP_THREADS) || TRILIB_FORCE_SYNC
        Async = false,
#else
                Async = true,
#endif
                IsZipFile = isZipFile,
                PersistentDataPath = Application.persistentDataPath
            };
            assetLoaderContext.Setup();
            LoadModelInternal(assetLoaderContext);
            return assetLoaderContext;
        }

        /// <summary>
        /// Loads a model from the specified <paramref name="path"/> synchronously on the main thread,
        /// returning an <see cref="AssetLoaderContext"/> with the resulting <see cref="GameObject"/> hierarchy.
        ///
        /// This method does not utilize background threads, which may be required on certain platforms
        /// or situations where multithreading is unsupported or disabled.
        /// </summary>
        /// <param name="path">The absolute or relative file path to the model.</param>
        /// <param name="onError">
        /// Callback invoked on the main thread if an error occurs at any point during loading.
        /// </param>
        /// <param name="wrapperGameObject">
        /// An optional parent <see cref="GameObject"/> under which the newly loaded model <see cref="GameObject"/>
        /// will be placed. If <c>null</c>, the model will be loaded at the root level.
        /// </param>
        /// <param name="assetLoaderOptions">
        /// The <see cref="AssetLoaderOptions"/> controlling how the model is loaded. If <c>null</c>,
        /// this method will create and use a default set of loading options.
        /// </param>
        /// <param name="customContextData">
        /// Any custom data you want to pass along to the <see cref="AssetLoaderContext"/> for use within the loading pipeline.
        /// </param>
        /// <param name="isZipFile">Indicates whether the specified <paramref name="path"/> points to a Zip file containing model data.</param>
        /// <returns>
        /// The <see cref="AssetLoaderContext"/> containing information about the load, including references to the
        /// loaded <see cref="GameObject"/> hierarchy and any encountered errors.
        /// </returns>
        public static AssetLoaderContext LoadModelFromFileNoThread(
            string path,
            Action<IContextualizedError> onError = null,
            GameObject wrapperGameObject = null,
            AssetLoaderOptions assetLoaderOptions = null,
            object customContextData = null,
            bool isZipFile = false)
        {
            var assetLoaderContext = new AssetLoaderContext
            {
                Options = assetLoaderOptions ? assetLoaderOptions : CreateDefaultLoaderOptions(),
                Filename = path,
                BasePath = FileUtils.GetFileDirectory(path),
                CustomData = customContextData,
                HandleError = HandleError,
                OnError = onError,
                WrapperGameObject = wrapperGameObject,
                Async = false,
                IsZipFile = isZipFile,
                PersistentDataPath = Application.persistentDataPath
            };
            assetLoaderContext.Setup();
            LoadModelInternal(assetLoaderContext);
            return assetLoaderContext;
        }

        /// <summary>
        /// Loads a model from the given <paramref name="stream"/> asynchronously, returning an
        /// <see cref="AssetLoaderContext"/> with the resulting <see cref="GameObject"/> hierarchy.
        ///
        /// Use this overload when the model data is not located in a file but is provided via a <see cref="Stream"/>.
        /// For instance, loading from memory or from a custom data source. 
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> 包含要加载的模型数据。</param>
        /// <param name="filename">
        /// 模型的文件名（如果知道）。这主要用于确定基目录或元数据。
        /// </param>
        /// <param name="fileExtension">
        /// The file extension (e.g., "fbx"). If <c>null</c>, this will be automatically determined
        /// using <see cref="FileUtils.GetFileExtension"/> based on <paramref name="filename"/>.
        /// </param>
        /// <param name="onLoad">
        /// Callback invoked on the main thread once the model is loaded (but materials may still be pending).
        /// </param>
        /// <param name="onMaterialsLoad">
        /// Callback invoked on the main thread after the model’s materials have finished loading.
        /// </param>
        /// <param name="onProgress">
        /// Callback invoked (potentially on a background thread) whenever the loading progress changes.
        /// The float parameter represents a value from 0 to 1 indicating progress.
        /// </param>
        /// <param name="onError">
        /// Callback invoked on the main thread if an error occurs at any point during the loading process.
        /// </param>
        /// <param name="wrapperGameObject">
        /// An optional parent <see cref="GameObject"/> under which the newly loaded model <see cref="GameObject"/>
        /// will be placed. If <c>null</c>, the model will be loaded at the root level.
        /// </param>
        /// <param name="assetLoaderOptions">
        /// The <see cref="AssetLoaderOptions"/> controlling how the model is loaded. If <c>null</c>,
        /// this method will create and use a default set of loading options.
        /// </param>
        /// <param name="customContextData">
        /// Any custom data you want to pass along to the <see cref="AssetLoaderContext"/> for use within the loading pipeline.
        /// </param>
        /// <param name="haltTask">
        /// If <c>true</c>, loading tasks are created but not started immediately. This can be used to queue or chain multiple tasks.
        /// </param>
        /// <param name="onPreLoad">
        /// Callback invoked on a background thread before Unity objects are created. Useful for any pre-instantiation preparation.
        /// </param>
        /// <param name="isZipFile">Indicates whether the provided <paramref name="stream"/> refers to a Zip file containing model data.</param>
        /// <returns>
        /// The <see cref="AssetLoaderContext"/> tracking all relevant loading data, including the resulting
        /// <see cref="GameObject"/> hierarchy and any progress or errors.
        /// </returns>
        public static AssetLoaderContext LoadModelFromStream(
            Stream stream,
            string filename = null,
            string fileExtension = null,
            Action<AssetLoaderContext> onLoad = null,
            Action<AssetLoaderContext> onMaterialsLoad = null,
            Action<AssetLoaderContext, float> onProgress = null,
            Action<IContextualizedError> onError = null,
            GameObject wrapperGameObject = null,
            AssetLoaderOptions assetLoaderOptions = null,
            object customContextData = null,
            bool haltTask = false,
            Action<AssetLoaderContext> onPreLoad = null,
            bool isZipFile = false)
        {
            var assetLoaderContext = new AssetLoaderContext
            {
                Options = assetLoaderOptions ? assetLoaderOptions : CreateDefaultLoaderOptions(),
                Stream = stream,
                Filename = filename,
                FileExtension = fileExtension ?? FileUtils.GetFileExtension(filename, false),
                BasePath = FileUtils.GetFileDirectory(filename),
                WrapperGameObject = wrapperGameObject,
                OnMaterialsLoad = onMaterialsLoad,
                OnLoad = onLoad,
                OnProgress = onProgress,
                HandleError = HandleError,
                OnError = onError,
                OnPreLoad = onPreLoad,
                CustomData = customContextData,
                HaltTasks = haltTask,
#if (UNITY_WEBGL && !TRILIB_ENABLE_WEBGL_THREADS) || (UNITY_WSA && !TRILIB_ENABLE_UWP_THREADS) || TRILIB_FORCE_SYNC
        Async = false,
#else
                Async = true,
#endif
                IsZipFile = isZipFile,
                PersistentDataPath = Application.persistentDataPath
            };
            assetLoaderContext.Setup();
            LoadModelInternal(assetLoaderContext);
            return assetLoaderContext;
        }

        /// <summary>
        /// Loads a model from the given <paramref name="stream"/> synchronously on the main thread,
        /// returning an <see cref="AssetLoaderContext"/> with the resulting <see cref="GameObject"/> hierarchy.
        ///
        /// This method does not utilize background threads, suitable for environments or platforms
        /// where asynchronous/parallel loading is not available or desired.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> containing the model data to load.</param>
        /// <param name="filename">
        /// The model’s filename (if known). Used to determine the base directory or any other relevant metadata.
        /// </param>
        /// <param name="fileExtension">
        /// The file extension (e.g., "fbx"). If <c>null</c>, this will be automatically determined
        /// using <see cref="FileUtils.GetFileExtension"/> based on <paramref name="filename"/>.
        /// </param>
        /// <param name="onError">
        /// Callback invoked on the main thread if an error occurs at any point during loading.
        /// </param>
        /// <param name="wrapperGameObject">
        /// An optional parent <see cref="GameObject"/> under which the newly loaded model <see cref="GameObject"/>
        /// will be placed. If <c>null</c>, the model will be loaded at the root level.
        /// </param>
        /// <param name="assetLoaderOptions">
        /// The <see cref="AssetLoaderOptions"/> controlling how the model is loaded. If <c>null</c>,
        /// a default configuration will be created and used.
        /// </param>
        /// <param name="customContextData">
        /// Any custom data you want to pass along to the <see cref="AssetLoaderContext"/> for use within the loading pipeline.
        /// </param>
        /// <param name="isZipFile">Indicates whether the provided <paramref name="stream"/> refers to a Zip file containing model data.</param>
        /// <returns>
        /// The <see cref="AssetLoaderContext"/> containing information about the load, including references to
        /// the loaded <see cref="GameObject"/> hierarchy and any encountered errors.
        /// </returns>
        public static AssetLoaderContext LoadModelFromStreamNoThread(
            Stream stream,
            string filename = null,
            string fileExtension = null,
            Action<IContextualizedError> onError = null,
            GameObject wrapperGameObject = null,
            AssetLoaderOptions assetLoaderOptions = null,
            object customContextData = null,
            bool isZipFile = false)
        {
            var assetLoaderContext = new AssetLoaderContext
            {
                Options = assetLoaderOptions ? assetLoaderOptions : CreateDefaultLoaderOptions(),
                Stream = stream,
                Filename = filename,
                FileExtension = fileExtension ?? FileUtils.GetFileExtension(filename, false),
                BasePath = FileUtils.GetFileDirectory(filename),
                CustomData = customContextData,
                HandleError = HandleError,
                OnError = onError,
                WrapperGameObject = wrapperGameObject,
                Async = false,
                IsZipFile = isZipFile,
                PersistentDataPath = Application.persistentDataPath
            };
            assetLoaderContext.Setup();
            LoadModelInternal(assetLoaderContext);
            return assetLoaderContext;
        }

        /// <summary>
        /// Attempts to load a <see cref="ScriptableObject"/> from the specified <paramref name="directory"/>.
        /// If not found, creates a new instance using <paramref name="typeName"/> and <paramref name="typeNamespace"/>.
        /// </summary>
        /// <param name="typeName">The class name of the <see cref="ScriptableObject"/> to load or create.</param>
        /// <param name="typeNamespace">The namespace in which <paramref name="typeName"/> resides.</param>
        /// <param name="directory">
        /// The Resources directory path where the <see cref="ScriptableObject"/> might be located.
        /// For example, if you have a Resource at "Assets/Resources/Mappers/Material/CustomMaterialMapper.asset",
        /// then <paramref name="directory"/> could be "Mappers/Material".
        /// </param>
        /// <param name="instantiate">
        /// If <c>true</c>, returns a cloned instance (via <see cref="Object.Instantiate"/>). 
        /// If <c>false</c>, returns the original <see cref="ScriptableObject"/> reference.
        /// </param>
        /// <returns>
        /// A reference to the loaded or newly created <see cref="ScriptableObject"/>, or <c>null</c>
        /// if the specified type or resource could not be found.
        /// </returns>
        public static ScriptableObject LoadOrCreateScriptableObjectSafe(
            string typeName,
            string typeNamespace,
            string directory,
            bool instantiate)
        {
            var scriptableObject = Resources.Load<ScriptableObject>($"{directory}/{typeName}");
            if (scriptableObject == null)
            {
                var type = Type.GetType($"{typeNamespace}.{typeName}");
                return type != null ? ScriptableObject.CreateInstance(typeName) : null;
            }
            return instantiate ? Object.Instantiate(scriptableObject) : scriptableObject;
        }

        /// <summary>
        /// Applies a <see cref="Material"/> from the specified <paramref name="materialMapperContext"/>
        /// to all corresponding <see cref="Renderer"/> components in the scene hierarchy.
        /// </summary>
        /// <param name="materialMapperContext">
        /// A <see cref="MaterialMapperContext"/> that holds both the original virtual material and 
        /// the resulting Unity <see cref="Material"/>.
        /// </param>
        private static void ApplyMaterialToRenderers(MaterialMapperContext materialMapperContext)
        {
            materialMapperContext.Completed = false;
            if (materialMapperContext.Context.MaterialRenderers.TryGetValue(materialMapperContext.Material, out var materialRendererList))
            {
                for (var i = 0; i < materialRendererList.Count; i++)
                {
                    var materialRendererContext = materialRendererList[i];
                    materialRendererContext.MaterialMapperContext = materialMapperContext;
                    var meshFilter = materialRendererContext.Renderer.GetComponentInChildren<MeshFilter>();
                    var skinnedMeshRenderer = materialRendererContext.Renderer.GetComponentInChildren<SkinnedMeshRenderer>();
                    if (meshFilter != null)
                    {
                        materialRendererContext.Mesh = materialMapperContext.Context.Options.UseSharedMeshes
                            ? meshFilter.sharedMesh
                            : meshFilter.mesh;
                    }
                    else
                    {
                        materialRendererContext.Mesh = skinnedMeshRenderer != null ? skinnedMeshRenderer.sharedMesh : null;
                    }
                    materialMapperContext.MaterialMapper.ApplyMaterialToRenderer(materialRendererContext);
                }
            }
            materialMapperContext.Completed = true;
        }

        /// <summary>
        /// Builds hierarchy paths for each <see cref="GameObject"/> within the <paramref name="assetLoaderContext"/>.
        /// These paths can be used for tracking, referencing, or debugging the loaded hierarchy.
        /// </summary>
        /// <param name="assetLoaderContext">
        /// The <see cref="AssetLoaderContext"/> containing the loaded <see cref="GameObject"/> references.
        /// </param>
        private static void BuildGameObjectsPaths(AssetLoaderContext assetLoaderContext)
        {
            foreach (var value in assetLoaderContext.GameObjects.Values)
            {
                assetLoaderContext.GameObjectPaths.Add(value, value.transform.BuildPath(assetLoaderContext.RootGameObject.transform));
            }
        }

        /// <summary>
        /// Performs final cleanup after model loading. If <see cref="AssetLoaderOptions.CloseStreamAutomatically"/>
        /// is enabled, this method attempts to close and dispose of the <see cref="Stream"/> used to load the model.
        /// Additionally, if <see cref="AssetLoaderOptions.CollectCG"/> is enabled, it forces .NET garbage collection,
        /// which can be useful to reclaim memory after large model loads.
        /// </summary>
        /// <param name="assetLoaderContext">
        /// The <see cref="AssetLoaderContext"/> containing information about the loaded model,
        /// including its <see cref="Stream"/> and <see cref="AssetLoaderOptions"/>.
        /// </param>
        private static void Cleanup(AssetLoaderContext assetLoaderContext)
        {
            if (assetLoaderContext.Stream != null && assetLoaderContext.Options.CloseStreamAutomatically)
            {
                assetLoaderContext.Stream.TryToDispose();
            }
            if (assetLoaderContext.Options.CollectCG)
            {
                // Forces garbage collection, which can help free memory after loading large models.
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        /// <summary>
        /// Converts the given <see cref="IAnimation"/> instance into a Unity <see cref="AnimationClip"/>.
        /// The method assigns animation curves to the relevant <see cref="GameObject"/> transforms
        /// (based on the paths stored in the <paramref name="assetLoaderContext"/>), respecting any
        /// required quaternion continuity setting.
        /// </summary>
        /// <param name="assetLoaderContext">
        /// The <see cref="AssetLoaderContext"/> providing references to the loaded <see cref="GameObject"/>s
        /// and paths where the animation curves will be applied.
        /// </param>
        /// <param name="animation">The source animation data to convert.</param>
        /// <returns>
        /// A newly created <see cref="AnimationClip"/> (marked as legacy) with populated animation curves.
        /// </returns>
        private static AnimationClip CreateAnimation(AssetLoaderContext assetLoaderContext, IAnimation animation)
        {
            var animationClip = new AnimationClip { name = animation.Name, legacy = true, frameRate = animation.FrameRate };
            var animationCurveBindings = animation.AnimationCurveBindings;
            if (animationCurveBindings == null)
            {
                return animationClip;
            }

            for (var i = animationCurveBindings.Count - 1; i >= 0; i--)
            {
                var animationCurveBinding = animationCurveBindings[i];
                var animationCurves = animationCurveBinding.AnimationCurves;
                if (!assetLoaderContext.GameObjects.ContainsKey(animationCurveBinding.Model))
                {
                    continue;
                }

                var gameObject = assetLoaderContext.GameObjects[animationCurveBinding.Model];
                for (var j = 0; j < animationCurves.Count; j++)
                {
                    var animationCurve = animationCurves[j];
                    var unityAnimationCurve = animationCurve.AnimationCurve;
                    var gameObjectPath = assetLoaderContext.GameObjectPaths[gameObject];
                    var propertyName = animationCurve.Property;
                    var propertyType = animationCurve.AnimatedType;
                    animationClip.SetCurve(gameObjectPath, propertyType, propertyName, unityAnimationCurve);
                }
            }

            // Ensures smooth quaternion transitions if enabled in the loader options.
            if (assetLoaderContext.Options.EnsureQuaternionContinuity)
            {
                animationClip.EnsureQuaternionContinuity();
            }
            return animationClip;
        }

        /// <summary>
        /// Creates and configures a Unity <see cref="Camera"/> component on the specified <paramref name="newGameObject"/>
        /// based on the provided <see cref="ICamera"/> data.
        /// </summary>
        /// <param name="assetLoaderContext">
        /// The <see cref="AssetLoaderContext"/> containing model loading data. (Not currently used for direct camera setup,
        /// but provided for consistency if future modifications require context.)
        /// </param>
        /// <param name="camera">The TriLib <see cref="ICamera"/> definition containing camera properties.</param>
        /// <param name="newGameObject">
        /// The <see cref="GameObject"/> on which to add and configure the <see cref="Camera"/> component.
        /// </param>
        private static void CreateCamera(AssetLoaderContext assetLoaderContext, ICamera camera, GameObject newGameObject)
        {
            var unityCamera = newGameObject.AddComponent<Camera>();
            unityCamera.aspect = camera.AspectRatio;
            unityCamera.orthographic = camera.Ortographic;
            unityCamera.orthographicSize = camera.OrtographicSize;
            unityCamera.fieldOfView = camera.FieldOfView;
            unityCamera.nearClipPlane = camera.NearClipPlane;
            unityCamera.farClipPlane = camera.FarClipPlane;
            unityCamera.focalLength = camera.FocalLength;
            unityCamera.sensorSize = camera.SensorSize;
            unityCamera.lensShift = camera.LensShift;
            unityCamera.gateFit = camera.GateFitMode;
            unityCamera.usePhysicalProperties = camera.PhysicalCamera;
            unityCamera.enabled = true;
        }

        /// <summary>
        /// Converts the geometry in the specified <see cref="IModel"/> into a Unity <see cref="Mesh"/> and associates it
        /// with either a <see cref="MeshRenderer"/> or <see cref="SkinnedMeshRenderer"/>, depending on the <see cref="AssetLoaderOptions"/>
        /// (e.g., animation or blend shape requirements).
        ///
        /// This method yields at various points to support incremental or coroutine-based loading.
        /// It also applies optional lip-sync mappings, sets up colliders, and creates relevant
        /// <see cref="Material"/> relationships for subsequent assignment.
        /// </summary>
        /// <param name="assetLoaderContext">
        /// The <see cref="AssetLoaderContext"/> containing references and settings for the entire model loading process.
        /// </param>
        /// <param name="meshGameObject">The <see cref="GameObject"/> that will host the generated mesh data.</param>
        /// <param name="rootModel">
        /// The root of the model hierarchy, containing references to all materials, animations, etc.
        /// </param>
        /// <param name="meshModel">
        /// The specific <see cref="IModel"/> whose geometry and associated data will be converted into a Unity mesh.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable"/> that yields control during the mesh creation process
        /// (to allow asynchronous or stepwise loading). Once complete, a Unity mesh (and optionally,
        /// colliders, lip-sync mapping, and materials) will be associated with <paramref name="meshGameObject"/>.
        /// </returns>
        private static IEnumerable CreateGeometry(
            AssetLoaderContext assetLoaderContext,
            GameObject meshGameObject,
            IRootModel rootModel,
            IModel meshModel)
        {
            var geometryGroup = meshModel.GeometryGroup;
            if (geometryGroup.GeometriesData != null)
            {
                // If the mesh has not been generated yet, do so and yield during the process.
                if (geometryGroup.Mesh == null)
                {
                    foreach (var item in geometryGroup.GenerateMesh(assetLoaderContext, meshGameObject, meshModel))
                    {
                        yield return item;
                    }
                }

                // Store reference to the newly created mesh for cleanup or further processing.
                assetLoaderContext.Allocations.Add(geometryGroup.Mesh);

                // Apply lip-sync mapping if available and appropriate.
                if (assetLoaderContext.Options.LipSyncMappers != null)
                {
                    // Sort the lip sync mappers by descending checking order to prioritize certain mappers first.
                    Array.Sort(assetLoaderContext.Options.LipSyncMappers, (a, b) => a.CheckingOrder > b.CheckingOrder ? -1 : 1);
                    foreach (var lipSyncMapper in assetLoaderContext.Options.LipSyncMappers)
                    {
                        if (lipSyncMapper.Map(assetLoaderContext, geometryGroup, out var visemeToBlendTargets))
                        {
                            var lipSyncMapping = meshGameObject.AddComponent<LipSyncMapping>();
                            lipSyncMapping.VisemeToBlendTargets = visemeToBlendTargets;
                            break;
                        }
                    }
                }

                // Optionally add a MeshCollider if requested.
                if (assetLoaderContext.Options.GenerateColliders)
                {
                    if (assetLoaderContext.RootModel.AllAnimations != null
                        && assetLoaderContext.RootModel.AllAnimations.Count > 0
                        && assetLoaderContext.Options.ShowLoadingWarnings)
                    {
                        Debug.LogWarning("Adding a MeshCollider to an animated object.");
                    }
                    var meshCollider = meshGameObject.AddComponent<MeshCollider>();
                    meshCollider.convex = assetLoaderContext.Options.ConvexColliders;
                    meshCollider.sharedMesh = geometryGroup.Mesh;

                    // Yield control again to allow asynchronous progress.
                    foreach (var item in assetLoaderContext.ReleaseMainThread())
                    {
                        yield return item;
                    }
                }

                // Determine whether to use SkinnedMeshRenderer or MeshRenderer based on animation or blend shapes.
                Renderer renderer = null;
                if (assetLoaderContext.Options.AnimationType != AnimationType.None || assetLoaderContext.Options.ImportBlendShapes)
                {
                    var bones = assetLoaderContext.Options.AddAllBonesToSkinnedMeshRenderers
                        ? GetAllBonesRecursive(assetLoaderContext)
                        : meshModel.Bones;

                    var geometryGroupBlendShapeGeometryBindings = geometryGroup.BlendShapeKeys;
                    var requiresSkinned = (bones != null && bones.Count > 0)
                        || (assetLoaderContext.Options.BlendShapeMapper == null && geometryGroupBlendShapeGeometryBindings != null && geometryGroupBlendShapeGeometryBindings.Count > 0);

                    if (requiresSkinned && assetLoaderContext.Options.AnimationType != AnimationType.None)
                    {
                        var skinnedMeshRenderer = meshGameObject.AddComponent<SkinnedMeshRenderer>();
                        skinnedMeshRenderer.enabled = !assetLoaderContext.Options.ImportVisibility || meshModel.Visibility;
                        skinnedMeshRenderer.updateWhenOffscreen = assetLoaderContext.Options.UpdateSkinnedMeshRendererWhenOffscreen;
                        renderer = skinnedMeshRenderer;
                    }
                }

                if (renderer == null)
                {
                    // Fallback to MeshFilter + MeshRenderer.
                    var meshFilter = meshGameObject.AddComponent<MeshFilter>();
                    if (assetLoaderContext.Options.UseSharedMeshes)
                    {
                        meshFilter.sharedMesh = geometryGroup.Mesh;
                    }
                    else
                    {
                        meshFilter.mesh = geometryGroup.Mesh;
                    }

                    if (!assetLoaderContext.Options.LoadPointClouds)
                    {
                        var meshRenderer = meshGameObject.AddComponent<MeshRenderer>();
                        meshRenderer.enabled = !assetLoaderContext.Options.ImportVisibility || meshModel.Visibility;
                        renderer = meshRenderer;
                    }
                }

                // Assign temporary or "loading" materials to the renderer until real materials can be processed.
                if (renderer != null)
                {
                    Material loadingMaterial = null;
                    if (assetLoaderContext.Options.MaterialMappers != null)
                    {
                        for (var i = 0; i < assetLoaderContext.Options.MaterialMappers.Length; i++)
                        {
                            var mapper = assetLoaderContext.Options.MaterialMappers[i];
                            if (mapper != null && mapper.IsCompatible(null))
                            {
                                loadingMaterial = mapper.LoadingMaterial;
                                break;
                            }
                        }
                    }

                    var unityMaterials = new Material[geometryGroup.GeometriesData.Count];
                    if (loadingMaterial == null)
                    {
                        if (assetLoaderContext.Options.ShowLoadingWarnings)
                        {
                            Debug.LogWarning("Could not find a suitable loading Material.");
                        }
                    }
                    else
                    {
                        for (var i = 0; i < unityMaterials.Length; i++)
                        {
                            unityMaterials[i] = loadingMaterial;
                        }
                    }

                    if (assetLoaderContext.Options.UseSharedMaterials)
                    {
                        renderer.sharedMaterials = unityMaterials;
                    }
                    else
                    {
                        renderer.materials = unityMaterials;
                    }

                    // Link each sub-geometry to the corresponding material index for later assignment.
                    var materialIndices = meshModel.MaterialIndices;
                    foreach (var geometryData in geometryGroup.GeometriesData)
                    {
                        var geometry = geometryData.Value;
                        if (geometry == null)
                        {
                            continue;
                        }

                        var originalGeometryIndex = geometry.OriginalIndex;
                        if (originalGeometryIndex < 0 || originalGeometryIndex >= unityMaterials.Length)
                        {
                            continue;
                        }

                        var materialIndex = materialIndices[originalGeometryIndex];
                        if (rootModel.AllMaterials == null)
                        {
                            rootModel.AllMaterials = new List<IMaterial>();
                        }

                        IMaterial sourceMaterial = null;
                        var hasInvalidMaterial = false;
                        if (materialIndex < 0 || materialIndex >= rootModel.AllMaterials.Count)
                        {
                            if (!assetLoaderContext.Options.CreateMaterialsForAllModels)
                            {
                                hasInvalidMaterial = true;
                            }
                            else
                            {
                                sourceMaterial = new DummyMaterial { Name = $"{geometryGroup.Name}_{geometry.Index}" };
                                rootModel.AllMaterials.Add(sourceMaterial);
                            }
                        }
                        else
                        {
                            sourceMaterial = rootModel.AllMaterials[materialIndex];
                            if (sourceMaterial == null)
                            {
                                if (!assetLoaderContext.Options.CreateMaterialsForAllModels)
                                {
                                    hasInvalidMaterial = true;
                                }
                                else
                                {
                                    sourceMaterial = new DummyMaterial { Name = $"{geometryGroup.Name}_{geometry.Index}" };
                                    rootModel.AllMaterials.Add(sourceMaterial);
                                }
                            }
                        }

                        if (hasInvalidMaterial)
                        {
                            // Clean up any temporary references if material mapping is invalid.
                            var materialRendererContext = new MaterialRendererContext
                            {
                                Mesh = geometryGroup.Mesh,
                                Context = assetLoaderContext
                            };
                            MaterialMapper.Cleanup(materialRendererContext);
                        }
                        else
                        {
                            // Register this geometry to the correct source material for post-processing.
                            var materialRenderersContext = new MaterialRendererContext
                            {
                                Context = assetLoaderContext,
                                Renderer = renderer,
                                GeometryIndex = geometry.Index,
                                Material = sourceMaterial
                            };
                            if (assetLoaderContext.MaterialRenderers.TryGetValue(sourceMaterial, out var materialRendererContextList))
                            {
                                materialRendererContextList.Add(materialRenderersContext);
                            }
                            else
                            {
                                assetLoaderContext.MaterialRenderers.Add(sourceMaterial, new List<MaterialRendererContext> { materialRenderersContext });
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates a Unity <see cref="HumanBone"/> from the specified <see cref="BoneMapping"/> data, linking
        /// a bone name to a human bone name and applying the configured limits for humanoid rig mapping.
        /// </summary>
        /// <param name="boneMapping">
        /// The <see cref="BoneMapping"/> containing humanoid bone information (e.g., <see cref="HumanBone"/> name
        /// and limit definitions).
        /// </param>
        /// <param name="boneName">The actual bone name in the Unity skeleton that maps to the <see cref="HumanBone"/>.</param>
        /// <returns>
        /// A fully configured <see cref="HumanBone"/> ready to be assigned to a <see cref="HumanDescription"/>.
        /// </returns>
        private static HumanBone CreateHumanBone(BoneMapping boneMapping, string boneName)
        {
            var humanBone = new HumanBone
            {
                boneName = boneName,
                humanName = GetHumanBodyName(boneMapping.HumanBone),
                limit =
        {
            useDefaultValues = boneMapping.HumanLimit.useDefaultValues,
            axisLength = boneMapping.HumanLimit.axisLength,
            center = boneMapping.HumanLimit.center,
            max = boneMapping.HumanLimit.max,
            min = boneMapping.HumanLimit.min
        }
            };
            return humanBone;
        }


        /// <summary>
        /// Creates a Unity <see cref="Light"/> component on the specified <paramref name="newGameObject"/> 
        /// using properties from the given TriLib <see cref="ILight"/>.
        /// </summary>
        /// <param name="assetLoaderContext">
        /// The <see cref="AssetLoaderContext"/> containing model loading settings and references.
        /// (Not currently used directly in light creation, but passed for consistency or potential future use.)
        /// </param>
        /// <param name="light">The TriLib light representation providing light color, intensity, type, etc.</param>
        /// <param name="newGameObject">
        /// The <see cref="GameObject"/> on which to add and configure the Unity <see cref="Light"/> component.
        /// </param>
        private static void CreateLight(AssetLoaderContext assetLoaderContext, ILight light, GameObject newGameObject)
        {
            var unityLight = newGameObject.AddComponent<Light>();
            unityLight.color = light.Color;
            unityLight.innerSpotAngle = light.InnerSpotAngle;
            unityLight.spotAngle = light.OuterSpotAngle;
            unityLight.intensity = light.Intensity;
            unityLight.range = light.Range;
            unityLight.type = light.LightType;
            unityLight.shadows = light.CastShadows ? LightShadows.Soft : LightShadows.None;
#if UNITY_EDITOR
            unityLight.areaSize = new Vector2(light.Width, light.Height);
#endif
        }

        /// <summary>
        /// Recursively creates a <see cref="GameObject"/> hierarchy corresponding to the supplied <see cref="IModel"/> 
        /// structure. This includes generating meshes, cameras, lights, and any user-defined properties.
        /// </summary>
        /// <param name="assetLoaderContext">
        /// The <see cref="AssetLoaderContext"/> containing references to loaded data, options, and utility methods.
        /// </param>
        /// <param name="parentTransform">
        /// The <see cref="Transform"/> under which the newly created <see cref="GameObject"/> will be parented. 
        /// Use <c>null</c> to create the model at the scene root level.
        /// </param>
        /// <param name="rootModel">
        /// The root of the TriLib model hierarchy, containing all sub-models, materials, and animations.
        /// </param>
        /// <param name="model">The current <see cref="IModel"/> node to convert into a <see cref="GameObject"/>.</param>
        /// <param name="isRootGameObject">
        /// Indicates whether the created <see cref="GameObject"/> is the root of the model hierarchy. 
        /// If <c>true</c>, this method assigns <see cref="AssetLoaderContext.RootGameObject"/> accordingly.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable"/> that yields control during mesh creation and child model creation steps
        /// (allowing for asynchronous or coroutine-based loading). When complete, the fully assembled 
        /// <see cref="GameObject"/> hierarchy is added to <see cref="AssetLoaderContext.GameObjects"/>.
        /// </returns>
        private static IEnumerable CreateModel(
            AssetLoaderContext assetLoaderContext,
            Transform parentTransform,
            IRootModel rootModel,
            IModel model,
            bool isRootGameObject)
        {
            var newGameObject = new GameObject(model.Name);
            assetLoaderContext.GameObjects.Add(model, newGameObject);
            assetLoaderContext.Models.Add(newGameObject, model);

            newGameObject.transform.parent = parentTransform;
            newGameObject.transform.localPosition = model.LocalPosition;
            newGameObject.transform.localRotation = model.LocalRotation;
            newGameObject.transform.localScale = model.LocalScale;

            // Create mesh geometry if present
            if (model.GeometryGroup != null)
            {
                foreach (var item in CreateGeometry(assetLoaderContext, newGameObject, rootModel, model))
                {
                    yield return item;
                }
            }

            // Optionally create camera if requested and the model node is a camera
            if (assetLoaderContext.Options.ImportCameras && model is ICamera camera)
            {
                CreateCamera(assetLoaderContext, camera, newGameObject);
            }

            // Optionally create light if requested and the model node is a light
            if (assetLoaderContext.Options.ImportLights && model is ILight light)
            {
                CreateLight(assetLoaderContext, light, newGameObject);
            }

            // Recursively create child GameObjects
            if (model.Children != null && model.Children.Count > 0)
            {
                for (var i = 0; i < model.Children.Count; i++)
                {
                    var child = model.Children[i];
                    foreach (var item in CreateModel(assetLoaderContext, newGameObject.transform, rootModel, child, false))
                    {
                        yield return item;
                    }
                }
            }

            // Process any user-defined properties via a user properties mapper
            if (assetLoaderContext.Options.UserPropertiesMapper != null && model.UserProperties != null)
            {
                foreach (var userProperty in model.UserProperties)
                {
                    assetLoaderContext.Options.UserPropertiesMapper.OnProcessUserData(
                        assetLoaderContext,
                        newGameObject,
                        userProperty.Key,
                        userProperty.Value
                    );
                }
            }

            // Set the RootGameObject if this is the top-level node
            if (isRootGameObject)
            {
                assetLoaderContext.RootGameObject = newGameObject;
            }
        }

        /// <summary>
        /// Creates the root <see cref="GameObject"/> for the model by invoking <see cref="CreateModel"/> 
        /// on the root node, then performing post-processing (e.g., assigning materials).
        /// </summary>
        /// <param name="assetLoaderContext">
        /// The <see cref="AssetLoaderContext"/> that holds all data and settings for model loading.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerator"/> that yields control during the root model creation and 
        /// material processing steps. By the end, the fully loaded model hierarchy is accessible 
        /// via <see cref="AssetLoaderContext.RootGameObject"/>.
        /// </returns>
        private static IEnumerator CreateRootModel(AssetLoaderContext assetLoaderContext)
        {
            var transform = assetLoaderContext.WrapperGameObject != null
                ? assetLoaderContext.WrapperGameObject.transform
                : null;

            var rootModel = assetLoaderContext.RootModel;
            foreach (var item in CreateModel(assetLoaderContext, transform, rootModel, rootModel, true))
            {
                yield return item;
            }

            // Perform any post-processing tasks (e.g., setting up rigs, adjusting transforms)
            PostProcessModels(assetLoaderContext);

            // Perform material processing to finalize and assign materials
            foreach (var item in ProcessMaterials(assetLoaderContext))
            {
                yield return item;
            }
        }

        /// <summary>
        /// Attempts to create a <see cref="ScriptableObject"/> of the specified <paramref name="typeName"/> 
        /// in the specified <paramref name="typeNamespace"/> without throwing an internal exception
        /// if the type cannot be found.
        /// </summary>
        /// <param name="typeName">The class name of the <see cref="ScriptableObject"/> to create.</param>
        /// <param name="typeNamespace">The namespace in which <paramref name="typeName"/> resides.</param>
        /// <returns>
        /// A newly instantiated <see cref="ScriptableObject"/> of the specified type, or <c>null</c> 
        /// if the type was not found or could not be created.
        /// </returns>
        private static ScriptableObject CreateScriptableObjectSafe(string typeName, string typeNamespace)
        {
            var type = Type.GetType($"{typeNamespace}.{typeName}");
            return type != null ? ScriptableObject.CreateInstance(typeName) : null;
        }

        /// <summary>
        /// Creates a <see cref="SkeletonBone"/> record from the given <paramref name="boneTransform"/>, 
        /// capturing its local transform data.
        /// </summary>
        /// <param name="boneTransform">The <see cref="Transform"/> representing a bone in a character rig.</param>
        /// <returns>
        /// A new <see cref="SkeletonBone"/> initialized with the name, position, rotation, and scale 
        /// from <paramref name="boneTransform"/>.
        /// </returns>
        private static SkeletonBone CreateSkeletonBone(Transform boneTransform)
        {
            var skeletonBone = new SkeletonBone
            {
                name = boneTransform.name,
                position = boneTransform.localPosition,
                rotation = boneTransform.localRotation,
                scale = boneTransform.localScale
            };
            return skeletonBone;
        }

        /// <summary>
        /// Completes the model loading process by optionally adding an <see cref="AssetUnloader"/> component 
        /// for memory tracking, discarding unused textures, and invoking the <c>OnMaterialsLoad</c> callback 
        /// if present. Finally, it calls <see cref="Cleanup"/> to free resources.
        /// </summary>
        /// <param name="assetLoaderContext">
        /// The <see cref="AssetLoaderContext"/> containing the loaded model hierarchy, resource lists,
        /// and callbacks.
        /// </param>
        private static void FinishLoading(AssetLoaderContext assetLoaderContext)
        {
            // Optionally attach an AssetUnloader to handle allocations tracking and cleanup
            if (assetLoaderContext.Options.AddAssetUnloader &&
               (assetLoaderContext.RootGameObject != null || assetLoaderContext.WrapperGameObject != null))
            {
                var gameObject = assetLoaderContext.RootGameObject ?? assetLoaderContext.WrapperGameObject;
                var assetUnloader = gameObject.AddComponent<AssetUnloader>();
                assetUnloader.Id = AssetUnloader.GetNextId();
                assetUnloader.Allocations = assetLoaderContext.Allocations;
                assetUnloader.CustomData = assetLoaderContext.CustomData;
            }

            // Discard unused textures if requested
            if (assetLoaderContext.Options.DiscardUnusedTextures)
            {
                assetLoaderContext.DiscardUnusedTextures();
            }

            // Update loading progress to 100% and invoke OnMaterialsLoad callback
            assetLoaderContext.Reader?.UpdateLoadingPercentage(
                1f,
                assetLoaderContext.Reader.LoadingStepsCount + (int)ReaderBase.PostLoadingSteps.FinishedProcessing
            );
            assetLoaderContext.OnMaterialsLoad?.Invoke(assetLoaderContext);

            // Perform final cleanup, disposing streams and possibly GC if requested
            Cleanup(assetLoaderContext);
        }

        /// <summary>
        /// Recursively gathers all <see cref="IModel"/> objects in the loaded model that are flagged as bones.
        /// This can be used, for instance, to prepare a list of all bones for a <see cref="SkinnedMeshRenderer"/>.
        /// </summary>
        /// <param name="assetLoaderContext">
        /// The <see cref="AssetLoaderContext"/> containing references to the root and all models in the hierarchy.
        /// </param>
        /// <returns>
        /// A list of all <see cref="IModel"/> instances that are designated as bones.
        /// </returns>
        private static IList<IModel> GetAllBonesRecursive(AssetLoaderContext assetLoaderContext)
        {
            var bones = new List<IModel>();
            foreach (var model in assetLoaderContext.RootModel.AllModels)
            {
                if (model.IsBone)
                {
                    bones.Add(model);
                }
            }
            return bones;
        }

        /// <summary>
        /// Retrieves the name of the specified <paramref name="humanBodyBones"/> index from Unity’s 
        /// <see cref="HumanTrait.BoneName"/> array.
        /// </summary>
        /// <param name="humanBodyBones">The humanoid bone identifier.</param>
        /// <returns>The corresponding bone name from <see cref="HumanTrait.BoneName"/>.</returns>
        private static string GetHumanBodyName(HumanBodyBones humanBodyBones)
        {
            return HumanTrait.BoneName[(int)humanBodyBones];
        }

        /// <summary>
        /// Handles errors that occur during model loading. Depending on the context, this may involve 
        /// unloading the partially loaded model, destroying the root <see cref="GameObject"/> if 
        /// <see cref="AssetLoaderOptions.DestroyOnError"/> is set, and invoking the error callback.
        /// </summary>
        /// <param name="error">The <see cref="IContextualizedError"/> describing the error that occurred.</param>
        private static void HandleError(IContextualizedError error)
        {
            var exception = error.GetInnerException();
            if (error.GetContext() is IAssetLoaderContext context)
            {
                var assetLoaderContext = context.Context;
                if (assetLoaderContext != null)
                {
                    // Perform cleanup and optionally destroy the root GameObject
                    Cleanup(assetLoaderContext);
                    if (assetLoaderContext.Options.DestroyOnError && assetLoaderContext.RootGameObject != null)
                    {
                        if (!Application.isPlaying)
                        {
                            Object.DestroyImmediate(assetLoaderContext.RootGameObject);
                        }
                        else
                        {
                            Object.Destroy(assetLoaderContext.RootGameObject);
                        }
                        assetLoaderContext.RootGameObject = null;
                    }

                    // Invoke custom error callback if provided
                    if (assetLoaderContext.OnError != null)
                    {
                        Dispatcher.InvokeAsync(assetLoaderContext.OnError, error, assetLoaderContext.Async);
                    }
                }
            }
            else
            {
                // If the context is missing or invalid, rethrow in a generic way
                var contextualizedError = new ContextualizedError<object>(exception, null);
                Rethrow(contextualizedError);
            }
        }

        /// <summary>
        /// Entry point for loading the root model data. This is an intermediate method where 
        /// additional setup or validation could be performed before <see cref="LoadModelInternal"/> 
        /// is invoked.
        /// </summary>
        /// <param name="assetLoaderContext">
        /// The <see cref="AssetLoaderContext"/> containing model data references and user-defined options.
        /// </param>
        private static void LoadModel(AssetLoaderContext assetLoaderContext)
        {
            SetupModelLoading(assetLoaderContext);
        }

        /// <summary>
        /// Begins the asynchronous or synchronous (depending on platform and configuration) model loading process.
        /// It handles special cases (e.g., a zip file extension) or delegates to <see cref="ThreadUtils"/> to perform 
        /// multi-threaded loading of the model data.
        /// </summary>
        /// <param name="assetLoaderContext">
        /// The <see cref="AssetLoaderContext"/> that contains references to the file/stream data, 
        /// loading options, callbacks, and more.
        /// </param>
        private static void LoadModelInternal(AssetLoaderContext assetLoaderContext)
        {
            // Optionally compact the LOH (Large Object Heap) if requested.
            if (assetLoaderContext.Options.CompactHeap)
            {
                GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            }

#if !TRILIB_DISABLE_VALIDATIONS
            ValidateAssetLoaderOptions(assetLoaderContext.Options);
#endif

#if TRILIB_USE_THREAD_NAMES && !UNITY_WSA
    var threadName = "TriLib_LoadModelFromStream";
#else
            string threadName = null;
#endif

            var fileExtension = assetLoaderContext.FileExtension;
            if (fileExtension == null && assetLoaderContext.Filename != null)
            {
                fileExtension = FileUtils.GetFileExtension(assetLoaderContext.Filename, false);
            }

            // If we are loading a zip file, use the Zip-specific loader.
            if (fileExtension == "zip")
            {
                AssetLoaderZip.LoadModelFromZipFile(
                    assetLoaderContext.Filename,
                    assetLoaderContext.OnLoad,
                    assetLoaderContext.OnMaterialsLoad,
                    assetLoaderContext.OnProgress,
                    assetLoaderContext.OnError,
                    assetLoaderContext.WrapperGameObject,
                    assetLoaderContext.Options,
                    assetLoaderContext.CustomData,
                    assetLoaderContext.FileExtension,
                    assetLoaderContext.HaltTasks,
                    assetLoaderContext.OnPreLoad
                );
            }
            else
            {
                // Otherwise, start a background thread (if permitted) to load the model data
                ThreadUtils.RequestNewThreadFor(
                    assetLoaderContext,
                    LoadModel,
                    ProcessRootModel,
                    HandleError,
                    assetLoaderContext.Options.Timeout,
                    threadName,
                    !assetLoaderContext.HaltTasks,
                    assetLoaderContext.OnPreLoad
                );
            }
        }

        /// <summary>
        /// Loads any remaining textures that have not been referenced yet and adds them to the
        /// <see cref="AssetLoaderContext.Allocations"/> list for resource tracking.
        /// If all textures are already loaded, the process immediately calls <see cref="FinishLoading"/>.
        /// </summary>
        /// <param name="assetLoaderContext">
        /// The <see cref="AssetLoaderContext"/> that contains model data, references to textures,
        /// and user-defined options.
        /// </param>
        private static void LoadUnusedTextures(AssetLoaderContext assetLoaderContext)
        {
            if (assetLoaderContext.RootGameObject != null)
            {
                if (assetLoaderContext.RootModel.AllTextures != null)
                {
                    foreach (var texture in assetLoaderContext.RootModel.AllTextures)
                    {
                        var textureLoadingContext = new TextureLoadingContext
                        {
                            Texture = texture,
                            Context = assetLoaderContext
                        };

                        // Check whether the texture is already loaded or combined
                        if (!assetLoaderContext.TryGetCompoundTexture(textureLoadingContext, out _))
                        {
                            // If using Unity’s native texture loader, just load the texture directly;
                            // otherwise, create and then load the texture.
                            if (assetLoaderContext.Options.UseUnityNativeTextureLoader)
                            {
                                TextureLoaders.LoadTexture(textureLoadingContext);
                            }
                            else
                            {
                                TextureLoaders.CreateTexture(textureLoadingContext);
                                TextureLoaders.LoadTexture(textureLoadingContext);
                            }
                        }
                    }
                }
            }

            // Once all unused textures are addressed, finalize the loading process
            FinishLoading(assetLoaderContext);
        }

        /// <summary>
        /// Performs any final setup or modifications to the loaded model hierarchy. 
        /// This includes adjusting the root scale if invalid, setting up LODs and bones (if necessary),
        /// and then triggering the <c>OnLoad</c> callback if present.
        /// </summary>
        /// <param name="assetLoaderContext">
        /// The <see cref="AssetLoaderContext"/> containing the root <see cref="GameObject"/> and other relevant data.
        /// </param>
        private static void PostProcessModels(AssetLoaderContext assetLoaderContext)
        {
            // Correct scale if zero to avoid issues with invisible or degenerate transforms
            if (assetLoaderContext.RootGameObject.transform.localScale.sqrMagnitude == 0f)
            {
                assetLoaderContext.RootGameObject.transform.localScale = Vector3.one;
            }

            // Set up Level of Detail for the model
            SetupModelLod(assetLoaderContext, assetLoaderContext.RootModel);

            // If animations or blend shapes are enabled, configure bones, build paths, and set up the rig
            if (assetLoaderContext.Options.AnimationType != AnimationType.None || assetLoaderContext.Options.ImportBlendShapes)
            {
                SetupModelBones(assetLoaderContext, assetLoaderContext.RootModel);
                BuildGameObjectsPaths(assetLoaderContext);
                SetupRig(assetLoaderContext);
            }

            // Mark the root GameObject as static if requested
            assetLoaderContext.RootGameObject.isStatic = assetLoaderContext.Options.Static;

            // Invoke the loading callback on the main thread
            assetLoaderContext.OnLoad?.Invoke(assetLoaderContext);
        }

        /// <summary>
        /// Iterates over all materials in the loaded model, mapping them via the available <see cref="MaterialMapper"/>
        /// implementations, and then applies the mapped materials to any associated <see cref="Renderer"/> components.
        /// </summary>
        /// <param name="assetLoaderContext">
        /// The <see cref="AssetLoaderContext"/> that holds references to all materials, their renderers, and loading options.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable"/> that yields control if any of the <see cref="MaterialMapper"/>s use coroutines for mapping.
        /// </returns>
        private static IEnumerable ProcessMaterialRenderers(AssetLoaderContext assetLoaderContext)
        {
            var materialCount = assetLoaderContext.RootModel.AllMaterials.Count;
            var materialMapperContexts = new MaterialMapperContext[materialCount];

            // Map each TriLib material to a Unity material
            for (var i = 0; i < materialCount; i++)
            {
                var material = assetLoaderContext.RootModel.AllMaterials[i];
                var materialMapperContext = new MaterialMapperContext
                {
                    Context = assetLoaderContext,
                    Material = material
                };
                materialMapperContexts[i] = materialMapperContext;

                // Find the first compatible MaterialMapper
                for (var j = 0; j < assetLoaderContext.Options.MaterialMappers.Length; j++)
                {
                    var materialMapper = assetLoaderContext.Options.MaterialMappers[j];
                    if (materialMapper != null && materialMapper.IsCompatible(materialMapperContext))
                    {
                        materialMapperContext.MaterialMapper = materialMapper;

                        // Some mappers might need coroutines to handle async logic
                        if (materialMapper.UsesCoroutines)
                        {
                            foreach (var item in materialMapper.MapCoroutine(materialMapperContext))
                            {
                                yield return item;
                            }
                        }
                        else
                        {
                            materialMapper.Map(materialMapperContext);
                        }

                        // Apply mapped material to all relevant renderers
                        ApplyMaterialToRenderers(materialMapperContext);
                        break;
                    }
                }

                // Update progress for the material mapping stage
                assetLoaderContext.Reader.UpdateLoadingPercentage(
                    i,
                    assetLoaderContext.Reader.LoadingStepsCount + (int)ReaderBase.PostLoadingSteps.PostProcessRenderers,
                    materialCount
                );
            }

            // Clean up any temporary references from the mappers
            for (var i = 0; i < materialCount; i++)
            {
                var material = assetLoaderContext.RootModel.AllMaterials[i];
                if (assetLoaderContext.MaterialRenderers.TryGetValue(material, out var materialRendererList))
                {
                    for (var j = 0; j < materialRendererList.Count; j++)
                    {
                        MaterialMapper.Cleanup(materialRendererList[j]);
                    }
                }
            }

            // Decide whether to discard or load any unused textures after material assignment
            if (assetLoaderContext.Options.DiscardUnusedTextures)
            {
                FinishLoading(assetLoaderContext);
            }
            else
            {
                LoadUnusedTextures(assetLoaderContext);
            }
        }

        /// <summary>
        /// Processes all materials within the model by mapping them to Unity materials (if any <see cref="MaterialMapper"/>s are defined),
        /// or logs a warning if no <see cref="MaterialMapper"/> is set. If no materials exist, it either discards unused textures
        /// or loads them depending on the <see cref="AssetLoaderOptions.DiscardUnusedTextures"/> flag.
        /// </summary>
        /// <param name="assetLoaderContext">
        /// The <see cref="AssetLoaderContext"/> referencing the root model, materials, and loader options.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable"/> that yields control if coroutine-based material mapping is used.
        /// Otherwise, returns immediately.
        /// </returns>
        private static IEnumerable ProcessMaterials(AssetLoaderContext assetLoaderContext)
        {
            var allMaterials = assetLoaderContext.RootModel?.AllMaterials;

            // If there are materials to process
            if (allMaterials != null && allMaterials.Count > 0)
            {
                // Use any defined MaterialMappers, or log a warning if none were provided
                if (assetLoaderContext.Options.MaterialMappers != null)
                {
                    foreach (var item in ProcessMaterialRenderers(assetLoaderContext))
                    {
                        yield return item;
                    }
                }
                else if (assetLoaderContext.Options.ShowLoadingWarnings)
                {
                    Debug.LogWarning("Please specify a TriLib Material Mapper; otherwise materials cannot be created.");
                }
            }
            else
            {
                // If no materials exist, either discard or load unused textures, then finish loading
                if (assetLoaderContext.Options.DiscardUnusedTextures)
                {
                    FinishLoading(assetLoaderContext);
                }
                else
                {
                    LoadUnusedTextures(assetLoaderContext);
                }
            }
        }

        /// <summary>
        /// Initiates the creation of the root <see cref="GameObject"/> and its sub-hierarchy (via <see cref="CreateRootModel"/>),
        /// then performs any post-processing (like material assignment) through coroutines or direct method calls 
        /// depending on the <see cref="AssetLoaderOptions.UseCoroutines"/> flag.
        /// </summary>
        /// <param name="assetLoaderContext">
        /// The <see cref="AssetLoaderContext"/> that holds model data, loader options, and callbacks.
        /// </param>
        private static void ProcessRootModel(AssetLoaderContext assetLoaderContext)
        {
            if (assetLoaderContext.RootModel != null)
            {
                // Create the hierarchy for the root model
                var enumerator = CreateRootModel(assetLoaderContext);

                // Either start a Unity coroutine or run synchronously
                if (assetLoaderContext.Options.UseCoroutines)
                {
                    CoroutineHelper.Instance.StartCoroutine(enumerator);
                }
                else
                {
                    CoroutineHelper.RunMethod(enumerator);
                }
            }
            else
            {
                // If no root model, immediately invoke OnLoad
                assetLoaderContext.OnLoad?.Invoke(assetLoaderContext);
            }
        }

        /// <summary>
        /// Rethrows the provided error on the main thread, preserving its context.
        /// </summary>
        /// <typeparam name="T">The context type associated with the error.</typeparam>
        /// <param name="contextualizedError">
        /// The <see cref="ContextualizedError{T}"/> encapsulating the exception to be rethrown.
        /// </param>
        private static void Rethrow<T>(ContextualizedError<T> contextualizedError)
        {
            throw contextualizedError;
        }

        /// <summary>
        /// Creates animation components and <see cref="AnimationClip"/> assets for the given list of TriLib animations,
        /// attaching them to the <see cref="AssetLoaderContext.RootGameObject"/>. This supports both legacy 
        /// <see cref="Animation"/> and <see cref="Animator"/>-based setups, depending on <see cref="AssetLoaderOptions.AnimationType"/>.
        /// </summary>
        /// <param name="assetLoaderContext">
        /// The <see cref="AssetLoaderContext"/> containing the model’s root <see cref="GameObject"/> and loading options.
        /// </param>
        /// <param name="animations">A list of TriLib <see cref="IAnimation"/> data to convert into Unity <see cref="AnimationClip"/>s.</param>
        /// <param name="animationClips">
        /// An output array that will contain references to the newly created <see cref="AnimationClip"/>s.
        /// </param>
        /// <param name="animator">
        /// An output parameter that will hold the newly created <see cref="Animator"/>, if applicable. 
        /// It can be <c>null</c> if using strictly legacy animation.
        /// </param>
        /// <param name="unityAnimation">
        /// An output parameter that will hold the created <see cref="Animation"/> component (legacy), 
        /// which can either coexist with or replace the <see cref="Animator"/> based on the selected configuration.
        /// </param>
        private static void SetupAnimationComponents(
            AssetLoaderContext assetLoaderContext,
            IList<IAnimation> animations,
            out AnimationClip[] animationClips,
            out Animator animator,
            out Animation unityAnimation)
        {
            // Determine if we need to create an Animator component, depending on animation type and enforcement settings
            if (assetLoaderContext.Options.AnimationType == AnimationType.Legacy && assetLoaderContext.Options.EnforceAnimatorWithLegacyAnimations
                || assetLoaderContext.Options.AnimationType != AnimationType.Legacy)
            {
                animator = assetLoaderContext.RootGameObject.AddComponent<Animator>();
            }
            else
            {
                animator = null;
            }

            // Always create a legacy Animation component
            unityAnimation = assetLoaderContext.RootGameObject.AddComponent<Animation>();
            unityAnimation.playAutomatically = assetLoaderContext.Options.AutomaticallyPlayLegacyAnimations;
            unityAnimation.wrapMode = assetLoaderContext.Options.AnimationWrapMode;

            // If there are animations to process, convert them into AnimationClips
            if (animations != null)
            {
                animationClips = new AnimationClip[animations.Count];

                for (var i = 0; i < animations.Count; i++)
                {
                    var triLibAnimation = animations[i];
                    var animationClip = CreateAnimation(assetLoaderContext, triLibAnimation);

                    // Add each clip to the legacy Animation component
                    unityAnimation.AddClip(animationClip, animationClip.name);
                    animationClips[i] = animationClip;

                    // Update load percentage for the post-processing step
                    assetLoaderContext.Reader.UpdateLoadingPercentage(
                        i,
                        assetLoaderContext.Reader.LoadingStepsCount + (int)ReaderBase.PostLoadingSteps.PostProcessAnimationClips,
                        animations.Count
                    );
                }

                // Optionally set and play the first clip if auto-play is enabled
                if (assetLoaderContext.Options.AutomaticallyPlayLegacyAnimations && animationClips.Length > 0)
                {
                    unityAnimation.clip = animationClips[0];
                    unityAnimation.Play(animationClips[0].name);
                }
            }
            else
            {
                // No animations, so the array is empty
                animationClips = null;
            }
        }
        /// <summary>
        /// Creates a generic Unity <see cref="Avatar"/> for the currently loaded model
        /// and assigns it to the specified <paramref name="animator"/> component.
        /// This method identifies and maps bones using <see cref="AssetLoaderOptions.RootBoneMapper"/>.
        /// </summary>
        /// <param name="assetLoaderContext">
        /// The <see cref="AssetLoaderContext"/> holding model data, including the root <see cref="GameObject"/> 
        /// and a collection of bones found in the model.
        /// </param>
        /// <param name="animator">
        /// The <see cref="Animator"/> attached to the model's root <see cref="GameObject"/>. 
        /// This method sets the newly created <see cref="Avatar"/> on this <paramref name="animator"/>.
        /// </param>
        private static void SetupGenericAvatar(AssetLoaderContext assetLoaderContext, Animator animator)
        {
            var parent = assetLoaderContext.RootGameObject.transform.parent;
            assetLoaderContext.RootGameObject.transform.SetParent(null, true);

            // Gather all bone transforms from the model
            var bones = new List<Transform>();
            assetLoaderContext.RootModel.GetBones(assetLoaderContext, bones);

            // Map the root bone using the configured RootBoneMapper
            var rootBone = assetLoaderContext.Options.RootBoneMapper.Map(assetLoaderContext, bones);

            // Build the generic avatar and assign it to the Animator
            var avatar = AvatarBuilder.BuildGenericAvatar(
                assetLoaderContext.RootGameObject,
                rootBone != null ? rootBone.name : ""
            );
            avatar.name = $"{assetLoaderContext.RootGameObject.name}Avatar";
            animator.avatar = avatar;

            // Restore the original parent transform
            assetLoaderContext.RootGameObject.transform.SetParent(parent, true);
        }

        /// <summary>
        /// Creates a humanoid Unity <see cref="Avatar"/> for the currently loaded model, if possible,
        /// and assigns it to the specified <paramref name="animator"/> component.
        /// This leverages <see cref="AssetLoaderOptions.HumanoidAvatarMapper"/> to perform bone mapping
        /// for the Unity humanoid rig system.
        /// </summary>
        /// <param name="assetLoaderContext">
        /// The <see cref="AssetLoaderContext"/> containing the root <see cref="GameObject"/> of the model
        /// and the associated <see cref="AssetLoaderOptions"/>.
        /// </param>
        /// <param name="animator">
        /// The <see cref="Animator"/> on the model's root <see cref="GameObject"/>, 
        /// to which the humanoid <see cref="Avatar"/> will be assigned.
        /// </param>
        /// <remarks>
        /// If the avatar creation fails (e.g., insufficient bones mapped), 
        /// a warning is logged if <see cref="AssetLoaderOptions.ShowLoadingWarnings"/> is <c>true</c>.
        /// </remarks>
        private static void SetupHumanoidAvatar(AssetLoaderContext assetLoaderContext, Animator animator)
        {
            var valid = false;

            // Mapping is performed by the user-provided HumanoidAvatarMapper
            var mapping = assetLoaderContext.Options.HumanoidAvatarMapper.Map(assetLoaderContext);

            if (mapping.Count > 0)
            {
                var parent = assetLoaderContext.RootGameObject.transform.parent;
                var rootGameObjectPosition = assetLoaderContext.RootGameObject.transform.position;
                assetLoaderContext.RootGameObject.transform.SetParent(null, false);

                // Allows custom post-processing on the mapped bones
                assetLoaderContext.Options.HumanoidAvatarMapper.PostSetup(assetLoaderContext, mapping);

                Transform hipsTransform = null;
                var humanBones = new HumanBone[mapping.Count];
                var boneIndex = 0;

                // Create a HumanBone array from the mapped data
                foreach (var kvp in mapping)
                {
                    if (kvp.Key.HumanBone == HumanBodyBones.Hips)
                    {
                        hipsTransform = kvp.Value;
                    }
                    humanBones[boneIndex++] = CreateHumanBone(kvp.Key, kvp.Value.name);
                }

                // If hips were successfully mapped, build the avatar
                if (hipsTransform != null)
                {
                    // Adjust model bounds and positions to ensure correct avatar configuration
                    var skeletonBones = new Dictionary<Transform, SkeletonBone>();
                    var bounds = assetLoaderContext.RootGameObject.CalculateBounds();

                    // Optional vertical compensation so hips are at or above ground level
                    if (assetLoaderContext.Options.ApplyAvatarHipsCompensation)
                    {
                        var toBottom = bounds.min.y;
                        if (toBottom < 0f)
                        {
                            var hipsTransformPosition = hipsTransform.position;
                            hipsTransformPosition.y -= toBottom;
                            hipsTransform.position = hipsTransformPosition;
                        }
                    }

                    // Optional horizontal compensation to recenter the model
                    var toCenter = Vector3.zero - bounds.center;
                    toCenter.y = 0f;
                    if (toCenter.sqrMagnitude > 0.01f)
                    {
                        var hipsTransformPosition = hipsTransform.position;
                        hipsTransformPosition += toCenter;
                        hipsTransform.position = hipsTransformPosition;
                    }

                    // Build a complete list of SkeletonBones for all transforms in the GameObject hierarchy
                    foreach (var kvp in assetLoaderContext.GameObjects)
                    {
                        if (!skeletonBones.ContainsKey(kvp.Value.transform))
                        {
                            skeletonBones.Add(kvp.Value.transform, CreateSkeletonBone(kvp.Value.transform));
                        }
                    }

                    // Gather human description settings from the AssetLoaderOptions, or use defaults
                    var triLibHumanDescription = assetLoaderContext.Options.HumanDescription ?? new General.HumanDescription();
                    var humanDescription = new HumanDescription
                    {
                        armStretch = triLibHumanDescription.armStretch,
                        feetSpacing = triLibHumanDescription.feetSpacing,
                        hasTranslationDoF = triLibHumanDescription.hasTranslationDof,
                        legStretch = triLibHumanDescription.legStretch,
                        lowerArmTwist = triLibHumanDescription.lowerArmTwist,
                        lowerLegTwist = triLibHumanDescription.lowerLegTwist,
                        upperArmTwist = triLibHumanDescription.upperArmTwist,
                        upperLegTwist = triLibHumanDescription.upperLegTwist,
                        skeleton = skeletonBones.Values.ToArray(),
                        human = humanBones
                    };

                    // Build the Unity humanoid avatar
                    var avatar = AvatarBuilder.BuildHumanAvatar(assetLoaderContext.RootGameObject, humanDescription);
                    avatar.name = $"{assetLoaderContext.RootGameObject.name}Avatar";
                    animator.avatar = avatar;
                }

                // Restore the original transform hierarchy
                assetLoaderContext.RootGameObject.transform.SetParent(parent, false);
                assetLoaderContext.RootGameObject.transform.position = rootGameObjectPosition;

                // Mark as valid if the avatar is recognized by Unity or warnings are disabled
                valid = animator.avatar.isValid || !assetLoaderContext.Options.ShowLoadingWarnings;
            }

            // If the avatar isn't valid, log a warning
            if (!valid)
            {
                var modelName = assetLoaderContext.Filename == null
                    ? "Unknown"
                    : FileUtils.GetShortFilename(assetLoaderContext.Filename);
                Debug.LogWarning($"Could not create an Avatar for the model \"{modelName}\"");
            }
        }

        /// <summary>
        /// Sets up the skinned mesh renderer and bone hierarchy for a single <see cref="IModel"/> node
        /// (and its children, if any). This is used for binding bones to a <see cref="SkinnedMeshRenderer"/> 
        /// so that animations can affect the mesh.
        /// </summary>
        /// <param name="assetLoaderContext">
        /// The <see cref="AssetLoaderContext"/> holding references to loaded <see cref="GameObject"/> instances
        /// and <see cref="AssetLoaderOptions"/> that specify how bones should be mapped.
        /// </param>
        /// <param name="model">
        /// The <see cref="IModel"/> whose bones and geometry data will be connected to a skinned mesh renderer, if applicable.
        /// </param>
        private static void SetupModelBones(AssetLoaderContext assetLoaderContext, IModel model)
        {
            var loadedGameObject = assetLoaderContext.GameObjects[model];
            var skinnedMeshRenderer = loadedGameObject.GetComponent<SkinnedMeshRenderer>();

            if (skinnedMeshRenderer != null)
            {
                var bones = model.Bones;
                if (bones != null && bones.Count > 0)
                {
                    // Convert model's bone references into Unity transforms
                    var gameObjectBones = new Transform[bones.Count];
                    for (var i = 0; i < bones.Count; i++)
                    {
                        var bone = bones[i];
                        gameObjectBones[i] = assetLoaderContext.GameObjects[bone].transform;
                    }

                    skinnedMeshRenderer.bones = gameObjectBones;

                    // Use the configured RootBoneMapper to identify the root bone transform
                    skinnedMeshRenderer.rootBone = assetLoaderContext.Options.RootBoneMapper.Map(
                        assetLoaderContext,
                        gameObjectBones
                    );
                }

                // Assign the model's Mesh to the skinned mesh renderer
                skinnedMeshRenderer.sharedMesh = model.GeometryGroup.Mesh;

                // If the bounding box is zero, compute local bounds for correct culling and rendering
                if (skinnedMeshRenderer.bounds.size.sqrMagnitude == 0f)
                {
                    skinnedMeshRenderer.localBounds = loadedGameObject.CalculateBounds(true);
                }
            }

            // Recursively process child models
            if (model.Children != null && model.Children.Count > 0)
            {
                for (var i = 0; i < model.Children.Count; i++)
                {
                    var subModel = model.Children[i];
                    SetupModelBones(assetLoaderContext, subModel);
                }
            }
        }

        /// <summary>
        /// Prepares the <see cref="AssetLoaderContext"/> for model loading by validating 
        /// the file or stream availability, sorting <see cref="MaterialMapper"/>s by priority, 
        /// and then initializing the root model via a suitable reader.
        /// </summary>
        /// <param name="assetLoaderContext">
        /// The <see cref="AssetLoaderContext"/> containing file paths, streams, and user options
        /// that govern how the model is loaded.
        /// </param>
        /// <exception cref="Exception">
        /// Thrown if no valid file/stream is provided, if no suitable reader can be found,
        /// or if <see cref="AssetLoaderContext.RootModel"/> cannot be loaded.
        /// </exception>
        private static void SetupModelLoading(AssetLoaderContext assetLoaderContext)
        {
            // Ensure there's a valid file path or stream
            if (assetLoaderContext.Stream == null && string.IsNullOrWhiteSpace(assetLoaderContext.Filename))
            {
                throw new Exception("TriLib is unable to load the given file.");
            }

            // Sort available MaterialMappers by descending CheckingOrder to prioritize certain mappers first
            if (assetLoaderContext.Options.MaterialMappers != null)
            {
                Array.Sort(assetLoaderContext.Options.MaterialMappers, (a, b) => a.CheckingOrder > b.CheckingOrder ? -1 : 1);
            }
            else if (assetLoaderContext.Options.ShowLoadingWarnings)
            {
                Debug.LogWarning("Your AssetLoaderOptions instance has no MaterialMappers. TriLib can't process materials without them.");
            }

#if TRILIB_DRACO
    // Configure the Draco decompressor callback if Draco is enabled
    GltfReader.DracoDecompressorCallback = DracoMeshLoader.DracoDecompressorCallback;
#endif

            // Determine the file extension
            var fileExtension = assetLoaderContext.FileExtension;
            if (fileExtension == null)
            {
                fileExtension = FileUtils.GetFileExtension(assetLoaderContext.Filename, false);
            }
            else if (fileExtension[0] == '.' && fileExtension.Length > 1)
            {
                // Remove the leading dot
                fileExtension = fileExtension.Substring(1);
            }

            // If no stream was provided, create one from the file path
            if (assetLoaderContext.Stream == null)
            {
                var fileStream = new FileStream(
                    assetLoaderContext.Filename,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read
                );
                assetLoaderContext.Stream = fileStream;

                // Attempt to find a reader for this file extension
                var reader = Readers.FindReaderForExtension(fileExtension);
                if (reader != null)
                {
                    assetLoaderContext.RootModel = reader.ReadStream(
                        fileStream,
                        assetLoaderContext,
                        assetLoaderContext.Filename,
                        assetLoaderContext.OnProgress
                    );
                }
            }
            else
            {
                // Already have a stream—just find and invoke the proper reader
                var reader = Readers.FindReaderForExtension(fileExtension);
                if (reader != null)
                {
                    assetLoaderContext.RootModel = reader.ReadStream(
                        assetLoaderContext.Stream,
                        assetLoaderContext,
                        assetLoaderContext.Filename,
                        assetLoaderContext.OnProgress
                    );
                }
                else
                {
                    throw new Exception(
                        "Could not find a suitable reader for the given model. " +
                        "Please fill the 'fileExtension' parameter when calling any model loading method."
                    );
                }

                // Confirm that RootModel was actually loaded
                if (assetLoaderContext.RootModel == null)
                {
                    throw new Exception("TriLib could not load the given model.");
                }
            }
        }

        /// <summary>
        /// Recursively configures levels-of-detail (LOD) for the specified <paramref name="model"/> by searching 
        /// for child nodes whose names match typical LOD naming patterns (e.g., "_LOD1" or "LOD_1"). If multiple
        /// LOD levels are detected, an <see cref="LODGroup"/> is added to the <see cref="GameObject"/> of 
        /// the <paramref name="model"/>, and individual <see cref="Renderer"/> components are assigned 
        /// to the appropriate LOD entries.
        /// </summary>
        /// <param name="assetLoaderContext">
        /// The <see cref="AssetLoaderContext"/> containing the root <see cref="GameObject"/> references and
        /// loader options (e.g., <see cref="AssetLoaderOptions.LODScreenRelativeTransitionHeightBase"/>).
        /// </param>
        /// <param name="model">
        /// The <see cref="IModel"/> whose children are examined for LOD naming patterns and possibly 
        /// aggregated into a new <see cref="LODGroup"/>.
        /// </param>
        private static void SetupModelLod(AssetLoaderContext assetLoaderContext, IModel model)
        {
            if (model.Children != null && model.Children.Count > 0)
            {
                // LOD dictionary grouped by LOD index
                var lodModels = new Dictionary<int, List<Renderer>>(model.Children.Count);
                var minLod = int.MaxValue;
                var maxLod = 0;

                // Search child models for LOD naming patterns and gather renderers
                for (var i = 0; i < model.Children.Count; i++)
                {
                    var child = model.Children[i];
                    var match = Regex.Match(child.Name, "_LOD(?<number>[0-9]+)|LOD_(?<number>[0-9]+)");
                    if (match.Success)
                    {
                        var lodNumber = Convert.ToInt32(match.Groups["number"].Value);
                        minLod = Mathf.Min(lodNumber, minLod);
                        maxLod = Mathf.Max(lodNumber, maxLod);

                        if (!lodModels.TryGetValue(lodNumber, out var renderers))
                        {
                            renderers = new List<Renderer>();
                            lodModels.Add(lodNumber, renderers);
                        }

                        // Collect all Renderers on this child
                        renderers.AddRange(
                            assetLoaderContext.GameObjects[child].GetComponentsInChildren<Renderer>()
                        );
                    }
                }

                // If more than one LOD level is found, create an LODGroup on the model’s GameObject
                if (lodModels.Count > 1)
                {
                    var newGameObject = assetLoaderContext.GameObjects[model];
                    var lods = new List<LOD>(lodModels.Count + 1);
                    var lodGroup = newGameObject.AddComponent<LODGroup>();
                    var lastPosition = assetLoaderContext.Options.LODScreenRelativeTransitionHeightBase;

                    // Create LOD entries in ascending order
                    for (var i = minLod; i <= maxLod; i++)
                    {
                        if (lodModels.TryGetValue(i, out var renderers))
                        {
                            lods.Add(new LOD(lastPosition, renderers.ToArray()));
                            lastPosition *= 0.5f;
                        }
                    }
                    lodGroup.SetLODs(lods.ToArray());
                }

                // Recursively process children for nested or separate LOD sets
                for (var i = 0; i < model.Children.Count; i++)
                {
                    var child = model.Children[i];
                    SetupModelLod(assetLoaderContext, child);
                }
            }
        }

        /// <summary>
        /// Configures the rig for the loaded model based on the selected <see cref="AnimationType"/> and 
        /// <see cref="AvatarDefinitionType"/>. If the model has animations, this method generates the relevant 
        /// animation components and optionally creates or assigns a suitable <see cref="Avatar"/> 
        /// (generic or humanoid).
        /// </summary>
        /// <param name="assetLoaderContext">
        /// The <see cref="AssetLoaderContext"/> containing references to the root model, animations,
        /// and loader options (e.g., <see cref="AnimationType"/>, <see cref="AvatarDefinitionType"/>).
        /// </param>
        private static void SetupRig(AssetLoaderContext assetLoaderContext)
        {
            var animations = assetLoaderContext.RootModel.AllAnimations;
            AnimationClip[] animationClips = null;

            // If we have humanoid or if any animations exist, proceed to configure rig/animations
            if (assetLoaderContext.Options.AnimationType == AnimationType.Humanoid
                || (animations != null && animations.Count > 0))
            {
                switch (assetLoaderContext.Options.AnimationType)
                {
                    case AnimationType.Legacy:
                        {
                            // Creates legacy animation components
                            SetupAnimationComponents(assetLoaderContext, animations, out animationClips, out var animator, out var unityAnimation);
                            break;
                        }
                    case AnimationType.Generic:
                        {
                            // Creates animation components, sets a generic avatar (or copies from another)
                            SetupAnimationComponents(assetLoaderContext, animations, out animationClips, out var animator, out var unityAnimation);

                            if (assetLoaderContext.Options.AvatarDefinition == AvatarDefinitionType.CopyFromOtherAvatar)
                            {
                                animator.avatar = assetLoaderContext.Options.Avatar;
                            }
                            else
                            {
                                SetupGenericAvatar(assetLoaderContext, animator);
                            }
                            break;
                        }
                    case AnimationType.Humanoid:
                        {
                            // Creates animation components, sets a humanoid avatar (or copies from another)
                            SetupAnimationComponents(assetLoaderContext, animations, out animationClips, out var animator, out var unityAnimation);

                            if (assetLoaderContext.Options.AvatarDefinition == AvatarDefinitionType.CopyFromOtherAvatar)
                            {
                                animator.avatar = assetLoaderContext.Options.Avatar;
                            }
                            else if (assetLoaderContext.Options.HumanoidAvatarMapper != null)
                            {
                                SetupHumanoidAvatar(assetLoaderContext, animator);
                            }
                            break;
                        }
                }

                // Optionally apply user-defined clip mappers to the resulting animation clips
                if (animationClips != null)
                {
                    if (assetLoaderContext.Options.AnimationClipMappers != null)
                    {
                        // Sort the mappers by priority (CheckingOrder) and apply the first that modifies the clips
                        Array.Sort(assetLoaderContext.Options.AnimationClipMappers, (a, b) => a.CheckingOrder > b.CheckingOrder ? -1 : 1);
                        foreach (var animationClipMapper in assetLoaderContext.Options.AnimationClipMappers)
                        {
                            animationClips = animationClipMapper.MapArray(assetLoaderContext, animationClips);
                            if (animationClips != null && animationClips.Length > 0)
                            {
                                break;
                            }
                        }
                    }

                    // Add the final clips to the allocations list for potential resource management
                    for (var i = 0; i < animationClips.Length; i++)
                    {
                        var animationClip = animationClips[i];
                        assetLoaderContext.Allocations.Add(animationClip);
                    }
                }
            }
        }

        /// <summary>
        /// Performs validation checks against the specified <paramref name="assetLoaderOptions"/>, 
        /// logging warnings for any potentially problematic settings based on the current platform
        /// (e.g., IL2CPP restrictions, SRGB textures in HDRP/URP, etc.).
        /// </summary>
        /// <param name="assetLoaderOptions">
        /// The <see cref="AssetLoaderOptions"/> to validate before loading assets.
        /// </param>
        private static void ValidateAssetLoaderOptions(AssetLoaderOptions assetLoaderOptions)
        {
            // Warn if using SRGB textures in URP or HDRP
            if ((GraphicsSettingsUtils.IsUsingUniversalPipeline || GraphicsSettingsUtils.IsUsingHDRPPipeline)
                && assetLoaderOptions.LoadTexturesAsSRGB)
            {
                Debug.LogWarning("Textures must be loaded as Linear on UniversalRP and HDRP.");
                Debug.LogWarning(ValidationMessage);
                assetLoaderOptions.LoadTexturesAsSRGB = false;
                assetLoaderOptions.ApplyGammaCurveToMaterialColors = false;
            }

            // Warn about advanced texture settings that may cause memory usage issues on some platforms
            if (assetLoaderOptions.GetCompatibleTextureFormat || assetLoaderOptions.EnforceAlphaChannelTextures)
            {
                Debug.LogWarning(
                    "On some platforms, disabling the 'AssetLoaderOptions.GetCompatibleTextureFormat' " +
                    "and 'AssetLoaderOptions.EnforceAlphaChannelTextures' options can improve memory usage, " +
                    "but it could cause issues on other platforms. Turn off these options and test your loader routine."
                );
                Debug.LogWarning(ValidationMessage);
            }

            // Suggest using Unity’s native texture loader for performance reasons
            if (!assetLoaderOptions.UseUnityNativeTextureLoader && !assetLoaderOptions.LoadTexturesViaWebRequest)
            {
                Debug.LogWarning(
                    "Please consider enabling 'AssetLoaderOptions.UseUnityNativeTextureLoader'. " +
                    "This restricts accepted file formats to PNG and JPG but is faster than TriLib's StbImage implementation."
                );
                Debug.LogWarning(ValidationMessage);
            }

            // Suggest using Unity’s normal calculator for faster performance
            if (!assetLoaderOptions.UseUnityNativeNormalCalculator)
            {
                Debug.LogWarning(
                    "Please consider enabling 'AssetLoaderOptions.UseUnityNativeNormalCalculator'. " +
                    "This is a simpler but faster normal calculation algorithm than TriLib's default."
                );
                Debug.LogWarning(ValidationMessage);
            }

#if !UNITY_WSA
            // Warn that heap compaction is only relevant on UWP
            if (assetLoaderOptions.CompactHeap)
            {
                Debug.LogWarning("Heap compaction only works on UWP builds. You can safely disable 'AssetLoaderOptions.CompactHeap'.");
                Debug.LogWarning(ValidationMessage);
            }
#endif

#if UNITY_ANDROID || UNITY_IOS || UNITY_WSA
    // Warn about embedded data extraction on mobile/UWP due to file system restrictions
    if (assetLoaderOptions.ExtractEmbeddedData)
    {
        Debug.LogWarning(
            "You have enabled TriLib embedded data caching. Some platforms like Android, iOS, " +
            "and UWP require special permission to access temporary file folders. " +
            "If you are encountering file-system errors when loading your models, " +
            "try setting 'AssetLoaderOptions.ExtractEmbeddedData' to false."
        );
        Debug.LogWarning(ValidationMessage);
    }
#endif
        }

    }
}