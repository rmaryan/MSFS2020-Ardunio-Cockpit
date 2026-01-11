using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MSFS2020_Ardunio_Cockpit
{
    internal class SelectPresetDialog : Form
    {
        private ListBox listBoxPresets;
        private Button okButton;
        private Button cancelButton;
        private Label descriptionLabel;

        public int SelectedPresetIndex { get; private set; } = -1;

        public SelectPresetDialog(IList<string> presetNames)
        {
            InitializeComponent();
            foreach (var name in presetNames)
            {
                listBoxPresets.Items.Add(name);
            }
            if (listBoxPresets.Items.Count > 0) listBoxPresets.SelectedIndex = 0;
        }

        private void InitializeComponent()
        {
            this.Text = "Select applicable preset";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ClientSize = new Size(460, 340);
            this.ShowInTaskbar = false;
            this.AutoScaleMode = AutoScaleMode.Font;
            this.StartPosition = FormStartPosition.Manual; // manual so we can place it exactly over the owner
            this.TopMost = true; // keep dialog above other windows

            descriptionLabel = new Label();
            descriptionLabel.Text = "Multiple applicable presets were identified. Please chose the one to activate.";
            descriptionLabel.AutoSize = false;
            descriptionLabel.Size = new Size(430, 40);
            descriptionLabel.Location = new Point(10, 10);

            listBoxPresets = new ListBox();
            listBoxPresets.Location = new Point(10, 60);
            listBoxPresets.Size = new Size(430, 200);
            listBoxPresets.SelectionMode = SelectionMode.One;
            listBoxPresets.DoubleClick += ListBoxPresets_DoubleClick;

            okButton = new Button();
            okButton.Text = "OK";
            okButton.Size = new Size(100, 30);
            okButton.Location = new Point(240, 275);
            okButton.Click += OkButton_Click;

            cancelButton = new Button();
            cancelButton.Text = "Cancel";
            cancelButton.Size = new Size(100, 30);
            cancelButton.Location = new Point(350, 275);
            cancelButton.Click += CancelButton_Click;

            this.Controls.Add(descriptionLabel);
            this.Controls.Add(listBoxPresets);
            this.Controls.Add(okButton);
            this.Controls.Add(cancelButton);

            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;
        }

        private void ListBoxPresets_DoubleClick(object sender, EventArgs e)
        {
            CommitSelectionAndClose();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            CommitSelectionAndClose();
        }

        private void CommitSelectionAndClose()
        {
            if (listBoxPresets.SelectedIndex >= 0)
            {
                SelectedPresetIndex = listBoxPresets.SelectedIndex;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select a preset or press Cancel.", "No selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            SelectedPresetIndex = -1;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // Show the dialog modally over the specified owner, keep it always on top,
        // and place it precisely centered over the owner window.
        public DialogResult ShowModalOver(Form owner)
        {
            if (owner == null)
            {
                // fallback to normal modal show
                this.TopMost = true;
                return this.ShowDialog();
            }

            // Ensure the dialog is owned by the main window so modality and Z-order are correct.
            this.Owner = owner;
            this.TopMost = true;

            // Calculate precise center position over owner (taking into account screen coordinates).
            Rectangle ownerRect = owner.Bounds;
            // If the owner is minimized, use its RestoreBounds to get last normal position.
            if (owner.WindowState == FormWindowState.Minimized)
            {
                ownerRect = owner.RestoreBounds;
            }

            int x = ownerRect.Left + (ownerRect.Width - this.Width) / 2;
            int y = ownerRect.Top + (ownerRect.Height - this.Height) / 2;

            // Make sure dialog is fully visible on screen(s)
            Rectangle working = Screen.FromRectangle(ownerRect).WorkingArea;
            if (x < working.Left) x = working.Left + 8;
            if (y < working.Top) y = working.Top + 8;
            if (x + this.Width > working.Right) x = working.Right - this.Width - 8;
            if (y + this.Height > working.Bottom) y = working.Bottom - this.Height - 8;

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(x, y);

            return this.ShowDialog(owner);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            // Ensure topmost and focused when shown
            this.TopMost = true;
            this.BringToFront();
            this.Activate();
        }
    }
}