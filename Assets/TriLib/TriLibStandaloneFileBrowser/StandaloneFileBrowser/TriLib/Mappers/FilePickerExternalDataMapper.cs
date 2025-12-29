using System;
using System.Collections.Generic;
using System.IO;
using TriLibCore.SFB;
using TriLibCore.Utils;
using UnityEngine;

namespace TriLibCore.Mappers
{
    /// <summary>
    /// Provides an external data mapping strategy for file picker–based workflows.
    /// This mapper searches through a collection of file items (each with an associated stream)
    /// to find one whose short filename matches the given <paramref name="originalFilename"/>.
    /// If a match is found, the file’s stream is returned, along with its full name as the final path.
    /// </summary>

    [CreateAssetMenu(menuName = "TriLib/Mappers/External Data/File Picker External Data Mapper")]
    public class FilePickerExternalDataMapper : ExternalDataMapper
    {
        /// <inheritdoc />
        public override Stream Map(AssetLoaderContext assetLoaderContext, string originalFilename, out string finalPath)
        {
            if (!string.IsNullOrEmpty(originalFilename))
            {
                var itemsWithStream = CustomDataHelper.GetCustomData<IList<ItemWithStream>>(assetLoaderContext.CustomData);
                if (itemsWithStream != null)
                {
                    var shortFileName = FileUtils.GetShortFilename(originalFilename).Trim().ToLowerInvariant();
                    foreach (var itemWithStream in itemsWithStream)
                    {
                        if (!itemWithStream.HasData)
                        {
                            continue;
                        }

                        var checkingFileShortName = FileUtils.GetShortFilename(itemWithStream.Name).Trim().ToLowerInvariant();
                        if (shortFileName == checkingFileShortName)
                        {
                            finalPath = itemWithStream.Name;
                            return itemWithStream.OpenStream();
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Missing custom context data.");
                }
            }
            finalPath = null;
            return null;
        }
    }
}
