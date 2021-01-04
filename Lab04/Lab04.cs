using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lab04
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab04 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Model model; 

        Matrix world, view, projection;
        Effect effect;

        //3D camera and light
        Vector3 cameraOffset = new Vector3(0, 0, 0);
        Vector3 cameraPosition = new Vector3(0, 0, 10);
        Vector3 lightPosition = new Vector3(0.5f, 1, 1);
        //float lightAngle = 0;
        //float lightAngle2 = 0;
        float angle = 0;
        float angle2 = 0;
        //float distance = 10;
        int toggleTechnique = 0;

        //object materials
        Vector4 ambient = new Vector4(0, 0, 0, 0);
        float ambientIntensity = 0.9f;

        Vector3 lightDirection = new Vector3(0.5f, 0.6f, 0.4f);

        float specularIntensity = 0.9f;
        Vector4 specularColor = new Vector4(1, 1, 1, 1);
        Vector4 diffuseColor = new Vector4(1, 1, 1, 1);
        Vector3 diffuseLightDirection = new Vector3(1, 1, 1);
        float diffuseIntensity = 0.6f;

        float shininess = 7.0f;

        MouseState PreviousMouseState;

        public Lab04()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            model = Content.Load<Model>("Torus");
            effect = Content.Load<Effect>("SimpleShader");
            world = Matrix.Identity;
            view = Matrix.CreateLookAt(
                new Vector3(0, 0, 10),
                Vector3.Zero,
                Vector3.Up);
            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(90),
                GraphicsDevice.Viewport.AspectRatio,
                0.1f,
                100);
            

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                shininess -= 0.05f;

            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                shininess += 0.05f;

            }
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton == ButtonState.Pressed)
            {
                float offsetx = 0.01f * (Mouse.GetState().X - PreviousMouseState.X);
                float offsety = 0.01f * (Mouse.GetState().Y - PreviousMouseState.Y);
                angle += offsetx;
                angle2 += offsety;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                toggleTechnique = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                toggleTechnique = 1;
            }
            Vector3 camera = Vector3.Transform(
                new Vector3(0, 0, 20), Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
            view = Matrix.CreateLookAt(camera, Vector3.Zero, Vector3.UnitY);
            //view = Matrix.CreateLookAt(cameraPosition, new Vector3(), new Vector3(0, 1, 0));
            effect.Parameters["View"].SetValue(view);

            PreviousMouseState = Mouse.GetState();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            effect.CurrentTechnique = effect.Techniques[toggleTechnique];
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        effect.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                        effect.Parameters["AmbientColor"].SetValue(ambient);
                        effect.Parameters["AmbientIntensity"].SetValue(ambientIntensity);
                        effect.Parameters["LightPosition"].SetValue(lightDirection);
                        effect.Parameters["SpecularColor"].SetValue(specularColor);
                        effect.Parameters["specularIntensity"].SetValue(specularIntensity);
                        effect.Parameters["Shininess"].SetValue(shininess);
                        effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                        effect.Parameters["DiffuseLightDirection"].SetValue(diffuseLightDirection);
                        effect.Parameters["DiffuseColor"].SetValue(diffuseColor);
                        effect.Parameters["DiffuseIntensity"].SetValue(diffuseIntensity);


                        pass.Apply();
                        Matrix worldInverseTranspose = Matrix.Transpose(
                            Matrix.Invert(mesh.ParentBone.Transform));
                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTranspose);

                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;

                        GraphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList, part.VertexOffset, 0,
                            part.NumVertices, part.StartIndex, part.PrimitiveCount);

                    }
                }

            }

            base.Draw(gameTime);
        }
    }
}
