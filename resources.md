# 📚 NextWatchAI – Developer Resources

All key references, terminal commands, VS Code extensions, NuGet packages, Docker setup, and GitHub Copilot prompts used during this tutorial — in one place.

---

## 🛠️ Required VS Code Extensions

> Install these extensions from the VS Code Marketplace for a smooth experience:

| Extension Name             | Purpose                                         |
|---------------------------|-------------------------------------------------|
| **C#** (by Microsoft)     | Base C# language support for .NET projects      |
| **C# Dev Kit**            | Enhanced C# development experience in VS Code   |
| **NuGet Package Manager** | Manage NuGet dependencies within VS Code        |
| **REST Client**           | Test APIs directly in `.http` or `.rest` files  |
| **SQL Server (mssql)**    | Connect to and manage SQL Server inside VS Code |
| **GitHub Copilot**        | AI code completion and scaffolding              |
| **GitHub Copilot Chat**   | Ask Copilot questions or generate prompts       |

---

## 📦 NuGet Packages Used

These are the packages used during the development of **NextWatchAI**. You can install them via CLI or use the NuGet Package Manager in VS Code.

| Package Name                              | Purpose                                               | Install Command (CLI)                             |
|------------------------------------------|-------------------------------------------------------|--------------------------------------------------|
| **Microsoft.EntityFrameworkCore.SqlServer**     | SQL Server provider for EF Core                 | `dotnet add package Microsoft.EntityFrameworkCore.SqlServer` |
| **Microsoft.EntityFrameworkCore.Tools**         | Enables EF CLI commands like migrations          | `dotnet add package Microsoft.EntityFrameworkCore.Tools` |
| **Microsoft.EntityFrameworkCore.Design**        | Required for migrations and design-time services | `dotnet add package Microsoft.EntityFrameworkCore.Design` |
| **Microsoft.AspNetCore.Identity.EntityFrameworkCore** | ASP.NET Identity system with EF integration | `dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore` |
| **OpenAI** (`OpenAI` by OkGoDoIt / betalgo)     | Used to send requests to OpenAI for recommendations | `dotnet add package OpenAI --version 2.1.0` |

---

## 🐳 Docker for SQL Server (Cross-platform, macOS Verified)

Since you're using macOS and Visual Studio Code (without SQL Server natively available), we used **Docker** to run SQL Server in a container.

This allows full compatibility with Entity Framework Core and ASP.NET Identity on any system — including Mac and Linux.

### 🔧 Run SQL Server via Docker

```bash
docker run -e "ACCEPT_EULA=Y" \
  -e "SA_PASSWORD=YourStrong@Passw0rd" \
  -p 1433:1433 \
  --name sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

📎 [Microsoft SQL Server on Docker Docs →](https://learn.microsoft.com/en-us/sql/linux/sql-server-linux-docker-container-deployment)

### 🛠️ Useful Commands

| Command                                | Purpose                              |
|----------------------------------------|--------------------------------------|
| `docker start sqlserver`               | Start your SQL container             |
| `docker stop sqlserver`                | Stop the container                   |
| `docker ps`                             | List running containers              |
| `docker logs sqlserver`               | View container logs                  |
| `docker exec -it sqlserver /bin/bash` | Access terminal inside SQL container |

---

## 🤖 GitHub Copilot Prompts Used

Below are the Copilot or ChatGPT prompts used to scaffold views and UI logic during this tutorial.

### 🧾 Bootstrap 5 Responsive Movie Table

> Generate a Bootstrap 5 responsive table with four columns: Poster (fixed width image), Title (narrow), Overview (wide), and Action. Leave `<tbody>` empty for Razor. Add pagination with only Prev, current page number, and Next.

🎯 Used in `/Views/Home/Index.cshtml` to display popular movie listings.

---

### 📝 Modal Form to Edit Note

> Create a Razor Bootstrap modal (id="noteModal") for editing FavoriteMovieVM.Note: form posts to Favorite/EditNote (asp-controller="Favorites" asp-action="EditNote") with @Html.AntiForgeryToken(), a hidden id input, a note textarea, a header title span #modalTitle, close & save buttons, and an @section Scripts block with plain JS on show.bs.modal that reads data-favid, data-note, data-title from the trigger button and sets #id.value, #note.value, and #modalTitle.textContent.

🎯 Used in `/Views/Favorites/Index.cshtml` to update a user's personal note with a smooth in-page experience.

---

> 💡 Prompts were adjusted during coding to match Razor and MVC patterns and avoid redundant HTML or logic.