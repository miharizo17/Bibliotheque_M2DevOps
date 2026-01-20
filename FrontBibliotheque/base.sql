create table typeLivre(
    id serial primary key,
    type_livre varchar(255)
);
insert into typeLivre(type_livre) values('Roman');
insert into typeLivre(type_livre) values('Action');
insert into typeLivre(type_livre) values('Fiction');

create table typeAbonnement(
    id serial primary key,
    type_abonnement varchar(255)
);

create table modePaiement(
    id serial primary key,
    mode varchar(255)
);

create table livre(
    id serial primary key,
    id_typelivre int  references typeLivre(id),
    titre varchar(255),
    sous_titre varchar(255),
    saison varchar(100),
    auteur varchar(255),
    date_edition date,
    description text,
    image text,
    document text,
    etat int default 0
);

create table utilisateur(
    id serial primary key,
    nom varchar(255),
    prenom  varchar(255),
    telephone varchar(255),
    mail varchar(255),
    mdp varchar(255),
    etat int default 0,
    dateentree date
);
insert into utilisateur(nom,prenom,telephone,mail,mdp) values ('Rakoto', 'Zo', '03345678923','zo@gmail.com','zo');

create table historiqueabonnement(
    id serial primary key,
    date_paiement date,
    id_typeabonnement int  references typeAbonnement(id),
    id_modepaiement int  references modePaiement(id),
    id_utilisateur int  references utilisateur(id),
    date_expiration date
);

create table historiquelecture(
    id serial primary key,
    date_lecture date,
    id_livre int  references livre(id),
    id_utilisateur  int references utilisateur(id)
);

create table historiquepaiementlivre(
    id serial primary key,
    date_lecture date,
    id_livre int  references livre(id),
    id_utilisateur  int references utilisateur(id),
    prix double precision
);


create or replace view v_livre as 
select livre.*,
typelivre.type_livre 
from livre 
join typelivre on typelivre.id = livre.id_typelivre ;


INSERT INTO livre (
    id_typelivre,
    titre,
    sous_titre,
    saison,
    auteur,
    date_edition,
    description,
    image,
    document,
    etat
) VALUES
(1, 'Le Chant du Vent', 'Voyage interieur', 'Printemps', 'Jean Rakoto', '2018-03-15',
 'Un roman poetique sur la quete de soi et la nature.',
 'chantduvent.jpg', 'chantduvent.pdf', 1),

(2, 'Les Ombres du Temps', 'Chroniques anciennes', 'Hiver', 'Marie Dupont', '2020-11-02',
 'Un thriller historique melant passe et present.',
 'ombresdutemps.jpg', 'ombresdutemps.pdf', 1),

(3, 'L Art du Code', 'Programmation moderne', 'Toute l annee', 'Paul Martin', '2022-06-10',
 'Un guide pratique pour apprendre la programmation propre et efficace.',
 'artducode.jpg', 'artducode.pdf', 1),

(1, 'Contes de Madagascar', 'Traditions et legendes', 'ete', 'Rabe Andrianina', '2015-08-25',
 'Recueil de contes traditionnels malgaches.',
 'contesmada.jpg', 'contesmada.pdf', 1),

(2, 'Science et Futur', 'Innovations de demain', 'Automne', 'Claire Bernard', '2021-09-18',
 'Exploration des avancees scientifiques et technologiques.',
 'sciencfutur.jpg', 'sciencefutur.pdf', 1);
