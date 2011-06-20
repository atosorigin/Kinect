using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Common;
using System.Windows;
using log4net;

namespace Kinect.XNA.User
{
    public class User
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(User));
        private Kinect.Core.User _user;
        private Dictionary<xn.SkeletonJoint, Bodypart> _bodyparts = new Dictionary<xn.SkeletonJoint, Bodypart>();
        private ContentManager _content;
        private Size WindowSize;

        public User(Kinect.Core.User user, ContentManager content, Size windowSize)
        {
            // TODO: Complete member initialization
            _user = user;
            this._content = content;
            this.WindowSize = windowSize;
            AddBodyParts();
            _user.Updated += Kinect_User_Updated;
        }
        
        private void AddBodyParts()
        {
            _bodyparts.Add(xn.SkeletonJoint.Torso,new Bodypart(_content.Load<Texture2D>("Images/Torso")));
            _bodyparts.Add(xn.SkeletonJoint.LeftShoulder, new Bodypart(_content.Load<Texture2D>("Images/LeftArm")));
            _bodyparts.Add(xn.SkeletonJoint.RightShoulder, new Bodypart(_content.Load<Texture2D>("Images/RightArm")));
            _bodyparts.Add(xn.SkeletonJoint.LeftHip, new Bodypart(_content.Load<Texture2D>("Images/LeftLeg")));
            _bodyparts.Add(xn.SkeletonJoint.RightHip, new Bodypart(_content.Load<Texture2D>("Images/RightLeg")));
        }

        private void Kinect_User_Updated(object sender, Core.Eventing.ProcessEventArgs<Core.IUserChangedEvent> e)
        {
            _bodyparts[xn.SkeletonJoint.Torso].LeftPoint = e.Event.Torso.ToScreenPosition(new System.Windows.Size(640, 480), WindowSize);
            _log.DebugFormat("{0}:{1}", xn.SkeletonJoint.LeftElbow, _bodyparts[xn.SkeletonJoint.LeftShoulder].LeftPoint);
            _log.DebugFormat("{0}:{1}", xn.SkeletonJoint.LeftShoulder, _bodyparts[xn.SkeletonJoint.LeftShoulder].RightPoint);
            //_bodyparts[xn.SkeletonJoint.RightShoulder].LeftTop = e.Event.RightShoulder.ToScreenPosition(new System.Windows.Size(640, 480), WindowSize); 
            //_bodyparts[xn.SkeletonJoint.RightShoulder].RightBottom = e.Event.RightElbow.ToScreenPosition(new System.Windows.Size(640, 480), WindowSize); 
            //_bodyparts[xn.SkeletonJoint.LeftHip].LeftTop = e.Event.LeftHip.ToScreenPosition(new System.Windows.Size(640, 480), WindowSize); ;
            //_bodyparts[xn.SkeletonJoint.LeftHip].RightBottom = e.Event.LeftKnee.ToScreenPosition(new System.Windows.Size(640, 480), WindowSize);
            //_bodyparts[xn.SkeletonJoint.RightHip].LeftTop = e.Event.RightHip.ToScreenPosition(new System.Windows.Size(640, 480), WindowSize);
            //_bodyparts[xn.SkeletonJoint.RightHip].RightBottom = e.Event.RightKnee.ToScreenPosition(new System.Windows.Size(640, 480), WindowSize);
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            foreach (var bodypart in _bodyparts)
            {
                bodypart.Value.Draw(spriteBatch);
            }
        }
    }
}
