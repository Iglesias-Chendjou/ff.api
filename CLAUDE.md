# CLAUDE.md — ff.api (Backend ASP.NET)

## Projet
Plateforme Anti-Gaspi Bruxelles — Backend API REST.
Marketplace B2C2B revalorisant trois sources d'approvisionnement coexistantes (focus n°1 = invendables magasin) :
1. **Invendables magasin** (`Reason = Unsellable`, focus) — produits consommables mais invendables en circuit normal : emballage abîmé, alvéole incomplète, surstock, défauts d'emballage 666.
2. **Invendus DLC magasin** (`Reason = NearExpiry`) — produits frais à péremption J+1.
3. **Achat en gros producteurs** (`SourceType = ProducerBulk`) — marchandise non conforme à la vente, conforme à la consommation. Stock permanent en entrepôt.

Décote par défaut -50% (`ProductTemplate.DiscountPercent`), surchargeable par inventaire via `StoreInventory.DiscountPercentOverride` selon la gravité du défaut.

## Stack technique
- **Framework** : ASP.NET 9 (C#)
- **ORM** : Entity Framework Core
- **BDD** : SQL Server
- **Auth** : JWT (Bearer tokens)
- **Paiement** : Stripe (PaymentIntents + Billing pour abonnements)
- **Notifications push** : Firebase Cloud Messaging (FCM)
- **Temps reel** : SignalR (WebSocket) pour le suivi GPS des livreurs
- **Architecture** : Clean Architecture (Controllers → Services → Repositories → DbContext)

## Structure attendue
```
ff.api/
├── Controllers/          # Endpoints REST
│   ├── AuthController.cs
│   ├── ProductsController.cs
│   ├── OrdersController.cs
│   ├── DeliveriesController.cs
│   ├── SubscriptionsController.cs
│   ├── SurpriseBoxController.cs
│   ├── SuppliersController.cs
│   ├── StoresController.cs
│   ├── AdminController.cs
│   └── NotificationsController.cs
├── Models/               # Entites EF Core (19 entites)
├── DTOs/                 # Data Transfer Objects
├── Services/             # Logique metier
├── Repositories/         # Acces donnees
├── Hubs/                 # SignalR (DeliveryTrackingHub)
├── Middlewares/           # Auth, error handling, CORS
├── Migrations/           # EF Core migrations
├── Webhooks/             # Stripe webhooks
└── Program.cs
```

## Entites du modele de donnees (19 entites)
User, Address, Store, Zone, ProductCategory, ProductTemplate, StoreInventory,
Order, OrderItem, Payment, Delivery, DeliveryPerson, Subscription,
SurpriseBoxPlan, SurpriseBoxSubscription, Supplier, BulkPurchaseRequest,
Notification, TraceabilityLog

## Endpoints principaux (extraits des diagrammes de sequence)
- `POST /api/auth/login` — Authentification (JWT)
- `POST /api/auth/store-login` — Auth magasin (storeId + PIN)
- `GET /api/products/available?zone={zoneId}` — Catalogue filtre par zone
- `POST /api/cart/validate` — Validation panier
- `POST /api/orders` — Creation commande + reservation stock
- `PUT /api/orders/{id}/status` — Mise a jour statut commande
- `POST /api/subscriptions` — Souscription abonnement recurrent
- `GET /api/surprise-box/plans` — Liste forfaits colis surprise
- `POST /api/surprise-box/subscribe` — Souscription colis surprise
- `POST /api/suppliers/register` — Inscription fournisseur B2B
- `POST /api/suppliers/bulk-offers` — Offre de rachat grandes quantites
- `GET /api/deliveries/mine` — Livraisons assignees au livreur
- `PUT /api/deliveries/{id}/pickup` — Prise en charge colis
- `PUT /api/deliveries/{id}/location` — MAJ position GPS (toutes les 30s)
- `PUT /api/deliveries/{id}/complete` — Confirmer livraison (photo/signature)
- `PUT /api/deliveries/{id}/fail` — Livraison echouee (client absent)
- `GET /api/stores/{id}/catalog` — Catalogue magasin
- `POST /api/stores/{id}/inventory/publish` — Publication inventaire
- Webhooks Stripe : `payment_intent.succeeded`, `invoice.paid`

## Regles metier critiques
- **Deadline commandes : 17h** — Apres 17h, la commande est traitee le lendemain
- **Assignation par zone** : chaque commande est assignee a une zone selon la geolocalisation du client
- **Itineraire de livraison** : entrepot → commande la plus proche → la plus eloignee (proximite croissante)
- **Client absent** : appel telephonique + 5 min d'attente. Apres 5 min → Delivery.Status = Failed, Order.Status = Cancelled. La plateforme n'est plus responsable.
- **Prix** : decote par defaut -50% (`ProductTemplate.DiscountPercent`), surchargeable par inventaire via `StoreInventory.DiscountPercentOverride` (ex. -30% pour un defaut cosmetique mineur, -70% pour un gros defaut). Les 3 fourchettes de prix (bas/milieu/haut) restent dans `ProductTemplate`.
- **Qualification de l'offre** (par `StoreInventory`) : `Reason` (Unsellable/NearExpiry) obligatoire ; si `Unsellable`, `UnsellableSubReason` requis (DamagedPackaging, IncompletePack, Overstock, PackagingDefect666) ; si `NearExpiry`, `ExpirationDate` doit etre <= J+1. Champ libre `ReasonNotes` pour precisions (ex. "alveole a 11 oeufs au lieu de 12").
- **Abonnements** : plans Mensuel / Trimestriel / Semestriel / Annuel
- **Colis Surprise** : Decouverte 30EUR/1 livraison, Classique 50EUR/3, Premium 80EUR/5
- **Fournisseurs B2B** : inscription → validation admin → offre de rachat → examen → integration au catalogue
- **Conformite AFSCA** : tracabilite complete (batch, DLC, temperatures collecte/livraison)

## Conventions
- Langue du code : anglais
- Langue des commentaires : francais si necessaire
- Nommage : PascalCase (C# standard)
- Utiliser les enums definis dans le diagramme de classes
- Tous les Id sont des Guid
- Toujours valider les entrees utilisateur (DTOs avec FluentValidation ou DataAnnotations)
- Ne jamais exposer les entites directement — utiliser des DTOs
