using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lab07
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab07 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Model model;
        Texture2D texture;
        Effect effect;

        Matrix world = Matrix.Identity;
        Matrix view = Matrix.CreateLookAt(
                new Vector3(0, 0, 20),
                Vector3.Zero,
                Vector3.Up);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(90), 800f / 600f, 0.1f, 100f);

        Vector3 cameraPosition = new Vector3(0, 0, 10);
        Vector3 lightPosition = new Vector3(0, 1, 0);
        float angle, angle2;
        float angleL, angleL2;

        MouseState previousMouseState; 

        public Lab07()
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

            model = Content.Load<Model>("Plane(1)");
            effect = Content.Load<Effect>("BumpMap");
            texture = Content.Load<Texture2D>("round");
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

            MouseState currentMouseState = Mouse.GetState();

            if (currentMouseState.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Pressed)
            {
                angle -= (previousMouseState.X - currentMouseState.X) / 100f;
                angle2 -= (previousMouseState.Y - currentMouseState.Y) / 100f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up)) angleL += 1.0f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) angleL -= 1.0f;
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) angleL2 += 1.0f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) angleL2 -= 1.0f;
            cameraPosition = Vector3.Transform(Vector3.Zero,
                Matrix.CreateTranslation(new Vector3(0, 0, 20)) *
                Matrix.CreateRotationX(angle2) *
                Matrix.CreateRotationY(angle));
            //view = Matrix.CreateRotationY(angle) * Matrix.CreateRotationX(angle2) *
            //    Matrix.CreateTranslation(-cameraPosition);
            view = Matrix.CreateLookAt(
                cameraPosition,
                Vector3.Zero,
                Vector3.Up);

            lightPosition = Vector3.Transform(new Vector3(0, 0, 1),
                Matrix.CreateRotationX(MathHelper.ToRadians(angleL)) *
                Matrix.CreateRotationY(MathHelper.ToRadians(angleL2)));
            previousMouseState = currentMouseState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = new DepthStencilState();
            effect.CurrentTechnique = effect.Techniques[0];
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        effect.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                        effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                        effect.Parameters["LightPosition"].SetValue(lightPosition);

                        Matrix worldInverseTranspose = Matrix.Transpose(
                            Matrix.Invert(mesh.ParentBone.Transform));

                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTranspose);
                        effect.Parameters["normalMap"].SetValue(texture);
                        effect.Parameters["Shininess"].SetValue(5f);

                        pass.Apply();
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
