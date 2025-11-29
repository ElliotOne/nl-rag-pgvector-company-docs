# nl-rag-pgvector-company-docs

A web application demonstrating Retrieval-Augmented Generation (RAG) for company document Q&A using local vector search with pgvector, PostgreSQL, and Ollama-hosted AI models. Intended for professionals seeking to explore semantic search using C# and .NET 10.0.

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- [PostgreSQL](https://www.postgresql.org/) with [pgvector](https://github.com/pgvector/pgvector) extension installed and running
- [Ollama](https://ollama.ai/) installed and running locally
- Required AI model downloaded in Ollama:
  - `mistral` (for embeddings and chat)

## Installation

1. Clone this repository:
   ```bash
   git clone https://github.com/your-username/nl-rag-pgvector-company-docs.git
   cd nl-rag-pgvector-company-docs
   ```

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

## Setup

1. **Set up PostgreSQL with pgvector:**
   - Install PostgreSQL and enable the pgvector extension:
     ```sql
     CREATE EXTENSION vector;
     ```
   - Create the database and table:
     ```sql
     CREATE TABLE text_contexts (
         id SERIAL PRIMARY KEY,
         title TEXT NOT NULL,
         category TEXT NOT NULL,
         content TEXT NOT NULL,
         embedding vector(4096)
     );
     ```
   - Update the connection string in `appsettings.json` if needed.

2. **Install and Start Ollama:**
   - Download Ollama from [ollama.ai](https://ollama.ai/), install and launch it.
   - Download and prepare the required model:
     ```bash
     ollama pull mistral
     ```
   - Ollama will run at [http://localhost:11434](http://localhost:11434)

3. **Verify Model & Service Configuration:**
   - Ensure services are running on their default endpoints:
     - PostgreSQL: As configured in connection string
     - Ollama: `http://localhost:11434`
   - Check the config in `Program.cs` for model versions and endpoints if you change ports/models.

## Usage

Run the web application to start the company docs RAG Q&A:

```bash
cd RAGPgvectorCompanyDocs
# Restore and build
dotnet restore
# (optional, if not yet run)
dotnet build
# Run
dotnet run
```

Access the application at [https://localhost:5001](https://localhost:5001) (or the configured URL). You can add company documents and ask questions against the database.

Example usage:
- Add content: Enter a title, select a category (HR, Tech, Sales, Legal, Culture), and provide content.
- Ask questions: Enter queries like "What is our vacation policy?" or "How do we handle remote work?"

## Example Flow

1. **Document Import:**
   - Use the web interface to add company documents with titles, categories, and content. The app processes them into semantic embeddings using `mistral` and stores them in the PostgreSQL vector table.
2. **Semantic Search & Q&A:**
   - Enter your query; the app searches for the most relevant text chunks using vector similarity in pgvector.
   - The app uses the `mistral` model (locally via Ollama) to generate a context-aware answer, showing relevant sources from the vector DB.

## Document Categories Included

- HR (Human Resources)
- Tech (Technology)
- Sales
- Legal
- Culture

## Key Technologies

- **.NET 10.0** (C# Web App with ASP.NET Core MVC)
- **pgvector** (Vector similarity search in PostgreSQL)
- **PostgreSQL** (Database with vector extension)
- **Ollama** (Local AI model runtime)
- **Mistral** (For embeddings and chat/QA)

## Dependencies

NuGet packages required (see `.csproj`):
```xml
<PackageReference Include="Npgsql" Version="10.0.0" />
```

## Project Structure

- `Program.cs`: Entry point and service registration
- `Controllers/HomeController.cs`: Handles web requests for Q&A and adding content
- `Services/RagService.cs`: Orchestrates retrieval and generation
- `Repository/TextRepository.cs`: Manages text storage and retrieval with pgvector
- `EmbeddingGenerator/`: Interfaces and implementations for generating embeddings via Ollama
- `Models/`: Data models for requests and responses
- `Views/Home/Index.cshtml`: Web interface for interaction

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please submit an issue or open a Pull Request if you find a bug or have an enhancement idea.
