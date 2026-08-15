namespace DAZWSaveTool
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            DecodeStart = new Button();
            ToolMode = new ComboBox();
            EncodeStart = new Button();
            signButton = new Button();
            label1 = new Label();
            label2 = new Label();
            originalValue = new TextBox();
            signedValue = new TextBox();
            label67 = new Label();
            label3 = new Label();
            aesKey = new TextBox();
            label4 = new Label();
            SuspendLayout();
            // 
            // DecodeStart
            // 
            resources.ApplyResources(DecodeStart, "DecodeStart");
            DecodeStart.Name = "DecodeStart";
            DecodeStart.UseVisualStyleBackColor = true;
            DecodeStart.Click += DecodeStart_Click;
            // 
            // ToolMode
            // 
            ToolMode.DropDownStyle = ComboBoxStyle.DropDownList;
            ToolMode.FormattingEnabled = true;
            ToolMode.Items.AddRange(new object[] { resources.GetString("ToolMode.Items"), resources.GetString("ToolMode.Items1"), resources.GetString("ToolMode.Items2"), resources.GetString("ToolMode.Items3"), resources.GetString("ToolMode.Items4"), resources.GetString("ToolMode.Items5") });
            resources.ApplyResources(ToolMode, "ToolMode");
            ToolMode.Name = "ToolMode";
            // 
            // EncodeStart
            // 
            resources.ApplyResources(EncodeStart, "EncodeStart");
            EncodeStart.Name = "EncodeStart";
            EncodeStart.UseVisualStyleBackColor = true;
            EncodeStart.Click += EncodeStart_Click;
            // 
            // signButton
            // 
            resources.ApplyResources(signButton, "signButton");
            signButton.Name = "signButton";
            signButton.UseVisualStyleBackColor = true;
            signButton.Click += signButton_Click;
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            // 
            // originalValue
            // 
            resources.ApplyResources(originalValue, "originalValue");
            originalValue.Name = "originalValue";
            // 
            // signedValue
            // 
            resources.ApplyResources(signedValue, "signedValue");
            signedValue.Name = "signedValue";
            // 
            // label67
            // 
            resources.ApplyResources(label67, "label67");
            label67.Name = "label67";
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.Name = "label3";
            // 
            // aesKey
            // 
            resources.ApplyResources(aesKey, "aesKey");
            aesKey.Name = "aesKey";
            // 
            // label4
            // 
            resources.ApplyResources(label4, "label4");
            label4.Name = "label4";
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(label4);
            Controls.Add(aesKey);
            Controls.Add(label3);
            Controls.Add(label67);
            Controls.Add(signedValue);
            Controls.Add(originalValue);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(signButton);
            Controls.Add(EncodeStart);
            Controls.Add(ToolMode);
            Controls.Add(DecodeStart);
            Name = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button DecodeStart;
        private ComboBox ToolMode;
        private Button EncodeStart;
        private Button signButton;
        private Label label1;
        private Label label2;
        private TextBox originalValue;
        private TextBox signedValue;
        private Label label67;
        private Label label3;
        private TextBox aesKey;
        private Label label4;
    }
}