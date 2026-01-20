using Microsoft.Data.SqlClient;
using FrontBibliotheque.Models;
using Microsoft.AspNetCore.Http;

namespace FrontBibliotheque.Data
{
    public class UtilisateurRepository
    {
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UtilisateurRepository(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string manquante");
            _httpContextAccessor = httpContextAccessor;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // ----------------------------
        // READ - Tous les utilisateurs
        // ----------------------------
        public List<UtilisateurModel> GetAll()
        {
            var list = new List<UtilisateurModel>();

            using var conn = GetConnection();
            conn.Open();

            using var cmd = new SqlCommand("SELECT * FROM utilisateur ORDER BY id", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new UtilisateurModel
                {
                    id = reader.GetInt32(0),
                    nom = reader.GetString(1),
                    prenom = reader.GetString(2),
                    telephone = reader.GetString(3),
                    mail = reader.GetString(4),
                    mdp = reader.GetString(5),
                    etat = reader.GetInt32(6),
                    dateentree = reader.GetDateTime(7)
                });
            }

            return list;
        }

        // ----------------------------
        // CREATE
        // ----------------------------
        public void Add(UtilisateurModel u)
        {
            using var conn = GetConnection();
            conn.Open();

            using var cmd = new SqlCommand(@"
                INSERT INTO utilisateur
                (nom, prenom, telephone, mail, mdp, etat, dateentree)
                VALUES (@nom, @prenom, @tel, @mail, @mdp, @etat, @dateentree)", conn);

            cmd.Parameters.AddWithValue("@nom", u.nom);
            cmd.Parameters.AddWithValue("@prenom", u.prenom);
            cmd.Parameters.AddWithValue("@tel", u.telephone);
            cmd.Parameters.AddWithValue("@mail", u.mail);
            cmd.Parameters.AddWithValue("@mdp", u.mdp);
            cmd.Parameters.AddWithValue("@etat", u.etat);
            cmd.Parameters.AddWithValue("@dateentree", DateTime.Now);

            cmd.ExecuteNonQuery();
        }

        // ----------------------------
        // UPDATE
        // ----------------------------
        public void Update(UtilisateurModel u)
        {
            using var conn = GetConnection();
            conn.Open();

            using var cmd = new SqlCommand(@"
                UPDATE utilisateur SET
                    nom = @nom,
                    prenom = @prenom,
                    telephone = @tel,
                    mail = @mail,
                    mdp = @mdp,
                    etat = @etat,
                    dateentree = @dateentree
                WHERE id = @id", conn);

            cmd.Parameters.AddWithValue("@id", u.id);
            cmd.Parameters.AddWithValue("@nom", u.nom);
            cmd.Parameters.AddWithValue("@prenom", u.prenom);
            cmd.Parameters.AddWithValue("@tel", u.telephone);
            cmd.Parameters.AddWithValue("@mail", u.mail);
            cmd.Parameters.AddWithValue("@mdp", u.mdp);
            cmd.Parameters.AddWithValue("@etat", u.etat);
            cmd.Parameters.AddWithValue("@dateentree", u.dateentree);

            cmd.ExecuteNonQuery();
        }

        // ----------------------------
        // DELETE
        // ----------------------------
        public void Delete(int id)
        {
            using var conn = GetConnection();
            conn.Open();

            using var cmd = new SqlCommand("DELETE FROM utilisateur WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
        }

        // ----------------------------
        // READ BY ID
        // ----------------------------
        public UtilisateurModel? GetById(int id)
        {
            using var conn = GetConnection();
            conn.Open();

            using var cmd = new SqlCommand("SELECT * FROM utilisateur WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new UtilisateurModel
                {
                    id = reader.GetInt32(0),
                    nom = reader.GetString(1),
                    prenom = reader.GetString(2),
                    telephone = reader.GetString(3),
                    mail = reader.GetString(4),
                    mdp = reader.GetString(5),
                    etat = reader.GetInt32(6)
                };
            }

            return null;
        }

        // ----------------------------
        // LOGIN
        // ----------------------------
        public UtilisateurModel? Login(string mail, string mdp)
        {
            using var conn = GetConnection();
            conn.Open();

            using var cmd = new SqlCommand(@"
                SELECT id, nom, mail
                FROM utilisateur
                WHERE mail = @mail AND mdp = @mdp", conn);

            cmd.Parameters.AddWithValue("@mail", mail);
            cmd.Parameters.AddWithValue("@mdp", mdp);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new UtilisateurModel
                {
                    id = reader.GetInt32(0),
                    nom = reader.GetString(1),
                    mail = reader.GetString(2)
                };
            }

            return null;
        }
    }
}
