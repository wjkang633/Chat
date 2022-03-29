namespace ChattingServer
{
    partial class FormServer
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
            this.btnStart = new System.Windows.Forms.Button();
            this.sendingMonitor = new System.Windows.Forms.ListBox();
            this.receivedMonitor = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnCurrentClient = new System.Windows.Forms.Button();
            this.btnAccessLog = new System.Windows.Forms.Button();
            this.btnChattingLog = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnStart.ForeColor = System.Drawing.Color.Black;
            this.btnStart.Location = new System.Drawing.Point(12, 12);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(166, 39);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "START";
            this.btnStart.UseVisualStyleBackColor = true;
            // 
            // sendMonitor
            // 
            this.sendingMonitor.FormattingEnabled = true;
            this.sendingMonitor.ItemHeight = 15;
            this.sendingMonitor.Location = new System.Drawing.Point(12, 285);
            this.sendingMonitor.Name = "sendMonitor";
            this.sendingMonitor.Size = new System.Drawing.Size(752, 184);
            this.sendingMonitor.TabIndex = 3;
            // 
            // receiveMonitor
            // 
            this.receivedMonitor.FormattingEnabled = true;
            this.receivedMonitor.ItemHeight = 15;
            this.receivedMonitor.Location = new System.Drawing.Point(12, 75);
            this.receivedMonitor.Name = "receiveMonitor";
            this.receivedMonitor.Size = new System.Drawing.Size(752, 184);
            this.receivedMonitor.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 267);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "발신";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "수신";
            // 
            // btnStop
            // 
            this.btnStop.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnStop.ForeColor = System.Drawing.Color.Black;
            this.btnStop.Location = new System.Drawing.Point(184, 12);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(166, 39);
            this.btnStop.TabIndex = 7;
            this.btnStop.Text = "STOP";
            this.btnStop.UseVisualStyleBackColor = true;
            // 
            // btnCurrentClient
            // 
            this.btnCurrentClient.Location = new System.Drawing.Point(356, 12);
            this.btnCurrentClient.Name = "btnCurrentClient";
            this.btnCurrentClient.Size = new System.Drawing.Size(132, 39);
            this.btnCurrentClient.TabIndex = 8;
            this.btnCurrentClient.Text = "접속 클라이언트";
            this.btnCurrentClient.UseVisualStyleBackColor = true;
            // 
            // btnAccessLog
            // 
            this.btnAccessLog.Location = new System.Drawing.Point(494, 12);
            this.btnAccessLog.Name = "btnAccessLog";
            this.btnAccessLog.Size = new System.Drawing.Size(132, 39);
            this.btnAccessLog.TabIndex = 9;
            this.btnAccessLog.Text = "접속 로그";
            this.btnAccessLog.UseVisualStyleBackColor = true;
            // 
            // btnChattingLog
            // 
            this.btnChattingLog.Location = new System.Drawing.Point(632, 12);
            this.btnChattingLog.Name = "btnChattingLog";
            this.btnChattingLog.Size = new System.Drawing.Size(132, 39);
            this.btnChattingLog.TabIndex = 10;
            this.btnChattingLog.Text = "채팅 로그";
            // 
            // FormServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(778, 481);
            this.Controls.Add(this.btnChattingLog);
            this.Controls.Add(this.btnAccessLog);
            this.Controls.Add(this.btnCurrentClient);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.receivedMonitor);
            this.Controls.Add(this.sendingMonitor);
            this.Controls.Add(this.btnStart);
            this.Name = "FormServer";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button btnStart;
        private ListBox sendingMonitor;
        private ListBox receivedMonitor;
        private Label label1;
        private Label label2;
        private Button btnStop;
        private Button btnCurrentClient;
        private Button btnAccessLog;
        private Button btnChattingLog;
    }
}