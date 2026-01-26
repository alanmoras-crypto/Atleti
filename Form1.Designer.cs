namespace Atleti
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.DataGridView dgvAtleti;
        private System.Windows.Forms.TextBox txtNome;
        private System.Windows.Forms.TextBox txtCognome;
        private System.Windows.Forms.NumericUpDown numEta;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblNome;
        private System.Windows.Forms.Label lblCognome;
        private System.Windows.Forms.Label lblEta;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            dgvAtleti = new DataGridView();
            txtNome = new TextBox();
            txtCognome = new TextBox();
            numEta = new NumericUpDown();
            btnAdd = new Button();
            btnUpdate = new Button();
            btnDelete = new Button();
            btnClear = new Button();
            lblNome = new Label();
            lblCognome = new Label();
            lblEta = new Label();
            btnGiorniTOT = new Label();
            btnGiornoFine = new Label();
            dateIniziale = new DateTimePicker();
            dateFinale = new DateTimePicker();
            numFrecceIniz = new NumericUpDown();
            numFrecceFin = new NumericUpDown();
            lblGiornoIniz = new Label();
            lblGiornoFin = new Label();
            btnConfermaSett = new Button();
            giorniSett = new ComboBox();
            gionriliberi = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvAtleti).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numEta).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numFrecceIniz).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numFrecceFin).BeginInit();
            SuspendLayout();
            // 
            // dgvAtleti
            // 
            dgvAtleti.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvAtleti.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvAtleti.Location = new Point(12, 12);
            dgvAtleti.MultiSelect = false;
            dgvAtleti.Name = "dgvAtleti";
            dgvAtleti.ReadOnly = true;
            dgvAtleti.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAtleti.Size = new Size(979, 227);
            dgvAtleti.TabIndex = 0;
            dgvAtleti.SelectionChanged += dgvAtleti_SelectionChanged;
            // 
            // txtNome
            // 
            txtNome.Location = new Point(90, 259);
            txtNome.Name = "txtNome";
            txtNome.Size = new Size(200, 23);
            txtNome.TabIndex = 1;
            // 
            // txtCognome
            // 
            txtCognome.Location = new Point(90, 288);
            txtCognome.Name = "txtCognome";
            txtCognome.Size = new Size(200, 23);
            txtCognome.TabIndex = 2;
            // 
            // numEta
            // 
            numEta.Location = new Point(90, 317);
            numEta.Maximum = new decimal(new int[] { 120, 0, 0, 0 });
            numEta.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numEta.Name = "numEta";
            numEta.Size = new Size(80, 23);
            numEta.TabIndex = 3;
            numEta.Value = new decimal(new int[] { 18, 0, 0, 0 });
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(524, 256);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(80, 27);
            btnAdd.TabIndex = 4;
            btnAdd.Text = "Aggiungi";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(524, 289);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(80, 27);
            btnUpdate.TabIndex = 5;
            btnUpdate.Text = "Aggiorna";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += btnUpdate_Click;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(616, 258);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(80, 27);
            btnDelete.TabIndex = 6;
            btnDelete.Text = "Elimina";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(616, 288);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(80, 27);
            btnClear.TabIndex = 7;
            btnClear.Text = "Pulisci";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            // 
            // lblNome
            // 
            lblNome.AutoSize = true;
            lblNome.Location = new Point(12, 262);
            lblNome.Name = "lblNome";
            lblNome.Size = new Size(43, 15);
            lblNome.TabIndex = 8;
            lblNome.Text = "Nome:";
            // 
            // lblCognome
            // 
            lblCognome.AutoSize = true;
            lblCognome.Location = new Point(12, 291);
            lblCognome.Name = "lblCognome";
            lblCognome.Size = new Size(63, 15);
            lblCognome.TabIndex = 9;
            lblCognome.Text = "Cognome:";
            // 
            // lblEta
            // 
            lblEta.AutoSize = true;
            lblEta.Location = new Point(12, 319);
            lblEta.Name = "lblEta";
            lblEta.Size = new Size(26, 15);
            lblEta.TabIndex = 10;
            lblEta.Text = "Età:";
            // 
            // btnGiorniTOT
            // 
            btnGiorniTOT.AutoSize = true;
            btnGiorniTOT.Location = new Point(316, 262);
            btnGiorniTOT.Name = "btnGiorniTOT";
            btnGiorniTOT.Size = new Size(82, 15);
            btnGiorniTOT.TabIndex = 11;
            btnGiorniTOT.Text = "Giorno iniziale";
            // 
            // btnGiornoFine
            // 
            btnGiornoFine.AutoSize = true;
            btnGiornoFine.Location = new Point(316, 294);
            btnGiornoFine.Name = "btnGiornoFine";
            btnGiornoFine.Size = new Size(80, 15);
            btnGiornoFine.TabIndex = 12;
            btnGiornoFine.Text = "Giorno Finale:";
            // 
            // dateIniziale
            // 
            dateIniziale.Format = DateTimePickerFormat.Short;
            dateIniziale.Location = new Point(403, 259);
            dateIniziale.Name = "dateIniziale";
            dateIniziale.Size = new Size(120, 23);
            dateIniziale.TabIndex = 14;
            dateIniziale.Value = new DateTime(2026, 1, 26, 0, 0, 0, 0);
            dateIniziale.ValueChanged += dateIniziale_ValueChanged;
            // 
            // dateFinale
            // 
            dateFinale.Format = DateTimePickerFormat.Short;
            dateFinale.Location = new Point(403, 288);
            dateFinale.Name = "dateFinale";
            dateFinale.Size = new Size(120, 23);
            dateFinale.TabIndex = 15;
            dateFinale.Value = new DateTime(2026, 1, 26, 0, 0, 0, 0);
            dateFinale.ValueChanged += dateFinale_ValueChanged;
            // 
            // numFrecceIniz
            // 
            numFrecceIniz.Location = new Point(381, 357);
            numFrecceIniz.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numFrecceIniz.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numFrecceIniz.Name = "numFrecceIniz";
            numFrecceIniz.Size = new Size(70, 23);
            numFrecceIniz.TabIndex = 16;
            numFrecceIniz.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // numFrecceFin
            // 
            numFrecceFin.Location = new Point(471, 357);
            numFrecceFin.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numFrecceFin.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            numFrecceFin.Name = "numFrecceFin";
            numFrecceFin.Size = new Size(70, 23);
            numFrecceFin.TabIndex = 17;
            numFrecceFin.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // lblGiornoIniz
            // 
            lblGiornoIniz.AutoSize = true;
            lblGiornoIniz.Location = new Point(616, 234);
            lblGiornoIniz.Name = "lblGiornoIniz";
            lblGiornoIniz.Size = new Size(0, 15);
            lblGiornoIniz.TabIndex = 18;
            // 
            // lblGiornoFin
            // 
            lblGiornoFin.AutoSize = true;
            lblGiornoFin.Location = new Point(616, 270);
            lblGiornoFin.Name = "lblGiornoFin";
            lblGiornoFin.Size = new Size(0, 15);
            lblGiornoFin.TabIndex = 19;
            // 
            // btnConfermaSett
            // 
            btnConfermaSett.Location = new Point(794, 353);
            btnConfermaSett.Name = "btnConfermaSett";
            btnConfermaSett.Size = new Size(80, 27);
            btnConfermaSett.TabIndex = 20;
            btnConfermaSett.Text = "Conferma";
            btnConfermaSett.UseVisualStyleBackColor = true;
            // 
            // giorniSett
            // 
            giorniSett.FormattingEnabled = true;
            giorniSett.Items.AddRange(new object[] { "Lunedi", "Martedi", "Mercoledi", "Giovedi", "Venerdi", "Sabato", "Domenica" });
            giorniSett.Location = new Point(667, 353);
            giorniSett.Name = "giorniSett";
            giorniSett.Size = new Size(121, 23);
            giorniSett.TabIndex = 21;
            // 
            // gionriliberi
            // 
            gionriliberi.AutoSize = true;
            gionriliberi.Location = new Point(776, 317);
            gionriliberi.Name = "gionriliberi";
            gionriliberi.Size = new Size(0, 15);
            gionriliberi.TabIndex = 23;
            // 
            // Form1
            // 
            ClientSize = new Size(993, 392);
            Controls.Add(gionriliberi);
            Controls.Add(giorniSett);
            Controls.Add(btnConfermaSett);
            Controls.Add(lblGiornoFin);
            Controls.Add(lblGiornoIniz);
            Controls.Add(numFrecceFin);
            Controls.Add(numFrecceIniz);
            Controls.Add(dateFinale);
            Controls.Add(dateIniziale);
            Controls.Add(btnGiornoFine);
            Controls.Add(btnGiorniTOT);
            Controls.Add(lblEta);
            Controls.Add(lblCognome);
            Controls.Add(lblNome);
            Controls.Add(btnClear);
            Controls.Add(btnDelete);
            Controls.Add(btnUpdate);
            Controls.Add(btnAdd);
            Controls.Add(numEta);
            Controls.Add(txtCognome);
            Controls.Add(txtNome);
            Controls.Add(dgvAtleti);
            Name = "Form1";
            Text = "Gestione Atleti";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dgvAtleti).EndInit();
            ((System.ComponentModel.ISupportInitialize)numEta).EndInit();
            ((System.ComponentModel.ISupportInitialize)numFrecceIniz).EndInit();
            ((System.ComponentModel.ISupportInitialize)numFrecceFin).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label btnGiorniTOT;
        private Label btnGiornoFine;
        private DateTimePicker dateIniziale;
        private DateTimePicker dateFinale;
        private NumericUpDown numFrecceIniz;
        private NumericUpDown numFrecceFin;
        private Label lblGiornoIniz;
        private Label lblGiornoFin;
        private Button btnConfermaSett;
        private ComboBox giorniSett;
        private Label gionriliberi;
    }
}
