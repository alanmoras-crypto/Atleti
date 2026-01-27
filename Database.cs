using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.Sqlite;

namespace Atleti
{
    // Modello Atleta con validazioni di base
    public class Atleta
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false)]
        [StringLength(100)]
        public string Cognome { get; set; } = string.Empty;

        [Range(1, 120)]
        public int Eta { get; set; }

        // Nuovi campi per le frecce
        [Range(1, int.MaxValue)]
        public int FrecceIniz { get; set; } = 1;

        [Range(2, int.MaxValue)]
        public int FrecceFin { get; set; } = 2;

        // Nuovi campi per le date iniziale e finale
        public DateTime DateIniziale { get; set; } = DateTime.Today;
        public DateTime DateFinale { get; set; } = DateTime.Today;

        // Giorni liberi (memorizzati come CSV, es: "Lunedi,Mercoledi")
        public string GiorniLiberi { get; set; } = string.Empty;
    }

    // Repository ADO.NET per SQLite (senza EF Core)
    public static class PalestraRepository
    {
        private const string ConnectionString = "Data Source=palestra.db";

        public static void EnsureDatabaseCreated()
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                @"CREATE TABLE IF NOT EXISTS Atleti (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Nome TEXT NOT NULL,
                    Cognome TEXT NOT NULL,
                    Eta INTEGER NOT NULL,
                    FrecceIniz INTEGER NOT NULL DEFAULT 1,
                    FrecceFin INTEGER NOT NULL DEFAULT 2,
                    DateIniziale TEXT NOT NULL DEFAULT '',
                    DateFinale TEXT NOT NULL DEFAULT '',
                    GiorniLiberi TEXT NOT NULL DEFAULT ''
                  );";
            cmd.ExecuteNonQuery();

            // Se la tabella esisteva senza le nuove colonne, aggiungile
            var existing = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using var pragma = conn.CreateCommand();
            pragma.CommandText = "PRAGMA table_info('Atleti');";
            using var reader = pragma.ExecuteReader();
            while (reader.Read())
            {
                existing.Add(reader.GetString(1));
            }

            if (!existing.Contains("FrecceIniz"))
            {
                using var alter = conn.CreateCommand();
                alter.CommandText = "ALTER TABLE Atleti ADD COLUMN FrecceIniz INTEGER NOT NULL DEFAULT 1;";
                alter.ExecuteNonQuery();
            }

            if (!existing.Contains("FrecceFin"))
            {
                using var alter2 = conn.CreateCommand();
                alter2.CommandText = "ALTER TABLE Atleti ADD COLUMN FrecceFin INTEGER NOT NULL DEFAULT 2;";
                alter2.ExecuteNonQuery();
            }

            if (!existing.Contains("DateIniziale"))
            {
                using var alter3 = conn.CreateCommand();
                alter3.CommandText = "ALTER TABLE Atleti ADD COLUMN DateIniziale TEXT NOT NULL DEFAULT '';";

                alter3.ExecuteNonQuery();
            }

            if (!existing.Contains("DateFinale"))
            {
                using var alter4 = conn.CreateCommand();
                alter4.CommandText = "ALTER TABLE Atleti ADD COLUMN DateFinale TEXT NOT NULL DEFAULT '';";

                alter4.ExecuteNonQuery();
            }

            if (!existing.Contains("GiorniLiberi"))
            {
                using var alter5 = conn.CreateCommand();
                alter5.CommandText = "ALTER TABLE Atleti ADD COLUMN GiorniLiberi TEXT NOT NULL DEFAULT '';";

                alter5.ExecuteNonQuery();
            }
        }

        public static List<Atleta> GetAll()
        {
            var list = new List<Atleta>();
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Nome, Cognome, Eta, FrecceIniz, FrecceFin, DateIniziale, DateFinale, GiorniLiberi FROM Atleti ORDER BY Id DESC;";
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                var dateInizStr = r.IsDBNull(6) ? string.Empty : r.GetString(6);
                var dateFinStr = r.IsDBNull(7) ? string.Empty : r.GetString(7);
                var giorniLiberiStr = r.IsDBNull(8) ? string.Empty : r.GetString(8);

                DateTime dateIniz = DateTime.Today;
                DateTime dateFin = DateTime.Today;
                if (!string.IsNullOrEmpty(dateInizStr))
                    DateTime.TryParse(dateInizStr, out dateIniz);
                if (!string.IsNullOrEmpty(dateFinStr))
                    DateTime.TryParse(dateFinStr, out dateFin);

                list.Add(new Atleta
                {
                    Id = r.GetInt32(0),
                    Nome = r.GetString(1),
                    Cognome = r.GetString(2),
                    Eta = r.GetInt32(3),
                    FrecceIniz = r.IsDBNull(4) ? 1 : r.GetInt32(4),
                    FrecceFin = r.IsDBNull(5) ? 2 : r.GetInt32(5),
                    DateIniziale = dateIniz,
                    DateFinale = dateFin,
                    GiorniLiberi = giorniLiberiStr
                });
            }
            return list;
        }

        // Restituisce l'id dell'inserimento appena fatto
        public static int Add(Atleta a)
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Atleti (Nome, Cognome, Eta, FrecceIniz, FrecceFin, DateIniziale, DateFinale, GiorniLiberi) VALUES (@n, @c, @e, @fi, @ff, @di, @df, @gl);";
            cmd.Parameters.AddWithValue("@n", a.Nome);
            cmd.Parameters.AddWithValue("@c", a.Cognome);
            cmd.Parameters.AddWithValue("@e", a.Eta);
            cmd.Parameters.AddWithValue("@fi", a.FrecceIniz);
            cmd.Parameters.AddWithValue("@ff", a.FrecceFin);
            cmd.Parameters.AddWithValue("@di", a.DateIniziale.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@df", a.DateFinale.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@gl", a.GiorniLiberi ?? string.Empty);
            cmd.ExecuteNonQuery();

            // Recupera l'ultimo id inserito nella connessione corrente
            cmd.CommandText = "SELECT last_insert_rowid();";
            var result = cmd.ExecuteScalar();
            if (result is long l) return (int)l;
            if (result is int i) return i;
            return -1;
        }

        public static Atleta? Find(int id)
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Nome, Cognome, Eta, FrecceIniz, FrecceFin, DateIniziale, DateFinale, GiorniLiberi FROM Atleti WHERE Id = @id;";
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                var dateInizStr = r.IsDBNull(6) ? string.Empty : r.GetString(6);
                var dateFinStr = r.IsDBNull(7) ? string.Empty : r.GetString(7);
                var giorniLiberiStr = r.IsDBNull(8) ? string.Empty : r.GetString(8);

                DateTime dateIniz = DateTime.Today;
                DateTime dateFin = DateTime.Today;
                if (!string.IsNullOrEmpty(dateInizStr))
                    DateTime.TryParse(dateInizStr, out dateIniz);
                if (!string.IsNullOrEmpty(dateFinStr))
                    DateTime.TryParse(dateFinStr, out dateFin);

                return new Atleta
                {
                    Id = r.GetInt32(0),
                    Nome = r.GetString(1),
                    Cognome = r.GetString(2),
                    Eta = r.GetInt32(3),
                    FrecceIniz = r.IsDBNull(4) ? 1 : r.GetInt32(4),
                    FrecceFin = r.IsDBNull(5) ? 2 : r.GetInt32(5),
                    DateIniziale = dateIniz,
                    DateFinale = dateFin,
                    GiorniLiberi = giorniLiberiStr
                };
            }
            return null;
        }

        public static void Update(Atleta a)
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Atleti SET Nome=@n, Cognome=@c, Eta=@e, FrecceIniz=@fi, FrecceFin=@ff, DateIniziale=@di, DateFinale=@df, GiorniLiberi=@gl WHERE Id=@id;";
            cmd.Parameters.AddWithValue("@n", a.Nome);
            cmd.Parameters.AddWithValue("@c", a.Cognome);
            cmd.Parameters.AddWithValue("@e", a.Eta);
            cmd.Parameters.AddWithValue("@fi", a.FrecceIniz);
            cmd.Parameters.AddWithValue("@ff", a.FrecceFin);
            cmd.Parameters.AddWithValue("@di", a.DateIniziale.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@df", a.DateFinale.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@gl", a.GiorniLiberi ?? string.Empty);
            cmd.Parameters.AddWithValue("@id", a.Id);
            cmd.ExecuteNonQuery();
        }

        public static void Delete(int id)
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Atleti WHERE Id = @id;";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}