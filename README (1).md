# 📦 Générateur de Données Synthétiques — Architecture & Documentation

---

## Table des matières

1. [Description de l'architecture](#1-description-de-larchitecture)
2. [Justification des choix de conception](#2-justification-des-choix-de-conception)
3. [Principes SOLID respectés](#3-principes-solid-respectés)

---

## 1. Description de l'architecture

Ce projet implémente un **système de génération de données synthétiques structurées**, exportables dans différents formats. L'architecture est organisée autour de plusieurs domaines fonctionnels indépendants et extensibles.

### Vue d'ensemble des composants

```
ProjectManager
    └── Project
            └── Entity (composite récursif)
                    └── Attribute
                            ├── DataType       (StringDataType | FloatDataType)
                            ├── Constraint     (LengthConstraint | RangeConstraint | StatConstraint)
                            └── DataGenerator  (RandomTextGenerator | RandomNumberGenerator | ApiDataGenerator)
                                                        └── ApiCaller  (FetchNameApi | FetchNumberApi)
ProjectManager
    └── Exporter   (CSVExporter | JSONExporter | XMLExporter | SQLExporter)
```

---

### 1.1 Gestion de projets (`ProjectManager`, `Project`)

| Classe | Rôle |
|---|---|
| `ProjectManager` | Point d'entrée principal. Gère une liste de `Project` et délègue l'export à un `Exporter`. |
| `Project` | Contient une liste d'`Entity` et expose la méthode `generateEntities(Integer)` pour produire des jeux de données. |

`ProjectManager` joue le rôle de **façade** : il simplifie l'interaction avec le système en centralisant `addProject`, `removeProject` et `export`.

---

### 1.2 Modélisation des données (`Entity`, `Attribute`)

| Classe | Rôle |
|---|---|
| `Entity` | Représente un objet métier. Peut contenir des **sous-entités** (composition récursive) et une liste d'`Attribute`. |
| `Attribute` | Porte un nom, un type (`DataType`), une valeur (`Object`) et une liste de contraintes (`Constraint`). Génère sa propre valeur via `generateValue(DataGenerator)`. |

La relation `Entity → subentities: List<Entity>` permet de modéliser des structures hiérarchiques imbriquées (ex : une commande contenant des lignes de commande).

---

### 1.3 Types de données (`DataType`)

L'interface `DataType` expose `getDataType(): String`. Elle est implémentée par :

- `StringDataType` — pour les attributs textuels
- `FloatDataType` — pour les attributs numériques flottants

Ce mécanisme permet d'ajouter facilement de nouveaux types (`BooleanDataType`, `DateDataType`, etc.) sans modifier le reste du système.

---

### 1.4 Contraintes de validation (`Constraint`)

L'interface `Constraint` expose `verify(Object, Class): boolean`. Trois implémentations existent :

| Implémentation | Rôle |
|---|---|
| `LengthConstraint` | Vérifie que la longueur d'une chaîne est comprise entre `minLength` et `maxLength`. |
| `RangeConstraint` | Vérifie qu'une valeur numérique est dans `[minValue, maxValue]`. |
| `StatConstraint` | Vérifie une contrainte statistique (valeur moyenne cible : `averageValue`). |

Un `Attribute` peut cumuler plusieurs contraintes, toutes évaluées indépendamment.

---

### 1.5 Génération de données (`DataGenerator`)

L'interface `DataGenerator` expose `generateData(List<Constraint>): Object`. Trois implémentations :

| Implémentation | Rôle |
|---|---|
| `RandomTextGenerator` | Génère du texte aléatoire respectant les contraintes fournies. |
| `RandomNumberGenerator` | Génère un nombre aléatoire dans les bornes définies. |
| `ApiDataGenerator` | Délègue la génération à une API externe via un `ApiCaller`. |

`ApiDataGenerator` suit le **pattern Stratégie** en acceptant n'importe quelle implémentation de `ApiCaller` :

- `FetchNameApi` — appelle une API de génération de noms
- `FetchNumberApi` — appelle une API de génération de nombres

---

### 1.6 Export (`Exporter`)

L'interface `Exporter` expose `exportDataset(Project): String`. Quatre formats sont supportés :

| Implémentation | Format produit |
|---|---|
| `CSVExporter` | Fichier CSV (séparateur virgule) |
| `JSONExporter` | Fichier JSON |
| `XMLExporter` | Fichier XML |
| `SQLExporter` | Script SQL (INSERT) |

`ProjectManager` contient une référence à `Exporter` (interface), ce qui lui permet de changer de format d'export à l'exécution.

---

## 2. Justification des choix de conception

### 2.1 Pattern Stratégie (Strategy)

**Appliqué à :** `DataGenerator`, `Exporter`, `ApiCaller`, `Constraint`, `DataType`

Chaque comportement variable du système est encapsulé derrière une interface. La classe cliente (ex : `Attribute`, `ProjectManager`) ne connaît que l'interface, pas l'implémentation concrète.

**Pourquoi ?**
- Permet de **permuter les comportements à l'exécution** (ex : changer le format d'export sans recompiler).
- Facilite les **tests unitaires** : chaque stratégie peut être mockée indépendamment.
- Évite les blocs `if/else` ou `switch` sur les types, qui deviendraient incontrôlables à mesure que le système évolue.

```
// Avant (sans Strategy) : fragile
if (type == "CSV") { ... } else if (type == "JSON") { ... }

// Après (avec Strategy) : extensible
exporter.exportDataset(project); // fonctionne quelle que soit l'implémentation
```

---

### 2.2 Pattern Composite

**Appliqué à :** `Entity` (via `subentities: List<Entity>`)

Une `Entity` peut contenir d'autres `Entity`, permettant de modéliser des structures arborescentes de profondeur arbitraire.

**Pourquoi ?**
- Modélisation naturelle de données hiérarchiques (ex : `Personne → Adresse → Ville`).
- L'appelant traite uniformément une entité feuille et une entité composite via la même interface.

---

### 2.3 Pattern Façade

**Appliqué à :** `ProjectManager`

`ProjectManager` expose une interface simplifiée (`addProject`, `removeProject`, `export`) qui masque la complexité interne (gestion de la liste de projets, sélection de l'exporteur, orchestration de la génération).

**Pourquoi ?**
- Réduit le couplage entre le code client et les sous-systèmes internes.
- Fournit un **point d'entrée unique** au système, facilitant l'intégration et les tests d'acceptation.

---

### 2.4 Délégation de la génération à l'`Attribute`

La méthode `generateValue(DataGenerator)` est portée par `Attribute` lui-même, plutôt que d'être externalisée.

**Pourquoi ?**
- L'`Attribute` connaît ses propres contraintes : il est le seul à pouvoir passer `List<Constraint>` au générateur.
- Cela respecte le principe d'**encapsulation** : la logique de génération est colocalisée avec la donnée qu'elle produit.

---

### 2.5 Typage via interface `DataType` plutôt qu'un enum

**Pourquoi ne pas utiliser un `enum { STRING, FLOAT }` ?**
- Un `enum` est fermé : ajouter `DATE` ou `BOOLEAN` nécessiterait de modifier la classe.
- Une interface `DataType` est **ouverte à l'extension** : on crée simplement `DateDataType implements DataType` sans rien toucher d'autre.

---

## 3. Principes SOLID respectés

### S — Single Responsibility Principle (Responsabilité unique)

> *"Une classe ne doit avoir qu'une seule raison de changer."*

| Classe | Responsabilité unique |
|---|---|
| `Attribute` | Modéliser un attribut et générer sa valeur |
| `LengthConstraint` | Vérifier une contrainte de longueur |
| `CSVExporter` | Sérialiser un projet en CSV |
| `FetchNameApi` | Appeler une API externe de noms |
| `ProjectManager` | Orchestrer les projets et les exports |

Aucune classe ne mélange plusieurs responsabilités (ex : `Exporter` n'est pas responsable de la génération de données).

---

### O — Open/Closed Principle (Ouvert/Fermé)

> *"Une entité logicielle doit être ouverte à l'extension, fermée à la modification."*

Toutes les interfaces (`DataGenerator`, `Exporter`, `Constraint`, `DataType`, `ApiCaller`) permettent d'ajouter de nouvelles implémentations **sans modifier le code existant** :

- Nouveau format → créer `ParquetExporter implements Exporter`
- Nouveau type → créer `BooleanDataType implements DataType`
- Nouvelle contrainte → créer `RegexConstraint implements Constraint`

Le code client (ex : `ProjectManager`) n'a pas besoin d'être modifié.

---

### L — Liskov Substitution Principle (Substitution de Liskov)

> *"Un objet d'une classe dérivée doit pouvoir remplacer un objet de la classe de base sans altérer le comportement du programme."*

Toute implémentation d'une interface peut être substituée à une autre :
- `CSVExporter` peut être remplacé par `JSONExporter` sans que `ProjectManager` ne le remarque.
- `RandomTextGenerator` peut être remplacé par `ApiDataGenerator` sans que `Attribute` ne soit affecté.

Aucune implémentation ne lève d'exceptions non prévues par le contrat de l'interface.

---

### I — Interface Segregation Principle (Ségrégation des interfaces)

> *"Les clients ne doivent pas être contraints de dépendre d'interfaces qu'ils n'utilisent pas."*

Les interfaces sont **petites et focalisées** :

| Interface | Méthode(s) exposée(s) |
|---|---|
| `DataType` | `getDataType()` |
| `Constraint` | `verify(Object, Class)` |
| `DataGenerator` | `generateData(List<Constraint>)` |
| `ApiCaller` | `callApi(String)` |
| `Exporter` | `exportDataset(Project)` |

Aucune interface "fourre-tout" n'existe. Chaque implémentation n'est contrainte qu'au strict nécessaire.

---

### D — Dependency Inversion Principle (Inversion des dépendances)

> *"Les modules de haut niveau ne doivent pas dépendre des modules de bas niveau. Les deux doivent dépendre d'abstractions."*

| Module haut niveau | Dépend de (abstraction) | Implémentations concrètes |
|---|---|---|
| `ProjectManager` | `Exporter` | CSV, JSON, XML, SQL |
| `Attribute` | `DataGenerator` | Random, Api |
| `ApiDataGenerator` | `ApiCaller` | FetchName, FetchNumber |
| `Attribute` | `Constraint` | Length, Range, Stat |
| `Attribute` | `DataType` | String, Float |

Aucune classe de haut niveau ne référence une implémentation concrète directement. Les dépendances sont **injectées via les interfaces**, ce qui rend le système entièrement testable et configurable.

---

## Résumé

| Aspect | Choix technique | Bénéfice |
|---|---|---|
| Extensibilité des exports | Interface `Exporter` + Strategy | Ajout de format sans régression |
| Extensibilité des générateurs | Interface `DataGenerator` + Strategy | Branchement d'API externe transparent |
| Modélisation hiérarchique | Pattern Composite sur `Entity` | Structures imbriquées illimitées |
| Validation des données | Interface `Constraint` + liste | Multi-contraintes cumulables |
| Point d'entrée simplifié | `ProjectManager` façade | Couplage faible avec le client |
| Typage flexible | Interface `DataType` | Ajout de types sans modification |
