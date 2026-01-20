using Microsoft.Data.SqlClient;
using FrontBibliotheque.Models;
using Microsoft.AspNetCore.Http;

namespace FrontBibliotheque.Data
{
    public class HistoriqueAbonnementRepository
    {
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HistoriqueAbonnementRepository(
            IConfiguration config,
            IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("ConnectionString manquante");

            _httpContextAccessor = httpContextAccessor;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // ======================================================
        // AJOUT ABONNEMENT UTILISATEUR
        // ======================================================
        public int AjoutAbonnementUtilisateur(HistoriqueAbonnementModel historique)
        {
            using var conn = GetConnection();
            conn.Open();

            var datePaiement = historique.date_paiement
                ?? throw new ArgumentNullException(nameof(historique.date_paiement));

            // Vérifier s'il existe déjà un abonnement actif
            using var checkCmd = new SqlCommand(@"
                SELECT COUNT(*)
                FROM historiqueabonnement
                WHERE date_paiement <= @date
                  AND date_expiration >= @date
                  AND id_utilisateur = @id_utilisateur
            ", conn);

            checkCmd.Parameters.AddWithValue("@date", datePaiement);
            checkCmd.Parameters.AddWithValue("@id_utilisateur", historique.id_utilisateur.Value);

            int count = (int)checkCmd.ExecuteScalar();

            if (count > 0)
                return 1;

            // Insertion abonnement
            using var insertCmd = new SqlCommand(@"
                INSERT INTO historiqueabonnement
                (date_paiement, id_typeabonnement, id_modepaiement, id_utilisateur, date_expiration)
                VALUES
                (@date_paiement, @id_typeabonnement, @id_modepaiement, @id_utilisateur, @date_expiration)
            ", conn);

            insertCmd.Parameters.AddWithValue("@date_paiement", datePaiement);
            insertCmd.Parameters.AddWithValue("@id_typeabonnement", historique.id_typeabonnement);
            insertCmd.Parameters.AddWithValue("@id_modepaiement", historique.id_modepaiement);
            insertCmd.Parameters.AddWithValue("@id_utilisateur", historique.id_utilisateur);
            insertCmd.Parameters.AddWithValue("@date_expiration", datePaiement.AddMonths(1));

            insertCmd.ExecuteNonQuery();

            return 0;
        }

        // ======================================================
        // LISTE TYPES ABONNEMENT
        // ======================================================
        public List<TypeAbonnementModel> listeTypeAbonnement()
        {
            var list = new List<TypeAbonnementModel>();

            using var conn = GetConnection();
            conn.Open();

            using var cmd = new SqlCommand(
                "SELECT id, type_abonnement FROM typeabonnement ORDER BY id DESC", conn);

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

        // ======================================================
        // LISTE MODES DE PAIEMENT
        // ======================================================
        public List<ModePaiementModel> listeModePaiement()
        {
            var list = new List<ModePaiementModel>();

            using var conn = GetConnection();
            conn.Open();

            using var cmd = new SqlCommand(
                "SELECT id, mode FROM modepaiement ORDER BY id", conn);

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

        // ======================================================
        // LISTE HISTORIQUE ABONNEMENTS UTILISATEUR
        // ======================================================
        public List<V_historiqueAbonnementModel> listeAbonnement(int idUtilisateur)
        {
            var list = new List<V_historiqueAbonnementModel>();

            using var conn = GetConnection();
            conn.Open();

            using var cmd = new SqlCommand(@"
                SELECT *
                FROM v_historiqueabonnement
                WHERE id_utilisateur = @id_utilisateur
                ORDER BY id DESC
            ", conn);

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

        // ======================================================
        // RESULTAT LECTURE
        // ======================================================
        public class LectureResult
        {
            public bool PeutLire { get; set; }
            public string Message { get; set; }
        }

        private LectureResult Autorise(string message)
            => new LectureResult { PeutLire = true, Message = message };

        private LectureResult Refus(string message)
            => new LectureResult { PeutLire = false, Message = message };

        // ======================================================
        // VERIFICATION LECTURE LIVRE
        // ======================================================
        public async Task<LectureResult> PeutLireLivreAsync(int idUtilisateur, int idLivre)
        {
            var today = DateTime.Today;

            using var conn = GetConnection();
            await conn.OpenAsync();

            int idTypeAbonnement;
            DateTime datePaiement;
            DateTime dateExpiration;

            // 1️⃣ Dernier abonnement
            using (var cmd = new SqlCommand(@"
                SELECT TOP 1 id_typeabonnement, date_paiement, date_expiration
                FROM historiqueabonnement
                WHERE id_utilisateur = @id_utilisateur
                ORDER BY date_paiement DESC
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

            // 2️⃣ Vérification dates
            if (today < datePaiement)
                return Refus($"Votre abonnement commence le {datePaiement:dd/MM/yyyy}");

            if (today > dateExpiration)
                return Refus("Votre abonnement est expiré, veuillez renouveler");

            // 3️⃣ Premium
            if (idTypeAbonnement == 1)
                return Autorise("Lecture autorisée (abonnement premium)");

            // 4️⃣ Livre déjà lu
            using (var cmd = new SqlCommand(@"
                SELECT TOP 1 1
                FROM historiquelecture
                WHERE id_utilisateur = @id_utilisateur
                  AND id_livre = @id_livre
            ", conn))
            {
                cmd.Parameters.AddWithValue("@id_utilisateur", idUtilisateur);
                cmd.Parameters.AddWithValue("@id_livre", idLivre);

                if (await cmd.ExecuteScalarAsync() != null)
                    return Autorise("Lecture autorisée (livre déjà lu)");
            }

            // 5️⃣ Quota
            int nbLivresLus;

            using (var cmd = new SqlCommand(@"
                SELECT COUNT(*)
                FROM historiquelecture
                WHERE id_utilisateur = @id_utilisateur
                  AND date_lecture BETWEEN @date_paiement AND @date_expiration
            ", conn))
            {
                cmd.Parameters.AddWithValue("@id_utilisateur", idUtilisateur);
                cmd.Parameters.AddWithValue("@date_paiement", datePaiement);
                cmd.Parameters.AddWithValue("@date_expiration", dateExpiration);

                nbLivresLus = (int)await cmd.ExecuteScalarAsync();
            }

            if (nbLivresLus < 5)
                return Autorise("Lecture autorisée (quota restant)");

            // 6️⃣ Livre acheté
            using (var cmd = new SqlCommand(@"
                SELECT TOP 1 1
                FROM historiquepaiementlivre
                WHERE id_utilisateur = @id_utilisateur
                  AND id_livre = @id_livre
            ", conn))
            {
                cmd.Parameters.AddWithValue("@id_utilisateur", idUtilisateur);
                cmd.Parameters.AddWithValue("@id_livre", idLivre);

                if (await cmd.ExecuteScalarAsync() != null)
                    return Autorise("Lecture autorisée (livre acheté)");
            }

            return Refus("Votre quota est atteint, vous pouvez acheter ce livre");
        }
    }
}
