# Prompt de developpement — ff.api (Backend ASP.NET)

## Instructions prealables OBLIGATOIRES

**AVANT de commencer a travailler sur ce projet, tu DOIS :**

1. **Lire le CLAUDE.md** de ce projet (`ff.api/CLAUDE.md`) pour comprendre la stack, la structure, les conventions et les regles metier.

2. **Lire TOUTE l'analyse fonctionnelle** dans le projet `ff.analyse/` :
   - `ff.analyse/01_use_case.puml` — Diagramme de cas d'utilisation (6 acteurs, 40+ use cases)
   - `ff.analyse/02_class_diagram.puml` — Diagramme de classes / modele de donnees (19 entites avec attributs, enums et relations)
   - `ff.analyse/03_sequence_diagrams.puml` — 5 diagrammes de sequence (commande, cochage magasin, abonnement, colis surprise, fournisseur B2B)
   - `ff.analyse/04_activity_diagrams.puml` — 4 diagrammes d'activite (workflow quotidien, abonnement, colis surprise, fournisseur B2B)

3. **Ne jamais inventer** de structure, d'endpoint ou de regle metier. Tout est defini dans l'analyse. S'y conformer strictement.

---

## Ta mission

Tu es le developpeur backend de la **Plateforme Anti-Gaspi Bruxelles**. Tu dois implementer l'API REST en **ASP.NET 9 + Entity Framework Core + SQL Server**.

### Ce que tu dois faire :

1. **Modele de donnees** : Creer les 19 entites EF Core en respectant exactement le diagramme de classes (`02_class_diagram.puml`) — types, enums, relations, FK.

2. **Endpoints API** : Implementer tous les endpoints documentes dans les diagrammes de sequence (`03_sequence_diagrams.puml`). Chaque appel API visible dans les sequences doit correspondre a un controller/action.

3. **Logique metier** : Implementer les regles metier decrites dans les diagrammes d'activite (`04_activity_diagrams.puml`) :
   - Deadline 17h pour les commandes
   - Assignation des commandes aux zones par geolocalisation
   - Calcul d'itineraire (entrepot → proche → eloigne)
   - Gestion client absent (appel + 5 min → echec)
   - Workflow abonnement avec Stripe Billing
   - Composition automatique des colis surprise
   - Processus fournisseur B2B (inscription → validation → rachat)

4. **Integrations** :
   - Stripe : PaymentIntents, Billing (subscriptions), Webhooks (`payment_intent.succeeded`, `invoice.paid`)
   - Firebase FCM : notifications push
   - SignalR : hub de suivi GPS temps reel

5. **Securite** : JWT auth, role-based authorization (Client, StoreManager, Delivery, Admin), validation des inputs, CORS.

6. **Tracabilite AFSCA** : TraceabilityLog avec batch, DLC, temperatures.

### Architecture :
- Clean Architecture : Controllers → Services → Repositories → DbContext
- DTOs pour toutes les reponses/requetes (jamais exposer les entites)
- FluentValidation ou DataAnnotations pour la validation
- Migrations EF Core pour le schema SQL Server
