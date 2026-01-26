using System;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;
using Atleti; // Serve per vedere la classe Atleta e PalestraRepository

namespace Atleti
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // All'avvio crea il DB (se necessario) e carica i dati
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                PalestraRepository.EnsureDatabaseCreated();

                // Popola la ComboBox dei giorni della settimana (italiano)
                giorniSett.Items.Clear();
                giorniSett.Items.AddRange(new object[]
                {
                    "Lunedì",
                    "Martedì",
                    "Mercoledì",
                    "Giovedì",
                    "Venerdì",
                    "Sabato",
                    "Domenica"
                });
                giorniSett.SelectedIndex = 0;
                gionriliberi.Text = string.Empty;

                // Assicura vincoli iniziali per le DateTimePicker
                dateIniziale.MaxDate = DateTimePicker.MaximumDateTime;
                dateFinale.MinDate = DateTimePicker.MinimumDateTime;
                // sincronizza limiti e mostra i giorni della settimana
                dateFinale.MinDate = dateIniziale.Value.Date;
                dateIniziale.MaxDate = dateFinale.Value.Date;
                UpdateWeekdayLabels();

                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore inizializzazione DB: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Aggiorna le label che mostrano il giorno della settimana per le date selezionate
        private void UpdateWeekdayLabels()
        {
            var it = new CultureInfo("it-IT");
            lblGiornoIniz.Text = Capitalize(dateIniziale.Value.ToString("dddd", it));
            lblGiornoFin.Text = Capitalize(dateFinale.Value.ToString("dddd", it));
        }

        private static string Capitalize(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return char.ToUpperInvariant(s[0]) + s.Substring(1);
        }

        private void dateIniziale_ValueChanged(object sender, EventArgs e)
        {
            // se l'utente imposta una data iniziale maggiore della finale, sposto la finale
            if (dateIniziale.Value.Date > dateFinale.Value.Date)
                dateFinale.Value = dateIniziale.Value.Date;

            // imposto il vincolo minimo per la data finale
            dateFinale.MinDate = dateIniziale.Value.Date;
            // aggiorna le label
            UpdateWeekdayLabels();
        }

        private void dateFinale_ValueChanged(object sender, EventArgs e)
        {
            // se l'utente imposta una data finale minore della iniziale, sposto la iniziale
            if (dateFinale.Value.Date < dateIniziale.Value.Date)
                dateIniziale.Value = dateFinale.Value.Date;

            // imposto il vincolo massimo per la data iniziale
            dateIniziale.MaxDate = dateFinale.Value.Date;
            // aggiorna le label
            UpdateWeekdayLabels();
        }

        // Carica gli atleti nella DataGridView
        private void LoadData()
        {
            var list = PalestraRepository.GetAll()
                         .Select(a => new
                         {
                             a.Id,
                             a.Nome,
                             a.Cognome,
                             a.Eta,
                             a.FrecceIniz,
                             a.FrecceFin,
                             DateIniziale = a.DateIniziale.ToString("yyyy-MM-dd"),
                             DateFinale = a.DateFinale.ToString("yyyy-MM-dd")
                         })
                         .ToList();

            dgvAtleti.DataSource = list;
            if (dgvAtleti.Columns.Contains("Id"))
                dgvAtleti.Columns["Id"].Visible = false;
        }

        // Aggiungi nuovo atleta
        private void btnAdd_Click(object sender, EventArgs e)
        {
            var nome = txtNome.Text.Trim();
            var cognome = txtCognome.Text.Trim();
            var eta = (int)numEta.Value;
            var frecceIniz = (int)numFrecceIniz.Value;
            var frecceFin = (int)numFrecceFin.Value;
            var dateIniz = dateIniziale.Value.Date;
            var dateFin = dateFinale.Value.Date;

            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(cognome))
            {
                MessageBox.Show("Nome e Cognome sono obbligatori.", "Validazione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (frecceIniz >= frecceFin)
            {
                MessageBox.Show("Le frecce del primo giorno devono essere minori di quelle dell'ultimo giorno.", "Validazione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dateIniz > dateFin)
            {
                MessageBox.Show("La data iniziale non può essere successiva alla data finale.", "Validazione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var atleta = new Atleta
                {
                    Nome = nome,
                    Cognome = cognome,
                    Eta = eta,
                    FrecceIniz = frecceIniz,
                    FrecceFin = frecceFin,
                    DateIniziale = dateIniz,
                    DateFinale = dateFin
                };
                PalestraRepository.Add(atleta);
                LoadData();
                ClearInputs();
                MessageBox.Show("Atleta aggiunto.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore salvataggio: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Aggiorna atleta selezionato
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvAtleti.CurrentRow == null) return;
            if (!int.TryParse(dgvAtleti.CurrentRow.Cells["Id"].Value?.ToString(), out int id))
            {
                MessageBox.Show("Seleziona un atleta valido.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var nome = txtNome.Text.Trim();
            var cognome = txtCognome.Text.Trim();
            var eta = (int)numEta.Value;
            var frecceIniz = (int)numFrecceIniz.Value;
            var frecceFin = (int)numFrecceFin.Value;
            var dateIniz = dateIniziale.Value.Date;
            var dateFin = dateFinale.Value.Date;

            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(cognome))
            {
                MessageBox.Show("Nome e Cognome sono obbligatori.", "Validazione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (frecceIniz >= frecceFin)
            {
                MessageBox.Show("Le frecce del primo giorno devono essere minori di quelle dell'ultimo giorno.", "Validazione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dateIniz > dateFin)
            {
                MessageBox.Show("La data iniziale non può essere successiva alla data finale.", "Validazione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var atleta = PalestraRepository.Find(id);
                if (atleta == null)
                {
                    MessageBox.Show("Atleta non trovato.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                atleta.Nome = nome;
                atleta.Cognome = cognome;
                atleta.Eta = eta;
                atleta.FrecceIniz = frecceIniz;
                atleta.FrecceFin = frecceFin;
                atleta.DateIniziale = dateIniz;
                atleta.DateFinale = dateFin;
                PalestraRepository.Update(atleta);
                LoadData();
                ClearInputs();
                MessageBox.Show("Atleta aggiornato.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore aggiornamento: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Elimina atleta selezionato
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvAtleti.CurrentRow == null) return;
            if (!int.TryParse(dgvAtleti.CurrentRow.Cells["Id"].Value?.ToString(), out int id))
            {
                MessageBox.Show("Seleziona un atleta valido.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var res = MessageBox.Show("Confermi eliminazione atleta selezionato?", "Conferma", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res != DialogResult.Yes) return;

            try
            {
                var atleta = PalestraRepository.Find(id);
                if (atleta == null)
                {
                    MessageBox.Show("Atleta non trovato.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                PalestraRepository.Delete(id);
                LoadData();
                ClearInputs();
                MessageBox.Show("Atleta eliminato.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore eliminazione: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Pulisce i campi
        private void btnClear_Click(object sender, EventArgs e) => ClearInputs();

        private void ClearInputs()
        {
            txtNome.Clear();
            txtCognome.Clear();
            numEta.Value = numEta.Minimum;
            numFrecceIniz.Value = Math.Max(1, numFrecceIniz.Minimum);
            numFrecceFin.Value = Math.Max(2, numFrecceFin.Minimum);
            dateIniziale.Value = DateTime.Today;
            dateFinale.Value = DateTime.Today;
            gionriliberi.Text = string.Empty;
            giorniSett.SelectedIndex = 0;
            UpdateWeekdayLabels();
            dgvAtleti.ClearSelection();
        }

        // Quando cambia selezione popola i campi
        private void dgvAtleti_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAtleti.CurrentRow == null) return;
            var row = dgvAtleti.CurrentRow;
            txtNome.Text = row.Cells["Nome"].Value?.ToString() ?? string.Empty;
            txtCognome.Text = row.Cells["Cognome"].Value?.ToString() ?? string.Empty;
            if (int.TryParse(row.Cells["Eta"].Value?.ToString(), out int eta))
                numEta.Value = Math.Max(numEta.Minimum, Math.Min(numEta.Maximum, eta));
            if (int.TryParse(row.Cells["FrecceIniz"].Value?.ToString(), out int fi))
                numFrecceIniz.Value = Math.Max(numFrecceIniz.Minimum, Math.Min(numFrecceIniz.Maximum, fi));
            if (int.TryParse(row.Cells["FrecceFin"].Value?.ToString(), out int ff))
                numFrecceFin.Value = Math.Max(numFrecceFin.Minimum, Math.Min(numFrecceFin.Maximum, ff));

            // DateIniziale/DateFinale potrebbero essere stringhe nel DataGridView
            if (DateTime.TryParse(row.Cells["DateIniziale"].Value?.ToString(), out DateTime di))
                dateIniziale.Value = di;
            else
                dateIniziale.Value = DateTime.Today;

            if (DateTime.TryParse(row.Cells["DateFinale"].Value?.ToString(), out DateTime df))
                dateFinale.Value = df;
            else
                dateFinale.Value = DateTime.Today;

            UpdateWeekdayLabels();
        }

        // Conferma giorno libero selezionato nella combo e lo mostra nella label
        private void btnConfermaSett_Click(object sender, EventArgs e)
        {
            if (giorniSett.SelectedItem == null)
            {
                MessageBox.Show("Seleziona un giorno della settimana.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Visualizza esattamente la voce selezionata (es: "Lunedì")
            gionriliberi.Text = giorniSett.SelectedItem.ToString();
        }
    }
}