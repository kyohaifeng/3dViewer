using Assimp;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.WinForms;
using System.ComponentModel.Design;

namespace _3dViewer
{
    public partial class MainForm : Form
    {
        public CusModel cusModel;

        private Camera _camera;
        private Shader _shader;

        public MainForm()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create and configure an OpenFileDialog instance
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Open File"; // Dialog title
                openFileDialog.Filter = "Obj Files|*.obj|All Files|*.*"; // Filter for file types
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // Initial directory

                // Show the file open dialog
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the selected file name
                    string selectedFileName = openFileDialog.FileName;
                    //
                    this.cusModel = new CusModel(selectedFileName);
                    //MyGLControl.Invalidate();
                    // Display the file contents or perform other actions
                    //MessageBox.Show("File : " + Path.GetFileName(selectedFileName), "File Opened");

                }
            }
        }


        #region "OpenTK"

        private void MyGLControl_Resize(object? sender, EventArgs e)
        {
            MyGLControl.MakeCurrent();    // Tell OpenGL to use MyGLControl.

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            // Update OpenGL on the new size of the control.
            GL.Viewport(0, 0, MyGLControl.ClientSize.Width, MyGLControl.ClientSize.Height);

            _camera.AspectRatio = MyGLControl.ClientSize.Width / (float)MyGLControl.ClientSize.Height;

        }

        private void MyGLControl_Paint(object? sender, PaintEventArgs e)
        {
            MyGLControl.MakeCurrent();    // Tell OpenGL to draw on MyGLControl.

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit); // Clear any prior drawing.

            /*
            ... use various other GL.*() calls here to draw stuff ...
            */

            _shader.Use();
            // view/projection transformations
            var projection = _camera.GetProjectionMatrix();
            var view = _camera.GetViewMatrix();
            _shader.SetMatrix4("projection", projection);
            _shader.SetMatrix4("view", view);

            //// render the loaded model
            Matrix4 model = Matrix4.Identity;
            // translate it down so it's at the center of the scene
            model *= Matrix4.CreateTranslation(new Vector3(0.0f, 0.0f, 0.0f)); 
            model *= Matrix4.CreateScale(new Vector3(1.0f, 1.0f, 1.0f)); 

            //
            _shader.SetMatrix4("model", model);
            if(cusModel != null)
            {
                //
                cusModel.Draw(_shader);
            }
            MyGLControl.SwapBuffers();    // Display the result.

        }
        #endregion

        private void MyGLControl_Load(object sender, EventArgs e)
        {
            MyGLControl.MakeCurrent();    // Tell OpenGL to draw on MyGLControl.
            //
            //GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            _camera = new Camera(Vector3.UnitZ * 3, MyGLControl.ClientSize.Width / (float)MyGLControl.ClientSize.Height);

            _shader = new Shader("model_loading.vs", "model_loading.fs");
            _shader.Use();

            // You can bind the events here or in the Designer.
            MyGLControl.Resize += MyGLControl_Resize;
            MyGLControl.Paint += MyGLControl_Paint;
        }
    }
}