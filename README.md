# Abysalto — Test zadatak

Repozitorij sadrži rješenje test zadatka za intervju u tvrtki Abysalto.

---

## Sadržaj repozitorija

### Nacrt arhitekture (`docs/`)

U mapi [`docs/`](docs/) nalazi se rješenje arhitekturalnog dijela zadatka.

Dokument [`docs/architecture.md`](docs/architecture.md)


---

### Aplikativni dio (`app/`)

U mapi [`app/`](app/) nalazi se implementacija aplikativnog dijela zadatka.

Kao primjer konkretne implementacije jednog od mikroservisa opisanih u arhitekturalnom nacrtu, razvijen je **Cart Service** — servis za upravljanje košaricom.

---

## Cart Service

### Opis

Cart Service je REST API mikroservis koji implementira upravljanje košaricom za online maloprodajnu platformu. Odgovoran je za kreiranje i vođenje košarica po korisniku, dodavanje i uklanjanje proizvoda te provjeru dostupnosti zaliha prema Product servisu.

### Arhitektura aplikacije

Projekt slijedi **Clean Architecture** s jasnom podjelom slojeva:

| Projekt | Odgovornost |
|---|---|
| `CartService.Api` | HTTP layer — endpointi, validatori, dependency injection |
| `CartService.Application` | Poslovna logika — CQRS handleri, sučelja repozitorija i eksternih servisa |
| `CartService.Domain` | Domenske entitete — `Cart`, `CartItem` |
| `CartService.Infrastructure` | Implementacije — EF Core, PostgreSQL, Redis, Product servis |
| `CartService.UnitTests` | Unit testovi |

Metode su implementirane u **Application layeru** kroz **CQRS pattern** — svaka operacija ima vlastiti `Command`/`Query` i odgovarajući `Handler`. Projekt koristi vlastitu implementaciju Mediatora.

> **Napomena o Mediatoru:** Implementacija Mediatora (`CartService.Application/Mediator`) zamišljena je kao zaseban **NuGet paket** koji bi se dijelio između više mikroservisa u sustavu — analogno tome kako se koristi `MediatR` u većim projektima, ali s potpunom kontrolom nad implementacijom i bez vanjskih ovisnosti.

### API endpointi

| Metoda | Ruta | Opis |
|---|---|---|
| `POST` | `/cart/add` | Dodaj u košaricu |
| `GET` | `/cart/{ownerId}` | Dohvati košaricu po ownerId-u |
| `DELETE` | `/cart/{id:int}` | Obriši košaricu po id-u |
| `PUT` | `/cart/remove-item` | Izbaci proizvod iz košarice |
| `POST` | `/cart/subtract` | Smanji količinu proizvoda u košarici |
| `GET` | `/health/live` | Liveness probe |
| `GET` | `/health/ready` | Readiness probe |

#### Detalji endpointa

**`POST /cart/add` — Dodaj u košaricu**
- Kreira novu košaricu ako košarica za zadanog `ownerId` još ne postoji
- Ako košarica postoji, dodaje novi proizvod u nju
- Ako navedeni proizvod već postoji u košarici, uvećava količinu za zadani broj
- `ownerId` identificira vlasnika košarice — može biti anonimni ili registrirani korisnik

**`GET /cart/{ownerId}` — Dohvati košaricu**
- Vraća košaricu za zadanog vlasnika
- Najprije provjerava Redis cache; ako košarica nije u cacheu, dohvaća iz baze i sprema u cache kako bi budući dohvat bio brži.

**`DELETE /cart/{id:int}` — Obriši košaricu**
- Briše cijelu košaricu po njenom internom `id`-u

**`PUT /cart/remove-item` — Izbaci proizvod**
- Uklanja određeni proizvod iz košarice

**`POST /cart/subtract` — Smanji količinu**
- Smanjuje količinu određenog proizvoda u košarici za zadani broj

### Validacija

Svaki endpoint s kompleksnim inputom ima vlastiti **validator** (FluentValidation), koji se poziva automatski putem `ValidationFilter`-a prije obrade requesta. Nekompleksni inputi (npr. path parametri) validiraju se direktno u metodi.

### Caching

Aplikacija koristi **Redis** kao cache layer. Prilikom dohvata košarice (`GET /cart/{ownerId}`) servis najprije provjerava postoji li košarica u cacheu te je, ako postoji, vraća bez upita prema bazi podataka.

Svaka operacija koja mijenja stanje košarice (`add`, `remove-item`, `subtract`, `delete`) ažurira ili invalidira cache kako bi podaci ostali konzistentni.

### Simulacija Product servisa

Unutar clustera dodan je i **simulirani Product servis** koji Cart Service koristi kao eksterni servis za provjeru dostupnosti i dohvat detalja proizvoda. Implementiran je kao **singleton servis** koji stanje kataloga čuva u memorijskom `Dictionary`-u — što znači da stanje perzistira za cijelo trajanje procesa i dijeli se između svih zahtjeva.

- **10 proizvoda** s početnom količinom od **40 komada** svaki
- Cijena po proizvodu je **nasumično generirana** u rasponu od 1 do 100

### Unit testovi

Dodani su unit testovi za prikaz koncepta testiranja. Testovi su implementirani za ključne slučajeve kako bi demonstrirali pristup — cilj nije bio 100%-tna pokrivenost, nego prikaz strukture i konvencija testiranja.

> **Napomena:** U stvarnom projektu ciljana bi bila potpuna pokrivenost svih edge-caseova. Ovdje su testovi pisani selektivno zbog vremenskog ograničenja.

Testirani su:
- CQRS handleri (`AddToCart`, `GetCart`)
- Repozitorij (`CartRepository` — insert, update, delete, get by id, get by ownerId)
- Mediator implementacija
- API validatori (`AddToCartCommandValidator`)

---

## Pokretanje aplikacije

Aplikacija se pokreće kroz **Docker Compose** koji podiže sve potrebne servise:

```bash
cd app/TestCartService
docker-compose up --build -d
```

Docker Compose podiže:

| Servis | Opis | Port |
|---|---|---|
| `cart_api` | Cart Service REST API | `8080` |
| `cart_postgres` | PostgreSQL 17 baza podataka | `5432` |
| `cart_redis` | Redis 7 cache | `6379` |

Nakon pokretanja, API je dostupan na: **`http://localhost:8080`**

Swagger UI je dostupan na: **`http://localhost:8080/swagger`**

Migracije baze podataka se automatski primjenjuju pri pokretanju aplikacije.

### Health checks

| Endpoint | Opis |
|---|---|
| `GET /health/live` | Liveness probe — je li proces živ |
| `GET /health/ready` | Readiness probe — je li servis spreman za promet (provjerava PostgreSQL i Redis) |

---

## Tehnologije

| Tehnologija | Svrha |
|---|---|
| .NET 9 / C# | Runtime i jezik |
| ASP.NET Core | Web framework, minimal API |
| Entity Framework Core | ORM za PostgreSQL |
| PostgreSQL 17 | Relacijska baza podataka |
| Redis 7 | Cache layer |
| FluentValidation | Validacija inputa |
| AutoMapper | Mapiranje domenskih modela |
| NUnit + AutoFixture + Moq | Unit testovi |
| Docker / Docker Compose | Kontejnerizacija i lokalno pokretanje |
