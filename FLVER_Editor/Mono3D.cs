using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pfim;
using SoulsFormats;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Color = Microsoft.Xna.Framework.Color;
using ImageFormat = Pfim.ImageFormat;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using MessageBox = System.Windows.Forms.MessageBox;
using Point = System.Drawing.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace FLVER_Editor
{
    internal enum RenderMode
    {
        Line,
        Triangle,
        Both,
        BothNoTex,
        TexOnly
    }

    public class VertexTexMap
    {
        public VertexPositionColorTexture[] faceSetTextures = new VertexPositionColorTexture[0];
        public string textureName = "";
    }
    internal class Mono3D : Game
    {
        private static GCHandle handle;
        public static Form f;
        public static Form mainForm;
        private static readonly int snapDist = 15;
        private static int mainFormLeft;
        private static int mainFormRight;

        private readonly float centerX = 0;
        private readonly float centerY = 0;
        private readonly float centerZ = 0;
        private readonly ContextMenuStrip cm = new ContextMenuStrip();
        private readonly GraphicsDeviceManager graphics;
        private readonly Dictionary<string, Texture2D> textureMap = new Dictionary<string, Texture2D>();
        private Rectangle bgRenderArea;
        private Texture2D bgTexture;

        private float cameraX;
        private float cameraY = 4;
        private float cameraZ = 2;
        private BasicEffect effect;
        public VertexPositionColor[] faceSets = new VertexPositionColor[0];
        public bool flatShading;
        private VertexPositionTexture[] floorVerts;

        public float lightX = 1;
        public float lightY = 1;
        public float lightZ = 1;

        private float offsetX;
        private float offsetY;
        private float offsetZ;
        private MouseState prevMState;
        private KeyboardState prevState;
        public RenderMode renderMode = RenderMode.Both;
        private bool rightClickSilence;
        public SpriteBatch spriteBatch;
        private FLVER.Vertex targetV;
        private VertexInfo targetVinfo;
        private Texture2D testTexture;
        public SpriteFont viewerFont;

        //  public VertexPositionColorTexture[] triTextureVertices = new VertexPositionColorTexture[0];
        public VertexTexMap[] vertexTexMapList = new VertexTexMap[0];
        public VertexPositionColor[] vertices = new VertexPositionColor[0];

        public Mono3D()
        {
            Window.Title = "FLVER Viewer";
            Window.AllowUserResizing = true;
            IsMouseVisible = true;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            f = (Form)Control.FromHandle(Window.Handle);
            cm.Items.Add("Cancel", null, delegate
            {
                Program.useCheckingPoint = false;
                MainWindow.UpdateMesh();
            });
            cm.Items.Add("Check Vertex", null, delegate { displayVerticesInfo(); });
            cm.Items.Add("Edit Vertex", null, delegate { editVerticesInfo(); });
            cm.Items.Add("Delete Selected Vertex's Faceset", null, delegate { deleteVertex(); });
            cm.Items.Add("Delete Vertices Above", null, delegate { deleteVertexAbove(); });
            cm.Items.Add("Delete Vertices Below", null, delegate { deleteVertexBelow(); });
            f.ContextMenuStrip = cm;
            f.MouseDown += pictureBox1_MouseDown;
            f.MouseUp += pictureBox1_MouseUp;
            f.LocationChanged += (s, e) =>
            {
                SetSnapDistance();
                if (ShouldSnapRight()) SnapRight();
                else if (ShouldSnapBottom()) SnapBottom();
                else Unsnap();
            };
        }

        private static void SnapBottom()
        {
            f.Left = mainForm.Left;
            f.Width = mainForm.Width;
            f.Top = mainForm.Bottom;
            MainWindow.isSnappedTop = false;
            MainWindow.isSnappedRight = false;
            MainWindow.isSnappedBottom = true;
            MainWindow.isSnappedLeft = false;
            MainWindow.isSnapped = true;
        }

        private static bool ShouldSnapRight()
        {
            return Math.Abs(mainFormRight - f.Left) < snapDist && Math.Abs(mainForm.Top - f.Top) < snapDist;
        }

        private static void SnapRight()
        {
            f.Height = mainForm.Height;
            f.Left = mainFormRight;
            f.Top = mainForm.Top;
            MainWindow.isSnappedTop = false;
            MainWindow.isSnappedRight = true;
            MainWindow.isSnappedBottom = false;
            MainWindow.isSnappedLeft = false;
            MainWindow.isSnapped = true;
        }

        private static bool ShouldSnapBottom()
        {
            return Math.Abs(mainForm.Bottom - f.Top) < snapDist;
        }

        private static void Unsnap()
        {
            MainWindow.isSnapped = false;
        }

        private static void SetSnapDistance()
        {
            mainFormLeft = mainForm.Left - 10;
            mainFormRight = mainForm.Right - 10;
        }

        private void deleteVertexBelow()
        {
            FLVER.Mesh m = Program.flver.Meshes[targetVinfo.meshIndex];
            uint index = targetVinfo.vertexIndex;
            float yValue = targetV.Positions[0].Y;
            for (var i = 0; i < m.Vertices.Count; i++)
            {
                if (m.Vertices[i].Positions[0].Y < yValue)
                {
                    deleteMeshVertexFaceset(m, (uint)i);
                    m.Vertices[i].Positions[0] = new System.Numerics.Vector3(0, 0, 0);
                }
            }
            MainWindow.UpdateMesh();
        }

        private void deleteVertexAbove()
        {
            FLVER.Mesh m = Program.flver.Meshes[targetVinfo.meshIndex];
            uint index = targetVinfo.vertexIndex;
            float yValue = targetV.Positions[0].Y;
            for (var i = 0; i < m.Vertices.Count; i++)
            {
                if (m.Vertices[i].Positions[0].Y > yValue)
                {
                    deleteMeshVertexFaceset(m, (uint)i);
                    m.Vertices[i].Positions[0] = new System.Numerics.Vector3(0, 0, 0);
                }
            }
            MainWindow.UpdateMesh();
        }

        private void deleteMeshVertexFaceset(FLVER.Mesh m, uint index)
        {
            foreach (FLVER.FaceSet fs in m.FaceSets)
            {
                for (uint i = 0; i + 2 < fs.Vertices.Length; i += 3)
                {
                    if (fs.Vertices[i] == index || fs.Vertices[i + 1] == index || fs.Vertices[i + 2] == index)
                    {
                        fs.Vertices[i] = index;
                        fs.Vertices[i + 1] = index;
                        fs.Vertices[i + 2] = index;
                    }
                }
            }
        }

        private void deleteVertex()
        {
            FLVER.Mesh m = Program.flver.Meshes[targetVinfo.meshIndex];
            uint index = targetVinfo.vertexIndex;
            deleteMeshVertexFaceset(m, index);
            targetV.Positions[0] = new System.Numerics.Vector3(0, 0, 0);
            MainWindow.UpdateMesh();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right:
                {
                    f.ContextMenu = null;
                    prevMState = Mouse.GetState();
                    checkVerticesSilent();
                    f.ContextMenu = null;
                    //f.ContextMenu.Show();
                }
                    break;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right:
                {
                    if (!rightClickSilence)
                    {
                        f.ContextMenuStrip = cm;
                        f.ContextMenuStrip.Show(f, new Point(e.X + 1, e.Y + 1)); //places the menu at the pointer position
                    }
                    else if (prevState.IsKeyDown(Keys.LeftAlt) && rightClickSilence)
                    {
                        deleteVertex();
                        // System.Windows.MessageBox.Show("Ctrl + Right Click pressed. Switch To Right Click Slience Mode.");
                    }
                }
                    break;
            }
        }

        /// <summary>
        ///     Allows the game to perform any initialization it needs to before starting to run.
        ///     This is where it can query for any required services and load any non-graphic
        ///     related content.  Calling base.Initialize will enumerate through any components
        ///     and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            floorVerts = new VertexPositionTexture[6];
            floorVerts[0].Position = new Vector3(-20, -20, 0);
            floorVerts[1].Position = new Vector3(-20, 20, 0);
            floorVerts[2].Position = new Vector3(20, -20, 0);
            floorVerts[3].Position = floorVerts[1].Position;
            floorVerts[4].Position = new Vector3(20, 20, 0);
            floorVerts[5].Position = floorVerts[2].Position;
            effect = new BasicEffect(graphics.GraphicsDevice);
            effect.VertexColorEnabled = true;
            base.Initialize();
        }

        private void ReadTPFTextureEntries(TPF tpf)
        {
            foreach (TPF.Texture t in tpf.Textures)
            {
                try
                {
                    textureMap.Add(t.Name, getTextureFromBitmap(readDdsStreamToBitmap(new MemoryStream(t.Bytes)), GraphicsDevice));
                }
                catch { }
            }
        }

        public void RefreshTextures()
        {
            if (!MainWindow.textureRefreshEnabled) return;
            string rawFilePath = Program.filePath;
            rawFilePath = rawFilePath.Replace("_1.", ".");
            rawFilePath = rawFilePath.Substring(0, rawFilePath.Length - 6);
            var tpfFile = $"{rawFilePath}.tpf";
            if (Program.tpf != null) ReadTPFTextureEntries(Program.tpf);
            else if (File.Exists(tpfFile))
            {
                Program.tpf = TPF.Read(tpfFile);
                ReadTPFTextureEntries(Program.tpf);
            }
        }

        /// <summary>
        ///     LoadContent will be called once per game and is the place to load
        ///     all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            testTexture = null;
            if (!Program.loadTexture)
            {
                return;
            }
            RefreshTextures();
            using (Stream stream = TitleContainer.OpenStream("singleColor.png"))
            {
                testTexture = Texture2D.FromStream(GraphicsDevice, stream);
            }
            var bgFileStream = new FileStream($"{MainWindow.rootFolderPath}\\bg.png", FileMode.Open);
            bgTexture = Texture2D.FromStream(GraphicsDevice, bgFileStream);
            bgFileStream.Close();
            viewerFont = Content.Load<SpriteFont>("Segoe UI");
            //  testTexture = getTextureFromBitmap(readDdsFileToBitmap("EliteKnight.dds"),this.GraphicsDevice);
            /*  string path = @"data\img\27.png";

              System.Drawing.Bitmap btt = new System.Drawing.Bitmap(path);
              test = Texture2D.FromStream(this.GraphicsDevice, File.OpenRead(path));
              test = getTextureFromBitmap(btt, this.GraphicsDevice);*/
            // TODO: use this.Content to load your game content here
        }

        //Read dds file to bitmap
        private Bitmap readDdsFileToBitmap(string f)
        {
            IImage image = Pfim.Pfim.FromFile(f);
            PixelFormat format;
            switch (image.Format)
            {
                case ImageFormat.Rgb24:
                    format = PixelFormat.Format24bppRgb;
                    break;
                case ImageFormat.Rgba32:
                    format = PixelFormat.Format32bppArgb;
                    break;
                case ImageFormat.R5g5b5:
                    format = PixelFormat.Format16bppRgb555;
                    break;
                case ImageFormat.R5g6b5:
                    format = PixelFormat.Format16bppRgb565;
                    break;
                case ImageFormat.R5g5b5a1:
                    format = PixelFormat.Format16bppArgb1555;
                    break;
                case ImageFormat.Rgb8:
                    format = PixelFormat.Format8bppIndexed;
                    break;
                default:
                    /* var msg = $"{image.Format} is not recognized for Bitmap on Windows Forms. " +
 
                                "You'd need to write a conversion function to convert the data to known format";
 
                     var caption = "Unrecognized format";
 
                     MessageBox.Show(msg, caption, MessageBoxButtons.OK);
                     */
                    return null;
            }
            if (handle.IsAllocated)
            {
                handle.Free();
            }
            handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
            IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);
            var bitmap = new Bitmap(image.Width, image.Height, image.Stride, format, ptr);
            return bitmap;
        }

        private Bitmap readDdsStreamToBitmap(Stream f)
        {
            IImage image = Pfim.Pfim.FromStream(f);
            PixelFormat format;
            switch (image.Format)
            {
                case ImageFormat.Rgb24:
                    format = PixelFormat.Format24bppRgb;
                    break;
                case ImageFormat.Rgba32:
                    format = PixelFormat.Format32bppArgb;
                    break;
                case ImageFormat.R5g5b5:
                    format = PixelFormat.Format16bppRgb555;
                    break;
                case ImageFormat.R5g6b5:
                    format = PixelFormat.Format16bppRgb565;
                    break;
                case ImageFormat.R5g5b5a1:
                    format = PixelFormat.Format16bppArgb1555;
                    break;
                case ImageFormat.Rgb8:
                    format = PixelFormat.Format8bppIndexed;
                    break;
                default:
                    /* var msg = $"{image.Format} is not recognized for Bitmap on Windows Forms. " +

                                "You'd need to write a conversion function to convert the data to known format";

                     var caption = "Unrecognized format";

                     MessageBox.Show(msg, caption, MessageBoxButtons.OK);
                     */
                    return null;
            }
            if (handle.IsAllocated)
            {
                handle.Free();
            }
            handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
            IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);
            var bitmap = new Bitmap(image.Width, image.Height, image.Stride, format, ptr);
            return bitmap;
        }

        //Refer to the code at http://florianblock.blogspot.com/2008/06/copying-dynamically-created-bitmap-to.html
        //Also refer to https://gamedev.stackexchange.com/questions/6440/bitmap-to-texture2d-problem-with-colors
        //Modied by Alan Zhang
        public static Texture2D getTextureFromBitmap(Bitmap b, GraphicsDevice graphicsDevice)
        {
            Texture2D tx = null;
            using (var s = new MemoryStream())
            {
                b.Save(s, System.Drawing.Imaging.ImageFormat.Png);
                s.Seek(0, SeekOrigin.Begin);
                tx = Texture2D.FromStream(graphicsDevice, s);
            }
            return tx;
        }

        /// <summary>
        ///     UnloadContent will be called once per game and is the place to unload
        ///     game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        ///     Allows the game to run logic such as updating the world,
        ///     checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected void checkVerticesSilent()
        {
            Ray r = GetMouseRay(new Vector2(prevMState.Position.X, prevMState.Position.Y), GraphicsDevice.Viewport, effect);
            r.Position = new Vector3(r.Position.X, r.Position.Z, r.Position.Y);
            r.Direction = new Vector3(r.Direction.X, r.Direction.Z, r.Direction.Y);
            // Vector3D x1 =  new Vector3D(cameraX + offsetX, cameraY + offsetY, cameraZ + offsetZ);
            //Vector3D x2 = new Vector3D(centerX + offsetX, centerY + offsetY, centerZ + offsetZ);
            var x1 = new Vector3D(r.Position);
            var x2 = new Vector3D(r.Position + r.Direction);
            //Program.useCheckingPoint = true;
            // Program.checkingPoint = new System.Numerics.Vector3(x2.X,x2.Z,x2.Y);
            // Program.updateVertices();
            var miniPoint = new Vector3D();
            var ptDistance = float.MaxValue;
            targetV = null;
            targetVinfo = null;
            for (var i = 0; i < Program.vertices.Count; i++)
                //  foreach (SoulsFormats.FLVER.Vertex v in Program.vertices)
            {
                FLVER.Vertex v = Program.vertices[i];
                if (v.Positions[0] == null)
                {
                    continue;
                }
                float dis = Vector3D.calculateDistanceFromLine(new Vector3D(v.Positions[0]), x1, x2);
                if (ptDistance > dis)
                {
                    miniPoint = new Vector3D(v.Positions[0]);
                    ptDistance = dis;
                    targetV = v;
                    targetVinfo = Program.verticesInfo[i];
                }
            }
            if (Program.setVertexPos)
            {
                targetV.Positions[0] = new Vector3D(Program.setVertexX, Program.setVertexY, Program.setVertexZ).toNumV3();
            }
            Program.useCheckingPoint = true;
            Program.checkingPoint = new System.Numerics.Vector3(miniPoint.X, miniPoint.Y, miniPoint.Z);
            if (targetV.Normals != null && targetV.Normals.Count > 0)
            {
                Program.checkingPointNormal = new System.Numerics.Vector3(targetV.Normals[0].X, targetV.Normals[0].Y, targetV.Normals[0].Z);
            }
            else
            {
                Program.checkingPointNormal = new System.Numerics.Vector3(0, 0, 0);
            }
            MainWindow.UpdateMesh();
        }

        protected void displayVerticesInfo()
        {
            if (targetV != null)
            {
                string text = Program.FormatOutput(new JavaScriptSerializer().Serialize(targetV));
                int l = text.Length / 2;
                MessageBox.Show("Parent mesh index:" + targetVinfo.meshIndex + "\nVertex index:" + targetVinfo.vertexIndex + "\n" + text.Substring(0, l),
                    "Vertex info1:");
                MessageBox.Show(text.Substring(l, text.Length - l), "Vertex info2:");
            }
        }

        protected void editVerticesInfo()
        {
            if (targetV != null)
            {
                //string text = Program.FormatOutput(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(targetV));
                //int l = text.Length / 2;
                var fn = new Form();
                fn.Size = new Size(350, 650);
                var tb = new TextBox();
                tb.Size = new Size(330, 550);
                tb.Location = new Point(5, 10);
                tb.Multiline = true;
                tb.Text = Program.FormatOutput(new JavaScriptSerializer().Serialize(targetV.Positions));
                var bn = new Button();
                bn.Size = new Size(330, 35);
                bn.Location = new Point(5, 560);
                bn.Text = "Modify";
                bn.Click += (s, o) =>
                {
                    var vn = new JavaScriptSerializer().Deserialize<List<System.Numerics.Vector3>>(tb.Text);
                    targetV.Positions = vn;
                    MainWindow.UpdateMesh();
                };
                fn.Controls.Add(tb);
                fn.Controls.Add(bn);
                fn.Show();
            }
        }

        protected void checkVertices()
        {
            Ray r = GetMouseRay(new Vector2(prevMState.Position.X, prevMState.Position.Y), GraphicsDevice.Viewport, effect);
            r.Position = new Vector3(r.Position.X, r.Position.Z, r.Position.Y);
            r.Direction = new Vector3(r.Direction.X, r.Direction.Z, r.Direction.Y);
            // Vector3D x1 =  new Vector3D(cameraX + offsetX, cameraY + offsetY, cameraZ + offsetZ);
            //Vector3D x2 = new Vector3D(centerX + offsetX, centerY + offsetY, centerZ + offsetZ);
            var x1 = new Vector3D(r.Position);
            var x2 = new Vector3D(r.Position + r.Direction);
            //Program.useCheckingPoint = true;
            // Program.checkingPoint = new System.Numerics.Vector3(x2.X,x2.Z,x2.Y);
            // Program.updateVertices();
            var miniPoint = new Vector3D();
            var ptDistance = float.MaxValue;
            targetV = null;
            foreach (FLVER.Vertex v in Program.vertices)
            {
                if (v.Positions[0] == null)
                {
                    continue;
                }
                float dis = Vector3D.calculateDistanceFromLine(new Vector3D(v.Positions[0]), x1, x2);
                if (ptDistance > dis)
                {
                    miniPoint = new Vector3D(v.Positions[0]);
                    ptDistance = dis;
                    targetV = v;
                }
            }
            if (Program.setVertexPos)
            {
                targetV.Positions[0] = new Vector3D(Program.setVertexX, Program.setVertexY, Program.setVertexZ).toNumV3();
            }
            Program.useCheckingPoint = true;
            Program.checkingPoint = new System.Numerics.Vector3(miniPoint.X, miniPoint.Y, miniPoint.Z);
            if (targetV.Normals != null && targetV.Normals.Count > 0)
            {
                Program.checkingPointNormal = new System.Numerics.Vector3(targetV.Normals[0].X, targetV.Normals[0].Y, targetV.Normals[0].Z);
            }
            else
            {
                Program.checkingPointNormal = new System.Numerics.Vector3(0, 0, 0);
            }
            MainWindow.UpdateMesh();
            if (targetV != null)
            {
                string text = Program.FormatOutput(new JavaScriptSerializer().Serialize(targetV));
                int l = text.Length / 2;
                MessageBox.Show(text.Substring(0, l), "Vertex info1:");
                MessageBox.Show(text.Substring(l, text.Length - l), "Vertex info2:");
            }
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            MouseState mState = Mouse.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Application.Exit();
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Marine_Yes00.mp3
            if (mState.LeftButton == ButtonState.Pressed && !state.IsKeyDown(Keys.LeftAlt) && IsActive)
            {
                float mdx = mState.X - prevMState.X;
                float mdy = mState.Y - prevMState.Y;
                {
                    var p = new System.Numerics.Vector3(cameraX, cameraY, cameraZ);
                    System.Numerics.Vector3 p2 = Program.RotatePoint(p, 0, 0, -mdx * 0.01f);
                    cameraX = p2.X;
                    cameraY = p2.Y;
                    cameraZ = p2.Z;
                }
                {
                    var p = new System.Numerics.Vector3(cameraX, cameraY, cameraZ);
                    float nX = cameraY;
                    float nY = -cameraX;
                    System.Numerics.Vector3 p2 = Program.RotateLine(p, new System.Numerics.Vector3(0, 0, 0),
                        new System.Numerics.Vector3(nX, nY, 0), mdy * 0.01f);
                    cameraX = p2.X;
                    cameraY = p2.Y;
                    cameraZ = p2.Z;
                }
            }
            if ((mState.MiddleButton == ButtonState.Pressed || state.IsKeyDown(Keys.LeftAlt) && mState.LeftButton == ButtonState.Pressed) && IsActive)
            {
                float mdx = mState.X - prevMState.X;
                float mdy = mState.Y - prevMState.Y;

                // offsetZ += mdy * 3 * delta;
                var upV = new Vector3D(0, 0, 1);
                var forwardV = new Vector3D(cameraX, cameraY, cameraZ);
                Vector3D rightV = Vector3D.crossPorduct(upV, forwardV).normalize();
                Vector3D camUpV = Vector3D.crossPorduct(forwardV, rightV).normalize();
                var offsetV = new Vector3D(offsetX, offsetY, offsetZ);
                offsetV = offsetV - new Vector3D(rightV.X * mdx * 0.01f, rightV.Y * mdx * 0.01f, rightV.Z * mdx * 0.01f);
                offsetV = offsetV + new Vector3D(camUpV.X * mdy * 0.01f, camUpV.Y * mdy * 0.01f, camUpV.Z * mdy * 0.01f);
                offsetX = offsetV.X;
                offsetY = offsetV.Y;
                offsetZ = offsetV.Z;
                //offsetX -= mdx* 1 * delta * rightV.X;
                //offsetY -= mdx * 1 * delta * rightV.Y;
            }
            if (state.IsKeyDown(Keys.F1))
            {
                renderMode = RenderMode.Line;
            }
            if (state.IsKeyDown(Keys.F2))
            {
                renderMode = RenderMode.Triangle;
            }
            if (state.IsKeyDown(Keys.F3))
            {
                renderMode = RenderMode.Both;
            }
            if (state.IsKeyDown(Keys.F4))
            {
                renderMode = RenderMode.BothNoTex;
            }
            if (state.IsKeyDown(Keys.F5))
            {
                renderMode = RenderMode.TexOnly;
            }
            if (state.IsKeyDown(Keys.F6) && !prevState.IsKeyDown(Keys.F6))
            {
                flatShading = !flatShading;
                MainWindow.UpdateMesh();
            }
            if (state.IsKeyDown(Keys.B) && !prevState.IsKeyDown(Keys.B))
            {
                Program.boneDisplay = !Program.boneDisplay;
                MainWindow.UpdateMesh();
            }
            if (state.IsKeyDown(Keys.M) && !prevState.IsKeyDown(Keys.M))
            {
                Program.dummyDisplay = !Program.dummyDisplay;
                MainWindow.UpdateMesh();
            }

            //1.73 Added focus detect
            if (mState.RightButton == ButtonState.Pressed && IsActive && false)
            {
                Ray r = GetMouseRay(new Vector2(mState.Position.X, mState.Position.Y), GraphicsDevice.Viewport, effect);
                r.Position = new Vector3(r.Position.X, r.Position.Z, r.Position.Y);
                r.Direction = new Vector3(r.Direction.X, r.Direction.Z, r.Direction.Y);
                // Vector3D x1 =  new Vector3D(cameraX + offsetX, cameraY + offsetY, cameraZ + offsetZ);
                //Vector3D x2 = new Vector3D(centerX + offsetX, centerY + offsetY, centerZ + offsetZ);
                var x1 = new Vector3D(r.Position);
                var x2 = new Vector3D(r.Position + r.Direction);
                //Program.useCheckingPoint = true;
                // Program.checkingPoint = new System.Numerics.Vector3(x2.X,x2.Z,x2.Y);
                // Program.updateVertices();
                var miniPoint = new Vector3D();
                var ptDistance = float.MaxValue;
                FLVER.Vertex targetV = null;
                foreach (FLVER.Vertex v in Program.vertices)
                {
                    if (v.Positions[0] == null)
                    {
                        continue;
                    }
                    float dis = Vector3D.calculateDistanceFromLine(new Vector3D(v.Positions[0]), x1, x2);
                    if (ptDistance > dis)
                    {
                        miniPoint = new Vector3D(v.Positions[0]);
                        ptDistance = dis;
                        targetV = v;
                    }
                }
                if (Program.setVertexPos)
                {
                    targetV.Positions[0] = new Vector3D(Program.setVertexX, Program.setVertexY, Program.setVertexZ).toNumV3();
                }
                Program.useCheckingPoint = true;
                Program.checkingPoint = new System.Numerics.Vector3(miniPoint.X, miniPoint.Y, miniPoint.Z);
                if (targetV.Normals != null && targetV.Normals.Count > 0)
                {
                    Program.checkingPointNormal = new System.Numerics.Vector3(targetV.Normals[0].X, targetV.Normals[0].Y, targetV.Normals[0].Z);
                }
                else
                {
                    Program.checkingPointNormal = new System.Numerics.Vector3(0, 0, 0);
                }
                MainWindow.UpdateMesh();
                if (targetV != null)
                {
                    string text = Program.FormatOutput(new JavaScriptSerializer().Serialize(targetV));
                    int l = text.Length / 2;
                    MessageBox.Show(text.Substring(0, l), "Vertex info1:");
                    MessageBox.Show(text.Substring(l, text.Length - l), "Vertex info2:");
                }
            }
            if (mState.ScrollWheelValue - prevMState.ScrollWheelValue > 0)
            {
                //mouseY -= (50 * delta);
                var p = new System.Numerics.Vector3(cameraX, cameraY, cameraZ);
                cameraX = p.X - 0.5f * (p.X / p.Length());
                cameraY = p.Y - 0.5f * (p.Y / p.Length());
                cameraZ = p.Z - 0.5f * (p.Z / p.Length());
            }
            if (mState.ScrollWheelValue - prevMState.ScrollWheelValue < 0)
            {
                var p = new System.Numerics.Vector3(cameraX, cameraY, cameraZ);
                cameraX = p.X + 0.5f * (p.X / p.Length());
                cameraY = p.Y + 0.5f * (p.Y / p.Length());
                cameraZ = p.Z + 0.5f * (p.Z / p.Length());
            }
            if (state.IsKeyDown(Keys.Right))
            {
                var p = new System.Numerics.Vector3(cameraX, cameraY, cameraZ);
                System.Numerics.Vector3 p2 = Program.RotatePoint(p, 0, 0, 5 * delta);
                cameraX = p2.X;
                cameraY = p2.Y;
                cameraZ = p2.Z;
            }
            if (state.IsKeyDown(Keys.Left))
            {
                var p = new System.Numerics.Vector3(cameraX, cameraY, cameraZ);
                System.Numerics.Vector3 p2 = Program.RotatePoint(p, 0, 0, -5 * delta);
                cameraX = p2.X;
                cameraY = p2.Y;
                cameraZ = p2.Z;
            }
            if (state.IsKeyDown(Keys.Up))
            {
                //mouseY -= (50 * delta);
                var p = new System.Numerics.Vector3(cameraX, cameraY, cameraZ);
                System.Numerics.Vector3 p2 = Program.RotateLine(p, new System.Numerics.Vector3(0, 0, 0),
                    new System.Numerics.Vector3(cameraY, -cameraX, 0), 3 * delta);
                cameraX = p2.X;
                cameraY = p2.Y;
                cameraZ = p2.Z;
            }
            if (state.IsKeyDown(Keys.Down))
            {
                var p = new System.Numerics.Vector3(cameraX, cameraY, cameraZ);
                System.Numerics.Vector3 p2 = Program.RotateLine(p, new System.Numerics.Vector3(0, 0, 0),
                    new System.Numerics.Vector3(cameraY, -cameraX, 0), -3 * delta);
                cameraX = p2.X;
                cameraY = p2.Y;
                cameraZ = p2.Z;
            }
            if (state.IsKeyDown(Keys.OemComma))
            {
                //mouseY -= (50 * delta);
                var p = new System.Numerics.Vector3(cameraX, cameraY, cameraZ);
                cameraX = p.X - 3 * delta * (p.X / p.Length());
                cameraY = p.Y - 3 * delta * (p.Y / p.Length());
                cameraZ = p.Z - 3 * delta * (p.Z / p.Length());
            }
            if (state.IsKeyDown(Keys.OemPeriod))
            {
                var p = new System.Numerics.Vector3(cameraX, cameraY, cameraZ);
                cameraX = p.X + 3 * delta * (p.X / p.Length());
                cameraY = p.Y + 3 * delta * (p.Y / p.Length());
                cameraZ = p.Z + 3 * delta * (p.Z / p.Length());
            }
            if (state.IsKeyDown(Keys.NumPad8))
            {
                offsetZ += 3 * delta;
            }
            if (state.IsKeyDown(Keys.NumPad2))
            {
                offsetZ -= 3 * delta;
                ;
            }
            if (state.IsKeyDown(Keys.NumPad4))
            {
                var upV = new Vector3D(0, 0, 1);
                var forwardV = new Vector3D(cameraX, cameraY, cameraZ);
                Vector3D rightV = Vector3D.crossPorduct(upV, forwardV).normalize();
                offsetX -= 3 * delta * rightV.X;
                offsetY -= 3 * delta * rightV.Y;
                //offsetZ -= 3 * delta; ;
            }
            if (state.IsKeyDown(Keys.NumPad6))
            {
                var upV = new Vector3D(0, 0, 1);
                var forwardV = new Vector3D(cameraX, cameraY, cameraZ);
                Vector3D rightV = Vector3D.crossPorduct(upV, forwardV).normalize();
                offsetX += 3 * delta * rightV.X;
                offsetY += 3 * delta * rightV.Y;
                //offsetZ -= 3 * delta; ;
            }
            if (state.IsKeyDown(Keys.NumPad5))
            {
                Vector3D forwardV = new Vector3D(cameraX, cameraY, cameraZ).normalize();
                offsetX -= 3 * delta * forwardV.X;
                offsetY -= 3 * delta * forwardV.Y;
                offsetZ -= 3 * delta * forwardV.Z;
                //offsetZ -= 3 * delta; ;
            }
            if (state.IsKeyDown(Keys.NumPad0))
            {
                Vector3D forwardV = new Vector3D(cameraX, cameraY, cameraZ).normalize();
                offsetX += 3 * delta * forwardV.X;
                offsetY += 3 * delta * forwardV.Y;
                offsetZ += 3 * delta * forwardV.Z;
                //offsetZ -= 3 * delta; ;
            }

            //new Vector3(cameraX + offsetX, cameraY + offsetY, cameraZ + offsetZ)
            /* if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D1)) { Program.rotOrder = RotationOrder.XYZ; Program.updateVertices(); }
             if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D2)) { Program.rotOrder = RotationOrder.XZY; Program.updateVertices(); }
             if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D3)) { Program.rotOrder = RotationOrder.YXZ; Program.updateVertices(); }
             if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D4)) { Program.rotOrder = RotationOrder.YZX; Program.updateVertices(); }
             if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D5)) { Program.rotOrder = RotationOrder.ZXY; Program.updateVertices(); }
             if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D6)) { Program.rotOrder = RotationOrder.ZYX; Program.updateVertices(); }*/
            if (state.IsKeyDown(Keys.F))
            {
                MainWindow.UpdateMesh();
            }
            //mouseX = Mouse.GetState().Position.X;
            //mouseY = Mouse.GetState().Position.Y;
            // TODO: Add your update logic here
            prevState = state;
            prevMState = mState;
            base.Update(gameTime);
        }

        private Vector3 RotatePoint()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(95, 95, 95, 0));
            spriteBatch.Begin();
            try
            {
                bgRenderArea = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
                spriteBatch.Draw(bgTexture, bgRenderArea, Color.White);
                spriteBatch.End();
                spriteBatch.Begin();
            }
            catch { }
            // TODO: Add your drawing code here
            // Rectangle screenRectangle = new Rectangle((int)mouseX, (int)mouseY, 50, 50);
            //  spriteBatch.Draw(test, screenRectangle, Color.White);
            DrawGround();
            //effect.EnableDefaultLighting();
            /*var vertices = new VertexPositionColor[4];
             vertices[0].Position = new Vector3(100, 100, 0);
             vertices[0].Color = Color.Black;
             vertices[1].Position = new Vector3(200, 100, 0);
             vertices[1].Color = Color.Red;
             vertices[2].Position = new Vector3(200, 200, 0);
             vertices[2].Color = Color.Black;
             vertices[3].Position = new Vector3(100, 200, 0);
             vertices[3].Color = Color.Red;
             */
            /*if (renderMode == RenderMode.Triangle)
            {
                effect.LightingEnabled = true;
                effect.VertexColorEnabled = false;

            }
            else
            {
                effect.LightingEnabled = false;
                effect.VertexColorEnabled = true;
                
            }*/
            if (vertices.Length > 0 || faceSets.Length > 0)
            {
                if (renderMode == RenderMode.Line && vertices.Length > 0)
                {
                    GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, vertices.Length / 2);
                }
                else if (renderMode == RenderMode.Triangle && faceSets.Length > 0)
                {
                    graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, faceSets, 0, faceSets.Length / 3);
                }
                else
                {
                    if (renderMode != RenderMode.TexOnly)
                    {
                        if (vertices.Length > 0)
                        {
                            GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, vertices.Length / 2);
                        }
                    }
                    if (renderMode == RenderMode.BothNoTex || Program.loadTexture == false)
                    {
                        if (faceSets.Length > 0)
                        {
                            graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, faceSets, 0, faceSets.Length / 3);
                        }
                    }
                    else
                    {
                        foreach (VertexTexMap mi in vertexTexMapList)
                        {
                            if (textureMap.ContainsKey(mi.textureName))
                            {
                                effect.TextureEnabled = true;
                                effect.Texture = textureMap[mi.textureName];
                                //no texture found, don't draw.
                                //continue;
                            }
                            else
                            {
                                effect.TextureEnabled = true;
                                effect.Texture = testTexture;
                            }
                            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                            {
                                pass.Apply();
                                if (mi.faceSetTextures.Length > 0)
                                {
                                    graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, mi.faceSetTextures, 0, mi.faceSetTextures.Length / 3);
                                }
                            }
                        }
                    }
                }
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private static Vector2 GetProjectPoint(Vector3 vec, Matrix viewMatrix, Matrix projMatrix, float Width, float Height)
        {
            Matrix mat = Matrix.Identity * viewMatrix * projMatrix;
            Vector4 v4 = Vector4.Transform(vec, mat);
            return new Vector2((int)((v4.X / v4.W + 1) * (Width / 2)), (int)((1 - v4.Y / v4.W) * (Height / 2)));
        }

        private void DrawScreenText<T>(string text, T position, Matrix viewMatrix, Matrix projection)
        {
            Vector3 posVector = typeof(T) == typeof(System.Numerics.Vector3) ? new Vector3(((System.Numerics.Vector3)(object)position).X,
                ((System.Numerics.Vector3)(object)position).Z, ((System.Numerics.Vector3)(object)position).Y) : (Vector3)(object)position;
            Vector2 screenLoc = GetProjectPoint(posVector, viewMatrix, projection, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            spriteBatch.DrawString(viewerFont, text, screenLoc, Color.Yellow, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 1);
        }

        private void DrawGround()
        {
            // The assignment of effect.View and effect.Projection
            // are nearly identical to the code in the Model drawing code.
            // var cameraPosition = new Vector3(0 + mouseX, 40 + mouseY, 20);
            var cameraPosition = new Vector3(cameraX + offsetX, cameraY + offsetY, cameraZ + offsetZ);
            var cameraLookAtVector = new Vector3(centerX + offsetX, centerY + offsetY, centerZ + offsetZ);
            Vector3 cameraUpVector = Vector3.UnitZ;
            var depthBufferState = new DepthStencilState();
            depthBufferState.DepthBufferEnable = true;
            depthBufferState.DepthBufferFunction = CompareFunction.LessEqual;
            GraphicsDevice.DepthStencilState = depthBufferState;
            var viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraLookAtVector, cameraUpVector);
            effect.View = viewMatrix;
            effect.VertexColorEnabled = true;
            float aspectRatio =
                graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight;
            float fieldOfView = MathHelper.PiOver4;
            var nearClipPlane = 0.1f;
            float farClipPlane = 200;
            var projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearClipPlane, farClipPlane);
            effect.Projection = projection;
            /* foreach (var pass in effect.CurrentTechnique.Passes)
             {
                 pass.Apply();

                 graphics.GraphicsDevice.DrawUserPrimitives(
                     // We’ll be rendering two trinalges
                     PrimitiveType.TriangleList,
                     // The array of verts that we want to render
                     floorVerts,
                     // The offset, which is 0 since we want to start 
                     // at the beginning of the floorVerts array
                     0,
                     // The number of triangles to draw
                     2);
             }*/
            VertexPositionColor[] lines;
            lines = new VertexPositionColor[6];
            var lineColor = new Color();
            lineColor.A = 255;
            lineColor.R = lineColor.G = lineColor.B = 100;
            lines[0] = new VertexPositionColor(new Vector3(-1000, 0, 0), Color.Red);
            lines[1] = new VertexPositionColor(new Vector3(1000, 0, 0), Color.Red);
            lines[2] = new VertexPositionColor(new Vector3(0, -1000, 0), Color.Blue);
            lines[3] = new VertexPositionColor(new Vector3(0, 1000, 0), Color.Blue);
            lines[4] = new VertexPositionColor(new Vector3(0, 0, 0), Color.Yellow);
            lines[5] = new VertexPositionColor(new Vector3(0, 0, 1000), Color.Yellow);
            effect.TextureEnabled = false;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, lines, 0, 3);
            }
            DrawScreenText("X", lines[0].Position, viewMatrix, projection);
            DrawScreenText("Z", lines[2].Position, viewMatrix, projection);
            DrawScreenText("Y", lines[5].Position, viewMatrix, projection);
            if (!MainWindow.areDummyIdsVisible) return;
            foreach (FLVER.Dummy d in MainWindow.flver.Dummies)
                DrawScreenText(d.ReferenceID.ToString(), d.Position, viewMatrix, projection);
        }

        public static Ray GetMouseRay(Vector2 mousePosition, Viewport viewport, BasicEffect camera)
        {
            var nearPoint = new Vector3(mousePosition, 0);
            var farPoint = new Vector3(mousePosition, 1);
            nearPoint = viewport.Unproject(nearPoint, camera.Projection, camera.View, Matrix.Identity);
            farPoint = viewport.Unproject(farPoint, camera.Projection, camera.View, Matrix.Identity);
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            return new Ray(nearPoint, direction);
        }
    }
}