using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using CPI411.SimpleEngine;

namespace Assignment4
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Assignment4 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        Effect effect;
        int currentTexture = 3;
        Matrix world = Matrix.Identity;
        Matrix view = Matrix.CreateLookAt(
            new Vector3(0, 0, 20),
            new Vector3(0, 0, 0),
            Vector3.UnitY);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(45),
            800f / 600f,
            0.1f,
            100f);
        Vector3 cameraPosition, cameraTarget, lightPosition;
        KeyboardState preKey;
        float angle = 0;
        float angle2 = 0;
        float angleL = 0;
        float angleL2 = 0;
        float distance = 20;
        MouseState preMouse;
        Model model;
        Model[] models;
        Texture2D texture;
        //Lab 8
        Matrix lightView;
        Matrix lightProjection;

        Matrix invertCamera;
        ParticleManager particleManager;
        Vector3 particlePosition;
        System.Random random;

        Texture2D[] textures;
        int emitterShape = 0;
        int fountainType = 0;
        int counter = 0;
        int time = 0;
        float bounciness = 1f;
        float friction = 0.5f;
        Vector3 wind = Vector3.Zero;
        bool usingWind = false;
        float gravity = -8f;
        float randomness = 5f;
        bool basicUpwards = true;
        bool usingRandomness = true;
        Vector3 velocityOverride = Vector3.Zero;
        bool usingUserVelocity = false;
        int lifeSpan = 4;
        int age = 0;
        bool usingAge = false;
        bool showHelp;
        bool showInformation;
        Model plane;

        public Assignment4()
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
            textures = new Texture2D[3];
            effect = Content.Load<Effect>("ParticleShader");
            font = Content.Load<SpriteFont>("font");
            // texture = Content.Load<Texture2D>("fire");
            textures[0] = Content.Load<Texture2D>("smoke");
            textures[1] = Content.Load<Texture2D>("water");
            textures[2] = Content.Load<Texture2D>("fire");
            // model = Content.Load<Model>("torus");
            plane = Content.Load<Model>("Plane");
            random = new System.Random();
            particleManager = new ParticleManager(GraphicsDevice, 100);
            particlePosition = new Vector3(0, 0, 0);
            showHelp = true;
            showInformation = true;
            angleL = 3.18f;
            angleL2 = 1.68f;
            distance = 30f;

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

            if (Keyboard.GetState().IsKeyDown(Keys.Left)) angleL += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) angleL -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) angleL2 += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) angleL2 -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                angle = angle2 = angleL = angleL2 = 0;
                distance = 30;
                cameraTarget = Vector3.Zero;
                angleL = 3.18f;
                angleL2 = 1.68f;
                counter = 0;
                time = 0;
                bounciness = 1f;
                friction = 0.5f;
                wind = Vector3.Zero;
                usingWind = false;
                gravity = -8f;
                randomness = 5f;
                basicUpwards = true;
                usingRandomness = true;
                velocityOverride = Vector3.Zero;
                lifeSpan = 4;
                age = 0;
                usingAge = false;
            }
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                angle -= (Mouse.GetState().X - preMouse.X) / 100f;
                angle2 += (Mouse.GetState().Y - preMouse.Y) / 100f;
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                distance += (Mouse.GetState().X - preMouse.X) / 100f;
            }

            if (Mouse.GetState().MiddleButton == ButtonState.Pressed)
            {
                Vector3 ViewRight = Vector3.Transform(Vector3.UnitX,
                    Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                Vector3 ViewUp = Vector3.Transform(Vector3.UnitY,
                    Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                cameraTarget -= ViewRight * (Mouse.GetState().X - preMouse.X) / 10f;
                cameraTarget += ViewUp * (Mouse.GetState().Y - preMouse.Y) / 10f;
            }
            preMouse = Mouse.GetState();
            cameraPosition = Vector3.Transform(new Vector3(0, 0, distance),
                Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle) * Matrix.CreateTranslation(cameraTarget));
            view = Matrix.CreateLookAt(
                cameraPosition,
                cameraTarget,
                Vector3.Transform(Vector3.UnitY, Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle)));
            lightPosition = Vector3.Transform(
                new Vector3(0, 0, 10),
                Matrix.CreateRotationX(angleL2) * Matrix.CreateRotationY(angleL));
            lightView = Matrix.CreateLookAt(lightPosition, Vector3.Zero, Vector3.Up);
            lightProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2,
                1f, 1f, 100f);

            if (Keyboard.GetState().IsKeyDown(Keys.P))
            {
                /*Particle particle = particleManager.getNext();
                particle.Position = particlePosition;
                particle.Velocity = new Vector3(0, 0, 0);
                particle.Acceleration = new Vector3(0, 0, 0);
                particle.MaxAge = 1;
                particle.Init();*/
                Particle particle = (Particle)null;
                if (emitterShape == 0)
                {
                    particle = particleManager.getNext();
                    particle.Position = particlePosition;
                    particle.MaxAge = (float)lifeSpan;
                    particle.Velocity = Vector3.Zero;
                }
                else if (emitterShape == 1)
                {
                    particle = particleManager.getNext();
                    particle.Position = particlePosition;
                    particle.MaxAge = (float)lifeSpan;
                    particle.Position += Vector3.UnitX * 2f * (float)Math.Sin((double)MathHelper.ToRadians((float)(5 * counter)));
                    particle.Velocity = Vector3.Zero;
                    counter++;
                }
                else if (emitterShape == 2)
                {
                    for (int index = 0; index < 60; ++index)
                    {
                        particle = particleManager.getNext();
                        particle.Position = particlePosition;
                        particle.MaxAge = (float)lifeSpan;
                        particle.Velocity = new Vector3(10f * (float)Math.Sin((double)MathHelper.ToRadians((float)(index * 6))), 0.0f, 10f * (float)Math.Cos((double)MathHelper.ToRadians((float)(index * 6))));
                        if (fountainType == 0)
                        {
                            particle.Velocity += Vector3.UnitY * 5f * (basicUpwards ? 1f : -1f);
                            particle.Acceleration = Vector3.Zero;
                        }
                        else if (fountainType == 1)
                        {
                            particle.Velocity += new Vector3((float)((random.NextDouble() - 0.5) * 2.0) * randomness,
                                                    (float)((random.NextDouble() - 0.5) * 2.0) * randomness,
                                                    (float)((random.NextDouble() - 0.5) * 2.0) * randomness);
                            particle.Acceleration = Vector3.UnitY * gravity;
                        }
                        else if (this.fountainType == 2)
                        {
                            particle.Velocity += new Vector3((float)((random.NextDouble() - 0.5) * 2.0) * randomness,
                                                            (float)((random.NextDouble() - 0.5) * 2.0) * randomness,
                                                            (float)((random.NextDouble() - 0.5) * 2.0) * randomness);
                            particle.Acceleration = Vector3.UnitY * gravity;
                        }
                        if (usingUserVelocity)
                            particle.Velocity = velocityOverride;
                        particle.Init();
                    }
                }
                else if (emitterShape == 3)
                {
                    for (int index1 = 0; index1 <= 31; ++index1)
                    {
                        for (int index2 = 0; index2 <= 31; ++index2)
                        {
                            if (index1 == 0 || index1 == 31 || index2 == 0 && index1 > 0 && index1 < 31 || index2 == 31 && index1 > 0 && index1 < 31)
                            {
                                particle = particleManager.getNext();
                                particle.Position = particlePosition;
                                particle.MaxAge = (float)lifeSpan;
                                particle.Velocity = new Vector3((float)(index1 - 16), 0.0f, (float)(index2 - 16));
                                if (fountainType == 0)
                                {
                                    particle.Velocity += Vector3.UnitY * 5f * (basicUpwards ? 1f : -1f);
                                    particle.Acceleration = Vector3.Zero;
                                }
                                else if (fountainType == 1)
                                {
                                    particle.Velocity += new Vector3((float)((random.NextDouble() - 0.5) * 2.0) * randomness,
                                                                    (float)((random.NextDouble() - 0.5) * 2.0) * randomness,
                                                                    (float)((random.NextDouble() - 0.5) * 2.0) * randomness);
                                    particle.Acceleration = Vector3.UnitY * gravity;
                                }
                                else if (fountainType == 2)
                                {
                                    particle.Velocity += new Vector3((float)((random.NextDouble() - 0.5) * 2.0) * randomness,
                                                                    (float)((random.NextDouble() - 0.5) * 2.0) * randomness,
                                                                    (float)((random.NextDouble() - 0.5) * 2.0) * randomness);
                                    particle.Acceleration = Vector3.UnitY * gravity;
                                }
                                if (usingUserVelocity)
                                    particle.Velocity = velocityOverride;
                                particle.Init();
                            }
                        }
                    }
                }
                if (emitterShape < 2)
                {
                    if (fountainType == 0)
                    {
                        particle.Velocity += Vector3.UnitY * 5f * (basicUpwards ? 1f : -1f);
                        particle.Acceleration = Vector3.Zero;
                    }
                    else if (fountainType == 1)
                    {
                        particle.Velocity += new Vector3((float)((random.NextDouble() - 0.5) * 2.0) * randomness, 
                                            (float)((random.NextDouble() - 0.5) * 2.0) * randomness, 
                                            (float)((random.NextDouble() - 0.5) * 2.0) * randomness);
                        particle.Acceleration = Vector3.UnitY * gravity;
                    }
                    else if (fountainType == 2)
                    {
                        particle.Velocity += new Vector3((float)((random.NextDouble() - 0.5) * 2.0) * randomness, 
                                                        (float)((random.NextDouble() - 0.5) * 2.0) * randomness, 
                                                        (float)((random.NextDouble() - 0.5) * 2.0) * randomness);
                        particle.Acceleration = Vector3.UnitY * gravity;
                    }
                    if (usingUserVelocity)
                        particle.Velocity = velocityOverride;
                    particle.Init();
                }
            }

        particleManager.Update(gameTime.ElapsedGameTime.Milliseconds * 0.001f);
        if (fountainType == 1)
            {
             foreach (Particle particle in particleManager.particles)
              {
               if (particle.IsActive())
                {
                  if ((double)particle.Position.Y < 1.75)
                   {
                      particle.Position = new Vector3(particle.Position.X, 1.75f, particle.Position.Z);
                      particle.Velocity = new Vector3(particle.Velocity.X, 0.0f, particle.Velocity.Z);
                      particle.Acceleration = new Vector3(particle.Acceleration.X, 0.0f, particle.Acceleration.Z);
                   }
                  if ((double)particle.Position.Y == 1.75)
                  particle.Velocity = new Vector3(particle.Velocity.X * (float)Math.Pow(1.0 - (double)friction, 0.03), 
                                0.0f, particle.Velocity.Z * (float)Math.Pow(1.0 - (double)friction, 0.03));
                }
              }
            }
        else if (fountainType == 2)
        {
          foreach (Particle particle in particleManager.particles)
           {
             if (particle.IsActive())
              {
                particle.Acceleration = new Vector3(particle.Acceleration.X, gravity, particle.Acceleration.Z);
                if ((double)particle.Position.Y < 1.75)
                 {
                   particle.Position = new Vector3(particle.Position.X, 1.75f, particle.Position.Z);
                   particle.Velocity = new Vector3(particle.Velocity.X, -particle.Velocity.Y * bounciness, 
                                            particle.Velocity.Z);
                 }
              }
           }
        }
        if (this.usingWind)
         {
          foreach (Particle particle in this.particleManager.particles)
           {
            if (particle.IsActive())
             {
               if (usingRandomness && time % 60 == 0)
               wind = new Vector3((float)((random.NextDouble() - 0.5) * 0.0500000007450581) * randomness, 0.0f, 
                    (float)((random.NextDouble() - 0.5) * 0.0500000007450581) * randomness);
               particle.Position += wind;
             }
           }
         }
        time++;
        if (usingAge)
         {
          foreach (Particle particle in particleManager.particles)
           {
            if (particle.IsActive())
            particle.Age = (float)age;
           }
         }

        if (Keyboard.GetState().IsKeyDown(Keys.D1))
                currentTexture = 3;            
        if (Keyboard.GetState().IsKeyDown(Keys.D2))
                currentTexture = 0;          
        if (Keyboard.GetState().IsKeyDown(Keys.D3))
                currentTexture = 1;          
        if (Keyboard.GetState().IsKeyDown(Keys.D4))
                currentTexture = 2;          
        if (Keyboard.GetState().IsKeyDown(Keys.F1))
                fountainType = 0;          
        if (Keyboard.GetState().IsKeyDown(Keys.F2))
                fountainType = 1;
        if (Keyboard.GetState().IsKeyDown(Keys.F3))
                fountainType = 2;          
        if (Keyboard.GetState().IsKeyDown(Keys.F4) && !preKey.IsKeyDown(Keys.F4))
                emitterShape = (emitterShape + 1) % 4;

            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {                
                int num;
                if (!Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {                   
                    num = !Keyboard.GetState().IsKeyDown(Keys.RightShift) ? 1 : 0;
                }
                else
                    num = 0;
                friction = num != 0 ? MathHelper.Clamp(friction - 0.005f, 0.0f, 1f) : 
                                        MathHelper.Clamp(friction + 0.005f, 0.0f, 1f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.B))
            {
                int num;
                if (!Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    num = !Keyboard.GetState().IsKeyDown(Keys.RightShift) ? 1 : 0;
                }
                else
                    num = 0;
                bounciness = num != 0 ? MathHelper.Clamp(bounciness - 0.005f, 0.0f, 1f) : 
                                        MathHelper.Clamp(bounciness + 0.005f, 0.0f, 1f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.G))
            {               
                int num;
                if (!Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {                   
                    num = !Keyboard.GetState().IsKeyDown(Keys.RightShift) ? 1 : 0;
                }
                else
                    num = 0;
                gravity = num != 0 ? MathHelper.Clamp(gravity - 0.05f, -20f, 20f) : 
                    MathHelper.Clamp(gravity + 0.05f, -20f, 20f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                int num;
                if (!Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    num = !Keyboard.GetState().IsKeyDown(Keys.RightShift) ? 1 : 0;
                }
                else
                    num = 0;
                randomness = num != 0 ? MathHelper.Clamp(randomness - 0.05f, 0.0f, 20f) : 
                    MathHelper.Clamp(randomness + 0.05f, 0.0f, 20f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.X))
            {
                int num;
                if (!Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    num = !Keyboard.GetState().IsKeyDown(Keys.RightShift) ? 1 : 0;
                }
                else
                    num = 0;
                wind.X = num != 0 ? MathHelper.Clamp(wind.X - 0.005f, -0.1f, 0.1f) : 
                    MathHelper.Clamp(wind.X + 0.005f, -0.1f, 0.1f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Y))
            {
                int num;
                if (!Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    num = !Keyboard.GetState().IsKeyDown(Keys.RightShift) ? 1 : 0;
                }
                else
                    num = 0;
                wind.Y = num != 0 ? MathHelper.Clamp(wind.Y - 0.005f, -0.1f, 0.1f) : 
                    MathHelper.Clamp(wind.Y + 0.005f, -0.1f, 0.1f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Z))
            {
                int num;
                if (!Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    num = !Keyboard.GetState().IsKeyDown(Keys.RightShift) ? 1 : 0;
                }
                else
                    num = 0;
                wind.Z = num != 0 ? MathHelper.Clamp(wind.Z - 0.005f, -0.1f, 0.1f) : 
                    MathHelper.Clamp(wind.Z + 0.005f, -0.1f, 0.1f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.U))
            {
                int num;
                if (!Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    num = !Keyboard.GetState().IsKeyDown(Keys.RightShift) ? 1 : 0;
                }
                else
                    num = 0;
                velocityOverride.X = num != 0 ? MathHelper.Clamp(velocityOverride.X - 0.05f, -5f, 5f) : 
                    MathHelper.Clamp(velocityOverride.X + 0.05f, -5f, 5f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.I))
            {
                int num;
                if (!Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    num = !Keyboard.GetState().IsKeyDown(Keys.RightShift) ? 1 : 0;
                }
                else
                    num = 0;
                velocityOverride.Y = num != 0 ? MathHelper.Clamp(velocityOverride.Y - 0.05f, -5f, 5f) : 
                    MathHelper.Clamp(velocityOverride.Y + 0.05f, -5f, 5f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.O))
            {
                int num;
                if (!Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    num = !Keyboard.GetState().IsKeyDown(Keys.RightShift) ? 1 : 0;
                }
                else
                    num = 0;
                velocityOverride.Z = num != 0 ? MathHelper.Clamp(velocityOverride.Z - 0.05f, -5f, 5f) : 
                    MathHelper.Clamp(velocityOverride.Z + 0.05f, -5f, 5f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.T) && !preKey.IsKeyDown(Keys.T))
                usingRandomness = !usingRandomness;
            if (Keyboard.GetState().IsKeyDown(Keys.D) && !preKey.IsKeyDown(Keys.D))
                basicUpwards = !basicUpwards;
            if (Keyboard.GetState().IsKeyDown(Keys.W) && !preKey.IsKeyDown(Keys.W))
                usingWind = !usingWind;
            if (Keyboard.GetState().IsKeyDown(Keys.V) && !preKey.IsKeyDown(Keys.V))
                usingUserVelocity = !usingUserVelocity;
            if (Keyboard.GetState().IsKeyDown(Keys.L) && !preKey.IsKeyDown(Keys.L))
            {

                int num;
                if (!Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    num = !Keyboard.GetState().IsKeyDown(Keys.RightShift) ? 1 : 0;
                }
                else
                    num = 0;
                lifeSpan = num != 0 ? (int)MathHelper.Clamp((float)(lifeSpan - 1), 0.0f, 20f) : 
                    (int)MathHelper.Clamp((float)(lifeSpan + 1), 0.0f, 20f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A) && !preKey.IsKeyDown(Keys.A))
            {
                int num;
                if (!Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    num = !Keyboard.GetState().IsKeyDown(Keys.RightShift) ? 1 : 0;
                }
                else
                    num = 0;
                age = num != 0 ? MathHelper.Clamp(age - 1, 0, lifeSpan) : 
                    MathHelper.Clamp(age + 1, 0, lifeSpan);
            }
            age = MathHelper.Clamp(age, 0, lifeSpan);
            if (Keyboard.GetState().IsKeyDown(Keys.Q) && !preKey.IsKeyDown(Keys.Q))
                usingAge = !usingAge;
            if (Keyboard.GetState().IsKeyDown(Keys.OemQuestion) && !preKey.IsKeyDown(Keys.OemQuestion))
                showHelp = !showHelp;
            if (Keyboard.GetState().IsKeyDown(Keys.H) && !preKey.IsKeyDown(Keys.H))
                showInformation = !showInformation;

            invertCamera = Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle);

        base.Update(gameTime);
        }
                    

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = new DepthStencilState();

            plane.Draw(world, view, projection);

            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            effect.CurrentTechnique = effect.Techniques[0];
            effect.CurrentTechnique.Passes[0].Apply();
            effect.Parameters["World"].SetValue(world);
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);
            effect.Parameters["InverseCamera"].SetValue(invertCamera);
            if(currentTexture != 3)
            effect.Parameters["Texture"].SetValue(textures[currentTexture]);

            particleManager.Draw(GraphicsDevice);
            drawHelp();
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            base.Draw(gameTime);
        }

        private void drawHelp()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, (BlendState)null, (SamplerState)null, (DepthStencilState)null, (RasterizerState)null, (Effect)null, new Matrix?());
            int num1;
            if (showInformation)
            {
                int num2 = 0;
                string str1;
                switch (fountainType)
                {
                    case 0:
                        str1 = "Basic";
                        break;
                    case 1:
                        str1 = "Medium";
                        break;
                    case 2:
                        str1 = "Advanced";
                        break;
                    default:
                        str1 = "Unknown";
                        break;
                }
                string str2 = "";
                switch (emitterShape)
                {
                    case 0:
                        str2 = "Single Particle";
                        break;
                    case 1:
                        str2 = "Sine Curve";
                        break;
                    case 2:
                        str2 = "Circle";
                        break;
                    case 3:
                        str2 = "Square";
                        break;
                    default:
                        str1 = "Unknown";
                        break;
                }
                string text1 = "Fountain " + str1;
                Vector2 unitX1 = Vector2.UnitX;
                Vector2 unitY1 = Vector2.UnitY;
                int num3 = num2;
                int num4 = num3 + 1;
                double num5 = (double)(12 + 18 * num3);
                Vector2 vector2_1 = unitY1 * (float)num5;
                Vector2 position1 = unitX1 + vector2_1;
                spriteBatch.DrawString(font, text1, position1, Color.White);
                string text2 = "Emitter Shape: " + str2;
                Vector2 unitX2 = Vector2.UnitX;
                Vector2 unitY2 = Vector2.UnitY;
                int num6 = num4;
                int num7 = num6 + 1;
                double num8 = (double)(12 + 18 * num6);
                Vector2 vector2_2 = unitY2 * (float)num8;
                Vector2 position2 = unitX2 + vector2_2;
                spriteBatch.DrawString(font, text2, position2, Color.White);
                string text3 = "Camera Position: " + (object)cameraPosition;
                Vector2 unitX3 = Vector2.UnitX;
                Vector2 unitY3 = Vector2.UnitY;
                int num9 = num7;
                int num10 = num9 + 1;
                double num11 = (double)(12 + 18 * num9);
                Vector2 vector2_3 = unitY3 * (float)num11;
                Vector2 position3 = unitX3 + vector2_3;
                spriteBatch.DrawString(font, text3, position3, Color.White);
                string text4 = "Camera Horizontal Angle: " + (object)angle;
                Vector2 unitX4 = Vector2.UnitX;
                Vector2 unitY4 = Vector2.UnitY;
                int num12 = num10;
                int num13 = num12 + 1;
                double num14 = (double)(12 + 18 * num12);
                Vector2 vector2_4 = unitY4 * (float)num14;
                Vector2 position4 = unitX4 + vector2_4;
                spriteBatch.DrawString(font, text4, position4, Color.White);
                string text5 = "Camera Verticle Angle: " + (object)angle2;
                Vector2 unitX5 = Vector2.UnitX;
                Vector2 unitY5 = Vector2.UnitY;
                int num15 = num13;
                int num16 = num15 + 1;
                double num17 = (double)(12 + 18 * num15);
                Vector2 vector2_5 = unitY5 * (float)num17;
                Vector2 position5 = unitX5 + vector2_5;
                spriteBatch.DrawString(font, text5, position5, Color.White);
                string text6 = "Light Position: " + (object)lightPosition;
                Vector2 unitX6 = Vector2.UnitX;
                Vector2 unitY6 = Vector2.UnitY;
                int num18 = num16;
                int num19 = num18 + 1;
                double num20 = (double)(12 + 18 * num18);
                Vector2 vector2_6 = unitY6 * (float)num20;
                Vector2 position6 = unitX6 + vector2_6;
                spriteBatch.DrawString(font, text6, position6, Color.White);
                string text7 = "Light Horizontal Angle: " + (object)angleL;
                Vector2 unitX7 = Vector2.UnitX;
                Vector2 unitY7 = Vector2.UnitY;
                int num21 = num19;
                int num22 = num21 + 1;
                double num23 = (double)(12 + 18 * num21);
                Vector2 vector2_7 = unitY7 * (float)num23;
                Vector2 position7 = unitX7 + vector2_7;
                spriteBatch.DrawString(font, text7, position7, Color.White);
                string text8 = "Light Verticle Angle: " + (object)angleL2;
                Vector2 unitX8 = Vector2.UnitX;
                Vector2 unitY8 = Vector2.UnitY;
                int num24 = num22;
                int num25 = num24 + 1;
                double num26 = (double)(12 + 18 * num24);
                Vector2 vector2_8 = unitY8 * (float)num26;
                Vector2 position8 = unitX8 + vector2_8;
                spriteBatch.DrawString(font, text8, position8, Color.White);
                string text9 = "Friction: " + (object)friction;
                Vector2 unitX9 = Vector2.UnitX;
                Vector2 unitY9 = Vector2.UnitY;
                int num27 = num25;
                int num28 = num27 + 1;
                double num29 = (double)(12 + 18 * num27);
                Vector2 vector2_9 = unitY9 * (float)num29;
                Vector2 position9 = unitX9 + vector2_9;
                spriteBatch.DrawString(font, text9, position9, Color.White);
                string text10 = "Bounciness: " + (object)bounciness;
                Vector2 unitX10 = Vector2.UnitX;
                Vector2 unitY10 = Vector2.UnitY;
                int num30 = num28;
                int num31 = num30 + 1;
                double num32 = (double)(12 + 18 * num30);
                Vector2 vector2_10 = unitY10 * (float)num32;
                Vector2 position10 = unitX10 + vector2_10;
                spriteBatch.DrawString(font, text10, position10, Color.White);
                string text11 = "Gravity: " + (object)gravity;
                Vector2 unitX11 = Vector2.UnitX;
                Vector2 unitY11 = Vector2.UnitY;
                int num33 = num31;
                int num34 = num33 + 1;
                double num35 = (double)(12 + 18 * num33);
                Vector2 vector2_11 = unitY11 * (float)num35;
                Vector2 position11 = unitX11 + vector2_11;
                spriteBatch.DrawString(font, text11, position11, Color.White);
                string text12 = "Randomness: " + (object)randomness;
                Vector2 unitX12 = Vector2.UnitX;
                Vector2 unitY12 = Vector2.UnitY;
                int num36 = num34;
                int num37 = num36 + 1;
                double num38 = (double)(12 + 18 * num36);
                Vector2 vector2_12 = unitY12 * (float)num38;
                Vector2 position12 = unitX12 + vector2_12;
                spriteBatch.DrawString(font, text12, position12, Color.White);
                string text13 = "Wind: " + (object)wind;
                Vector2 unitX13 = Vector2.UnitX;
                Vector2 unitY13 = Vector2.UnitY;
                int num39 = num37;
                int num40 = num39 + 1;
                double num41 = (double)(12 + 18 * num39);
                Vector2 vector2_13 = unitY13 * (float)num41;
                Vector2 position13 = unitX13 + vector2_13;
                spriteBatch.DrawString(font, text13, position13, Color.White);
                string text14 = "Using Wind: " + (object)this.usingWind;
                Vector2 unitX14 = Vector2.UnitX;
                Vector2 unitY14 = Vector2.UnitY;
                int num42 = num40;
                int num43 = num42 + 1;
                double num44 = (double)(12 + 18 * num42);
                Vector2 vector2_14 = unitY14 * (float)num44;
                Vector2 position14 = unitX14 + vector2_14;
                spriteBatch.DrawString(font, text14, position14, Color.White);
                string text15 = "Using Randomness: " + (object)usingRandomness;
                Vector2 unitX15 = Vector2.UnitX;
                Vector2 unitY15 = Vector2.UnitY;
                int num45 = num43;
                int num46 = num45 + 1;
                double num47 = (double)(12 + 18 * num45);
                Vector2 vector2_15 = unitY15 * (float)num47;
                Vector2 position15 = unitX15 + vector2_15;
                spriteBatch.DrawString(font, text15, position15, Color.White);
                string text16 = "User Override Velocity: " + (object)velocityOverride;
                Vector2 unitX16 = Vector2.UnitX;
                Vector2 unitY16 = Vector2.UnitY;
                int num48 = num46;
                int num49 = num48 + 1;
                double num50 = (double)(12 + 18 * num48);
                Vector2 vector2_16 = unitY16 * (float)num50;
                Vector2 position16 = unitX16 + vector2_16;
                spriteBatch.DrawString(font, text16, position16, Color.White);
                string text17 = "Using User Override Velocity: " + (object)usingUserVelocity;
                Vector2 unitX17 = Vector2.UnitX;
                Vector2 unitY17 = Vector2.UnitY;
                int num51 = num49;
                int num52 = num51 + 1;
                double num53 = (double)(12 + 18 * num51);
                Vector2 vector2_17 = unitY17 * (float)num53;
                Vector2 position17 = unitX17 + vector2_17;
                spriteBatch.DrawString(font, text17, position17, Color.White);
                string text18 = "LifeSpan: " + (object)this.lifeSpan;
                Vector2 unitX18 = Vector2.UnitX;
                Vector2 unitY18 = Vector2.UnitY;
                int num54 = num52;
                int num55 = num54 + 1;
                double num56 = (double)(12 + 18 * num54);
                Vector2 vector2_18 = unitY18 * (float)num56;
                Vector2 position18 = unitX18 + vector2_18;
                spriteBatch.DrawString(font, text18, position18, Color.White);
                string text19 = "Age: " + (object)this.age;
                Vector2 unitX19 = Vector2.UnitX;
                Vector2 unitY19 = Vector2.UnitY;
                int num57 = num55;
                int num58 = num57 + 1;
                double num59 = (double)(12 + 18 * num57);
                Vector2 vector2_19 = unitY19 * (float)num59;
                Vector2 position19 = unitX19 + vector2_19;
                spriteBatch.DrawString(font, text19, position19, Color.White);
                string text20 = "Permanent Age: " + (object)this.usingAge;
                Vector2 unitX20 = Vector2.UnitX;
                Vector2 unitY20 = Vector2.UnitY;
                int num60 = num58;
                num1 = num60 + 1;
                double num61 = (double)(12 + 18 * num60);
                Vector2 vector2_20 = unitY20 * (float)num61;
                Vector2 position20 = unitX20 + vector2_20;
                spriteBatch.DrawString(font, text20, position20, Color.White);
            }
            if (this.showHelp)
            {
                int num2 = 0;
                Vector2 vector2_1 = Vector2.UnitX * 600f;
                Vector2 unitY1 = Vector2.UnitY;
                int num3 = num2;
                int num4 = num3 + 1;
                double num5 = (double)(12 + 18 * num3);
                Vector2 vector2_2 = unitY1 * (float)num5;
                Vector2 position1 = vector2_1 + vector2_2;
                Color white1 = Color.White;
                spriteBatch.DrawString(font, "Mouse Left: Rotate camera", position1, white1);
                Vector2 vector2_3 = Vector2.UnitX * 600f;
                Vector2 unitY2 = Vector2.UnitY;
                int num6 = num4;
                int num7 = num6 + 1;
                double num8 = (double)(12 + 18 * num6);
                Vector2 vector2_4 = unitY2 * (float)num8;
                Vector2 position2 = vector2_3 + vector2_4;
                Color white2 = Color.White;
                spriteBatch.DrawString(font, "Mouse Right: Change distance", position2, white2);
                Vector2 vector2_5 = Vector2.UnitX * 600f;
                Vector2 unitY3 = Vector2.UnitY;
                int num9 = num7;
                int num10 = num9 + 1;
                double num11 = (double)(12 + 18 * num9);
                Vector2 vector2_6 = unitY3 * (float)num11;
                Vector2 position3 = vector2_5 + vector2_6;
                Color white3 = Color.White;
                spriteBatch.DrawString(font, "Mouse Middle: Translate", position3, white3);
                Vector2 vector2_7 = Vector2.UnitX * 600f;
                Vector2 unitY4 = Vector2.UnitY;
                int num12 = num10;
                int num13 = num12 + 1;
                double num14 = (double)(12 + 18 * num12);
                Vector2 vector2_8 = unitY4 * (float)num14;
                Vector2 position4 = vector2_7 + vector2_8;
                Color white4 = Color.White;
                spriteBatch.DrawString(font, "Arrows: Rotate light", position4, white4);
                Vector2 vector2_9 = Vector2.UnitX * 600f;
                Vector2 unitY5 = Vector2.UnitY;
                int num15 = num13;
                int num16 = num15 + 1;
                double num17 = (double)(12 + 18 * num15);
                Vector2 vector2_10 = unitY5 * (float)num17;
                Vector2 position5 = vector2_9 + vector2_10;
                Color white5 = Color.White;
                spriteBatch.DrawString(font, "S: Reset camera/light", position5, white5);
                Vector2 vector2_11 = Vector2.UnitX * 600f;
                Vector2 unitY6 = Vector2.UnitY;
                int num18 = num16;
                int num19 = num18 + 1;
                double num20 = (double)(12 + 18 * num18);
                Vector2 vector2_12 = unitY6 * (float)num20;
                Vector2 position6 = vector2_11 + vector2_12;
                Color white6 = Color.White;
                spriteBatch.DrawString(font, "?: Toggle help", position6, white6);
                Vector2 vector2_13 = Vector2.UnitX * 600f;
                Vector2 unitY7 = Vector2.UnitY;
                int num21 = num19;
                int num22 = num21 + 1;
                double num23 = (double)(12 + 18 * num21);
                Vector2 vector2_14 = unitY7 * (float)num23;
                Vector2 position7 = vector2_13 + vector2_14;
                Color white7 = Color.White;
                spriteBatch.DrawString(font, "H: Toggle information", position7, white7);
                Vector2 vector2_15 = Vector2.UnitX * 600f;
                Vector2 unitY8 = Vector2.UnitY;
                int num24 = num22;
                int num25 = num24 + 1;
                double num26 = (double)(12 + 18 * num24);
                Vector2 vector2_16 = unitY8 * (float)num26;
                Vector2 position8 = vector2_15 + vector2_16;
                Color white8 = Color.White;
                spriteBatch.DrawString(font, "F/f: Change friction", position8, white8);
                Vector2 vector2_17 = Vector2.UnitX * 600f;
                Vector2 unitY9 = Vector2.UnitY;
                int num27 = num25;
                int num28 = num27 + 1;
                double num29 = (double)(12 + 18 * num27);
                Vector2 vector2_18 = unitY9 * (float)num29;
                Vector2 position9 = vector2_17 + vector2_18;
                Color white9 = Color.White;
                spriteBatch.DrawString(font, "B/b: Change bounciness", position9, white9);
                Vector2 vector2_19 = Vector2.UnitX * 600f;
                Vector2 unitY10 = Vector2.UnitY;
                int num30 = num28;
                int num31 = num30 + 1;
                double num32 = (double)(12 + 18 * num30);
                Vector2 vector2_20 = unitY10 * (float)num32;
                Vector2 position10 = vector2_19 + vector2_20;
                Color white10 = Color.White;
                spriteBatch.DrawString(font, "G/g: Change gravity", position10, white10);
                Vector2 vector2_21 = Vector2.UnitX * 600f;
                Vector2 unitY11 = Vector2.UnitY;
                int num33 = num31;
                int num34 = num33 + 1;
                double num35 = (double)(12 + 18 * num33);
                Vector2 vector2_22 = unitY11 * (float)num35;
                Vector2 position11 = vector2_21 + vector2_22;
                Color white11 = Color.White;
                spriteBatch.DrawString(font, "R/r: Change randomness", position11, white11);
                Vector2 vector2_23 = Vector2.UnitX * 600f;
                Vector2 unitY12 = Vector2.UnitY;
                int num36 = num34;
                int num37 = num36 + 1;
                double num38 = (double)(12 + 18 * num36);
                Vector2 vector2_24 = unitY12 * (float)num38;
                Vector2 position12 = vector2_23 + vector2_24;
                Color white12 = Color.White;
                spriteBatch.DrawString(font, "W: Toggle wind", position12, white12);
                Vector2 vector2_25 = Vector2.UnitX * 600f;
                Vector2 unitY13 = Vector2.UnitY;
                int num39 = num37;
                int num40 = num39 + 1;
                double num41 = (double)(12 + 18 * num39);
                Vector2 vector2_26 = unitY13 * (float)num41;
                Vector2 position13 = vector2_25 + vector2_26;
                Color white13 = Color.White;
                spriteBatch.DrawString(font, "X/x, Y/y, Z/z: Wind direction", position13, white13);
                Vector2 vector2_27 = Vector2.UnitX * 600f;
                Vector2 unitY14 = Vector2.UnitY;
                int num42 = num40;
                int num43 = num42 + 1;
                double num44 = (double)(12 + 18 * num42);
                Vector2 vector2_28 = unitY14 * (float)num44;
                Vector2 position14 = vector2_27 + vector2_28;
                Color white14 = Color.White;
                spriteBatch.DrawString(font, "T: Toggle randomness", position14, white14);
                Vector2 vector2_29 = Vector2.UnitX * 600f;
                Vector2 unitY15 = Vector2.UnitY;
                int num45 = num43;
                int num46 = num45 + 1;
                double num47 = (double)(12 + 18 * num45);
                Vector2 vector2_30 = unitY15 * (float)num47;
                Vector2 position15 = vector2_29 + vector2_30;
                Color white15 = Color.White;
                spriteBatch.DrawString(font, "D: Toggle basic founctain direction", position15, white15);
                Vector2 vector2_31 = Vector2.UnitX * 600f;
                Vector2 unitY16 = Vector2.UnitY;
                int num48 = num46;
                int num49 = num48 + 1;
                double num50 = (double)(12 + 18 * num48);
                Vector2 vector2_32 = unitY16 * (float)num50;
                Vector2 position16 = vector2_31 + vector2_32;
                Color white16 = Color.White;
                spriteBatch.DrawString(font, "V: Toggle user override velocity", position16, white16);
                Vector2 vector2_33 = Vector2.UnitX * 600f;
                Vector2 unitY17 = Vector2.UnitY;
                int num51 = num49;
                int num52 = num51 + 1;
                double num53 = (double)(12 + 18 * num51);
                Vector2 vector2_34 = unitY17 * (float)num53;
                Vector2 position17 = vector2_33 + vector2_34;
                Color white17 = Color.White;
                spriteBatch.DrawString(font, "U/u, I/i, O/o: Change user override velocity", position17, white17);
                Vector2 vector2_35 = Vector2.UnitX * 600f;
                Vector2 unitY18 = Vector2.UnitY;
                int num54 = num52;
                int num55 = num54 + 1;
                double num56 = (double)(12 + 18 * num54);
                Vector2 vector2_36 = unitY18 * (float)num56;
                Vector2 position18 = vector2_35 + vector2_36;
                Color white18 = Color.White;
                spriteBatch.DrawString(font, "L/l: LifeSpan", position18, white18);;
                Vector2 vector2_37 = Vector2.UnitX * 600f;
                Vector2 unitY19 = Vector2.UnitY;
                int num57 = num55;
                int num58 = num57 + 1;
                double num59 = (double)(12 + 18 * num57);
                Vector2 vector2_38 = unitY19 * (float)num59;
                Vector2 position19 = vector2_37 + vector2_38;
                Color white19 = Color.White;
                spriteBatch.DrawString(font, "A/a: Age", position19, white19);
                Vector2 vector2_39 = Vector2.UnitX * 600f;
                Vector2 unitY20 = Vector2.UnitY;
                int num60 = num58;
                num1 = num60 + 1;
                double num61 = (double)(12 + 18 * num60);
                Vector2 vector2_40 = unitY20 * (float)num61;
                Vector2 position20 = vector2_39 + vector2_40;
                Color white20 = Color.White;
                spriteBatch.DrawString(font, "Q: Toggle Permanent Age", position20, white20);
            }
            this.spriteBatch.End();
        }

    }
}
