namespace WinFormsApp1
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
            dataGridViewOutput = new DataGridView();
            richTextBoxInput = new RichTextBox();
            buttonScan = new Button();
            Lexeme = new DataGridViewTextBoxColumn();
            Column1 = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dataGridViewOutput).BeginInit();
            SuspendLayout();
            // 
            // dataGridViewOutput
            // 
            dataGridViewOutput.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewOutput.Columns.AddRange(new DataGridViewColumn[] { Lexeme, Column1 });
            dataGridViewOutput.Location = new Point(404, 12);
            dataGridViewOutput.Name = "dataGridViewOutput";
            dataGridViewOutput.Size = new Size(490, 575);
            dataGridViewOutput.TabIndex = 0;
            dataGridViewOutput.CellContentClick += dataGridViewOutput_CellContentClick;
            // 
            // richTextBoxInput
            // 
            richTextBoxInput.Location = new Point(12, 12);
            richTextBoxInput.Name = "richTextBoxInput";
            richTextBoxInput.Size = new Size(386, 523);
            richTextBoxInput.TabIndex = 1;
            richTextBoxInput.Text = "";
            richTextBoxInput.TextChanged += richTextBoxInput_TextChanged;
            // 
            // buttonScan
            // 
            buttonScan.Location = new Point(12, 541);
            buttonScan.Name = "buttonScan";
            buttonScan.Size = new Size(386, 46);
            buttonScan.TabIndex = 2;
            buttonScan.Text = "Scan";
            buttonScan.UseVisualStyleBackColor = true;
            buttonScan.Click += button1_Click;
            // 
            // Lexeme
            // 
            Lexeme.HeaderText = "Lexeme";
            Lexeme.Name = "Lexeme";
            // 
            // Column1
            // 
            Column1.HeaderText = "Token Class";
            Column1.MinimumWidth = 15;
            Column1.Name = "Column1";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(906, 599);
            Controls.Add(buttonScan);
            Controls.Add(richTextBoxInput);
            Controls.Add(dataGridViewOutput);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)dataGridViewOutput).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridViewOutput;
        private RichTextBox richTextBoxInput;
        private Button buttonScan;
        private DataGridViewTextBoxColumn Lexeme;
        private DataGridViewTextBoxColumn Column1;
    }
}
