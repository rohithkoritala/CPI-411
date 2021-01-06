using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI411.SimpleEngine;

namespace Assignment2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Assignment2 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Effect effect;
        Model model;
        Texture2D texture;
        SpriteFont font;

        Vector3 cameraOffset = new Vector3(0, 0, 0);
        Vector3 cameraPosition = new Vector3(0, 0, 0);
        float lightAngleX = 0;
        float lightAngleY = 0;
        float cameraAngleX = 0;
        float cameraAngleY = 0;

        Vector4 ambient = new Vector4(0.1f, 0.1f, 0.1f, 1);
        float ambientIntensity = 0.1f;
        Vector4 diffuseColor = new Vector4(1, 1, 1, 1);
        float diffuseIntensity = 1.0f;
        Vector4 specularColor = new Vector4(1, 1, 1, 1);
        float shininess = 100.0f;
        float etaRatio = 0.658f;
        Vector3 fresnelEtaRatio = new Vector3(0.1f, 0.1f, 0.1f);
        float reflectivity = 0.5f;
        float fresnelPower = 2;
        float fresnelScale = 15;
        float fresnelBias = 0.5f;

        Skybox skybox; //Added reference of SimpleEngine

        Matrix world = Matrix.Identity;
        Matrix view = Matrix.CreateLookAt(
                new Vector3(0, 0, 20),
                Vector3.Zero,
                Vector3.Up);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(90), 800f / 600f, 0.1f, 1000f);


        Vector3 lightPosition = new Vector3(0, 0, 1);
        float angle, angle2;
        float distance = 10.0f;
        MouseState previousMouseState;
        KeyboardState previousKeyboardState;
        int shaderID = 0;
        int skyboxID = 0;
        int modelID = 0;
        Model modBunny, modTorus, modCube, modSphere, modTeapot, modHeli;
        bool showMenu, showValue, texon;


        public Assignment2()
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

            font = Content.Load<SpriteFont>("Info");
            modBunny = Content.Load<Model>("bunny");
            modTorus = Content.Load<Model>("Torus");
            modCube = Content.Load<Model>("Box");
            modTeapot = Content.Load<Model>("Teapot");
            modSphere = Content.Load<Model>("Sphere");
            modHeli = Content.Load<Model>("Helicopter");

            effect = Content.Load<Effect>("Shader");
            model = Content.Load<Model>("cube"); 
            texture = Content.Load<Texture2D>("HelicopterTexture");
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

            bool shift = false;
            Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();
            foreach (Keys key in pressedKeys)
                if (key == Keys.LeftShift || key == Keys.RightShift) shift = true;
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) lightAngleX += 1.0f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) lightAngleX -= 1.0f;
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) lightAngleY += 1.0f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) lightAngleY -= 1.0f;
            if (Keyboard.GetState().IsKeyDown(Keys.S)) { distance = 10; cameraAngleX = cameraAngleY = lightAngleX = lightAngleY = 0; }

            if (Keyboard.GetState().IsKeyDown(Keys.D1)) { model = modCube; texon = false; }
            if (Keyboard.GetState().IsKeyDown(Keys.D2)) { model = modSphere; texon = false; }
            if (Keyboard.GetState().IsKeyDown(Keys.D3)) { model = modTorus; texon = false; }
            if (Keyboard.GetState().IsKeyDown(Keys.D4)) { model = modTeapot; texon = false; }
            if (Keyboard.GetState().IsKeyDown(Keys.D5)) { model = modBunny; texon = false; }
            if (Keyboard.GetState().IsKeyDown(Keys.D6)) { model = modHeli; texon = true; }

            if (Keyboard.GetState().IsKeyDown(Keys.OemPlus)) reflectivity += 0.01f;
            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus)) reflectivity -= 0.01f;

            if (Keyboard.GetState().IsKeyDown(Keys.F7)) shaderID = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.F8)) shaderID = 1;
            if (Keyboard.GetState().IsKeyDown(Keys.F9)) shaderID = 2;
            if (Keyboard.GetState().IsKeyDown(Keys.F10)) shaderID = 3;

            if (Keyboard.GetState().IsKeyDown(Keys.D7)) skyboxID = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.D8)) skyboxID = 1;
            if (Keyboard.GetState().IsKeyDown(Keys.D9)) skyboxID = 2;
            if (Keyboard.GetState().IsKeyDown(Keys.D0)) skyboxID = 3;
            if (Keyboard.GetState().IsKeyDown(Keys.P)) skyboxID = 4;

            if (Keyboard.GetState().IsKeyDown(Keys.R) && !shift) fresnelEtaRatio.X += 0.04f;
            if (Keyboard.GetState().IsKeyDown(Keys.G) && !shift) fresnelEtaRatio.Y += 0.04f;
            if (Keyboard.GetState().IsKeyDown(Keys.B) && !shift) fresnelEtaRatio.Z += 0.04f;

            if (Keyboard.GetState().IsKeyDown(Keys.R) && shift) fresnelEtaRatio.X -= 0.04f;
            if (Keyboard.GetState().IsKeyDown(Keys.G) && shift) fresnelEtaRatio.Y -= 0.04f;
            if (Keyboard.GetState().IsKeyDown(Keys.B) && shift) fresnelEtaRatio.Z -= 0.04f;

            if (Keyboard.GetState().IsKeyDown(Keys.Q) && !shift) fresnelPower += 0.2f;
            if (Keyboard.GetState().IsKeyDown(Keys.W) && !shift) fresnelScale += 1f;
            if (Keyboard.GetState().IsKeyDown(Keys.E) && !shift) fresnelBias += 0.02f;

            if (Keyboard.GetState().IsKeyDown(Keys.Q) && shift) fresnelPower -= 0.2f;
            if (Keyboard.GetState().IsKeyDown(Keys.W) && shift) fresnelScale -= 1f;
            if (Keyboard.GetState().IsKeyDown(Keys.E) && shift) fresnelBias -= 0.02f;

            if (Keyboard.GetState().IsKeyDown(Keys.H) && !previousKeyboardState.IsKeyDown(Keys.H)) showValue = !showValue;
            if (Keyboard.GetState().IsKeyDown(Keys.OemQuestion) && !previousKeyboardState.IsKeyDown(Keys.OemQuestion)) showMenu = !showMenu;


            if (previousMouseState.RightButton == ButtonState.Pressed && Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                cameraOffset.Z += (Mouse.GetState().X - previousMouseState.X) / 100f;
                distance += (Mouse.GetState().X - previousMouseState.X) / 100f;
            }
            if (previousMouseState.LeftButton == ButtonState.Pressed && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                cameraAngleX += (previousMouseState.X - Mouse.GetState().X);
                cameraAngleY += (previousMouseState.Y - Mouse.GetState().Y);
            }
            if (previousMouseState.MiddleButton == ButtonState.Pressed && Mouse.GetState().MiddleButton == ButtonState.Pressed)
            {
                cameraOffset.X += (Mouse.GetState().X - previousMouseState.X) / 100f;
                cameraOffset.Y -= (Mouse.GetState().Y - previousMouseState.Y) / 100f;
            }
            cameraPosition = Vector3.Transform(new Vector3(0, 0, distance),
                Matrix.CreateRotationX(MathHelper.ToRadians(cameraAngleY)) *
                Matrix.CreateRotationY(MathHelper.ToRadians(cameraAngleX)));

            view = Matrix.CreateLookAt(
                cameraPosition,
                Vector3.Zero,
                Vector3.Transform(
                    Vector3.UnitY,
                    Matrix.CreateRotationX(MathHelper.ToRadians(cameraAngleY)) *
                    Matrix.CreateRotationY(MathHelper.ToRadians(cameraAngleX))
                    )
                );

            lightPosition = Vector3.Transform(new Vector3(0, 0, 1),
                Matrix.CreateRotationX(MathHelper.ToRadians(lightAngleX)) *
                Matrix.CreateRotationY(MathHelper.ToRadians(lightAngleY)));

            previousMouseState = Mouse.GetState();

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

            if (skyboxID == 0)
            {
                string[] skyboxTextures =
                {
                    "test_posx", "test_negx",
                    "test_posy", "test_negy",
                    "test_posz", "test_negz",
                };
                skybox = new Skybox(skyboxTextures, Content, GraphicsDevice);
            }
            else if (skyboxID == 1)
            {
                string[] skyboxTextures =
                {
                    "arch_posx", "arch_negx",
                    "arch_posy", "arch_negy",
                    "arch_posz", "arch_negz",
                };
                skybox = new Skybox(skyboxTextures, Content, GraphicsDevice);
            }
            else if (skyboxID == 2)
            {
                string[] skyboxTextures =
                {
                    "grandcanyon_posx", "grandcanyon_negx",
                    "grandcanyon_posy", "grandcanyon_negy",
                    "grandcanyon_posz", "grandcanyon_negz",
                };
                skybox = new Skybox(skyboxTextures, Content, GraphicsDevice);
            }
            else if (skyboxID == 3)
            {
                string[] skyboxTextures =
                {
                    "hills_posx", "hills_negx",
                    "hills_posy", "hills_negy",
                    "hills_posz", "hills_negz",
                };
                skybox = new Skybox(skyboxTextures, Content, GraphicsDevice);
            }
            else if (skyboxID == 4)
            {
                string[] skyboxTextures =
                {
                    "nvlobby_new_posx", "nvlobby_new_negx",
                    "nvlobby_new_posy", "nvlobby_new_negy",
                    "nvlobby_new_posz", "nvlobby_new_negz",
                };
                skybox = new Skybox(skyboxTextures, Content, GraphicsDevice);
            }
 
            skybox.Draw(view, projection, cameraPosition);
            //Draw 3D Helicopter
            graphics.GraphicsDevice.RasterizerState = originalRasterizerState;
            DrawModelWithEffect();
            spriteBatch.Begin();
            if (showMenu) showHelp();
            if (showValue) showInfo();
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void showHelp()
        {
            int i = 0;
            spriteBatch.DrawString(font, "Mouse: Views", Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Arrow Keys: Light Position", Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Type S Key: Reset Camera", Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Type 1,2,3,4,5,6: Model Change", Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Type 7,8,9,0,P: Skybox Texture change", Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Type F7,F8,F9,F10: Shader Change", Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Type plus, minus: Reflectivity Change", Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Type R, G, B Keys (+shift decreases): Color Change", Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Type Q, W, E Keys(+shift decreases): Fresnel Power, Scale, Bias change", Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Type H Key: Display Parameters", Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Type ? Key: Help Menu", Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
        }

        private void showInfo()
        {
            int i = 0;
            spriteBatch.DrawString(font, "Light Angles X(" + lightAngleX + "), Y(" + lightAngleY + ")", Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Light Pos" + show(lightPosition), Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Camera Angles X(" + cameraAngleX + "), Y(" + cameraAngleY + ")", Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Camera Pos" + show(cameraPosition), Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Shader: " + shaderID, Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Reflectivity: " + reflectivity, Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Fresnel Power: " + fresnelPower, Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Fresnel Bias: " + fresnelBias, Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Fresnel Scale: " + fresnelScale, Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Fresnel etaRatio: " + show(fresnelEtaRatio), Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);

        }

        private string show(Vector4 v)
        {
            return "(" + v.X.ToString("0.00") + "," + v.Y.ToString("0.00") + "," + v.Z.ToString("0.00") + "," + v.W.ToString("0.00") + ")";
        }
        private string show(Vector3 v)
        {
            return "(" + v.X.ToString("0.00") + "," + v.Y.ToString("0.00") + "," + v.Z.ToString("0.00") + ")";
        }
        void DrawModelWithEffect()
        {
            effect.CurrentTechnique = effect.Techniques[shaderID];
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
                        effect.Parameters["DiffuseLightDirection"].SetValue(lightPosition);
                        effect.Parameters["AmbientColor"].SetValue(ambient);
                        effect.Parameters["AmbientIntensity"].SetValue(ambientIntensity);
                        effect.Parameters["DiffuseColor"].SetValue(diffuseColor);
                        effect.Parameters["DiffuseIntensity"].SetValue(diffuseIntensity);
                        effect.Parameters["SpecularColor"].SetValue(specularColor);
                        effect.Parameters["SpecularIntensity"].SetValue(shininess);
                        effect.Parameters["EtaRatio"].SetValue(etaRatio);
                        effect.Parameters["FresnelEtaRatio"].SetValue(fresnelEtaRatio);
                        effect.Parameters["Reflectivity"].SetValue(reflectivity);
                        effect.Parameters["FresnelBias"].SetValue(fresnelBias);
                        effect.Parameters["FresnelScale"].SetValue(fresnelScale);
                        effect.Parameters["FresnelPower"].SetValue(fresnelPower);


                        if (texon) effect.Parameters["decalMap"].SetValue(texture);
                        effect.Parameters["environmentMap"].SetValue(skybox.skyBoxTexture);
                        pass.Apply();
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;
                        GraphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList, part.VertexOffset, 0,
                            part.NumVertices, part.StartIndex, part.PrimitiveCount);
                    }
                }
            }
        }
    }
}
