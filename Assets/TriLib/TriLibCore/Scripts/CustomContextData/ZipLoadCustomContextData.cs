using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace TriLibCore
{
    /// <summary>
    /// Provides additional context data to the <see cref="AssetLoaderContext"/> when loading models 
    /// from a <c>.zip</c> archive. This includes references to the <see cref="ZipFile"/>, 
    /// its corresponding <see cref="ZipEntry"/>, the underlying <see cref="Stream"/>, 
    /// and optional callbacks for errors and material-loading events.
    /// </summary>
    public class ZipLoadCustomContextData
    {
        /// <summary>
        /// The <see cref="ZipFile"/> object representing the entire archive from which the model is extracted.
        /// </summary>
        public ZipFile ZipFile;

        /// <summary>
        /// The individual <see cref="ZipEntry"/> that corresponds to the model file within the archive.
        /// May be <c>null</c> if searching for recognized model extensions or if multiple entries are processed.
        /// </summary>
        public ZipEntry ZipEntry;

        /// <summary>
        /// The <see cref="Stream"/> used to access the contents of the .zip file. 
        /// This is opened at the start of loading and may be closed automatically 
        /// once loading is finished, depending on <see cref="AssetLoaderOptions.CloseStreamAutomatically"/>.
        /// </summary>
        public Stream Stream;

        /// <summary>
        /// A reference to the original error callback passed during zip-based model loading, 
        /// allowing custom handlers to be invoked if an error occurs at any point in the load process.
        /// </summary>
        public Action<IContextualizedError> OnError;

        /// <summary>
        /// A reference to the original material load callback passed during zip-based model loading. 
        /// This is invoked on the main thread once all model materials, textures, and other resources 
        /// have successfully loaded.
        /// </summary>
        public Action<AssetLoaderContext> OnMaterialsLoad;
    }
}
