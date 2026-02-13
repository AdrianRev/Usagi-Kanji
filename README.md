# Usagi Kanji

Fullstack web application for studying Japanese kanji.  
Developed as part of a Bachelor's thesis in Computer Science.

## 🐳 Running the Application (Docker)

To run the application using Docker containers, make sure the following software is installed:

- Docker Desktop  
- Git (for cloning the repository)

---

### 🔧 Setup Instructions

#### 1. Clone the repository

```bash
git clone https://github.com/AdrianRev/Usagi-Kanji
cd UsagiKanji
```

#### 2. Configure environment variables

Rename the file:

```
.env.example
```

to:

```
.env
```

Adjust the environment variables inside the `.env` file if necessary.

---

#### 3. Prepare required dictionary data

To populate the database, the following JSON files are required:

- `kanjidic2-en`
- `jmdict-eng` (preferably the **common** version)

These files can be obtained from the `jmdict-simplified` repository.

After downloading them, place both files in the following directory:

```
UsagiKanji/Backend/Importer/Data
```

---

#### 4. Start the Docker containers

From the root directory of the project (`/UsagiKanji`), run:

```bash
docker-compose up -d
```

---

#### 5. Access the application

Once the containers are running, the application should be available at:

```
http://localhost:3000
```
