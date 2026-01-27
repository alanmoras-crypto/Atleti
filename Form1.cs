using System;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;
using System.Text;
using System.Collections.Generic;
using Atleti; // Serve per vedere la classe Atleta e PalestraRepository

namespace Atleti
{
    public partial class Form1 : Form
    {
        private WebBrowser wbChart;

        public Form1()
        {
            InitializeComponent();
            wbChart = new WebBrowser();
            wbChart.Dock = DockStyle.Bottom;
            wbChart.Height = 200;
            this.Controls.Add(wbChart);
        }

        // All'avvio crea il DB (se necessario) e carica i dati
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                PalestraRepository.EnsureDatabaseCreated();

                // Popola la CheckedListBox dei giorni della settimana
                

                GiorniLiberi.Items.Clear();
                GiorniLiberi.Items.AddRange(new object[]
                {
                    "Lunedì",
                    "Martedì",
                    "Mercoledì",
                    "Giovedì",
                    "Venerdì",
                    "Sabato",
                    "Domenica"
                });
                // Assicura vincoli iniziali per le DateTimePicker
                dateIniziale.MaxDate = DateTimePicker.MaximumDateTime;
                dateFinale.MinDate = DateTimePicker.MinimumDateTime;
                dateFinale.MinDate = dateIniziale.Value.Date;
                dateIniziale.MaxDate = dateFinale.Value.Date;
                UpdateActionButtonsState();

                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore inizializzazione DB: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GiorniLiberi_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Dopo il change dell'item, aggiorna lo stato dei pulsanti
            // BeginInvoke per aspettare che CheckedItems venga aggiornato
            this.BeginInvoke(new Action(UpdateActionButtonsState));
        }

        private void UpdateActionButtonsState()
        {
            // disabilita Aggiungi/Aggiorna se data iniziale è dopo la finale
            bool dateOk = dateIniziale.Value.Date <= dateFinale.Value.Date;
            // richiede almeno un giorno selezionato nella CheckedListBox (null-safe)
            bool daysOk = (GiorniLiberi?.CheckedItems?.Count ?? 0) > 0;

            btnAdd.Enabled = dateOk && daysOk;
            // btnUpdate abilitato solo se esiste selezione e le date e i giorni sono validi
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

        // Conta i giorni liberi tra due date (inclusivo) basandosi sui giorni selezionati nella CheckedListBox
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
                    case "lunedì":
                        selected.Add(DayOfWeek.Monday); break;
                    case "martedi":
                    case "martedì":
                        selected.Add(DayOfWeek.Tuesday); break;
                    case "mercoledi":
                    case "mercoledì":
                        selected.Add(DayOfWeek.Wednesday); break;
                    case "giovedi":
                    case "giovedì":
                        selected.Add(DayOfWeek.Thursday); break;
                    case "venerdi":
                    case "venerdì":
                        selected.Add(DayOfWeek.Friday); break;
                    case "sabato":
                        selected.Add(DayOfWeek.Saturday); break;
                    case "domenica":
                        selected.Add(DayOfWeek.Sunday); break;
                }
            }

            var freeDates = new List<DateTime>();
            for (var d = start.Date; d <= end.Date; d = d.AddDays(1))
            {
                if (selected.Contains(d.DayOfWeek))
                    freeDates.Add(d);
            }

            return (freeDates.Count, freeDates);
        }

        // Crea HTML con Chart.js (line chart) basato su date e valori (1 se giorno libero, 0 altrimenti)
        private string BuildChartHtml(List<DateTime> labelsDates, List<int> values)
        {
            labelsDates = labelsDates ?? new List<DateTime>();
            values = values ?? new List<int>();

            var labels = string.Join(",", labelsDates.Select(d => $"\"{d:yyyy-MM-dd}\""));
            var dataPoints = string.Join(",", values);

            var sb = new StringBuilder();
            sb.AppendLine("<!doctype html><html><head><meta charset='utf-8'><meta http-equiv='X-UA-Compatible' content='IE=edge'/>");
            sb.AppendLine("<meta name='viewport' content='width=device-width, initial-scale=1'/>");
            sb.AppendLine("<title>Grafico Giorni Liberi</title>");
            sb.AppendLine("<script src='https://cdn.jsdelivr.net/npm/chart.js'></script>");
            sb.AppendLine("</head><body>");
            sb.AppendLine("<canvas id='myChart' width='700' height='160'></canvas>");
            sb.AppendLine("<script>");
            sb.AppendLine("const data = {");
            sb.AppendLine($"  labels: [{labels}],");
            sb.AppendLine("  datasets: [{");
            sb.AppendLine("    label: 'Giorno libero (1 = libero, 0 = non libero)',");
            sb.AppendLine($"    data: [{dataPoints}],");
            sb.AppendLine("    fill: false,");
            sb.AppendLine("    borderColor: 'rgb(75, 192, 192)',");
            sb.AppendLine("    tension: 0.4");
            sb.AppendLine("  }]");
            sb.AppendLine("};");
            sb.AppendLine("const config = {");
            sb.AppendLine("  type: 'line',");
            sb.AppendLine("  data: data,");
            sb.AppendLine("  options: {");
            sb.AppendLine("    responsive: true,");
            sb.AppendLine("    plugins: {");
            sb.AppendLine("      title: { display: true, text: 'Giorni liberi disponibili' }");
            sb.AppendLine("    },");
            sb.AppendLine("    interaction: { intersect: false },");
            sb.AppendLine("    scales: { x: { display: true, title: { display: true } }, y: { display: true, title: { display: true, text: 'Libero' }, suggestedMin: 0, suggestedMax: 1 } }");
            sb.AppendLine("  }");
            sb.AppendLine("};");
            sb.AppendLine("const ctx = document.getElementById('myChart').getContext('2d');");
            sb.AppendLine("new Chart(ctx, config);");
            sb.AppendLine("</script></body></html>");
            return sb.ToString();
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
                             DateFinale = a.DateFinale.ToString("yyyy-MM-dd"),
                             GiorniLiberi = a.GiorniLiberi
                         })
                         .ToList();

            if (dgvAtleti != null)
            {
                dgvAtleti.DataSource = list;
                if (dgvAtleti.Columns != null && dgvAtleti.Columns.Contains("Id"))
                    dgvAtleti.Columns["Id"].Visible = false;
            }

            UpdateActionButtonsState();
        }

        // Aggiungi nuovo atleta (ora include anche la funzionalità di conferma giorni e selezione)
        private void btnAdd_Click(object sender, EventArgs e)
        {
            var nome = txtNome.Text.Trim();
            var cognome = txtCognome.Text.Trim();
            var eta = (int)numEta.Value;
            var frecceIniz = (int)numFrecceIniz.Value;
            var frecceFin = (int)numFrecceFin.Value;
            var dateIniz = dateIniziale.Value.Date;
            var dateFin = dateFinale.Value.Date;

            // obbligatorio almeno un giorno selezionato
            if ((GiorniLiberi?.CheckedItems?.Count ?? 0) == 0)
            {
                MessageBox.Show("Seleziona almeno un giorno della settimana.", "Validazione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var giorniSelezionati = string.Join(',', GiorniLiberi.CheckedItems.Cast<string>());

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

            // Impedisce l'inserimento se la data finale è prima della iniziale
            if (dateIniz > dateFin)
            {
                MessageBox.Show("La data iniziale non può essere successiva alla data finale. Impossibile inserire l'atleta.", "Validazione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                // salva e ottieni id nuovo
                int newId = PalestraRepository.Add(atleta);

                // Calcola i giorni liberi disponibili e crea il grafico
                var selectedNames = GiorniLiberi.CheckedItems.Cast<string>().ToList();
                var (totalFreeDays, freeDates) = CountFreeDays(dateIniz, dateFin, selectedNames);

                // Prepara serie di valori 1/0 per ogni giorno dell'intervallo (grafico)
                var allDates = new List<DateTime>();
                var values = new List<int>();
                for (var d = dateIniz; d <= dateFin; d = d.AddDays(1))
                {
                    allDates.Add(d);
                    values.Add(freeDates.Contains(d) ? 1 : 0);
                }

                var html = BuildChartHtml(allDates, values);
                if (wbChart != null)
                    wbChart.DocumentText = html;

                LoadData();

                // deseleziona tutte le voci nella CheckedListBox (comportamento di conferma spostato qui)
                if (GiorniLiberi != null)
                {
                    for (int i = 0; i < GiorniLiberi.Items.Count; i++)
                        GiorniLiberi.SetItemChecked(i, false);
                }

                // riposiziona la selezione sull'atleta appena aggiunto (se trovato nella griglia)
                if (dgvAtleti != null)
                {
                    foreach (DataGridViewRow r in dgvAtleti.Rows)
                    {
                        var cell = r.Cells["Id"];
                        var cellVal = cell?.Value;
                        if (cellVal != null && int.TryParse(cellVal.ToString(), out int rid) && rid == newId)
                        {
                            r.Selected = true;
                            if (r.Cells.Count > 1)
                                dgvAtleti.CurrentCell = r.Cells[1];
                            break;
                        }
                    }
                }

                ClearInputs(); // la funzione di pulizia ora è invocata dalla aggiunta
                MessageBox.Show($"Atleta aggiunto. Giorni liberi disponibili: {totalFreeDays}", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore salvataggio: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Aggiorna atleta selezionato
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvAtleti?.CurrentRow == null) return;
            if (!int.TryParse(dgvAtleti.CurrentRow.Cells["Id"].Value?.ToString(), out int id))
            {
                MessageBox.Show("Seleziona un atleta valido.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // obbligatorio almeno un giorno selezionato per aggiornare
            if ((GiorniLiberi?.CheckedItems?.Count ?? 0) == 0)
            {
                MessageBox.Show("Seleziona almeno un giorno della settimana.", "Validazione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var nome = txtNome.Text.Trim();
            var cognome = txtCognome.Text.Trim();
            var eta = (int)numEta.Value;
            var frecceIniz = (int)numFrecceIniz.Value;
            var frecceFin = (int)numFrecceFin.Value;
            var dateIniz = dateIniziale.Value.Date;
            var dateFin = dateFinale.Value.Date;
            var giorniSelezionati = string.Join(',', GiorniLiberi.CheckedItems.Cast<string>());

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

        // Elimina atleta selezionato
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

        // Pulisce i campi (chiamata ora anche da btnAdd dopo salvataggio)
        private void ClearInputs()
        {
            txtNome.Clear();
            txtCognome.Clear();
            numEta.Value = numEta.Minimum;
            numFrecceIniz.Value = Math.Max(1, numFrecceIniz.Minimum);
            numFrecceFin.Value = Math.Max(2, numFrecceFin.Minimum);
            dateIniziale.Value = DateTime.Today;
            dateFinale.Value = DateTime.Today;
            // deseleziona tutte le voci nella CheckedListBox
            if (GiorniLiberi != null)
            {
                for (int i = 0; i < GiorniLiberi.Items.Count; i++)
                {
                    GiorniLiberi.SetItemChecked(i, false);
                }
            }
            if (dgvAtleti != null) dgvAtleti.ClearSelection();
            UpdateActionButtonsState();
        }

        // Quando cambia selezione popola i campi
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

            // DateIniziale/DateFinale potrebbero essere stringhe nel DataGridView
            if (DateTime.TryParse(row.Cells["DateIniziale"].Value?.ToString(), out DateTime di))
                dateIniziale.Value = di;
            else
                dateIniziale.Value = DateTime.Today;

            if (DateTime.TryParse(row.Cells["DateFinale"].Value?.ToString(), out DateTime df))
                dateFinale.Value = df;
            else
                dateFinale.Value = DateTime.Today;

            // Legge la colonna GiorniLiberi e aggiorna la CheckedListBox (null-safe)
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
    }
}