using System;
using System.Collections.Generic;
using System.Windows;
using Kinect.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Kinect.XNA
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        private readonly List<User.User> _users = new List<User.User>();
        private readonly GraphicsDeviceManager graphics;
        private MyKinect _kinect;
        private SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            if (_kinect != null)
            {
                _kinect.StopKinect();
            }
            base.OnExiting(sender, args);
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
            _kinect = MyKinect.Instance;
            _kinect.StartKinect();
            _kinect.UserCreated += _kinect_UserCreated;
            _kinect.UserRemoved += _kinect_UserRemoved;
            base.Initialize();
        }

        private void _kinect_UserRemoved(object sender, KinectUserEventArgs e)
        {
            //Delete user
        }

        private void _kinect_UserCreated(object sender, KinectUserEventArgs e)
        {
            Core.User kinect_user = _kinect.GetUser(e.User.Id);
            var xnaUser = new User.User(kinect_user, Content,
                                        new Size(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));
            _users.Add(xnaUser);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (User.User user in _users)
            {
                user.Draw(spriteBatch);
            }

            base.Draw(gameTime);
        }
    }
}