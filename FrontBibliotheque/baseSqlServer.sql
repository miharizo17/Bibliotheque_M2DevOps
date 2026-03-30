USE bibliotheque;
GO

-- Table typeLivre
CREATE TABLE typeLivre (
    id INT IDENTITY(1,1) PRIMARY KEY,
    type_livre VARCHAR(255)
);
INSERT INTO typeLivre(type_livre) VALUES ('Roman');
INSERT INTO typeLivre(type_livre) VALUES ('Action');
INSERT INTO typeLivre(type_livre) VALUES ('Fiction');

-- Table typeAbonnement
CREATE TABLE typeAbonnement (
    id INT IDENTITY(1,1) PRIMARY KEY,
    type_abonnement VARCHAR(255)
);

-- Table modePaiement
CREATE TABLE modePaiement (
    id INT IDENTITY(1,1) PRIMARY KEY,
    mode VARCHAR(255)
);

-- Table livre
CREATE TABLE livre (
    id INT IDENTITY(1,1) PRIMARY KEY,
    id_typelivre INT REFERENCES typeLivre(id),
    titre VARCHAR(255),
    sous_titre VARCHAR(255),
    saison VARCHAR(100),
    auteur VARCHAR(255),
    date_edition DATE,
    description NVARCHAR(MAX),
    image NVARCHAR(MAX),
    document NVARCHAR(MAX),
    etat INT DEFAULT 0
);

-- Table utilisateur
CREATE TABLE utilisateur (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nom VARCHAR(255),
    prenom VARCHAR(255),
    telephone VARCHAR(255),
    mail VARCHAR(255),
    mdp VARCHAR(255),
    etat INT DEFAULT 0,
    dateentree DATE
);
INSERT INTO utilisateur(nom, prenom, telephone, mail, mdp)
VALUES ('Rakoto', 'Zo', '03345678923', 'zo@gmail.com', 'zo');

-- Table historiqueabonnement
CREATE TABLE historiqueabonnement (
    id INT IDENTITY(1,1) PRIMARY KEY,
    date_paiement DATE,
    id_typeabonnement INT REFERENCES typeAbonnement(id),
    id_modepaiement INT REFERENCES modePaiement(id),
    id_utilisateur INT REFERENCES utilisateur(id),
    date_expiration DATE
);

Go
create or replace view v_historiqueabonnement as 
select historiqueabonnement.*,
modePaiement.mode,
typeAbonnement.type_abonnement
from historiqueabonnement
join typeAbonnement on historiqueabonnement.id_typeabonnement=typeAbonnement.id
join modePaiement on historiqueabonnement.id_modepaiement=modePaiement.id;
Go

-- Table historiquelecture
CREATE TABLE historiquelecture (
    id INT IDENTITY(1,1) PRIMARY KEY,
    date_lecture DATE,
    id_livre INT REFERENCES livre(id),
    id_utilisateur INT REFERENCES utilisateur(id)
);

-- Table historiquepaiementlivre
CREATE TABLE historiquepaiementlivre (
    id INT IDENTITY(1,1) PRIMARY KEY,
    date_lecture DATE,
    id_livre INT REFERENCES livre(id),
    id_utilisateur INT REFERENCES utilisateur(id),
    prix FLOAT
);
GO

-- Vue v_livre (isolée dans son propre batch)
CREATE VIEW v_livre AS
SELECT livre.*,
       typelivre.type_livre
FROM livre
JOIN typelivre ON typelivre.id = livre.id_typelivre;
GO

-- Insertion des livres
INSERT INTO livre (id_typelivre, titre, sous_titre, saison, auteur, date_edition, description, image, document, etat)
VALUES
(1, 'Le Chant du Vent', 'Voyage interieur', 'Printemps', 'Jean Rakoto', '2018-03-15',
 'Un roman poetique sur la quete de soi et la nature.', 'chantduvent.jpg', 'chantduvent.pdf', 1),

(2, 'Les Ombres du Temps', 'Chroniques anciennes', 'Hiver', 'Marie Dupont', '2020-11-02',
 'Un thriller historique melant passe et present.', 'ombresdutemps.jpg', 'ombresdutemps.pdf', 1),

(3, 'L Art du Code', 'Programmation moderne', 'Toute l annee', 'Paul Martin', '2022-06-10',
 'Un guide pratique pour apprendre la programmation propre et efficace.', 'artducode.jpg', 'artducode.pdf', 1),

(1, 'Contes de Madagascar', 'Traditions et legendes', 'ete', 'Rabe Andrianina', '2015-08-25',
 'Recueil de contes traditionnels malgaches.', 'contesmada.jpg', 'contesmada.pdf', 1),

(2, 'Science et Futur', 'Innovations de demain', 'Automne', 'Claire Bernard', '2021-09-18',
 'Exploration des avancees scientifiques et technologiques.', 'sciencfutur.jpg', 'sciencefutur.pdf', 1);