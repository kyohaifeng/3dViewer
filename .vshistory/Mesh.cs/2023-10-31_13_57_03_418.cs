using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.DataFormats;
using OpenTK.Graphics.OpenGL;
using System.Numerics;

namespace _3dViewer
{
   public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector3 TexCoords;
    };

   public struct Texture
    {
        public int id;
        public string type;
    };
    public class Mesh
    {
        public List<Vertex> vertices;
        public List<uint> indices;
        public List<Texture> textures;

        private int _VAO, _VBO, _EBO;
        public Mesh(List<Vertex> vertices, List<uint> indices, List<Texture> textures)
        {
            this.vertices = vertices;
            this.indices = indices;
            this.textures = textures;
            setupMesh();
        }

        private void setupMesh()
        {

        }
    }
}
