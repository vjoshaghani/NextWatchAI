using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextWatchAI.Migrations
{
    /// <inheritdoc />
    public partial class AddTmdbIdToMovie_ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TmdbId",
                table: "Movies",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TmdbId",
                table: "Movies");
        }
    }
}
