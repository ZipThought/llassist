using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace llassist.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class IndexProjectIdOnEstimateRelevanceJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EstimateRelevanceJobs_ProjectId",
                table: "EstimateRelevanceJobs",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EstimateRelevanceJobs_ProjectId",
                table: "EstimateRelevanceJobs");
        }
    }
}
