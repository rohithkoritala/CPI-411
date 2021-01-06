using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI411.SimpleEngine;

namespace Assignment3
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Assignment3 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        Model model;
        int pic;
        Texture2D texture;
        Effect effect;
        Skybox skybox;
        int mode;
        float bumpHeight;
        int toggleTechnique = 0;
        float etaRatio = 0.658f;
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

        public Assignment3()
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
            string[] skyboxTextures =
              {
                    "nvlobby_new_posx", "nvlobby_new_negx",
                    "nvlobby_new_posy", "nvlobby_new_negy",
                    "nvlobby_new_posz", "nvlobby_new_negz",
                };
            skybox = new Skybox(skyboxTextures, Content, GraphicsDevice);
            model = Content.Load<Model>("Torus");
            effect = Content.Load<Effect>("BumpNomal");
            texture = Content.Load<Texture2D>("crossHatch");
          

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
            if (Keyboard.GetState().IsKeyDown(Keys.F1))
            {
                effect = Content.Load<Effect>("BumpNomal");
                pic++;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F3))
            {
                effect = Content.Load<Effect>("BumpMap");
                pic++;
               
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F4))
            {
                effect = Content.Load<Effect>("BumpReflect");
                effect.Parameters["environmentMap"].SetValue(skybox.skyBoxTexture);
                pic++;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F5))
            {
                effect = Content.Load<Effect>("BumpRefract");
                effect.Parameters["environmentMap"].SetValue(skybox.skyBoxTexture);
                effect.Parameters["EtaRatio"].SetValue(etaRatio);
                pic++;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F10))
            {
                pic = 5;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up)) angleL += 2.0f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) angleL -= 1.0f;
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) angleL2 += 2.0f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) angleL2 -= 1.0f;


            if (Keyboard.GetState().IsKeyDown(Keys.D1)) texture = Content.Load<Texture2D>("art");
            if (Keyboard.GetState().IsKeyDown(Keys.D2)) texture = Content.Load<Texture2D>("BumpTest");
            if (Keyboard.GetState().IsKeyDown(Keys.D3)) texture = Content.Load<Texture2D>("crossHatch");
            if (Keyboard.GetState().IsKeyDown(Keys.D4)) texture = Content.Load<Texture2D>("monkey");
            if (Keyboard.GetState().IsKeyDown(Keys.D5)) texture = Content.Load<Texture2D>("nm");
            if (Keyboard.GetState().IsKeyDown(Keys.D6)) texture = Content.Load<Texture2D>("round");
            if (Keyboard.GetState().IsKeyDown(Keys.D7)) texture = Content.Load<Texture2D>("saint");
            if (Keyboard.GetState().IsKeyDown(Keys.D8)) texture = Content.Load<Texture2D>("science");
            if (Keyboard.GetState().IsKeyDown(Keys.D9)) texture = Content.Load<Texture2D>("square");

            if (Keyboard.GetState().IsKeyDown(Keys.U))
            {
                bumpHeight += 0.25f;              
            }
            if (Keyboard.GetState().IsKeyDown(Keys.V))
            {
                bumpHeight -= 0.25f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                angle = 0; 
                angle2 = 0;
                angleL = 0;
                angleL2 = 0;
            }

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

            lightPosition = Vector3.Transform(new Vector3(0, 0, 10),
                Matrix.CreateRotationX(MathHelper.ToRadians(angleL)) *
                Matrix.CreateRotationY(MathHelper.ToRadians(angleL2)));

            effect.Parameters["height"].SetValue(bumpHeight);
            if (Keyboard.GetState().IsKeyDown(Keys.Z))
            {
                toggleTechnique = 0;//Gouraud
                ;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.X))
            {
                toggleTechnique = 1;//Phong
                
            }


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
            RasterizerState originalRasterizerState = graphics.GraphicsDevice.RasterizerState;

            RasterizerState ras = new RasterizerState();
            ras.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = ras;

            skybox.Draw(view, projection, cameraPosition);
            graphics.GraphicsDevice.RasterizerState = originalRasterizerState;
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
                        Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform));
                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                        effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                        effect.Parameters["LightPosition"].SetValue(lightPosition);
                        effect.Parameters["DiffuseIntensity"].SetValue(1.0f);
                        effect.Parameters["DiffuseColor"].SetValue(new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
                        effect.Parameters["SpecularIntensity"].SetValue(1.0f);
                        effect.Parameters["SpecularColor"].SetValue(new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
                        effect.Parameters["Shininess"].SetValue(100.0f);
                        effect.Parameters["normalMap"].SetValue(texture);
                      //  effect.Parameters["decalMap"].SetValue(texture);
                        

                        pass.Apply();
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;
                        GraphicsDevice.DrawIndexedPrimitives(
              PrimitiveType.TriangleList, part.VertexOffset, 0,
              part.NumVertices, part.StartIndex, part.PrimitiveCount);
                    }
                }
            }
            spriteBatch.Begin();
            if(pic == 5) spriteBatch.Draw(texture, Vector2.Zero);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
