using System;
using System.Collections.Generic;
using System.Numerics;
using Assimp;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using StbImageSharp;
using TextureWrapMode = OpenTK.Graphics.OpenGL.TextureWrapMode;

namespace _3dViewer
{
    public class CusModel
    {
        private List<Texture> texturesLoaded = new List<Texture>();
        private List<CusMesh> meshes = new List<CusMesh>();
        private string directory;
        private bool gammaCorrection;

        public CusModel(string path, bool gamma = false)
        {
            gammaCorrection = gamma;
            LoadModel(path);
        }

        // draws the model, and thus all its meshes
        public void Draw(Shader shader)
        {
            foreach (var mesh in meshes)
                mesh.Draw(shader);
        }

        // loads a model with supported ASSIMP extensions from file and stores the resulting meshes in the meshes vector.
        private void LoadModel(string path)
        {
            AssimpContext importer = new AssimpContext();
            Scene scene = importer.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.GenerateSmoothNormals | PostProcessSteps.FlipUVs | PostProcessSteps.CalculateTangentSpace);

            if (scene == null || scene.SceneFlags != SceneFlags.Incomplete || scene.RootNode == null)
            {
                MessageBox.Show(" ERROR::ASSIMP::File : " + Path.GetFileName(path));
                return;
            }

            directory = Path.GetDirectoryName(path);
            ProcessNode(scene.RootNode, scene);
        }

        // processes a node in a recursive fashion. Processes each individual mesh located at the node and repeats this process on its children nodes (if any).
        private void ProcessNode(Node node, Scene scene)
        {
            // process each mesh located at the current node
            for (int i = 0; i < node.MeshCount; i++)
            {
                Mesh mesh = scene.Meshes[node.MeshIndices[i]];
                meshes.Add(ProcessMesh(mesh, scene));
            }

            for (int i = 0; i < node.ChildCount; i++)
            {
                ProcessNode(node.Children[i], scene);
            }
        }

        private CusMesh ProcessMesh(Assimp.Mesh mesh, Scene scene)
        {
            List<Vertex> vertices = new List<Vertex>();
            List<int> indices = new List<int>();
            List<Texture> textures = new List<Texture>();

            for (int i = 0; i < mesh.VertexCount; i++)
            {
                Vertex vertex = new Vertex();

                Vector3D position = mesh.Vertices[i];
                vertex.Position = new Vector3(position.X, position.Y, position.Z);

                if (mesh.HasNormals)
                {
                    Vector3D normal = mesh.Normals[i];
                    vertex.Normal = new Vector3(normal.X, normal.Y, normal.Z);
                }

                if (mesh.TextureCoordinateChannelCount > 0)
                {
                    Vector3D texCoord = mesh.TextureCoordinateChannels[0][i];
                    vertex.TexCoords = new Vector2(texCoord.X, texCoord.Y);
                }

                if (mesh.HasTangentBasis)
                {
                    Vector3D tangent = mesh.Tangents[i];
                    vertex.Tangent = new Vector3(tangent.X, tangent.Y, tangent.Z);

                    Vector3D bitangent = mesh.BiTangents[i];
                    vertex.Bitangent = new Vector3(bitangent.X, bitangent.Y, bitangent.Z);
                }

                vertices.Add(vertex);
            }

            for (int i = 0; i < mesh.FaceCount; i++)
            {
                Face face = mesh.Faces[i];
                foreach (int index in face.Indices)
                {
                    indices.Add(index);
                }
            }

            Material material = scene.Materials[mesh.MaterialIndex];

            List<Texture> diffuseMaps = LoadMaterialTextures(material, TextureType.Diffuse, "texture_diffuse");
            textures.AddRange(diffuseMaps);

            List<Texture> specularMaps = LoadMaterialTextures(material, TextureType.Specular, "texture_specular");
            textures.AddRange(specularMaps);

            List<Texture> normalMaps = LoadMaterialTextures(material, TextureType.Height, "texture_normal");
            textures.AddRange(normalMaps);

            List<Texture> heightMaps = LoadMaterialTextures(material, TextureType.Ambient, "texture_height");
            textures.AddRange(heightMaps);

            return new CusMesh(vertices, indices, textures);
        }

        private List<Texture> LoadMaterialTextures(Material mat, TextureType type, string typeName)
        {
            List<Texture> textures = new List<Texture>();
            for (int i = 0; i < mat.GetMaterialTextureCount(type); i++)
            {
                TextureSlot slot;
                if (mat.GetMaterialTexture(type, i, out slot))
                {
                    bool skip = false;
                    foreach (Texture texture in texturesLoaded)
                    {
                        if (texture.Path == slot.FilePath)
                        {
                            textures.Add(texture);
                            skip = true;
                            break;
                        }
                    }

                    if (!skip)
                    {
                        Texture texture = new Texture();
                        texture.ID = TextureFromFile(slot.FilePath, directory);
                        texture.Type = typeName;
                        texture.Path = slot.FilePath;
                        textures.Add(texture);
                        texturesLoaded.Add(texture);
                    }
                }
            }
            return textures;
        }

        private int TextureFromFile(string path, string directory)
        {
            string filename = System.IO.Path.Combine(directory, path);
            int textureID = GL.GenTexture();
            int width, height, nrComponents;
            byte[] data;
            // reade 
            using (var stream = File.OpenRead(path))
            {
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                width = image.Width;
                height = image.Height;
                nrComponents = (int)image.Comp;
                data = image.Data;
            }

            if (data != null)
            {
                PixelFormat format = PixelFormat.Rgb;
                PixelInternalFormat internalFormat = PixelInternalFormat.Rgba;
                if (nrComponents == 1)
                {
                    format = PixelFormat.Red;
                    internalFormat = PixelInternalFormat.R8;
                }
                else if (nrComponents == 3)
                {
                    format = PixelFormat.Rgb;
                    internalFormat = PixelInternalFormat.Rgb;
                }
                else if (nrComponents == 4)
                {
                    format = PixelFormat.Rgba;
                    internalFormat = PixelInternalFormat.Rgba;
                }


                GL.BindTexture(TextureTarget.Texture2D, textureID);
                GL.TexImage2D(TextureTarget.Texture2D, 0, internalFormat, width, height, 0, format, PixelType.UnsignedByte, data);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                return textureID;
            }
            else
            {
                MessageBox.Show("Texture failed to load at path: " + path);
                return 0;
            }
        }
    }

}
