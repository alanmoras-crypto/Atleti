using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atleti;

namespace Atleti
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Inizializzazione + WebView2
        private async void Form1_Load(object sender, EventArgs e)
        {
            // Inizializza WebView2
            if (webView.CoreWebView2 == null)
                await webView.EnsureCoreWebView2Async(null);

            // Inizializza DB
            PalestraRepository.EnsureDatabaseCreated();

            // Popola giorni della settimana
            GiorniLiberi.Items.Clear();
            GiorniLiberi.Items.AddRange(new object[]
            {
                "Lunedì","Martedì","Mercoledì","Giovedì","Venerdì","Sabato","Domenica"
            });

            // Vincoli date
            dateFinale.MinDate = dateIniziale.Value.Date;
            dateIniziale.MaxDate = dateFinale.Value.Date;

            UpdateActionButtonsState();
            LoadData();
        }

        private void GiorniLiberi_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            this.BeginInvoke(new Action(UpdateActionButtonsState));
        }

        private void UpdateActionButtonsState()
        {
            bool dateOk = dateIniziale.Value.Date <= dateFinale.Value.Date;
            bool daysOk = (GiorniLiberi?.CheckedItems?.Count ?? 0) > 0;

            btnAdd.Enabled = dateOk && daysOk;
            btnUpdate.Enabled = dateOk && (dgvAtleti?.CurrentRow != null) && daysOk;
        }

        private void dateIniziale_ValueChanged(object sender, EventArgs e)
        {
            if (dateIniziale.Value.Date > dateFinale.Value.Date)
                dateFinale.Value = dateIniziale.Value.Date;

            dateFinale.MinDate = dateIniziale.Value.Date;
            UpdateActionButtonsState();
        }

        private void dateFinale_ValueChanged(object sender, EventArgs e)
        {
            if (dateFinale.Value.Date < dateIniziale.Value.Date)
                dateIniziale.Value = dateFinale.Value.Date;

            dateIniziale.MaxDate = dateFinale.Value.Date;
            UpdateActionButtonsState();
        }

        // Conta giorni liberi (rimane invariato)
        private (int totalFreeDays, List<DateTime> freeDates) CountFreeDays(DateTime start, DateTime end, IEnumerable<string> selectedWeekdayNames)
        {
            var selected = new HashSet<DayOfWeek>();
            if (selectedWeekdayNames == null) return (0, new List<DateTime>());

            foreach (var name in selectedWeekdayNames)
            {
                if (string.IsNullOrWhiteSpace(name)) continue;
                switch (name.Trim().ToLowerInvariant())
                {
                    case "lunedi":
                    case "lunedì": selected.Add(DayOfWeek.Monday); break;
                    case "martedi":
                    case "martedì": selected.Add(DayOfWeek.Tuesday); break;
                    case "mercoledi":
                    case "mercoledì": selected.Add(DayOfWeek.Wednesday); break;
                    case "giovedi":
                    case "giovedì": selected.Add(DayOfWeek.Thursday); break;
                    case "venerdi":
                    case "venerdì": selected.Add(DayOfWeek.Friday); break;
                    case "sabato": selected.Add(DayOfWeek.Saturday); break;
                    case "domenica": selected.Add(DayOfWeek.Sunday); break;
                }
            }

            var freeDates = new List<DateTime>();
            for (var d = start.Date; d <= end.Date; d = d.AddDays(1))
                if (selected.Contains(d.DayOfWeek))
                    freeDates.Add(d);

            return (freeDates.Count, freeDates);
        }

        // Carica atleti
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
                             DateFinale = a.DateFinale.ToString("yyyy-MM-dd"),
                             GiorniLiberi = a.GiorniLiberi
                         })
                         .ToList();

            dgvAtleti.DataSource = list;

            if (dgvAtleti.Columns.Contains("Id"))
                dgvAtleti.Columns["Id"].Visible = false;

            UpdateActionButtonsState();
        }

        // ============================
        //     AGGIUNGI NUOVO ATLETA
        // ============================
        private void btnAdd_Click(object sender, EventArgs e)
        {
            var nome = txtNome.Text.Trim();
            var cognome = txtCognome.Text.Trim();
            var eta = (int)numEta.Value;
            var frecceIniz = (int)numFrecceIniz.Value;
            var frecceFin = (int)numFrecceFin.Value;
            var dateIniz = dateIniziale.Value.Date;
            var dateFin = dateFinale.Value.Date;

            if ((GiorniLiberi?.CheckedItems?.Count ?? 0) == 0)
            {
                MessageBox.Show("Seleziona almeno un giorno della settimana.", "Validazione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedNames = GiorniLiberi?.CheckedItems?
                                   .Cast<object>()
                                   .Select(o => o?.ToString())
                                   .Where(s => !string.IsNullOrWhiteSpace(s))
                                   .ToList()
                               ?? new List<string>();

            var giorniSelezionati = string.Join(',', selectedNames);

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
                    DateFinale = dateFin,
                    GiorniLiberi = giorniSelezionati
                };

                int newId = PalestraRepository.Add(atleta);
                LoadData();
                ClearInputs();

                // seleziona l'atleta appena inserito
                foreach (DataGridViewRow r in dgvAtleti.Rows)
                {
                    if (int.TryParse(r.Cells["Id"].Value?.ToString(), out int rid) && rid == newId)
                    {
                        r.Selected = true;
                        if (r.Cells.Count > 1)
                            dgvAtleti.CurrentCell = r.Cells[1];
                        break;
                    }
                }

                MessageBox.Show("Atleta aggiunto.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore salvataggio: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================
        //     AGGIORNA ATLETA
        // ============================
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvAtleti?.CurrentRow == null) return;
            if (!int.TryParse(dgvAtleti.CurrentRow.Cells["Id"].Value?.ToString(), out int id))
            {
                MessageBox.Show("Seleziona un atleta valido.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if ((GiorniLiberi?.CheckedItems?.Count ?? 0) == 0)
            {
                MessageBox.Show("Seleziona almeno un giorno della settimana.", "Validazione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedForUpdate = GiorniLiberi?.CheckedItems?
                                        .Cast<object>()
                                        .Select(o => o?.ToString())
                                        .Where(s => !string.IsNullOrWhiteSpace(s))
                                        .ToList()
                                    ?? new List<string>();

            var nome = txtNome.Text.Trim();
            var cognome = txtCognome.Text.Trim();
            var eta = (int)numEta.Value;
            var frecceIniz = (int)numFrecceIniz.Value;
            var frecceFin = (int)numFrecceFin.Value;
            var dateIniz = dateIniziale.Value.Date;
            var dateFin = dateFinale.Value.Date;
            var giorniSelezionati = string.Join(',', selectedForUpdate);

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
                atleta.GiorniLiberi = giorniSelezionati;

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

        // ============================
        //     ELIMINA ATLETA
        // ============================
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvAtleti?.CurrentRow == null) return;
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

        // ============================
        //     PULIZIA CAMPI
        // ============================
        private void ClearInputs()
        {
            txtNome.Clear();
            txtCognome.Clear();
            numEta.Value = numEta.Minimum;
            numFrecceIniz.Value = Math.Max(1, numFrecceIniz.Minimum);
            numFrecceFin.Value = Math.Max(2, numFrecceFin.Minimum);
            dateIniziale.Value = DateTime.Today;
            dateFinale.Value = DateTime.Today;

            if (GiorniLiberi != null)
            {
                for (int i = 0; i < GiorniLiberi.Items.Count; i++)
                    GiorniLiberi.SetItemChecked(i, false);
            }

            if (dgvAtleti != null)
                dgvAtleti.ClearSelection();

            UpdateActionButtonsState();
        }

        // ============================
        //     SELEZIONE ATLETA
        // ============================
        private void dgvAtleti_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAtleti?.CurrentRow == null)
            {
                UpdateActionButtonsState();
                return;
            }

            var row = dgvAtleti.CurrentRow;

            txtNome.Text = row.Cells["Nome"].Value?.ToString() ?? string.Empty;
            txtCognome.Text = row.Cells["Cognome"].Value?.ToString() ?? string.Empty;

            if (int.TryParse(row.Cells["Eta"].Value?.ToString(), out int eta))
                numEta.Value = Math.Max(numEta.Minimum, Math.Min(numEta.Maximum, eta));

            if (int.TryParse(row.Cells["FrecceIniz"].Value?.ToString(), out int fi))
                numFrecceIniz.Value = Math.Max(numFrecceIniz.Minimum, Math.Min(numFrecceIniz.Maximum, fi));

            if (int.TryParse(row.Cells["FrecceFin"].Value?.ToString(), out int ff))
                numFrecceFin.Value = Math.Max(numFrecceFin.Minimum, Math.Min(numFrecceFin.Maximum, ff));

            if (DateTime.TryParse(row.Cells["DateIniziale"].Value?.ToString(), out DateTime di))
                dateIniziale.Value = di;
            else
                dateIniziale.Value = DateTime.Today;

            if (DateTime.TryParse(row.Cells["DateFinale"].Value?.ToString(), out DateTime df))
                dateFinale.Value = df;
            else
                dateFinale.Value = DateTime.Today;

            var giorniStr = row.Cells["GiorniLiberi"]?.Value?.ToString() ?? string.Empty;
            var selezionati = giorniStr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(s => s.Trim())
                                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (GiorniLiberi != null)
            {
                for (int i = 0; i < GiorniLiberi.Items.Count; i++)
                {
                    var itemObj = GiorniLiberi.Items[i];
                    var item = itemObj?.ToString() ?? string.Empty;
                    GiorniLiberi.SetItemChecked(i, selezionati.Contains(item));
                }
            }

            UpdateActionButtonsState();
        }

        // ============================================
        //   GRAFICO LINEARE DELLE FRECCE (y = mx + q)
        // ============================================
        private async void btnGT_Click_1(object sender, EventArgs e)
        {
            // Assicura che WebView2 sia inizializzato
            if (webView.CoreWebView2 == null)
                await webView.EnsureCoreWebView2Async(null);

            // Verifica selezione atleta
            if (dgvAtleti?.CurrentRow == null)
            {
                MessageBox.Show("Seleziona un atleta nella tabella.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!int.TryParse(dgvAtleti.CurrentRow.Cells["Id"].Value?.ToString(), out int id))
            {
                MessageBox.Show("Atleta non valido.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var atleta = PalestraRepository.Find(id);
            if (atleta == null)
            {
                MessageBox.Show("Atleta non trovato.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // ============================================
            //   CALCOLO PROGRESSIONE LINEARE (y = mx + q)
            // ============================================

            int totalDays = (atleta.DateFinale - atleta.DateIniziale).Days;

            double y0 = atleta.FrecceIniz;   // primo giorno (minimo)
            double y1 = atleta.FrecceFin;    // ultimo giorno (massimo)

            double m = (y1 - y0) / totalDays;   // coefficiente angolare
            double q = y0;                      // intercetta

            var labels = new List<string>();
            var values = new List<double>();

            for (int x = 0; x <= totalDays; x++)
            {
                DateTime day = atleta.DateIniziale.AddDays(x);
                labels.Add(day.ToString("yyyy-MM-dd"));

                double y = m * x + q;   // equazione della retta
                values.Add(Math.Round(y, 2));
            }

            // ============================================
            //   COSTRUZIONE HTML + CHART.JS 3.7.1
            // ============================================

            string html = $@"
<!DOCTYPE html>
<html>
<head>
<meta charset='UTF-8'>
<script src='https://cdn.jsdelivr.net/npm/chart.js@3.7.1'></script>
</head>
<body style='font-family: Arial;'>
<h3>Frecce da tirare ogni giorno (progressione lineare)</h3>

<canvas id='chart' width='800' height='350'></canvas>

<script>
const labels = [{string.Join(",", labels.Select(l => $"'{l}'"))}];
const data = [{string.Join(",", values)}];

new Chart(document.getElementById('chart'), {{
    type: 'line',
    data: {{
        labels: labels,
        datasets: [{{
            label: 'Frecce da tirare',
            data: data,
            borderColor: 'rgb(255, 99, 132)',
            backgroundColor: 'rgba(255, 99, 132, 0.2)',
            tension: 0.3,
            pointRadius: 4
        }}]
    }},
    options: {{
        scales: {{
            y: {{
                title: {{
                    display: true,
                    text: 'Numero frecce'
                }}
            }},
            x: {{
                title: {{
                    display: true,
                    text: 'Data'
                }}
            }}
        }}
    }}
}});
</script>

</body>
</html>";

            // Mostra il grafico
            webView.NavigateToString(html);
        }
    }
}


