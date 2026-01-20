using Microsoft.Data.SqlClient; // <-- important pour SQL Server
using FrontBibliotheque.Models;

namespace FrontBibliotheque.Data
{
    public class V_livreRepository
    {
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public V_livreRepository(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string manquante");
            _httpContextAccessor = httpContextAccessor;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // Récupérer tous les livres avec filtres facultatifs
        public List<V_livreModel> GetAll(int? idtypelivre = null, string autre = null)
        {
            var list = new List<V_livreModel>();

            using var conn = GetConnection();
            conn.Open();

            string sql = "SELECT * FROM v_livre WHERE 1=1"; 
            using var cmd = new SqlCommand();
            cmd.Connection = conn;

            if (idtypelivre.HasValue)
            {
                sql += " AND id_typelivre = @idtypelivre";
                cmd.Parameters.AddWithValue("@idtypelivre", idtypelivre.Value);
            }

            if (!string.IsNullOrEmpty(autre))
            {
                // SQL Server LIKE est sensible à la casse selon la collation
                sql += " AND autre LIKE @autre";
                cmd.Parameters.AddWithValue("@autre", $"%{autre}%");
            }

            sql += " ORDER BY id";
            cmd.CommandText = sql;

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new V_livreModel
                {
                    id = reader.GetInt32(0),
                    id_typelivre = reader.GetInt32(1),
                    titre = reader.GetString(2),
                    sous_titre = reader.GetString(3),
                    saison = reader.GetString(4),
                    auteur = reader.GetString(5),
                    date_edition = reader.GetDateTime(6),
                    description = reader.GetString(7),
                    image = reader.GetString(8),
                    document = reader.GetString(9),
                    etat = reader.GetInt32(10),
                    type_livre = reader.GetString(11)
                });
            }

            return list;
        }

        // Liste des types de livre
        public List<TypeLivreModel> listeTypeLivre()
        {
            var list = new List<TypeLivreModel>();

            using var conn = GetConnection();
            conn.Open();

            using var cmd = new SqlCommand("SELECT * FROM typeLivre ORDER BY id", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new TypeLivreModel
                {
                    id = reader.GetInt32(0),
                    type_livre = reader.GetString(1)
                });
            }

            return list;
        }
    }
}
