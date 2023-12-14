using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.DataFormats;
using OpenTK.Graphics.OpenGL;
using System.Numerics;
using System.Runtime.InteropServices;

namespace _3dViewer
{
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexCoords;
        public Vector3 Tangent;
        public Vector3 Bitangent;
        //bone indexes which will influence this vertex
        public int[] BoneIDs = new int[4];
        //weights from each bone
        public float[] Weights = new int[4];
    };

   public struct Texture
    {
        public int ID;
        public string Type;
        public string Path;
    };

    public class CusMesh
    {
        private List<Vertex> vertices;
        private List<int> indices;
        private List<Texture> textures;
        private int VAO, VBO, EBO;

        public CusMesh(List<Vertex> vertices, List<int> indices, List<Texture> textures)
        {
            this.vertices = vertices;
            this.indices = indices;
            this.textures = textures;
            // now that we have all the required data, set the vertex buffers and its attribute pointers.
            SetupMesh();
        }

        // render the mesh
        public void Draw(Shader shader)
        {
            // bind appropriate textures
            int diffuseNr = 1;
            int specularNr = 1;
            int normalNr = 1;
            int heightNr = 1;

            for (int i = 0; i < textures.Count; i++)
            {
                // active proper texture unit before binding
                GL.ActiveTexture(TextureUnit.Texture0 + i);
                // retrieve texture number (the N in diffuse_textureN)
                string number = "";
                string name = textures[i].Type;

                if (name == "texture_diffuse")
                    number = (diffuseNr++).ToString();
                else if (name == "texture_specular")
                    number = (specularNr++).ToString();
                else if (name == "texture_normal")
                    number = (normalNr++).ToString();
                else if (name == "texture_height")
                    number = (heightNr++).ToString();

                // now set the sampler to the correct texture unit
                int uniformLocation = GL.GetUniformLocation(shader.Handle, name + number);
                GL.Uniform1(uniformLocation, i);
                // and finally bind the texture
                GL.BindTexture(TextureTarget.Texture2D, textures[i].ID);
            }

            // draw mesh
            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, indices.Count, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            // always good practice to set everything back to defaults once configured.
            GL.ActiveTexture(TextureUnit.Texture0);
        }

        // initializes all the buffer objects/arrays
        private void SetupMesh()
        {
            // Create VAO, VBO, and EBO
            GL.GenVertexArrays(1, out VAO);
            GL.GenBuffers(1, out VBO);
            GL.GenBuffers(1, out EBO);

            GL.BindVertexArray(VAO);

            //
            int VertexSize = 2 * 3 * sizeof(float) + 2 * sizeof(float) + 2 * 3 * sizeof(float) + 2 * 4 * sizeof(int);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * VertexSize, vertices.ToArray(), BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(int), indices.ToArray(), BufferUsageHint.StaticDraw);

            // Set vertex attribute pointers
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, VertexSize, 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, VertexSize, 3 * sizeof(float));

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, VertexSize, 2 * 3 * sizeof(float));

            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, VertexSize, 2 * 3 * sizeof(float) + 2 * sizeof(float));

            GL.EnableVertexAttribArray(4);
            GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, VertexSize, 2 * 3 * sizeof(float) + 2 * sizeof(float) + 3 * sizeof(float));

            GL.EnableVertexAttribArray(5);
            GL.VertexAttribIPointer(5, 4, VertexAttribIntegerType.Int, VertexSize, (IntPtr)(2 * 3 * sizeof(float) + 2 * sizeof(float) + 2 *3 * sizeof(float)));

            GL.EnableVertexAttribArray(6);
            GL.VertexAttribPointer(6, 4, VertexAttribPointerType.Float, false, VertexSize, 2 * 3 * sizeof(float) + 2 * sizeof(float) + 2 * 3 * sizeof(float) + 4 * sizeof(int));

            GL.BindVertexArray(0);
        }
    }

}
