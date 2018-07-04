namespace Collision_Simulator
{
    partial class CollisonSimulator
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
            this.SaveButton = new System.Windows.Forms.Button();
            this.LoadButton = new System.Windows.Forms.Button();
            this.CircleButton = new System.Windows.Forms.Button();
            this.SquareButton = new System.Windows.Forms.Button();
            this.speedBar = new System.Windows.Forms.TrackBar();
            this.ClearButton = new System.Windows.Forms.Button();
            this.performanceLabel = new System.Windows.Forms.Label();
            this.PauseButton = new System.Windows.Forms.Button();
            this.PlayButton = new System.Windows.Forms.Button();
            this.ReverseButton = new System.Windows.Forms.Button();
            this.PixelPerfectButton = new System.Windows.Forms.Button();
            this.BoundingBoxButton = new System.Windows.Forms.Button();
            this.BruceForceButton = new System.Windows.Forms.Button();
            this.SpatialHashingButton = new System.Windows.Forms.Button();
            this.QuadTreeButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.speedBar)).BeginInit();
            this.SuspendLayout();
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(2, 2);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(68, 26);
            this.SaveButton.TabIndex = 0;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // LoadButton
            // 
            this.LoadButton.Location = new System.Drawing.Point(76, 2);
            this.LoadButton.Name = "LoadButton";
            this.LoadButton.Size = new System.Drawing.Size(68, 26);
            this.LoadButton.TabIndex = 1;
            this.LoadButton.Text = "Load";
            this.LoadButton.UseVisualStyleBackColor = true;
            this.LoadButton.Click += new System.EventHandler(this.LoadButton_Click);
            // 
            // CircleButton
            // 
            this.CircleButton.Location = new System.Drawing.Point(150, 2);
            this.CircleButton.Name = "CircleButton";
            this.CircleButton.Size = new System.Drawing.Size(68, 26);
            this.CircleButton.TabIndex = 2;
            this.CircleButton.Text = "Circle";
            this.CircleButton.Click += new System.EventHandler(this.CircleButton_Click);
            // 
            // SquareButton
            // 
            this.SquareButton.BackColor = System.Drawing.Color.Yellow;
            this.SquareButton.ForeColor = System.Drawing.Color.Gray;
            this.SquareButton.Location = new System.Drawing.Point(224, 2);
            this.SquareButton.Name = "SquareButton";
            this.SquareButton.Size = new System.Drawing.Size(68, 26);
            this.SquareButton.TabIndex = 3;
            this.SquareButton.Text = "Square";
            this.SquareButton.UseVisualStyleBackColor = false;
            this.SquareButton.Click += new System.EventHandler(this.SquareButton_Click);
            // 
            // speedBar
            // 
            this.speedBar.LargeChange = 1;
            this.speedBar.Location = new System.Drawing.Point(298, 2);
            this.speedBar.Maximum = 5;
            this.speedBar.Minimum = -5;
            this.speedBar.Name = "speedBar";
            this.speedBar.Size = new System.Drawing.Size(210, 56);
            this.speedBar.TabIndex = 4;
            this.speedBar.TabStop = false;
            this.speedBar.Value = 5;
            this.speedBar.Scroll += new System.EventHandler(this.speedBar_Scroll);
            // 
            // ClearButton
            // 
            this.ClearButton.ForeColor = System.Drawing.Color.Black;
            this.ClearButton.Location = new System.Drawing.Point(514, 2);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(68, 26);
            this.ClearButton.TabIndex = 5;
            this.ClearButton.Text = "Clear";
            this.ClearButton.UseVisualStyleBackColor = true;
            this.ClearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // performanceLabel
            // 
            this.performanceLabel.AutoSize = true;
            this.performanceLabel.Location = new System.Drawing.Point(588, 7);
            this.performanceLabel.Name = "performanceLabel";
            this.performanceLabel.Size = new System.Drawing.Size(16, 17);
            this.performanceLabel.TabIndex = 6;
            this.performanceLabel.Text = "0";
            // 
            // PauseButton
            // 
            this.PauseButton.ForeColor = System.Drawing.Color.Black;
            this.PauseButton.Location = new System.Drawing.Point(369, 32);
            this.PauseButton.Name = "PauseButton";
            this.PauseButton.Size = new System.Drawing.Size(68, 26);
            this.PauseButton.TabIndex = 7;
            this.PauseButton.Text = "Pause";
            this.PauseButton.UseVisualStyleBackColor = true;
            this.PauseButton.Click += new System.EventHandler(this.PauseButton_Click);
            // 
            // PlayButton
            // 
            this.PlayButton.ForeColor = System.Drawing.Color.Black;
            this.PlayButton.Location = new System.Drawing.Point(443, 32);
            this.PlayButton.Name = "PlayButton";
            this.PlayButton.Size = new System.Drawing.Size(68, 26);
            this.PlayButton.TabIndex = 8;
            this.PlayButton.Text = "Play";
            this.PlayButton.UseVisualStyleBackColor = true;
            this.PlayButton.Click += new System.EventHandler(this.PlayButton_Click);
            // 
            // ReverseButton
            // 
            this.ReverseButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReverseButton.ForeColor = System.Drawing.Color.Black;
            this.ReverseButton.Location = new System.Drawing.Point(295, 32);
            this.ReverseButton.Name = "ReverseButton";
            this.ReverseButton.Size = new System.Drawing.Size(68, 26);
            this.ReverseButton.TabIndex = 9;
            this.ReverseButton.Text = "Reverse";
            this.ReverseButton.UseVisualStyleBackColor = true;
            this.ReverseButton.Click += new System.EventHandler(this.ReverseButton_Click);
            // 
            // PixelPerfectButton
            // 
            this.PixelPerfectButton.BackColor = System.Drawing.Color.Yellow;
            this.PixelPerfectButton.ForeColor = System.Drawing.Color.Gray;
            this.PixelPerfectButton.Location = new System.Drawing.Point(680, 2);
            this.PixelPerfectButton.Name = "PixelPerfectButton";
            this.PixelPerfectButton.Size = new System.Drawing.Size(99, 26);
            this.PixelPerfectButton.TabIndex = 10;
            this.PixelPerfectButton.Text = "PixelPerfect";
            this.PixelPerfectButton.UseVisualStyleBackColor = false;
            this.PixelPerfectButton.Click += new System.EventHandler(this.PixelPerfectButton_Click);
            // 
            // BoundingBoxButton
            // 
            this.BoundingBoxButton.Location = new System.Drawing.Point(785, 2);
            this.BoundingBoxButton.Name = "BoundingBoxButton";
            this.BoundingBoxButton.Size = new System.Drawing.Size(99, 26);
            this.BoundingBoxButton.TabIndex = 11;
            this.BoundingBoxButton.Text = "BoundingBox";
            this.BoundingBoxButton.Click += new System.EventHandler(this.BoundingBoxButton_Click);
            // 
            // BruceForceButton
            // 
            this.BruceForceButton.BackColor = System.Drawing.Color.Yellow;
            this.BruceForceButton.ForeColor = System.Drawing.Color.Gray;
            this.BruceForceButton.Location = new System.Drawing.Point(929, 2);
            this.BruceForceButton.Name = "BruceForceButton";
            this.BruceForceButton.Size = new System.Drawing.Size(118, 26);
            this.BruceForceButton.TabIndex = 12;
            this.BruceForceButton.Text = "Brute Force";
            this.BruceForceButton.UseVisualStyleBackColor = false;
            this.BruceForceButton.Click += new System.EventHandler(this.BruceForceButton_Click);
            // 
            // SpatialHashingButton
            // 
            this.SpatialHashingButton.Location = new System.Drawing.Point(1053, 2);
            this.SpatialHashingButton.Name = "SpatialHashingButton";
            this.SpatialHashingButton.Size = new System.Drawing.Size(118, 26);
            this.SpatialHashingButton.TabIndex = 13;
            this.SpatialHashingButton.Text = "Spatial Hashing";
            this.SpatialHashingButton.Click += new System.EventHandler(this.SpatialHashingButton_Click);
            // 
            // QuadTreeButton
            // 
            this.QuadTreeButton.Location = new System.Drawing.Point(1177, 2);
            this.QuadTreeButton.Name = "QuadTreeButton";
            this.QuadTreeButton.Size = new System.Drawing.Size(118, 26);
            this.QuadTreeButton.TabIndex = 14;
            this.QuadTreeButton.Text = "Quad-Tree";
            this.QuadTreeButton.Click += new System.EventHandler(this.QuadTreeButton_Click);
            // 
            // CollisonSimulator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1310, 740);
            this.Controls.Add(this.QuadTreeButton);
            this.Controls.Add(this.SpatialHashingButton);
            this.Controls.Add(this.BruceForceButton);
            this.Controls.Add(this.BoundingBoxButton);
            this.Controls.Add(this.PixelPerfectButton);
            this.Controls.Add(this.ReverseButton);
            this.Controls.Add(this.PlayButton);
            this.Controls.Add(this.PauseButton);
            this.Controls.Add(this.performanceLabel);
            this.Controls.Add(this.ClearButton);
            this.Controls.Add(this.speedBar);
            this.Controls.Add(this.SquareButton);
            this.Controls.Add(this.CircleButton);
            this.Controls.Add(this.LoadButton);
            this.Controls.Add(this.SaveButton);
            this.Name = "CollisonSimulator";
            this.Text = "Collison Simulator";
            ((System.ComponentModel.ISupportInitialize)(this.speedBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button LoadButton;
        private System.Windows.Forms.Button CircleButton;
        private System.Windows.Forms.Button SquareButton;
        private System.Windows.Forms.TrackBar speedBar;
        private System.Windows.Forms.Button ClearButton;
        private System.Windows.Forms.Label performanceLabel;
        private System.Windows.Forms.Button PauseButton;
        private System.Windows.Forms.Button PlayButton;
        private System.Windows.Forms.Button ReverseButton;
        private System.Windows.Forms.Button PixelPerfectButton;
        private System.Windows.Forms.Button BoundingBoxButton;
        private System.Windows.Forms.Button BruceForceButton;
        private System.Windows.Forms.Button SpatialHashingButton;
        private System.Windows.Forms.Button QuadTreeButton;
    }
}

