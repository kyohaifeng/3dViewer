using Assimp;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.WinForms;

namespace _3dViewer
{
    public partial class MainForm : Form
    {
        public CusModel cusModel;

        private Camera _camera;
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
                    // Display the file contents or perform other actions
                    MessageBox.Show("File : " + Path.GetFileName(selectedFileName), "File Opened");

                }
            }
        }


        #region "OpenTK"

        private void MyGLControl_Resize(object? sender, EventArgs e)
        {
            MyGLControl.MakeCurrent();    // Tell OpenGL to use MyGLControl.

            // Update OpenGL on the new size of the control.
            GL.Viewport(0, 0, MyGLControl.ClientSize.Width, MyGLControl.ClientSize.Height);

            /*
                Usually you compute projection matrices here too, like this:

                float aspect_ratio = MyGLControl.ClientSize.Width / (float)MyGLControl.ClientSize.Height;
                Matrix4 perpective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ratio, 1, 64);

                And then you load that into OpenGL with a call like GL.LoadMatrix() or GL.Uniform().
            */
        }

        private void MyGLControl_Paint(object? sender, PaintEventArgs e)
        {
            MyGLControl.MakeCurrent();    // Tell OpenGL to draw on MyGLControl.
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit); // Clear any prior drawing.

            /*
            ... use various other GL.*() calls here to draw stuff ...
            */
            // loop 
            GL.Enable(EnableCap.DepthTest);
            Shader shader = new Shader("model_loading.vs", "model_loading.fs");
            shader.Use();

            // view/projection transformations
            var projection = _camera.GetProjectionMatrix();
            var view = _camera.GetViewMatrix();
            shader.SetMatrix4("projection", projection);
            shader.SetMatrix4("view", view);

            //// render the loaded model
            Matrix4 model = Matrix4.Identity;
            //
            shader.SetMatrix4("model", model);
            //
            this.cusModel.Draw(shader);

            MyGLControl.SwapBuffers();    // Display the result.
        }
        #endregion

        private void MainForm_Load(object sender, EventArgs e)
        {
            //
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            _camera = new Camera(Vector3.UnitZ * 3, MyGLControl.Width / (float)MyGLControl.Height);

            // You can bind the events here or in the Designer.
            MyGLControl.Resize += MyGLControl_Resize;
            MyGLControl.Paint += MyGLControl_Paint;
        }
    }
}