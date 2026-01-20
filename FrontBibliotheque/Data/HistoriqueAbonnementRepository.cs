using Npgsql;
using FrontBibliotheque.Models;
using Microsoft.AspNetCore.Http;

namespace FrontBibliotheque.Data
{
    public class HistoriqueAbonnementRepository
    {
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HistoriqueAbonnementRepository(IConfiguration config,IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
            _httpContextAccessor = httpContextAccessor;
        }

        private NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public int AjoutAbonnementUtilisateur(HistoriqueAbonnementModel historique)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var checkCmd = new NpgsqlCommand(
                @"SELECT COUNT(*) 
                FROM historiqueabonnement 
                WHERE date_paiement <= @date_debut 
                    AND date_expiration >= @date_fin
                    AND id_utilisateur = @id_utilisateur", conn);

            var datePaiement = historique.date_paiement
                ?? throw new ArgumentNullException(nameof(historique.date_paiement));

            checkCmd.Parameters.AddWithValue("@date_debut", datePaiement);
            checkCmd.Parameters.AddWithValue("@date_fin", datePaiement);
            checkCmd.Parameters.AddWithValue("@id_utilisateur", historique.id_utilisateur.Value);

            var count = (long)checkCmd.ExecuteScalar();

            if (count > 0)
            {
                Console.WriteLine("Abonnement déjà existant");
                return 1;
            }

            using var insertCmd = new NpgsqlCommand(
                @"INSERT INTO historiqueabonnement
                (date_paiement, id_typeabonnement, id_modepaiement, id_utilisateur, date_expiration)
                VALUES
                (@date_paiement, @id_typeabonnement, @id_modepaiement, @id_utilisateur, @date_expiration)", conn);

            var dateExpiration = datePaiement.AddMonths(1);

            insertCmd.Parameters.AddWithValue("@date_paiement", datePaiement);
            insertCmd.Parameters.AddWithValue("@id_typeabonnement", historique.id_typeabonnement);
            insertCmd.Parameters.AddWithValue("@id_modepaiement", historique.id_modepaiement);
            insertCmd.Parameters.AddWithValue("@id_utilisateur", historique.id_utilisateur);
            insertCmd.Parameters.AddWithValue("@date_expiration", dateExpiration);

            insertCmd.ExecuteNonQuery();

            return 0;
        }

        public List<TypeAbonnementModel> listeTypeAbonnement()
        {
            var list = new List<TypeAbonnementModel>();

            using var conn = GetConnection();
            conn.Open();

            var cmd = new NpgsqlCommand("SELECT * FROM typeabonnement ORDER BY id desc", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new TypeAbonnementModel
                {
                    id = reader.GetInt32(0),
                    type_abonnement = reader.GetString(1)
                });
            }
            return list;
        }

        public List<ModePaiementModel> listeModePaiement()
        {
            var list = new List<ModePaiementModel>();

            using var conn = GetConnection();
            conn.Open();

            var cmd = new NpgsqlCommand("SELECT * FROM modepaiement ORDER BY id", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new ModePaiementModel
                {
                    id = reader.GetInt32(0),
                    mode = reader.GetString(1)
                });
            }
            return list;
        }

        public List<V_historiqueAbonnementModel> listeAbonnement(int idUtilisateur)
        {
            var list = new List<V_historiqueAbonnementModel>();

            using var conn = GetConnection();
            conn.Open();

            using var cmd = new NpgsqlCommand(
                @"SELECT *
                FROM v_historiqueabonnement
                WHERE id_utilisateur = @id_utilisateur
                ORDER BY id desc", conn);

            cmd.Parameters.AddWithValue("@id_utilisateur", idUtilisateur);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new V_historiqueAbonnementModel
                {
                    id = reader.GetInt32(0),
                    date_paiement = reader.GetDateTime(1),
                    id_typeabonnement = reader.GetInt32(2),
                    id_modepaiement = reader.GetInt32(3),
                    id_utilisateur = reader.GetInt32(4),
                    date_expiration = reader.GetDateTime(5),
                    type_abonnement = reader.GetString(6),
                    mode = reader.GetString(7)
                });
            }

            return list;
        }


        public class LectureResult
        {
            public bool PeutLire { get; set; }
            public string Message { get; set; }
        }

        private LectureResult Autorise(string message)
        {
            return new LectureResult
            {
                PeutLire = true,
                Message = message
            };
        }

        private LectureResult Refus(string message)
        {
            return new LectureResult
            {
                PeutLire = false,
                Message = message
            };
        }


     public async Task<LectureResult> PeutLireLivreAsync(int idUtilisateur, int idLivre)
{
    var today = DateTime.Today;

    using var conn = GetConnection();
    await conn.OpenAsync();

    // ===============================
    // 1️⃣ Dernier abonnement de l’utilisateur
    // ===============================
    int idTypeAbonnement;
    DateTime datePaiement;
    DateTime dateExpiration;

    using (var cmd = new NpgsqlCommand(@"
        SELECT id_typeabonnement, date_paiement, date_expiration
        FROM historiqueabonnement
        WHERE id_utilisateur = @id_utilisateur
        ORDER BY date_paiement DESC
        LIMIT 1
    ", conn))
    {
        cmd.Parameters.AddWithValue("@id_utilisateur", idUtilisateur);

        using var reader = await cmd.ExecuteReaderAsync();

        if (!reader.Read())
            return Refus("Vous devez vous abonner pour lire ce livre");

        idTypeAbonnement = reader.GetInt32(0);
        datePaiement = reader.GetDateTime(1);
        dateExpiration = reader.GetDateTime(2);
    }

    // ===============================
    // 2️⃣ Vérification dates abonnement
    // ===============================

    // Abonnement futur
    if (today < datePaiement)
    {
        return Refus(
            $"Votre abonnement commence le {datePaiement:dd/MM/yyyy}"
        );
    }

    // Abonnement expiré
    if (today > dateExpiration)
    {
        return Refus(
            "Votre abonnement est expiré, veuillez renouveler"
        );
    }

    // ===============================
    // 3️⃣ Abonnement premium
    // ===============================
    if (idTypeAbonnement == 1)
        return Autorise("Lecture autorisée (abonnement premium)");

    // ===============================
    // 4️⃣ Déjà lu ce livre ?
    // ===============================
    using (var cmd = new NpgsqlCommand(@"
        SELECT 1
        FROM historiquelecture
        WHERE id_utilisateur = @id_utilisateur
          AND id_livre = @id_livre
        LIMIT 1
    ", conn))
    {
        cmd.Parameters.AddWithValue("@id_utilisateur", idUtilisateur);
        cmd.Parameters.AddWithValue("@id_livre", idLivre);

        var result = await cmd.ExecuteScalarAsync();
        if (result != null)
            return Autorise("Lecture autorisée (livre déjà lu)");
    }

    // ===============================
    // 5️⃣ Compter livres lus pendant l’abonnement actif
    // ===============================
    int nbLivresLus;

    using (var cmd = new NpgsqlCommand(@"
        SELECT COUNT(*)
        FROM historiquelecture
        WHERE id_utilisateur = @id_utilisateur
          AND date_lecture BETWEEN @date_paiement AND @date_expiration
    ", conn))
    {
        cmd.Parameters.AddWithValue("@id_utilisateur", idUtilisateur);
        cmd.Parameters.AddWithValue("@date_paiement", datePaiement);
        cmd.Parameters.AddWithValue("@date_expiration", dateExpiration);

        nbLivresLus = Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }

    if (nbLivresLus < 5)
        return Autorise("Lecture autorisée (quota restant)");

    // ===============================
    // 6️⃣ Livre acheté ?
    // ===============================
    using (var cmd = new NpgsqlCommand(@"
        SELECT 1
        FROM historiquepaiementlivre
        WHERE id_utilisateur = @id_utilisateur
          AND id_livre = @id_livre
        LIMIT 1
    ", conn))
    {
        cmd.Parameters.AddWithValue("@id_utilisateur", idUtilisateur);
        cmd.Parameters.AddWithValue("@id_livre", idLivre);

        var result = await cmd.ExecuteScalarAsync();
        if (result != null)
            return Autorise("Lecture autorisée (livre acheté)");
    }

    // ===============================
    // ❌ Refus final
    // ===============================
    return Refus("Votre quota est atteint, vous pouvez acheter ce livre");
}




    }


    
}
