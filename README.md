## Backend (CesiZen-Backend)

### 1. Configuration des variables d'environnement

À la racine du dossier `CesiZen-Backend/CesiZen-Backend`, créez un fichier `.env` avec :

```env
POSTGRES_USER=cesi
POSTGRES_PASSWORD=VotreMotDePasse
POSTGRES_DB=cesizen_db
JWT_SECRET=UneCléTrèsSecrète
```

### 2. Lancer la base de données

Dans un terminal PowerShell :

```powershell
cd CesiZen-Backend/CesiZen-Backend
docker-compose up -d
```

Le service PostgreSQL sera exposé sur le port 5432.

### 3. Mettre à jour la base de données

Si vous préférez EF Core localement :

```powershell
cd CesiZen-Backend/CesiZen-Backend
dotnet restore
dotnet tool install --global dotnet-ef
dotnet ef database update --project CesiZen-Backend.csproj
```

### 4. Lancer l'API

```powershell
cd CesiZen-Backend/CesiZen-Backend
dotnet run
```

L'API écoute par défaut sur http://localhost:5000 (ou 5001 en HTTPS).

---
