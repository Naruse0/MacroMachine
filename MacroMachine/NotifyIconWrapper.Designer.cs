namespace MacroMachine
{
	partial class NotifyIconWrapper
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

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NotifyIconWrapper));
			this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem_Open = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_Exit = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip1.SuspendLayout();
			// 
			// notifyIcon1
			// 
			this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
			this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
			this.notifyIcon1.Text = "MacroMachine";
			this.notifyIcon1.Visible = true;
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_Open,
            this.toolStripMenuItem_Exit});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(121, 64);
			// 
			// toolStripMenuItem_Open
			// 
			this.toolStripMenuItem_Open.Name = "toolStripMenuItem_Open";
			this.toolStripMenuItem_Open.Size = new System.Drawing.Size(120, 30);
			this.toolStripMenuItem_Open.Text = "表示";
			// 
			// toolStripMenuItem_Exit
			// 
			this.toolStripMenuItem_Exit.Name = "toolStripMenuItem_Exit";
			this.toolStripMenuItem_Exit.Size = new System.Drawing.Size(120, 30);
			this.toolStripMenuItem_Exit.Text = "終了";
			this.contextMenuStrip1.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.NotifyIcon notifyIcon1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Open;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Exit;
	}
}
