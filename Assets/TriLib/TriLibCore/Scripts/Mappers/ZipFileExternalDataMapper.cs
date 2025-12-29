using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using TriLibCore.Utils;
using UnityEngine;

namespace TriLibCore.Mappers
{
    /// <summary>
    /// Provides an external data mapping strategy for extracting data from Zip files.
    /// This mapper searches through the entries of a Zip file (provided via custom context data)
    /// for an entry whose short filename matches the specified <paramref name="originalFilename"/>.
    /// If a match is found, it opens a stream for that Zip entry.
    /// </summary>
    [CreateAssetMenu(menuName = "TriLib/Mappers/External Data/Zip File External Data Mapper")]
    public class ZipFileExternalDataMapper : ExternalDataMapper
    {
        /// <inheritdoc />
        public override Stream Map(AssetLoaderContext assetLoaderContext, string originalFilename, out string finalPath)
        {
            var zipLoadCustomContextData = CustomDataHelper.GetCustomData<ZipLoadCustomContextData>(assetLoaderContext.CustomData);
            if (zipLoadCustomContextData == null)
            {
                finalPath = null;
                return null;
            }

            var zipFile = zipLoadCustomContextData.ZipFile;
            if (zipFile == null)
            {
                throw new Exception("Zip file instance is null.");
            }

            var shortFileName = FileUtils.GetShortFilename(originalFilename).Trim().ToLowerInvariant();
            foreach (ZipEntry zipEntry in zipFile)
            {
                if (!zipEntry.IsFile)
                {
                    continue;
                }
                var checkingFileShortName = FileUtils.GetShortFilename(zipEntry.Name).Trim().ToLowerInvariant();
                if (shortFileName == checkingFileShortName)
                {
                    finalPath = zipFile.Name;
                    string _;
                    return AssetLoaderZip.ZipFileEntryToStream(out _, zipEntry, zipFile);
                }
            }
            finalPath = null;
            return null;
        }
    }
}
