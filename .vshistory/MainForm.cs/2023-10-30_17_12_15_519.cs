using OpenTK.Graphics.OpenGL;
using OpenTK.WinForms;

namespace _3dViewer
{
    public partial class MainForm : Form
    {
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

                    // You can now process the selected file as needed
                    
                    // Display the file contents or perform other actions
                    //MessageBox.Show("File Contents: " + fileContents, "File Opened");
                }
            }
        }


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
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);                // Clear any prior drawing.

            /*
            ... use various other GL.*() calls here to draw stuff ...
            */

            MyGLControl.SwapBuffers();    // Display the result.
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // You can bind the events here or in the Designer.
            MyGLControl.Resize += MyGLControl_Resize;
            MyGLControl.Paint += MyGLControl_Paint;
        }
    }
}