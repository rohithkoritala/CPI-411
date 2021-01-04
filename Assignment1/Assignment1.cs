using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Assignment1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Assignment1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Model model; 

        Matrix world, view, projection;
        Effect effect;

        Vector3 cameraOffset = new Vector3(0, 0, 0);
        Vector3 cameraPosition = new Vector3(0, 0, 10);
        Vector3 lightPosition = new Vector3(1f, 1, 1);

        float lightAngle = 0;
        float lightAngle2 = 0;
        float angle = 0;
        float angle2 = 0;
        float distance = 0f;
        float midMouseX = 0f;
        float midMouseY = 0f;
        int toggleTechnique = 0;
        bool boolHelp = true;
        bool boolInfo = false;
        string shaderType = "Gauraud";

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

        SpriteFont font;

        MouseState previousMouseState;

        public Assignment1()
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

            model = Content.Load<Model>("Box");
            world = Matrix.Identity;
            view = Matrix.CreateLookAt(
                new Vector3(0, 0, 20),
                Vector3.Zero,
                Vector3.Up);
            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(90),
                GraphicsDevice.Viewport.AspectRatio,
                0.1f,
                100);
            effect = Content.Load<Effect>("SimpleShader");
            font = Content.Load<SpriteFont>("Demo");
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

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Pressed)
            {

                angle += 0.01f * (Mouse.GetState().X - previousMouseState.X);
                angle2 += 0.01f * (Mouse.GetState().Y - previousMouseState.Y);
                Vector3 camera = Vector3.Transform(
                new Vector3(0, 0, 20), Matrix.CreateTranslation(0, 0, distance) * Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                view = Matrix.CreateLookAt(camera, Vector3.Zero, Vector3.UnitY);
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Pressed)
            {
                distance += 0.1f * (Mouse.GetState().Y - previousMouseState.Y);
                Vector3 camera = Vector3.Transform(
                    new Vector3(0, 0, 20), Matrix.CreateTranslation(0, 0, distance) * Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                view = Matrix.CreateLookAt(camera, Vector3.Zero, Vector3.UnitY);

            }
            if (Mouse.GetState().MiddleButton == ButtonState.Pressed && previousMouseState.MiddleButton == ButtonState.Pressed)
            {
                midMouseX = 1f * (Mouse.GetState().X - previousMouseState.X);
                midMouseY = 1f * (Mouse.GetState().Y - previousMouseState.Y);
                Vector3 camera = Vector3.Transform(
                new Vector3(0, 0, 20), Matrix.CreateTranslation(midMouseX, midMouseY, distance) * Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                view = Matrix.CreateLookAt(camera, Vector3.Zero, Vector3.UnitY);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                lightAngle -= 0.02f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                lightAngle += 0.02f;               
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                lightAngle2 += 0.02f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                lightAngle2 -= 0.02f;                
            }
            lightPosition = Vector3.Transform(new Vector3(0.5f, 1, 1), Matrix.CreateRotationX(lightAngle2) * Matrix.CreateRotationY(lightAngle));

            if (Keyboard.GetState().IsKeyDown(Keys.T))
            {
                view = Matrix.CreateLookAt(
                new Vector3(0, 0, 20),
                Vector3.Zero,
                Vector3.Up);
                lightPosition = new Vector3(0.5f, 1, 1);
                angle = 0;
                angle2 = 0;
                lightAngle = 0;
                lightAngle2 = 0;
                distance = 0f;
                midMouseX = 0f;
                midMouseY = 0f;
                ambient = new Vector4(0, 0, 0, 0);
            }


            effect.Parameters["View"].SetValue(view);
            effect.Parameters["World"].SetValue(world);
            effect.Parameters["LightPosition"].SetValue(lightPosition);

            if (Keyboard.GetState().IsKeyDown(Keys.D1))
            {
                model = Content.Load<Model>("Box");
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D2))
            {
                model = Content.Load<Model>("Sphere");
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D3))
            {
                model = Content.Load<Model>("Torus");
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D4))
            {
                model = Content.Load<Model>("Teapot");
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D5))
            {
                model = Content.Load<Model>("bunny");
            }


            if (Keyboard.GetState().IsKeyDown(Keys.Z))
            {
                toggleTechnique = 0;//Gouraud
                shaderType = "Gouraud";
            }
            if (Keyboard.GetState().IsKeyDown(Keys.X))
            {
                toggleTechnique = 1;//Phong
                shaderType = "Phong";
            }
            if (Keyboard.GetState().IsKeyDown(Keys.C))
            {
                toggleTechnique = 2;//PhongBlinn
                shaderType = "PhongBlinn";
            }
            if (Keyboard.GetState().IsKeyDown(Keys.V))
            {
                toggleTechnique = 3;//Schlick
                shaderType = "Schlick";
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                toggleTechnique = 4;//Toon
                shaderType = "Toon";

            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                toggleTechnique = 5;//HalfLife
                shaderType = "HalfLife";
            }

            if (Keyboard.GetState().IsKeyDown(Keys.L))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift))
                {
                    ambientIntensity -= 0.02f;
                }
                else
                {
                    ambientIntensity += 0.02f;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift))
                {
                    ambient -= new Vector4(0.02f, 0, 0, 0);
                }
                else
                {
                    ambient += new Vector4(0.02f, 0, 0, 0);
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.G))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift))
                {
                    ambient -= new Vector4(0, 0.02f, 0, 0);
                }
                else
                {
                    ambient += new Vector4(0, 0.02f, 0, 0);
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.B))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift))
                {
                    ambient -= new Vector4(0, 0, 0.02f, 0);
                }
                else
                {
                    ambient += new Vector4(0, 0, 0.02f, 0);
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                {
                    shininess += 0.02f;
                }
                else
                {
                    specularIntensity += 0.02f;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                {
                    shininess -= 0.02f;
                }
                else
                {
                    specularIntensity -= 0.02f;
                }

            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemQuestion))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                {
                    boolHelp = false;
                }
                else
                {
                    boolHelp = true;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.H))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                {
                    boolInfo = false;
                }
                else
                {
                    boolInfo = true;
                }
            }

            effect.Parameters["AmbientIntensity"].SetValue(ambientIntensity);
            effect.Parameters["AmbientColor"].SetValue(ambient);

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

            spriteBatch.Begin();
            if (boolHelp)
            {
                spriteBatch.DrawString(font, "light angle: " + lightAngle + "\nlight angle2: " + lightAngle2 +
                "\ncamera angle: " + angle + "\ncamera angle2: " + angle2 + "\ndistance: " + distance +
                "\nmidMouseX: " + midMouseX + "\nmidMouseY: " + midMouseY + "\nambient: " + ambient +
                "\nambientIntensity: " + ambientIntensity + "\nspecularIntensity: " + specularIntensity +
                "\nshininess: " + shininess + "\nview: " + view + "\nShader Type: " + shaderType, Vector2.UnitX + Vector2.UnitY * 12, Color.White);

            }
            if (boolInfo)
            {
                spriteBatch.DrawString(font, "To Rotate Camera drag Left Mouse\nTo change the distance of camera to the center drag Right Mouse \n" +
                    "To translate the camera drag middle mouse \nArrow keys to change light direction \nReset camera and light: T Key \n" +
                    "1: A box\n2: A Sphere \n3: A Torus \n4: A Tea Pot \n5: A Bunny\n" +
                    "Z: Gouraud (Phong per vertex) \nX: Phong per pixel\nC: PhongBlinn\nV: Schlick\nA: Toon \nS: HalfLife \n" +
                    "L: Increase the intensity of light (+ Shift key: decrease)\nR: Increase the red value of light (+ Shift key: decrease)\nG: Increase the green value of light (+ Shift key: decrease)\nB: Increase the blue value of light (+ Shift key: decrease)\n" +
                    "+ (plus):  Increases the intensity\n- (minus): Decreases the intensity", Vector2.UnitX + Vector2.UnitY * 12, Color.White);
            }
            spriteBatch.End();
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

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
                        effect.Parameters["LightPosition"].SetValue(lightPosition);
                        effect.Parameters["SpecularColor"].SetValue(specularColor);
                        effect.Parameters["SpecularIntensity"].SetValue(specularIntensity);
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
