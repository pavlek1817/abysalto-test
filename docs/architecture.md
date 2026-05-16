# Opis arhitekture za Online Retail platform

## Opis zadataka

Tvoja tvrtka će razvijati online maloprodajnu platformu za klijenta koji posluje na globalnom
tržištu s milijunima korisnika dnevno. Na sustavu će raditi dva cross-funkcionalna tima.
Sustav će omogućiti prodaju proizvoda kroz više kanala:
Web shop
Mobilne aplikacije
Marketplace integracije
B2B integracije
Ključni zahtjevi klijenta:
Skalabilnost i podrška za visoki promet (milijuni korisnika dnevno)
Sigurne transakcije i zaštita podataka
Real-time obrada podataka
Za 3 sati imati ćeš sastanak na kojem trebaš prezentirati svoju viziju high-level dizajna
sustava i strategiju implementacije.
Od tebe se očekuje da imaš dokument koji će uključivati:
- Arhitekturalni pogled sustava (high-level skica sustava)
o Kako će komponente međusobno komunicirati
o Odabir glavnih tehnologija sustava
- Strategiju skaliranja (kako rješavati probleme povećanog prometa)
- Sigurnost i autentifikaciju
- Definiciju ključnih komponenti i njihovih odgovornosti
- Integracija s vanjskim servisima (npr. Porezna uprava)
- Monitoring i alerting (healthcheck)
- Plan isporuke koda
o CI/CD
o Branching strategija

---

## Razrada — Mikroservisna arhitektura

S obzirom na milijune korisnika dnevno i potrebu za visokom dostupnošću, odabiremo
**mikroservisnu arhitekturu** deployanu na **Kubernetes**. To nam daje mogućnost neovisnog
skaliranja pojedinih servisa, neovisnih deploymenata po timu i izolaciju grešaka.

Vizualni prikaz sustava dostupan je u [architecture.mermaid](architecture.mermaid).

---

## 1. Arhitekturalni pogled sustava

### Kako komponente međusobno komuniciraju

Koristimo **dva kanala komunikacije** između servisa:

**Sinkrona komunikacija — REST/gRPC**
- API Gateway ↔ Core servisi (Product, Cart, Order, Payment)
- Koristi se kad klijent čeka odgovor (npr. dodavanje u košaricu, dohvat proizvoda)
- gRPC je bolji za internu servis-servis komunikaciju zbog performansi (binarni protokol, manja latencija)

**Asinkrona komunikacija — Apache Kafka**
- Core servisi → Kafka → ostali servisi (Notification, Search, Integration)
- Koristi se za event-driven tokove gdje rezultat ne treba biti trenutan
- Primjer: `OrderCreated` event → Notification šalje email, Integration obavještava Poreznu upravu
- **Zašto Kafka**: garantira trajnost poruka, horizontalno skalira, podržava replay evenata, dead letter.

### Odabir glavnih tehnologija

| Sloj               | Tehnologija              | Razlog odabira                                                              |
|--------------------|--------------------------|-----------------------------------------------------------------------------|
| API Gateway        | ASP.NET Core (.NET 9)    | Potpuna kontrola nad rutingom, lako dodavanje middleware-a, npr. za autorizaciju. |
| Core servisi       | ASP.NET Core (.NET 9)    | Visoke performanse, bogat ekosustav, type safety, odlična K8s podrška       |
| Event bus          | Apache Kafka             | Trajnost poruka, visoki throughput, replay evenata                          |
| Baze podataka      | PostgreSQL (po servisu)  | Izolacija podataka po servisu, ACID transakcije                             |
| Cache              | Redis                    | In-memory, podrška za session                      |
| Auth               | Keycloak                 | Battle-tested, podržava OAuth2/OIDC, SSO, lako se podiže u Kubernetesu     |
| CDN / WAF          | Azure Front Door         | Globalna mreža, DDoS zaštita, WAF, SSL/TLS, edge caching — nativna integracija s Azure stackom |
| Pohrana fajlova    | Azure Blob Storage       | Skalabilna object pohrana za slike, dokumente i statički sadržaj           |
| Orkestracija       | Kubernetes (K8s)         | Industry standard, HPA za auto-scaling, self-healing                       |
| Infrastructure     | Terraform                | IaC, reproducibilna infrastruktura, verzioniranje                           |

---

## 2. Strategija skaliranja

### Horizontalno skaliranje servisa

Svaki mikroservis je stateless i horizontalno skalirabilan. Kubernetes **Horizontal Pod Autoscaler (HPA)**
automatski povećava broj replika na temelju CPU/memory metrika ili custom metrika broj dolaznih request-ova.

Npr. normalan promet skalira Product Service na 2 poda, u peak prometu HPA automatski skalira na 10+ podova.

### Caching strategija

Višeslojni cache smanjuje opterećenje baza podataka:

1. **CDN cache (Azure Front Door)** — statički sadržaj i javni API odgovori; fajlovi se servaju iz Azure Blob Storagea
2. **Redis cache** — dohvat detalja proizvoda, session podataka korisnika, košarice
3. **Database read replicas** — PostgreSQL read replica za čitanje uz primarnu bazu za pisanje

**Zašto**: na milijun zahtjeva dnevno, 90% su čitanja. Ako 80% tih čitanja odgovori Redis,
baza prima samo 20% prometa — drastično smanjuje latenciju i troškove.

### Database sharding i particioniranje

- Order servis: **particioniranje tablice po datumu** (monthly partitions) — stariji orderi se arhiviraju
- Product servis: **read replicas** za search i listing endpointe

### Rate limiting

API Gateway servis (ASP.NET Core) primjenjuje rate limiting putem ASP.NET Core Rate Limiting
middlewarea po korisniku/IP-u kako bi spriječio zlouporabu i zaštitio sustav od spike prometa.

---

## 3. Sigurnost i autentifikacija

### Autentifikacija i autorizacija — Keycloak

Keycloak je centralni identity provider. Deployamo ga kao **Kubernetes StatefulSet** s
persistent volumeom i read replikasom za visoku dostupnost.

Tok autentifikacije: Korisnik šalje zahtjev prema API Gatewayu, koji validira JWT token s Keycloakom
te prosljeđuje zahtjev odgovarajućem mikroservisu s identity claimsima u headeru.

- Korisnici se autenticiraju putem **OAuth2 Authorization Code Flow** (web/mobile)
- B2B integracije koriste **Client Credentials Flow** (machine-to-machine)
- JWT tokeni imaju kratki TTL (15 min) + refresh token mehanizam
- Keycloak upravlja rolama: `CUSTOMER`, `ADMIN`, `B2B_PARTNER`, `MARKETPLACE`

API Gateway validira JWT token pri **svakom zahtjevu** — mikroservisi ne trebaju vlastitu auth logiku.

### Zaštita podataka

- Sva komunikacija enkriptirana **TLS 1.3** (u transportu)
- Osjetljivi podaci (payment info) enkriptirani **at rest** u bazi (AES-256)
- Payment Service **nikad ne čuva** broj kartice — delegira PCI-DSS compliant provideru (Stripe)
- **Secrets management**: Kubernetes Secrets s enkriptiranim etcd-om
- **GDPR**: User Service implementira pravo na zaborav (`DELETE /users/{id}` briše sve PII podatke)

### Zaštita perimetra

- **WAF (Azure Front Door)** blokira SQL injection, XSS i ostale OWASP Top 10 napade na rubu mreže
- **Network policies** u Kubernetes-u — servisi međusobno komuniciraju samo na eksplicitno dozvoljenoj listi
- **Pod Security Standards** — kontejneri rade kao non-root korisnici

---

## 4. Definicija ključnih komponenti i odgovornosti

### API Gateway (ASP.NET Core)
- Jedina ulazna točka prema vanjskim klijentima
- Implementiran kao ASP.NET Core aplikacija.
- JWT validacija putem Microsoft.AspNetCore.Authentication.JwtBearer (integracija s Keycloakom)
- Rate limiting, logging svih zahtjeva, CORS
- **YARP**: Microsoft-ova biblioteka dizajnirana upravo za .NET reverse proxy scenarije.
### Product Service (ASP.NET Core)
- CRUD upravljanje katalogom proizvoda
- Upravljanje kategorijama, atributima i varijantama
- Objavljuje `ProductUpdated` event na Kafku (Search servis indeksira promjene)
- **Baza**: PostgreSQL (EF Core) + Redis za cache

### Cart Service (ASP.NET Core)
- Upravljanje košaricom po sesiji/korisniku
- Provjera dostupnosti zaliha (gRPC poziv prema Product servisu)
- TTL za anonimne košarice (npr. 7 dana u Redisu)
- **Baza**: Redis (košarica je privremena struktura)

### Order Service (ASP.NET Core)
- Kreiranje narudžbe iz košarice 
- Orchestracija saga patternom: rezervacija zaliha → naplata → potvrda
- Objavljuje `OrderCreated`, `OrderPaid`, `OrderCancelled` evente na Kafku
- **Baza**: PostgreSQL (EF Core)

### Payment Service (ASP.NET Core)
- Procesiranje plaćanja putem Stripe-a 
- Nikad ne sprema osjetljive podatke kartice (PCI-DSS scope minimizacija)
- Webhook handler za asinkrone potvrde od Stripea
- **Baza**: PostgreSQL (EF Core)

### Notification Service (ASP.NET Core Worker Service)
- Konzumira Kafka evente (`OrderCreated`, `OrderPaid` itd.)
- Šalje email, SMS i push notifikacije
- Template engine za lokalizirane poruke
- **Baza**: PostgreSQL za log poslanih poruka

### Search Service (ASP.NET Core)
- Indeksira proizvode u Elasticsearch
- Podržava full-text search, filtriranje i facetiranu navigaciju
- Konzumira `ProductUpdated` evente za real-time indeksiranje
- Koristi NEST/Elastic.Clients.Elasticsearch SDK

### Integration Service (ASP.NET Core Worker Service)
- Adapter prema vanjskim sustavima (Porezna uprava, ERP, Marketplace API-ji)
- Transformacija podataka između internih i eksternih formata
- **Circuit breaker** obrazac putem Polly biblioteke za zaštitu od nedostupnih vanjskih servisa

---

## 5. Integracija s vanjskim servisima

### Porezna uprava (Fiskalizacija)

**Problem**: Fiskalizacijski pozivi su obavezni, ali vanjska usluga može biti spora ili nedostupna.

**Rješenje**: Integration Service konzumira `OrderPaid` event asinkrono putem Kafke.
Ako poziv ne uspije, Polly provodi retry s exponential backoffom (3 pokušaja).
Poruke koje persistentno ne uspijevaju završavaju u **Dead Letter Queue (DLQ)** —
alerting obavještava tim, a rezultat uspješne fiskalizacije se objavljuje natrag na Kafku.

**Zašto asinkrono**: korisnik ne smije čekati na odgovor Porezne uprave. Narudžba
je već kreirana i plaćena — fiskalizacija se ponavlja u pozadini bez utjecaja na UX.

### ERP integracija (B2B)

- B2B partneri komuniciraju putem dedicirane rute na API Gatewayu s Client Credentials JWT
- Integration Service provodi transformaciju u format ERP sustava (npr. SAP IDOC, EDI)
- Webhook podrška za obavijesti prema B2B partnerima

### Marketplace integracije (Amazon, eBay)

- .NET Worker Service u sklopu Integration Servicea periodički sinkronizira zalihe prema marketplace API-jima
- Inbound orderi s marketplace-a se transformiraju u interni `OrderCreated` format

---

## 6. Monitoring i alerting

### Observability stack

| Alat           | Svrha                                                         |
|----------------|---------------------------------------------------------------|
| Prometheus     | Scraping i pohrana metrika svih servisa i K8s čvorova         |
| Grafana        | Dashboardi — per-servis latencija, error rate, throughput     |
| Loki           | Centralizirani log agregation (sve instance šalju logove)     |
| Jaeger         | Distributed tracing — praćenje zahtjeva kroz više servisa     |
| Alertmanager   | Routing alerta na Slack / PagerDuty po severity               |

Svaki .NET servis exposes Prometheus metrike putem `prometheus-net` biblioteke na `/metrics` endpointu.
OpenTelemetry SDK se koristi za distributed tracing prema Jaegeru.

### Ključni alerte

| Alert                          | Threshold       | Akcija                              |
|-------------------------------|-----------------|--------------------------------------|
| API error rate                | > 1% na 5 min   | Slack on-call                   |
| Order Service latencija       | p99 > 2s        | Slack #alerts-critical               |
| Kafka consumer lag            | > 10.000 poruka | Slack #alerts-warning               |
| Pod restart loop              | > 3x u 10 min   | PagerDuty on-call                   |
| DLQ poruke                    | > 0             | Slack #alerts-integration           |
| Disk usage (baza)             | > 80%           | Slack #alerts-infra                 |

### Healthcheck i readiness

Svaki mikroservis exposes:
- `GET /health/live` — Kubernetes liveness probe (je li proces živ?)
- `GET /health/ready` — Kubernetes readiness probe (je li servis spreman primati promet?)

Implementirano putem `Microsoft.Extensions.Diagnostics.HealthChecks` paketa. Readiness probe
provjerava konekciju na bazu i Redis — ako baza nije dostupna, pod se izvlači iz load balancera.

---

## 7. Plan isporuke koda

### Branching strategija — Trunk-Based Development

- `main` (trunk) — jedina dugotrajna grana, uvijek u deployabilnom stanju, zaštićena
- `feature/*` — kratkoživuće grane (max 1–2 dana), spajaju se direktno u `main`
- Nema `develop`, `release/*` ni `hotfix/*` grana — sve ide kroz `main`
- Nedovršene funkcionalnosti skrivaju se **feature flagovima** (npr. LaunchDarkly ili vlastiti config)

**Pull Request pravila:**
- Svaki PR zahtijeva review od minimalno 1 osobe (na kritičnim servisima 2)
- PR title slijedi Conventional Commits format (`feat:`, `fix:`, `chore:`)
- Squash merge u `main` — čista, linearna historija
- PR mora biti otvoren kratko — ako PR živi dulje od 2 dana, signal je da je scope prevelik

**Zašto trunk-based**: s dva cross-funkcionalna tima i visokim tempom isporuke, dugotrajne grane
uzrokuju merge hell. Kratke integracije smanjuju konflikte, ubrzavaju feedback iz CI-a i drže
`main` uvijek spreman za deploy.

### CI/CD pipeline — GitHub Actions

**CI (svaki push / PR):**
1. Lint + build (`dotnet build`)
2. Unit testovi (`dotnet test`)
3. Integration testovi (Testcontainers — PostgreSQL i Redis u Dockeru za vrijeme testa)
4. Security scan (Snyk)
5. Build Docker image + push na container registry ili docker hub

**CD na main → STAGING:**
- Automatski deploy na staging okruženje pri svakom mergu u `main` ako CI prođe
- Smoke testovi na staging okruženju, ne cijeli svaki edge case testova, neko manji skup testova.

**CD main → PRODUCTION:**
- Canary deploy putem Argo Rollouts (10% prometa na novu verziju)
- Monitoring 15 minuta → automatski rollout ili rollback na temelju error rate i latencije

**Zašto Testcontainers za integration testove**: testovi se izvršavaju s pravim instancama
PostgreSQL-a i Redisa bez mockiranja, čime se eliminira klasa grešaka gdje mock i produkcija divergiraju.

**Zašto canary deployment**: na produkciji s milijunima korisnika, full deployment bez
testiranja na manjem prometu je prevelik rizik. Argo Rollouts automatizira rollback te omogućuje testiranje na manjem prometu (stara verzija 90% a novi 10%).

### Okruženja

| Okruženje  | Namjena                                    | Skaliranje       |
|------------|---------------------------------------------|------------------|
| dev        | Lokalni razvoj (docker-compose)             | Minimal          |
| staging    | Integracijsko testiranje, demo klijentu     | ~10% prod        |
| production | Stvarni promet                              | Full HPA         |

---

## Sažetak arhitekturalnih odluka

| Odluka                        | Odabir                     | Razlog                                                       |
|-------------------------------|----------------------------|--------------------------------------------------------------|
| Arhitektura                   | Mikroservisi na K8s        | Neovisno skaliranje, izolacija grešaka, neovisni deployovi   |
| Jezik / runtime               | .NET 9 / C#                | Visoke performanse, jedinstven stack kroz sve servise        |
| API Gateway                   | ASP.NET Core + YARP        | Potpuna kontrola, isti stack, visoke performanse             |
| Inter-servis komunikacija     | REST/gRPC + Kafka          | Sinkrono gdje treba odgovor, async za event-driven tokove    |
| Auth                          | Keycloak + JWT             | OAuth2/OIDC standard, centralizirana auth logika             |
| Baza po servisu               | PostgreSQL + EF Core       | Izolacija podataka, ACID, sprečava tight coupling            |
| Cache                         | Redis                      | Smanjuje DB opterećenje, podrška za session i rate limiting  |
| Vanjski integracije           | Async + Polly + DLQ        | Sustav radi čak i kad su vanjski servisi nedostupni          |
| Deployment                    | Canary + Argo Rollouts     | Siguran rollout bez rizika full outage-a                     |
