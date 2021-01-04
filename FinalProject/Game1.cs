using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinalProject
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Model meshObject;
        Matrix projection, view;
        Effect effect; // perlinNoiseEffect;
        Effect effect1;
        float height;
        Texture2D permTexture2d;
        Texture2D permGradTexture;
        double timer = 0;
        float E;
        float noise;
        SpriteFont font;
        float angle = 0;
        float angle2 = 0;
        float distance = 0f;
        Vector3 lightPosition = new Vector3(1f, 1, 1);
        float lightAngle = 0;
        float lightAngle2 = 0;

        MouseState PreviousMouseState;

        PerlinNoise noiseEngine = new PerlinNoise();

        public Game1()
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
            float aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
            projection =
                Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f),
                aspectRatio, 1.0f, 10000.0f);


            noiseEngine.InitNoiseFunctions(1337, graphics.GraphicsDevice);


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

            meshObject = Content.Load<Model>("Sphere");
            effect = Content.Load<Effect>("PerlinNoiseEffect");
           // effect1 = Content.Load<Effect>("test_inoise");
            font = Content.Load<SpriteFont>("font");

            foreach (ModelMesh mesh in meshObject.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                }
            }


            permTexture2d = noiseEngine.GeneratePermTexture2d();
            permGradTexture = noiseEngine.GeneratePermGradTexture();

            view = Matrix.CreateLookAt(
               new Vector3(0, 0, 100),
                Vector3.Zero,
               Vector3.Up);
           // projection = Matrix.CreatePerspectiveFieldOfView(
            //    MathHelper.ToRadians(90),
            //    GraphicsDevice.Viewport.AspectRatio,
              // 0.1f,
              //  100);

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

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton == ButtonState.Pressed)
            {

                angle += 0.01f * (Mouse.GetState().X - PreviousMouseState.X);
                angle2 += 0.01f * (Mouse.GetState().Y - PreviousMouseState.Y);
                Vector3 camera = Vector3.Transform(
                new Vector3(0, 0, 100), Matrix.CreateTranslation(0, 0, distance) * Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                view = Matrix.CreateLookAt(camera, Vector3.Zero, Vector3.UnitY);
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed && PreviousMouseState.RightButton == ButtonState.Pressed)
            {
                distance += 0.1f * (Mouse.GetState().Y - PreviousMouseState.Y);
                Vector3 camera = Vector3.Transform(
                    new Vector3(0, 0, 100), Matrix.CreateTranslation(0, 0, distance) * Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                view = Matrix.CreateLookAt(camera, Vector3.Zero, Vector3.UnitY);

            }
            //  Vector3 camera = Vector3.Transform(
            //     new Vector3(0, 0, 100), Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
            //  view = Matrix.CreateLookAt(camera, Vector3.Zero, Vector3.UnitY);
            // perlinNoiseEffect.Parameters["View"].SetValue(view);

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


            if (Keyboard.GetState().IsKeyDown(Keys.U))
            {
               // height += 10f;
               // E += 0.5f;
                noise += 0.1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.V))
            {
               // height -= 10f;
               // E -= 0.5f;
                noise -= 0.1f;
            }
            //timer += gameTime.ElapsedGameTime.Milliseconds / 5000.0;
            // view = Matrix.CreateLookAt(new Vector3(100.0f * (float)Math.Sin(timer), 0.0f, 100.0f * (float)Math.Cos(timer)),
            //  Vector3.Zero, Vector3.Up);


            effect.Parameters["View"].SetValue(view);
            //effect.Parameters["inz"].SetValue(height);
          //  effect.Parameters["E"].SetValue(E);
            effect.Parameters["height"].SetValue(noise);
            effect.Parameters["LightPosition"].SetValue(lightPosition);

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

            DrawModel(meshObject, projection, view);
           // DrawModel1(meshObject, projection, view);

            spriteBatch.Begin(SpriteSortMode.Deferred, new BlendState());
            spriteBatch.DrawString(font, "light angle: " + lightAngle + "\nlight angle2: " + lightAngle2 +
                "\ncamera angle: " + angle + "\ncamera angle2: " + angle2 + "\ndistance: " + distance + 
                "\nRight, Left, Up and Down Keys \nto change light direction." + "\nRight mouse key to move. " +
                "\n left mouse key to rotate", Vector2.UnitX + Vector2.UnitY * 12, Color.Yellow);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawModel(Model m, Matrix projection, Matrix view)
        {
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);


            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.CurrentTechnique = effect.Techniques["PerlinNoise"];
                    effect.Parameters["permTexture2d"].SetValue(permTexture2d);
                    effect.Parameters["permGradTexture"].SetValue(permGradTexture);
                    effect.Parameters["World"].SetValue(transforms[mesh.ParentBone.Index] * Matrix.CreateScale(new Vector3(0.7f, 0.7f, 0.7f)));
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                    effect.Parameters["LightPosition"].SetValue(lightPosition);
                }
                mesh.Draw();
            }
        }

    }
}
