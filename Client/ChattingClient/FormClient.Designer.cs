namespace ChattingClient
{
    partial class FormClient
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
            this.components = new System.ComponentModel.Container();
            this.chatLabel = new System.Windows.Forms.Label();
            this.idLabel = new System.Windows.Forms.Label();
            this.pwLabel = new System.Windows.Forms.Label();
            this.txtLoginID = new System.Windows.Forms.TextBox();
            this.txtLoginPW = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.pnlLogin = new System.Windows.Forms.Panel();
            this.btnMoveToJoin = new System.Windows.Forms.Button();
            this.signInLabel = new System.Windows.Forms.Label();
            this.pnlJoin = new System.Windows.Forms.Panel();
            this.btnJoinClose = new System.Windows.Forms.Button();
            this.btnJoin = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtJoinID = new System.Windows.Forms.TextBox();
            this.txtJoinName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtJoinPW = new System.Windows.Forms.TextBox();
            this.lstChat = new System.Windows.Forms.ListBox();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.btnChatListLogout = new System.Windows.Forms.Button();
            this.userId = new System.Windows.Forms.LinkLabel();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.friendListTab = new System.Windows.Forms.TabPage();
            this.btnChat = new System.Windows.Forms.Button();
            this.lstFriendLoginState = new System.Windows.Forms.ListBox();
            this.lstFriend = new System.Windows.Forms.ListBox();
            this.chatListTab = new System.Windows.Forms.TabPage();
            this.noChatRoom = new System.Windows.Forms.Label();
            this.pnlTalk = new System.Windows.Forms.Panel();
            this.btnSendFile = new System.Windows.Forms.Button();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnSendMsg = new System.Windows.Forms.Button();
            this.btnDownload = new System.Windows.Forms.Button();
            this.btnChatRoomClose = new System.Windows.Forms.Button();
            this.btnChatRoomExit = new System.Windows.Forms.Button();
            this.btnInvite = new System.Windows.Forms.Button();
            this.lstTalk = new System.Windows.Forms.ListBox();
            this.btnMember = new System.Windows.Forms.Button();
            this.pnlMemberList = new System.Windows.Forms.Panel();
            this.lstMemberLoginState = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnMemberClose = new System.Windows.Forms.Button();
            this.lstMember = new System.Windows.Forms.ListBox();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.pnlInvitingList = new System.Windows.Forms.Panel();
            this.lstInvitationLoginState = new System.Windows.Forms.ListBox();
            this.btnInviting = new System.Windows.Forms.Button();
            this.lstInvitation = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnInviteClose = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.pnlLogin.SuspendLayout();
            this.pnlJoin.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.friendListTab.SuspendLayout();
            this.chatListTab.SuspendLayout();
            this.pnlTalk.SuspendLayout();
            this.pnlMemberList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.pnlInvitingList.SuspendLayout();
            this.SuspendLayout();
            // 
            // chatLabel
            // 
            this.chatLabel.AutoSize = true;
            this.chatLabel.Font = new System.Drawing.Font("D2Coding", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.chatLabel.Location = new System.Drawing.Point(118, 102);
            this.chatLabel.Name = "chatLabel";
            this.chatLabel.Size = new System.Drawing.Size(72, 18);
            this.chatLabel.TabIndex = 0;
            this.chatLabel.Text = "CHATTING";
            // 
            // idLabel
            // 
            this.idLabel.AutoSize = true;
            this.idLabel.Font = new System.Drawing.Font("D2Coding", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.idLabel.Location = new System.Drawing.Point(18, 220);
            this.idLabel.Name = "idLabel";
            this.idLabel.Size = new System.Drawing.Size(24, 18);
            this.idLabel.TabIndex = 1;
            this.idLabel.Text = "ID";
            // 
            // pwLabel
            // 
            this.pwLabel.AutoSize = true;
            this.pwLabel.Font = new System.Drawing.Font("D2Coding", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.pwLabel.Location = new System.Drawing.Point(17, 256);
            this.pwLabel.Name = "pwLabel";
            this.pwLabel.Size = new System.Drawing.Size(24, 18);
            this.pwLabel.TabIndex = 2;
            this.pwLabel.Text = "PW";
            // 
            // txtLoginID
            // 
            this.txtLoginID.Font = new System.Drawing.Font("D2Coding", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtLoginID.Location = new System.Drawing.Point(54, 215);
            this.txtLoginID.Name = "txtLoginID";
            this.txtLoginID.Size = new System.Drawing.Size(228, 26);
            this.txtLoginID.TabIndex = 3;
            // 
            // txtLoginPW
            // 
            this.txtLoginPW.Font = new System.Drawing.Font("D2Coding", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtLoginPW.Location = new System.Drawing.Point(54, 253);
            this.txtLoginPW.Name = "txtLoginPW";
            this.txtLoginPW.PasswordChar = '*';
            this.txtLoginPW.Size = new System.Drawing.Size(228, 26);
            this.txtLoginPW.TabIndex = 4;
            // 
            // btnLogin
            // 
            this.btnLogin.Font = new System.Drawing.Font("D2Coding", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnLogin.Location = new System.Drawing.Point(17, 391);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(264, 40);
            this.btnLogin.TabIndex = 5;
            this.btnLogin.Text = "로그인";
            this.btnLogin.UseVisualStyleBackColor = true;
            // 
            // pnlLogin
            // 
            this.pnlLogin.Controls.Add(this.btnMoveToJoin);
            this.pnlLogin.Controls.Add(this.btnLogin);
            this.pnlLogin.Controls.Add(this.pwLabel);
            this.pnlLogin.Controls.Add(this.chatLabel);
            this.pnlLogin.Controls.Add(this.txtLoginID);
            this.pnlLogin.Controls.Add(this.idLabel);
            this.pnlLogin.Controls.Add(this.txtLoginPW);
            this.pnlLogin.Location = new System.Drawing.Point(21, 21);
            this.pnlLogin.Name = "pnlLogin";
            this.pnlLogin.Size = new System.Drawing.Size(300, 500);
            this.pnlLogin.TabIndex = 7;
            // 
            // btnMoveToJoin
            // 
            this.btnMoveToJoin.Font = new System.Drawing.Font("D2Coding", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnMoveToJoin.Location = new System.Drawing.Point(17, 437);
            this.btnMoveToJoin.Name = "btnMoveToJoin";
            this.btnMoveToJoin.Size = new System.Drawing.Size(264, 40);
            this.btnMoveToJoin.TabIndex = 6;
            this.btnMoveToJoin.Text = "회원가입";
            this.btnMoveToJoin.UseVisualStyleBackColor = true;
            // 
            // signInLabel
            // 
            this.signInLabel.AutoSize = true;
            this.signInLabel.Font = new System.Drawing.Font("D2Coding", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.signInLabel.Location = new System.Drawing.Point(117, 102);
            this.signInLabel.Name = "signInLabel";
            this.signInLabel.Size = new System.Drawing.Size(72, 18);
            this.signInLabel.TabIndex = 0;
            this.signInLabel.Text = "회원가입";
            // 
            // pnlJoin
            // 
            this.pnlJoin.Controls.Add(this.btnJoinClose);
            this.pnlJoin.Controls.Add(this.btnJoin);
            this.pnlJoin.Controls.Add(this.label1);
            this.pnlJoin.Controls.Add(this.signInLabel);
            this.pnlJoin.Controls.Add(this.txtJoinID);
            this.pnlJoin.Controls.Add(this.txtJoinName);
            this.pnlJoin.Controls.Add(this.label2);
            this.pnlJoin.Controls.Add(this.label3);
            this.pnlJoin.Controls.Add(this.txtJoinPW);
            this.pnlJoin.Location = new System.Drawing.Point(336, 21);
            this.pnlJoin.Name = "pnlJoin";
            this.pnlJoin.Size = new System.Drawing.Size(300, 500);
            this.pnlJoin.TabIndex = 8;
            // 
            // btnJoinClose
            // 
            this.btnJoinClose.Font = new System.Drawing.Font("D2Coding", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnJoinClose.Location = new System.Drawing.Point(266, 9);
            this.btnJoinClose.Name = "btnJoinClose";
            this.btnJoinClose.Size = new System.Drawing.Size(25, 25);
            this.btnJoinClose.TabIndex = 15;
            this.btnJoinClose.Text = "X";
            this.btnJoinClose.UseVisualStyleBackColor = true;
            // 
            // btnJoin
            // 
            this.btnJoin.Font = new System.Drawing.Font("D2Coding", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnJoin.Location = new System.Drawing.Point(21, 437);
            this.btnJoin.Name = "btnJoin";
            this.btnJoin.Size = new System.Drawing.Size(259, 40);
            this.btnJoin.TabIndex = 7;
            this.btnJoin.Text = "가입";
            this.btnJoin.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("D2Coding", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(11, 268);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "이름";
            // 
            // txtJoinID
            // 
            this.txtJoinID.Font = new System.Drawing.Font("D2Coding", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtJoinID.Location = new System.Drawing.Point(52, 181);
            this.txtJoinID.Name = "txtJoinID";
            this.txtJoinID.Size = new System.Drawing.Size(228, 26);
            this.txtJoinID.TabIndex = 3;
            // 
            // txtJoinName
            // 
            this.txtJoinName.Font = new System.Drawing.Font("D2Coding", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtJoinName.Location = new System.Drawing.Point(52, 261);
            this.txtJoinName.Name = "txtJoinName";
            this.txtJoinName.Size = new System.Drawing.Size(228, 26);
            this.txtJoinName.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("D2Coding", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(21, 187);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 18);
            this.label2.TabIndex = 1;
            this.label2.Text = "ID";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("D2Coding", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(21, 226);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 18);
            this.label3.TabIndex = 2;
            this.label3.Text = "PW";
            // 
            // txtJoinPW
            // 
            this.txtJoinPW.Font = new System.Drawing.Font("D2Coding", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtJoinPW.Location = new System.Drawing.Point(52, 221);
            this.txtJoinPW.Name = "txtJoinPW";
            this.txtJoinPW.PasswordChar = '*';
            this.txtJoinPW.Size = new System.Drawing.Size(228, 26);
            this.txtJoinPW.TabIndex = 4;
            // 
            // lstChat
            // 
            this.lstChat.BackColor = System.Drawing.SystemColors.Window;
            this.lstChat.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstChat.Font = new System.Drawing.Font("D2Coding", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lstChat.FormattingEnabled = true;
            this.lstChat.ItemHeight = 14;
            this.lstChat.Location = new System.Drawing.Point(2, 4);
            this.lstChat.Name = "lstChat";
            this.lstChat.Size = new System.Drawing.Size(281, 378);
            this.lstChat.TabIndex = 11;
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.btnChatListLogout);
            this.pnlMain.Controls.Add(this.userId);
            this.pnlMain.Controls.Add(this.label6);
            this.pnlMain.Controls.Add(this.panel1);
            this.pnlMain.Controls.Add(this.tabControl);
            this.pnlMain.Location = new System.Drawing.Point(651, 21);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(300, 500);
            this.pnlMain.TabIndex = 11;
            // 
            // btnChatListLogout
            // 
            this.btnChatListLogout.Font = new System.Drawing.Font("D2Coding", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnChatListLogout.Location = new System.Drawing.Point(210, 16);
            this.btnChatListLogout.Name = "btnChatListLogout";
            this.btnChatListLogout.Size = new System.Drawing.Size(67, 30);
            this.btnChatListLogout.TabIndex = 12;
            this.btnChatListLogout.Text = "로그아웃";
            this.btnChatListLogout.UseVisualStyleBackColor = true;
            // 
            // userId
            // 
            this.userId.AutoSize = true;
            this.userId.Font = new System.Drawing.Font("D2Coding", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.userId.LinkColor = System.Drawing.Color.SteelBlue;
            this.userId.Location = new System.Drawing.Point(31, 35);
            this.userId.Name = "userId";
            this.userId.Size = new System.Drawing.Size(21, 14);
            this.userId.TabIndex = 14;
            this.userId.TabStop = true;
            this.userId.Text = "ID";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("D2Coding", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label6.Location = new System.Drawing.Point(31, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 14);
            this.label6.TabIndex = 12;
            this.label6.Text = "내 아이디";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(300, 64);
            this.panel1.TabIndex = 16;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.friendListTab);
            this.tabControl.Controls.Add(this.chatListTab);
            this.tabControl.Font = new System.Drawing.Font("D2Coding", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tabControl.Location = new System.Drawing.Point(3, 78);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(294, 419);
            this.tabControl.TabIndex = 15;
            // 
            // friendListTab
            // 
            this.friendListTab.Controls.Add(this.btnChat);
            this.friendListTab.Controls.Add(this.lstFriendLoginState);
            this.friendListTab.Controls.Add(this.lstFriend);
            this.friendListTab.Location = new System.Drawing.Point(4, 23);
            this.friendListTab.Name = "friendListTab";
            this.friendListTab.Padding = new System.Windows.Forms.Padding(3);
            this.friendListTab.Size = new System.Drawing.Size(286, 392);
            this.friendListTab.TabIndex = 0;
            this.friendListTab.Text = "친구 목록";
            this.friendListTab.UseVisualStyleBackColor = true;
            // 
            // btnChat
            // 
            this.btnChat.Font = new System.Drawing.Font("D2Coding", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnChat.Location = new System.Drawing.Point(2, 349);
            this.btnChat.Name = "btnChat";
            this.btnChat.Size = new System.Drawing.Size(281, 38);
            this.btnChat.TabIndex = 7;
            this.btnChat.Text = "방 만들기";
            this.btnChat.UseVisualStyleBackColor = true;
            // 
            // lstFriendLoginState
            // 
            this.lstFriendLoginState.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstFriendLoginState.Font = new System.Drawing.Font("D2Coding", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lstFriendLoginState.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.lstFriendLoginState.FormattingEnabled = true;
            this.lstFriendLoginState.ItemHeight = 14;
            this.lstFriendLoginState.Location = new System.Drawing.Point(168, 6);
            this.lstFriendLoginState.Name = "lstFriendLoginState";
            this.lstFriendLoginState.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lstFriendLoginState.Size = new System.Drawing.Size(114, 336);
            this.lstFriendLoginState.TabIndex = 12;
            // 
            // lstFriend
            // 
            this.lstFriend.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstFriend.Font = new System.Drawing.Font("D2Coding", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lstFriend.FormattingEnabled = true;
            this.lstFriend.ItemHeight = 14;
            this.lstFriend.Location = new System.Drawing.Point(2, 5);
            this.lstFriend.Name = "lstFriend";
            this.lstFriend.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lstFriend.Size = new System.Drawing.Size(281, 336);
            this.lstFriend.TabIndex = 19;
            // 
            // chatListTab
            // 
            this.chatListTab.Controls.Add(this.noChatRoom);
            this.chatListTab.Controls.Add(this.lstChat);
            this.chatListTab.Location = new System.Drawing.Point(4, 23);
            this.chatListTab.Name = "chatListTab";
            this.chatListTab.Padding = new System.Windows.Forms.Padding(3);
            this.chatListTab.Size = new System.Drawing.Size(286, 392);
            this.chatListTab.TabIndex = 1;
            this.chatListTab.Text = "대화 목록";
            this.chatListTab.UseVisualStyleBackColor = true;
            // 
            // noChatRoom
            // 
            this.noChatRoom.AutoSize = true;
            this.noChatRoom.Font = new System.Drawing.Font("D2Coding", 8.249999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.noChatRoom.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.noChatRoom.Location = new System.Drawing.Point(72, 187);
            this.noChatRoom.Name = "noChatRoom";
            this.noChatRoom.Size = new System.Drawing.Size(157, 13);
            this.noChatRoom.TabIndex = 13;
            this.noChatRoom.Text = "개설된 대화방이 없습니다.";
            this.noChatRoom.Visible = false;
            // 
            // pnlTalk
            // 
            this.pnlTalk.Controls.Add(this.btnSendFile);
            this.pnlTalk.Controls.Add(this.txtMessage);
            this.pnlTalk.Controls.Add(this.btnSendMsg);
            this.pnlTalk.Controls.Add(this.btnDownload);
            this.pnlTalk.Controls.Add(this.btnChatRoomClose);
            this.pnlTalk.Controls.Add(this.btnChatRoomExit);
            this.pnlTalk.Controls.Add(this.btnInvite);
            this.pnlTalk.Controls.Add(this.lstTalk);
            this.pnlTalk.Controls.Add(this.btnMember);
            this.pnlTalk.Location = new System.Drawing.Point(21, 541);
            this.pnlTalk.Name = "pnlTalk";
            this.pnlTalk.Size = new System.Drawing.Size(300, 500);
            this.pnlTalk.TabIndex = 12;
            // 
            // btnSendFile
            // 
            this.btnSendFile.Font = new System.Drawing.Font("D2Coding", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnSendFile.Location = new System.Drawing.Point(234, 421);
            this.btnSendFile.Name = "btnSendFile";
            this.btnSendFile.Size = new System.Drawing.Size(54, 23);
            this.btnSendFile.TabIndex = 20;
            this.btnSendFile.Text = "파일";
            this.btnSendFile.UseVisualStyleBackColor = true;
            // 
            // txtMessage
            // 
            this.txtMessage.Font = new System.Drawing.Font("D2Coding", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtMessage.Location = new System.Drawing.Point(11, 421);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessage.Size = new System.Drawing.Size(217, 66);
            this.txtMessage.TabIndex = 19;
            // 
            // btnSendMsg
            // 
            this.btnSendMsg.Font = new System.Drawing.Font("D2Coding", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnSendMsg.Location = new System.Drawing.Point(234, 447);
            this.btnSendMsg.Name = "btnSendMsg";
            this.btnSendMsg.Size = new System.Drawing.Size(54, 40);
            this.btnSendMsg.TabIndex = 16;
            this.btnSendMsg.Text = "보내기";
            this.btnSendMsg.UseVisualStyleBackColor = true;
            // 
            // btnDownload
            // 
            this.btnDownload.Font = new System.Drawing.Font("D2Coding", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnDownload.Location = new System.Drawing.Point(227, 77);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(54, 25);
            this.btnDownload.TabIndex = 15;
            this.btnDownload.Text = "저장";
            this.btnDownload.UseVisualStyleBackColor = true;
            // 
            // btnChatRoomClose
            // 
            this.btnChatRoomClose.Font = new System.Drawing.Font("D2Coding", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnChatRoomClose.Location = new System.Drawing.Point(238, 20);
            this.btnChatRoomClose.Name = "btnChatRoomClose";
            this.btnChatRoomClose.Size = new System.Drawing.Size(50, 30);
            this.btnChatRoomClose.TabIndex = 14;
            this.btnChatRoomClose.Text = "X";
            this.btnChatRoomClose.UseVisualStyleBackColor = true;
            // 
            // btnChatRoomExit
            // 
            this.btnChatRoomExit.Font = new System.Drawing.Font("D2Coding", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnChatRoomExit.Location = new System.Drawing.Point(156, 20);
            this.btnChatRoomExit.Name = "btnChatRoomExit";
            this.btnChatRoomExit.Size = new System.Drawing.Size(80, 30);
            this.btnChatRoomExit.TabIndex = 13;
            this.btnChatRoomExit.Text = "방 나가기";
            this.btnChatRoomExit.UseVisualStyleBackColor = true;
            // 
            // btnInvite
            // 
            this.btnInvite.Font = new System.Drawing.Font("D2Coding", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnInvite.Location = new System.Drawing.Point(84, 20);
            this.btnInvite.Name = "btnInvite";
            this.btnInvite.Size = new System.Drawing.Size(70, 30);
            this.btnInvite.TabIndex = 12;
            this.btnInvite.Text = "초대하기";
            this.btnInvite.UseVisualStyleBackColor = true;
            // 
            // lstTalk
            // 
            this.lstTalk.Font = new System.Drawing.Font("D2Coding", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lstTalk.FormattingEnabled = true;
            this.lstTalk.ItemHeight = 14;
            this.lstTalk.Location = new System.Drawing.Point(11, 61);
            this.lstTalk.Name = "lstTalk";
            this.lstTalk.Size = new System.Drawing.Size(277, 354);
            this.lstTalk.TabIndex = 11;
            // 
            // btnMember
            // 
            this.btnMember.Font = new System.Drawing.Font("D2Coding", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnMember.Location = new System.Drawing.Point(12, 20);
            this.btnMember.Name = "btnMember";
            this.btnMember.Size = new System.Drawing.Size(70, 30);
            this.btnMember.TabIndex = 9;
            this.btnMember.Text = "대화 맴버";
            this.btnMember.UseVisualStyleBackColor = true;
            // 
            // pnlMemberList
            // 
            this.pnlMemberList.Controls.Add(this.lstMemberLoginState);
            this.pnlMemberList.Controls.Add(this.label4);
            this.pnlMemberList.Controls.Add(this.btnMemberClose);
            this.pnlMemberList.Controls.Add(this.lstMember);
            this.pnlMemberList.Location = new System.Drawing.Point(336, 541);
            this.pnlMemberList.Name = "pnlMemberList";
            this.pnlMemberList.Size = new System.Drawing.Size(300, 500);
            this.pnlMemberList.TabIndex = 17;
            // 
            // lstMemberLoginState
            // 
            this.lstMemberLoginState.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstMemberLoginState.Font = new System.Drawing.Font("D2Coding", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lstMemberLoginState.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.lstMemberLoginState.FormattingEnabled = true;
            this.lstMemberLoginState.ItemHeight = 14;
            this.lstMemberLoginState.Location = new System.Drawing.Point(179, 50);
            this.lstMemberLoginState.Name = "lstMemberLoginState";
            this.lstMemberLoginState.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lstMemberLoginState.Size = new System.Drawing.Size(103, 434);
            this.lstMemberLoginState.TabIndex = 22;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("D2Coding", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(15, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 14);
            this.label4.TabIndex = 15;
            this.label4.Text = "대화 맴버";
            // 
            // btnMemberClose
            // 
            this.btnMemberClose.Font = new System.Drawing.Font("D2Coding", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnMemberClose.Location = new System.Drawing.Point(260, 12);
            this.btnMemberClose.Name = "btnMemberClose";
            this.btnMemberClose.Size = new System.Drawing.Size(25, 25);
            this.btnMemberClose.TabIndex = 14;
            this.btnMemberClose.Text = "X";
            this.btnMemberClose.UseVisualStyleBackColor = true;
            // 
            // lstMember
            // 
            this.lstMember.Font = new System.Drawing.Font("D2Coding", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lstMember.FormattingEnabled = true;
            this.lstMember.ItemHeight = 14;
            this.lstMember.Location = new System.Drawing.Point(15, 48);
            this.lstMember.Name = "lstMember";
            this.lstMember.Size = new System.Drawing.Size(270, 438);
            this.lstMember.TabIndex = 11;
            // 
            // pnlInvitingList
            // 
            this.pnlInvitingList.Controls.Add(this.lstInvitationLoginState);
            this.pnlInvitingList.Controls.Add(this.btnInviting);
            this.pnlInvitingList.Controls.Add(this.lstInvitation);
            this.pnlInvitingList.Controls.Add(this.label5);
            this.pnlInvitingList.Controls.Add(this.btnInviteClose);
            this.pnlInvitingList.Location = new System.Drawing.Point(651, 541);
            this.pnlInvitingList.Name = "pnlInvitingList";
            this.pnlInvitingList.Size = new System.Drawing.Size(300, 500);
            this.pnlInvitingList.TabIndex = 18;
            // 
            // lstInvitationLoginState
            // 
            this.lstInvitationLoginState.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstInvitationLoginState.Font = new System.Drawing.Font("D2Coding", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lstInvitationLoginState.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.lstInvitationLoginState.FormattingEnabled = true;
            this.lstInvitationLoginState.ItemHeight = 14;
            this.lstInvitationLoginState.Location = new System.Drawing.Point(180, 49);
            this.lstInvitationLoginState.Name = "lstInvitationLoginState";
            this.lstInvitationLoginState.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lstInvitationLoginState.Size = new System.Drawing.Size(103, 392);
            this.lstInvitationLoginState.TabIndex = 20;
            // 
            // btnInviting
            // 
            this.btnInviting.Font = new System.Drawing.Font("D2Coding", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnInviting.Location = new System.Drawing.Point(14, 455);
            this.btnInviting.Name = "btnInviting";
            this.btnInviting.Size = new System.Drawing.Size(270, 35);
            this.btnInviting.TabIndex = 18;
            this.btnInviting.Text = "초대";
            this.btnInviting.UseVisualStyleBackColor = true;
            // 
            // lstInvitation
            // 
            this.lstInvitation.Font = new System.Drawing.Font("D2Coding", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lstInvitation.FormattingEnabled = true;
            this.lstInvitation.ItemHeight = 14;
            this.lstInvitation.Location = new System.Drawing.Point(14, 48);
            this.lstInvitation.Name = "lstInvitation";
            this.lstInvitation.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lstInvitation.Size = new System.Drawing.Size(270, 396);
            this.lstInvitation.TabIndex = 21;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("D2Coding", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label5.Location = new System.Drawing.Point(14, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 14);
            this.label5.TabIndex = 16;
            this.label5.Text = "초대 가능 친구";
            // 
            // btnInviteClose
            // 
            this.btnInviteClose.Font = new System.Drawing.Font("D2Coding", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnInviteClose.Location = new System.Drawing.Point(259, 12);
            this.btnInviteClose.Name = "btnInviteClose";
            this.btnInviteClose.Size = new System.Drawing.Size(25, 25);
            this.btnInviteClose.TabIndex = 14;
            this.btnInviteClose.Text = "X";
            this.btnInviteClose.UseVisualStyleBackColor = true;
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog1";
            // 
            // FormClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(959, 1061);
            this.Controls.Add(this.pnlInvitingList);
            this.Controls.Add(this.pnlMemberList);
            this.Controls.Add(this.pnlTalk);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlJoin);
            this.Controls.Add(this.pnlLogin);
            this.Name = "FormClient";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.pnlLogin.ResumeLayout(false);
            this.pnlLogin.PerformLayout();
            this.pnlJoin.ResumeLayout(false);
            this.pnlJoin.PerformLayout();
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.friendListTab.ResumeLayout(false);
            this.chatListTab.ResumeLayout(false);
            this.chatListTab.PerformLayout();
            this.pnlTalk.ResumeLayout(false);
            this.pnlTalk.PerformLayout();
            this.pnlMemberList.ResumeLayout(false);
            this.pnlMemberList.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.pnlInvitingList.ResumeLayout(false);
            this.pnlInvitingList.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Label chatLabel;
        private Label idLabel;
        private Label pwLabel;
        private TextBox txtLoginID;
        private TextBox txtLoginPW;
        private Button btnLogin;
        private Panel pnlLogin;
        private Label signInLabel;
        private Panel pnlJoin;
        private TextBox txtJoinID;
        private Label label2;
        private Label label3;
        private TextBox txtJoinPW;
        private Button btnJoin;
        private Label label1;
        private TextBox txtJoinName;
        private Panel pnlMain;
        private Panel pnlTalk;
        private ListBox lstTalk;
        private Button btnMember;
        private Button btnChatRoomClose;
        private Button btnChatRoomExit;
        private Button btnInvite;
        private Button btnDownload;
        private Button btnSendMsg;
        private Panel pnlMemberList;
        private Label label4;
        private Button btnMemberClose;
        private ListBox lstMember;
        private BindingSource bindingSource1;
        private Panel pnlInvitingList;
        private Label label5;
        private Button btnInviteClose;
        private Button btnInviting;
        private Button btnChatListLogout;
        private TextBox txtMessage;
        private Button btnJoinClose;
        private Label labelMoveToJoin;
        private Button btnMoveToJoin;
        private LinkLabel userId;
        private Label label6;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Button btnChat;
        private ListBox lstFriendLoginState;
        private ListBox lstFriend;
        private ListBox lstChat;
        private Panel panel1;
        private TabControl tabControl;
        private TabPage friendListTab;
        private TabPage chatListTab;
        private Label noChatRoom;
        private Button btnSendFile;
        private OpenFileDialog openFileDialog;
        private ListBox lstInvitationLoginState;
        private ListBox lstInvitation;
        private ListBox lstMemberLoginState;
    }
}