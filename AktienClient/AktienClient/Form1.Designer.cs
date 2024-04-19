namespace UserClient
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
            checkedListBox1 = new CheckedListBox();
            button1 = new Button();
            transactionsDisplay = new RichTextBox();
            radioButton1 = new RadioButton();
            radioButton2 = new RadioButton();
            radioButton3 = new RadioButton();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            textBox3 = new TextBox();
            SuspendLayout();
            // 
            // checkedListBox1
            // 
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Items.AddRange(new object[] { "AAPL", "MSFT", "AMZN", "NVDA", "AMD" });
            checkedListBox1.Location = new Point(33, 60);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(195, 292);
            checkedListBox1.TabIndex = 3;
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 7F);
            button1.Location = new Point(33, 368);
            button1.Name = "button1";
            button1.Size = new Size(195, 40);
            button1.TabIndex = 5;
            button1.Text = "Update selection";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // transactionsDisplay
            // 
            transactionsDisplay.Location = new Point(33, 429);
            transactionsDisplay.Name = "transactionsDisplay";
            transactionsDisplay.ReadOnly = true;
            transactionsDisplay.Size = new Size(779, 187);
            transactionsDisplay.TabIndex = 6;
            transactionsDisplay.Text = "";
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Location = new Point(969, 60);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(121, 34);
            radioButton1.TabIndex = 7;
            radioButton1.TabStop = true;
            radioButton1.Text = "Frankfurt";
            radioButton1.UseVisualStyleBackColor = true;
            radioButton1.Click += onRadioButtonClick;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Location = new Point(969, 100);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(118, 34);
            radioButton2.TabIndex = 8;
            radioButton2.TabStop = true;
            radioButton2.Text = "Stuttgart";
            radioButton2.UseVisualStyleBackColor = true;
            radioButton2.Click += onRadioButtonClick;
            // 
            // radioButton3
            // 
            radioButton3.AutoSize = true;
            radioButton3.Location = new Point(969, 140);
            radioButton3.Name = "radioButton3";
            radioButton3.Size = new Size(126, 34);
            radioButton3.TabIndex = 9;
            radioButton3.TabStop = true;
            radioButton3.Text = "München";
            radioButton3.UseVisualStyleBackColor = true;
            radioButton3.Click += onRadioButtonClick;
            // 
            // textBox1
            // 
            textBox1.BorderStyle = BorderStyle.None;
            textBox1.Location = new Point(969, 26);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(175, 28);
            textBox1.TabIndex = 10;
            textBox1.Text = "Stock exchange:";
            // 
            // textBox2
            // 
            textBox2.BorderStyle = BorderStyle.None;
            textBox2.Location = new Point(33, 26);
            textBox2.Name = "textBox2";
            textBox2.ReadOnly = true;
            textBox2.Size = new Size(195, 28);
            textBox2.TabIndex = 11;
            textBox2.Text = "Stocks to track:";
            // 
            // textBox3
            // 
            textBox3.BorderStyle = BorderStyle.None;
            textBox3.Location = new Point(288, 26);
            textBox3.Name = "textBox3";
            textBox3.ReadOnly = true;
            textBox3.Size = new Size(175, 28);
            textBox3.TabIndex = 12;
            textBox3.Text = "Stocks:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(12F, 30F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1218, 717);
            Controls.Add(textBox3);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(radioButton3);
            Controls.Add(radioButton2);
            Controls.Add(radioButton1);
            Controls.Add(transactionsDisplay);
            Controls.Add(button1);
            Controls.Add(checkedListBox1);
            Name = "Form1";
            Text = "Form1";
            FormClosed += MainForm_FormClosed;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private CheckedListBox checkedListBox1;
        private Button button1;
        private RichTextBox transactionsDisplay;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private RadioButton radioButton3;
        private TextBox textBox1;
        private TextBox textBox2;
        private TextBox textBox3;
    }
}
