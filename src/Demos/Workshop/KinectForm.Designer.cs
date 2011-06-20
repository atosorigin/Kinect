namespace Kinect.Workshop.Winforms
{
    partial class KinectForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnStart = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.lblHead = new System.Windows.Forms.Label();
            this.lblNeck = new System.Windows.Forms.Label();
            this.lblLeftShoulder = new System.Windows.Forms.Label();
            this.lblRightShoulder = new System.Windows.Forms.Label();
            this.lblLeftElbow = new System.Windows.Forms.Label();
            this.lblRightElbow = new System.Windows.Forms.Label();
            this.lblRightHand = new System.Windows.Forms.Label();
            this.lblLeftHand = new System.Windows.Forms.Label();
            this.lblTorso = new System.Windows.Forms.Label();
            this.lblLeftHip = new System.Windows.Forms.Label();
            this.lblRightHip = new System.Windows.Forms.Label();
            this.lblRightKnee = new System.Windows.Forms.Label();
            this.lblLeftKnee = new System.Windows.Forms.Label();
            this.lblLeftFoot = new System.Windows.Forms.Label();
            this.lblRightFoot = new System.Windows.Forms.Label();
            this.lbCameraMessages = new System.Windows.Forms.ListBox();
            this.lbMessages = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(12, 12);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start Kinect";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.BtnStartClick);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = global::Kinect.Workshop.Winforms.Properties.Resources.skelet;
            this.pictureBox1.Location = new System.Drawing.Point(263, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(407, 639);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(93, 12);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop Kinect";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.BtnStopClick);
            // 
            // lblHead
            // 
            this.lblHead.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblHead.AutoSize = true;
            this.lblHead.Location = new System.Drawing.Point(425, 31);
            this.lblHead.Name = "lblHead";
            this.lblHead.Size = new System.Drawing.Size(33, 13);
            this.lblHead.TabIndex = 5;
            this.lblHead.Text = "Head";
            this.lblHead.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblNeck
            // 
            this.lblNeck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNeck.AutoSize = true;
            this.lblNeck.Location = new System.Drawing.Point(425, 164);
            this.lblNeck.Name = "lblNeck";
            this.lblNeck.Size = new System.Drawing.Size(33, 13);
            this.lblNeck.TabIndex = 6;
            this.lblNeck.Text = "Neck";
            // 
            // lblLeftShoulder
            // 
            this.lblLeftShoulder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLeftShoulder.AutoSize = true;
            this.lblLeftShoulder.Location = new System.Drawing.Point(335, 188);
            this.lblLeftShoulder.Name = "lblLeftShoulder";
            this.lblLeftShoulder.Size = new System.Drawing.Size(68, 13);
            this.lblLeftShoulder.TabIndex = 7;
            this.lblLeftShoulder.Text = "Left shoulder";
            // 
            // lblRightShoulder
            // 
            this.lblRightShoulder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRightShoulder.AutoSize = true;
            this.lblRightShoulder.Location = new System.Drawing.Point(487, 188);
            this.lblRightShoulder.Name = "lblRightShoulder";
            this.lblRightShoulder.Size = new System.Drawing.Size(75, 13);
            this.lblRightShoulder.TabIndex = 8;
            this.lblRightShoulder.Text = "Right shoulder";
            // 
            // lblLeftElbow
            // 
            this.lblLeftElbow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLeftElbow.AutoSize = true;
            this.lblLeftElbow.Location = new System.Drawing.Point(269, 266);
            this.lblLeftElbow.Name = "lblLeftElbow";
            this.lblLeftElbow.Size = new System.Drawing.Size(56, 13);
            this.lblLeftElbow.TabIndex = 9;
            this.lblLeftElbow.Text = "Left elbow";
            // 
            // lblRightElbow
            // 
            this.lblRightElbow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRightElbow.AutoSize = true;
            this.lblRightElbow.Location = new System.Drawing.Point(537, 266);
            this.lblRightElbow.Name = "lblRightElbow";
            this.lblRightElbow.Size = new System.Drawing.Size(63, 13);
            this.lblRightElbow.TabIndex = 10;
            this.lblRightElbow.Text = "Right elbow";
            // 
            // lblRightHand
            // 
            this.lblRightHand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRightHand.AutoSize = true;
            this.lblRightHand.Location = new System.Drawing.Point(576, 383);
            this.lblRightHand.Name = "lblRightHand";
            this.lblRightHand.Size = new System.Drawing.Size(59, 13);
            this.lblRightHand.TabIndex = 11;
            this.lblRightHand.Text = "Right hand";
            // 
            // lblLeftHand
            // 
            this.lblLeftHand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLeftHand.AutoSize = true;
            this.lblLeftHand.Location = new System.Drawing.Point(233, 383);
            this.lblLeftHand.Name = "lblLeftHand";
            this.lblLeftHand.Size = new System.Drawing.Size(52, 13);
            this.lblLeftHand.TabIndex = 12;
            this.lblLeftHand.Text = "Left hand";
            // 
            // lblTorso
            // 
            this.lblTorso.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTorso.AutoSize = true;
            this.lblTorso.Location = new System.Drawing.Point(425, 266);
            this.lblTorso.Name = "lblTorso";
            this.lblTorso.Size = new System.Drawing.Size(34, 13);
            this.lblTorso.TabIndex = 13;
            this.lblTorso.Text = "Torso";
            // 
            // lblLeftHip
            // 
            this.lblLeftHip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLeftHip.AutoSize = true;
            this.lblLeftHip.Location = new System.Drawing.Point(383, 383);
            this.lblLeftHip.Name = "lblLeftHip";
            this.lblLeftHip.Size = new System.Drawing.Size(42, 13);
            this.lblLeftHip.TabIndex = 14;
            this.lblLeftHip.Text = "Left hip";
            // 
            // lblRightHip
            // 
            this.lblRightHip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRightHip.AutoSize = true;
            this.lblRightHip.Location = new System.Drawing.Point(456, 383);
            this.lblRightHip.Name = "lblRightHip";
            this.lblRightHip.Size = new System.Drawing.Size(49, 13);
            this.lblRightHip.TabIndex = 15;
            this.lblRightHip.Text = "Right hip";
            // 
            // lblRightKnee
            // 
            this.lblRightKnee.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRightKnee.AutoSize = true;
            this.lblRightKnee.Location = new System.Drawing.Point(452, 514);
            this.lblRightKnee.Name = "lblRightKnee";
            this.lblRightKnee.Size = new System.Drawing.Size(59, 13);
            this.lblRightKnee.TabIndex = 16;
            this.lblRightKnee.Text = "Right knee";
            // 
            // lblLeftKnee
            // 
            this.lblLeftKnee.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLeftKnee.AutoSize = true;
            this.lblLeftKnee.Location = new System.Drawing.Point(373, 514);
            this.lblLeftKnee.Name = "lblLeftKnee";
            this.lblLeftKnee.Size = new System.Drawing.Size(52, 13);
            this.lblLeftKnee.TabIndex = 17;
            this.lblLeftKnee.Text = "Left knee";
            // 
            // lblLeftFoot
            // 
            this.lblLeftFoot.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLeftFoot.AutoSize = true;
            this.lblLeftFoot.Location = new System.Drawing.Point(357, 612);
            this.lblLeftFoot.Name = "lblLeftFoot";
            this.lblLeftFoot.Size = new System.Drawing.Size(46, 13);
            this.lblLeftFoot.TabIndex = 18;
            this.lblLeftFoot.Text = "Left foot";
            // 
            // lblRightFoot
            // 
            this.lblRightFoot.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRightFoot.AutoSize = true;
            this.lblRightFoot.Location = new System.Drawing.Point(452, 612);
            this.lblRightFoot.Name = "lblRightFoot";
            this.lblRightFoot.Size = new System.Drawing.Size(53, 13);
            this.lblRightFoot.TabIndex = 19;
            this.lblRightFoot.Text = "Right foot";
            // 
            // lbCameraMessages
            // 
            this.lbCameraMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbCameraMessages.FormattingEnabled = true;
            this.lbCameraMessages.Location = new System.Drawing.Point(13, 325);
            this.lbCameraMessages.Name = "lbCameraMessages";
            this.lbCameraMessages.Size = new System.Drawing.Size(244, 329);
            this.lbCameraMessages.TabIndex = 20;
            // 
            // lbMessages
            // 
            this.lbMessages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbMessages.FormattingEnabled = true;
            this.lbMessages.Location = new System.Drawing.Point(12, 42);
            this.lbMessages.Name = "lbMessages";
            this.lbMessages.Size = new System.Drawing.Size(244, 277);
            this.lbMessages.TabIndex = 21;
            // 
            // KinectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 663);
            this.Controls.Add(this.lbMessages);
            this.Controls.Add(this.lbCameraMessages);
            this.Controls.Add(this.lblRightFoot);
            this.Controls.Add(this.lblLeftFoot);
            this.Controls.Add(this.lblLeftKnee);
            this.Controls.Add(this.lblRightKnee);
            this.Controls.Add(this.lblRightHip);
            this.Controls.Add(this.lblLeftHip);
            this.Controls.Add(this.lblTorso);
            this.Controls.Add(this.lblLeftHand);
            this.Controls.Add(this.lblRightHand);
            this.Controls.Add(this.lblRightElbow);
            this.Controls.Add(this.lblLeftElbow);
            this.Controls.Add(this.lblRightShoulder);
            this.Controls.Add(this.lblLeftShoulder);
            this.Controls.Add(this.lblNeck);
            this.Controls.Add(this.lblHead);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnStart);
            this.Name = "KinectForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Kinect Workshop";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.KinectForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label lblHead;
        private System.Windows.Forms.Label lblNeck;
        private System.Windows.Forms.Label lblLeftShoulder;
        private System.Windows.Forms.Label lblRightShoulder;
        private System.Windows.Forms.Label lblLeftElbow;
        private System.Windows.Forms.Label lblRightElbow;
        private System.Windows.Forms.Label lblRightHand;
        private System.Windows.Forms.Label lblLeftHand;
        private System.Windows.Forms.Label lblTorso;
        private System.Windows.Forms.Label lblLeftHip;
        private System.Windows.Forms.Label lblRightHip;
        private System.Windows.Forms.Label lblRightKnee;
        private System.Windows.Forms.Label lblLeftKnee;
        private System.Windows.Forms.Label lblLeftFoot;
        private System.Windows.Forms.Label lblRightFoot;
        private System.Windows.Forms.ListBox lbCameraMessages;
        private System.Windows.Forms.ListBox lbMessages;
    }
}

