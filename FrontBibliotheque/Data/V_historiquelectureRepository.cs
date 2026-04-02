using Microsoft.Data.SqlClient;
using FrontBibliotheque.Models;

namespace FrontBibliotheque.Data
{
    public class V_historiquelectureRepository
    {
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public V_historiquelectureRepository(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string manquante");
            _httpContextAccessor = httpContextAccessor;
        }

        private SqlConnection GetConnection() => new SqlConnection(_connectionString);

        // liste livre deja lu avec filtre
        public List<V_historiquelecture> GetByUtilisateur(
            int idUtilisateur,
            string? titre = null,
            DateTime? dateDebut = null,
            DateTime? dateFin = null)
        {
            var list = new List<V_historiquelecture>();

            using var conn = GetConnection();
            conn.Open();

            string sql = "SELECT * FROM v_historiquelecture WHERE id_utilisateur = @id_utilisateur";
            using var cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.Parameters.AddWithValue("@id_utilisateur", idUtilisateur);

            if (!string.IsNullOrEmpty(titre))
            {
                sql += " AND (titre LIKE @titre OR auteur LIKE @titre)";
                cmd.Parameters.AddWithValue("@titre", $"%{titre}%");
            }

            if (dateDebut.HasValue)
            {
                sql += " AND date_lecture >= @dateDebut";
                cmd.Parameters.AddWithValue("@dateDebut", dateDebut.Value);
            }

            if (dateFin.HasValue)
            {
                sql += " AND date_lecture <= @dateFin";
                cmd.Parameters.AddWithValue("@dateFin", dateFin.Value);
            }

            sql += " ORDER BY id DESC";
            cmd.CommandText = sql;

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new V_historiquelecture
                {
                    id              = reader.GetInt32(0),
                    date_lecture    = reader.IsDBNull(1) ? null : reader.GetDateTime(1),
                    id_livre        = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                    id_utilisateur  = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                    titre           = reader.IsDBNull(4) ? null : reader.GetString(4),
                    auteur          = reader.IsDBNull(5) ? null : reader.GetString(5),
                    image           = reader.IsDBNull(6) ? null : reader.GetString(6),
                    document        = reader.IsDBNull(7) ? null : reader.GetString(7),
                });
            }

            return list;
        }
    }
}
