using Assimp;
using Assimp.Unmanaged;
using System.Numerics;


namespace _3dViewer
{
    public class CusModel
    {
        private List<CusMesh> meshes;
        private string? directory;

        public CusModel(string path)
        {
            loadModel(path);
        }

        private void loadModel(string path)
        {
            try
            {

                AssimpContext assimpContext = new AssimpContext();
                // Import the OBJ file
                Scene scene = assimpContext.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.JoinIdenticalVertices);
                // check for errors
                if (scene == null || scene.RootNode == null)
                {
                    MessageBox.Show("Failed to import :{0}", path);
                }
                // set directory
                directory = Path.GetDirectoryName(path);
                // process ASSIMP's root node recursively
                processNode(scene.RootNode, scene);

            }
            catch(Exception ex){
                throw new Exception(ex.Message);
            }
            
        }
        private void processNode(Node node,Scene scene)
        {
            // process each mesh located at the current node
            if (node.HasMeshes)
            {
                for(int i =0; i<node.MeshCount; i++)
                {
                    Mesh mesh = scene.Meshes[i];
                    meshes.Add(processMesh(mesh, scene);
                }
            }

            // after we've processed all of the meshes (if any) we then recursively process each of the children nodes
            if (node.HasChildren)
            {
                for (int i = 0; i < node.ChildCount; i++)
                {
                    processNode(node.Children[i], scene);
                }
            }
            
        }

        private CusMesh processMesh(Mesh mesh,Scene scene)
        {
            // data to fill
            List<Vertex> vertices = new List<Vertex>();
            List<uint> indices = new List<uint>();
            List<Texture> textures = new List<Texture>();


            // walk through each of the mesh's vertices
            for(int i = 0; i < mesh.Vertices.Count; i++)
            {
                Vertex vertex;
                Vector3 vector;
                // vertice
                vector.X = mesh.Vertices[i].X;
                vector.Y = mesh.Vertices[i].Y;
                vector.Z = mesh.Vertices[i].Z;
                vertex.Position = vector;

                //normals
                if (mesh.HasNormals)
                {
                    vector.X = mesh.Normals[i].X;
                    vector.Y = mesh.Normals[i].Y;
                    vector.Z = mesh.Normals[i].Z;
                    vertex.Normal = vector;
                }
                // texture coordinates
                if (mesh.HasTextureCoords(0))
                {
                    Vector2 vec;
                    List<Vector3D> vector3Ds=mesh.TextureCoordinateChannels[0];

                    vec.X = vector3Ds[i].X;
                    vec.Y = vector3Ds[i].Y;

                    vertex.TexCoords = vec;
                    //tangent

                    // bitangent
                }
                else
                {
                    vertex.TexCoords = new Vector2(0.0f, 0.0f);
                }

                vertices.Add(vertex);
            }

        }


    }
}
