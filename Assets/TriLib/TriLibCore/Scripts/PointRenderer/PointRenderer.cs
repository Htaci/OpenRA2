using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace TriLibCore.Extras
{
    /// <summary>
    /// Renders point clouds using a custom shader and generated textures. This component is designed to
    /// work on all Unity platforms. It creates textures from the mesh’s vertex colors and positions and
    /// uses them to render the point cloud via a custom material.
    /// </summary>
    public class PointRenderer : MonoBehaviour
    {
        /// <summary>
        /// The size of each rendered point in world units.
        /// </summary>
        public float PointSize = 0.01f;

        /// <summary>
        /// The texture that stores the colors of the mesh vertices.
        /// </summary>
        private Texture2D _colorTexture;

        /// <summary>
        /// The texture that stores the positions of the mesh vertices as Vector4 values.
        /// </summary>
        private Texture2D _positionTexture;

        /// <summary>
        /// Gets the material used to render the point cloud.
        /// </summary>
        public Material Material { get; private set; }

        /// <summary>
        /// Gets the generated mesh that is used to draw the point cloud. This mesh is required to simulate
        /// DrawProceduralNow because WebGL does not support the SV_VertexID semantic.
        /// </summary>
        public Mesh Mesh { get; private set; }

        /// <summary>
        /// Initializes the point renderer with the given mesh data. This method creates two textures from the
        /// mesh (one for vertex colors and one for vertex positions), configures the rendering material, and
        /// creates a compatible mesh for rendering the point cloud.
        /// </summary>
        /// <param name="mesh">
        /// The source mesh whose vertex data (positions and colors) will be used to render the point cloud.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the provided <paramref name="mesh"/> is null.
        /// </exception>
        public void Initialize(Mesh mesh)
        {
            if (mesh == null)
            {
                throw new ArgumentNullException(nameof(mesh));
            }

            // Compute texture resolution based on the number of vertices.
            var textureResolution = Mathf.CeilToInt(Mathf.Sqrt(mesh.vertexCount));

            // Create the color texture with RGBA32 format.
            _colorTexture = new Texture2D(textureResolution, textureResolution, TextureFormat.RGBA32, false);
            var colorData = _colorTexture.GetRawTextureData<Color32>();

            // Create the position texture with RGBAFloat format.
            _positionTexture = new Texture2D(textureResolution, textureResolution, TextureFormat.RGBAFloat, false);
            var positionData = _positionTexture.GetRawTextureData<Vector4>();

            // Retrieve vertices and colors from the mesh.
            var vertices = mesh.vertices;
            var colors = mesh.colors32;
            if (colors.Length < vertices.Length)
            {
                // Generate default white color if no colors are provided.
                colors = new Color32[vertices.Length];
                for (var i = 0; i < colors.Length; i++)
                {
                    colors[i] = new Color32(255, 255, 255, 255);
                }
            }

            // Copy vertex colors and positions to the textures.
            for (var i = 0; i < mesh.vertexCount; i++)
            {
                colorData[i] = colors[i];
                positionData[i] = vertices[i];
            }

            // Apply the texture changes and mark them as non-readable.
            _colorTexture.Apply(false, true);
            _positionTexture.Apply(false, true);

            // Create the material using a hidden shader for point rendering.
            Material = new Material(Shader.Find("Hidden/TriLib/PointRenderer"));
            Material.SetTexture("_ColorTex", _colorTexture);
            Material.SetTexture("_PositionTex", _positionTexture);
            Material.SetInt("_TextureResolution", textureResolution);

            // Create a mesh suitable for point cloud rendering using quads.
            Mesh = CreateCompatibleMesh(MeshTopology.Quads, mesh.vertexCount * 4);
        }

        /// <summary>
        /// Creates a new mesh that simulates procedural drawing. 
        /// This is necessary to workaround the lack of SV_VertexID semantic in WebGL.
        /// </summary>
        /// <param name="meshTopology">
        /// The mesh topology (e.g., Triangles, Quads, Lines) for the generated mesh.
        /// </param>
        /// <param name="vertexCount">
        /// The total number of vertices for the created mesh.
        /// </param>
        /// <returns>
        /// A new <see cref="Mesh"/> that uses the specified topology and vertex count.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the specified <paramref name="meshTopology"/> is not supported.
        /// </exception>
        private static Mesh CreateCompatibleMesh(MeshTopology meshTopology, int vertexCount)
        {
            var mesh = new Mesh();
            mesh.subMeshCount = 1;
            // Use 32-bit indices if necessary.
            mesh.indexFormat = vertexCount > ushort.MaxValue ? IndexFormat.UInt32 : IndexFormat.UInt16;
            mesh.vertices = new Vector3[vertexCount];
            var uv = new Vector2[vertexCount];
            // Initialize the UVs with dummy values.
            for (var i = 0; i < vertexCount; i++)
            {
                uv[i] = new Vector2(i, 0);
            }
            mesh.uv = uv;
            var indices = new List<int>(vertexCount);
            switch (meshTopology)
            {
                case MeshTopology.Triangles:
                    for (var i = 0; i < vertexCount; i += 3)
                    {
                        indices.Add(i + 0);
                        indices.Add(i + 1);
                        indices.Add(i + 2);
                    }
                    break;
                case MeshTopology.Quads:
                    for (var i = 0; i < vertexCount; i += 4)
                    {
                        indices.Add(i + 0);
                        indices.Add(i + 1);
                        indices.Add(i + 2);
                        indices.Add(i + 3);
                    }
                    break;
                case MeshTopology.Lines:
                    for (var i = 0; i < vertexCount; i += 2)
                    {
                        indices.Add(i + 0);
                        indices.Add(i + 1);
                    }
                    break;
                case MeshTopology.LineStrip:
                case MeshTopology.Points:
                    for (var i = 0; i < vertexCount; i++)
                    {
                        indices.Add(i);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(meshTopology), meshTopology, null);
            }
            mesh.SetIndices(indices, 0, indices.Count, meshTopology, 0);
            mesh.UploadMeshData(true);
            return mesh;
        }

        /// <summary>
        /// Renders the point cloud using the previously created mesh and custom material.
        /// Called by Unity during the rendering phase.
        /// </summary>
        private void OnRenderObject()
        {
            // Calculate the aspect ratio of the screen and pass it to the shader.
            var aspectRatio = (float)Screen.width / Screen.height;
            Material.SetFloat("_AspectRatio", aspectRatio);
            Material.SetFloat("_PointSize", PointSize);
            Material.SetPass(0);
            // Draw the mesh with the current transformation.
            Graphics.DrawMeshNow(Mesh, transform.localToWorldMatrix);
        }
    }
}
