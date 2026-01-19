using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UniversitySystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sale",
                columns: table => new
                {
                    salaid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    numersali = table.Column<string>(type: "text", nullable: false),
                    liczbamiejsc = table.Column<int>(type: "integer", nullable: false),
                    czykomputery = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sale", x => x.salaid);
                });

            migrationBuilder.CreateTable(
                name: "stanowiska",
                columns: table => new
                {
                    stanowiskoid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nazwa = table.Column<string>(type: "text", nullable: false),
                    stawkagodzinowa = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stanowiska", x => x.stanowiskoid);
                });

            migrationBuilder.CreateTable(
                name: "statusystudentow",
                columns: table => new
                {
                    statusstudentaid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nazwa = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_statusystudentow", x => x.statusstudentaid);
                });

            migrationBuilder.CreateTable(
                name: "wydzialy",
                columns: table => new
                {
                    wydzialid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nazwa = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    dziekan = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wydzialy", x => x.wydzialid);
                });

            migrationBuilder.CreateTable(
                name: "pracownicy",
                columns: table => new
                {
                    pracownikid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    imie = table.Column<string>(type: "text", nullable: false),
                    nazwisko = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    datazatrudnienia = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    stanowiskoid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pracownicy", x => x.pracownikid);
                    table.ForeignKey(
                        name: "FK_pracownicy_stanowiska_stanowiskoid",
                        column: x => x.stanowiskoid,
                        principalTable: "stanowiska",
                        principalColumn: "stanowiskoid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "kierunki",
                columns: table => new
                {
                    kierunekid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nazwa = table.Column<string>(type: "text", nullable: false),
                    stopien = table.Column<int>(type: "integer", nullable: false),
                    wydzialid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kierunki", x => x.kierunekid);
                    table.ForeignKey(
                        name: "FK_kierunki_wydzialy_wydzialid",
                        column: x => x.wydzialid,
                        principalTable: "wydzialy",
                        principalColumn: "wydzialid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "grupydziekanskie",
                columns: table => new
                {
                    grupaid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    kodgrupy = table.Column<string>(type: "text", nullable: false),
                    rokstudiow = table.Column<int>(type: "integer", nullable: false),
                    kierunekid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grupydziekanskie", x => x.grupaid);
                    table.ForeignKey(
                        name: "FK_grupydziekanskie_kierunki_kierunekid",
                        column: x => x.kierunekid,
                        principalTable: "kierunki",
                        principalColumn: "kierunekid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "przedmioty",
                columns: table => new
                {
                    przedmiotid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nazwa = table.Column<string>(type: "text", nullable: false),
                    ects = table.Column<int>(type: "integer", nullable: false),
                    semestr = table.Column<int>(type: "integer", nullable: false),
                    kierunekid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_przedmioty", x => x.przedmiotid);
                    table.ForeignKey(
                        name: "FK_przedmioty_kierunki_kierunekid",
                        column: x => x.kierunekid,
                        principalTable: "kierunki",
                        principalColumn: "kierunekid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "studenci",
                columns: table => new
                {
                    studentid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nrindeksu = table.Column<string>(type: "text", nullable: false),
                    pesel = table.Column<string>(type: "text", nullable: false),
                    imie = table.Column<string>(type: "text", nullable: false),
                    nazwisko = table.Column<string>(type: "text", nullable: false),
                    grupaid = table.Column<int>(type: "integer", nullable: false),
                    statusstudentaid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_studenci", x => x.studentid);
                    table.ForeignKey(
                        name: "FK_studenci_grupydziekanskie_grupaid",
                        column: x => x.grupaid,
                        principalTable: "grupydziekanskie",
                        principalColumn: "grupaid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_studenci_statusystudentow_statusstudentaid",
                        column: x => x.statusstudentaid,
                        principalTable: "statusystudentow",
                        principalColumn: "statusstudentaid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "planzajec",
                columns: table => new
                {
                    planzajecid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    dzientygodnia = table.Column<int>(type: "integer", nullable: false),
                    godzinarozpoczecia = table.Column<TimeSpan>(type: "interval", nullable: false),
                    godzinazakonczenia = table.Column<TimeSpan>(type: "interval", nullable: false),
                    pracownikid = table.Column<int>(type: "integer", nullable: false),
                    przedmiotid = table.Column<int>(type: "integer", nullable: false),
                    grupaid = table.Column<int>(type: "integer", nullable: false),
                    salaid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_planzajec", x => x.planzajecid);
                    table.ForeignKey(
                        name: "FK_planzajec_grupydziekanskie_grupaid",
                        column: x => x.grupaid,
                        principalTable: "grupydziekanskie",
                        principalColumn: "grupaid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_planzajec_pracownicy_pracownikid",
                        column: x => x.pracownikid,
                        principalTable: "pracownicy",
                        principalColumn: "pracownikid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_planzajec_przedmioty_przedmiotid",
                        column: x => x.przedmiotid,
                        principalTable: "przedmioty",
                        principalColumn: "przedmiotid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_planzajec_sale_salaid",
                        column: x => x.salaid,
                        principalTable: "sale",
                        principalColumn: "salaid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "oceny",
                columns: table => new
                {
                    ocenaid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    wartosc = table.Column<double>(type: "double precision", nullable: false),
                    datawystawienia = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    typoceny = table.Column<string>(type: "text", nullable: false),
                    studentid = table.Column<int>(type: "integer", nullable: false),
                    planzajecid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_oceny", x => x.ocenaid);
                    table.ForeignKey(
                        name: "FK_oceny_planzajec_planzajecid",
                        column: x => x.planzajecid,
                        principalTable: "planzajec",
                        principalColumn: "planzajecid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_oceny_studenci_studentid",
                        column: x => x.studentid,
                        principalTable: "studenci",
                        principalColumn: "studentid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_grupydziekanskie_kierunekid",
                table: "grupydziekanskie",
                column: "kierunekid");

            migrationBuilder.CreateIndex(
                name: "IX_kierunki_wydzialid",
                table: "kierunki",
                column: "wydzialid");

            migrationBuilder.CreateIndex(
                name: "IX_oceny_planzajecid",
                table: "oceny",
                column: "planzajecid");

            migrationBuilder.CreateIndex(
                name: "IX_oceny_studentid",
                table: "oceny",
                column: "studentid");

            migrationBuilder.CreateIndex(
                name: "IX_planzajec_grupaid",
                table: "planzajec",
                column: "grupaid");

            migrationBuilder.CreateIndex(
                name: "IX_planzajec_pracownikid",
                table: "planzajec",
                column: "pracownikid");

            migrationBuilder.CreateIndex(
                name: "IX_planzajec_przedmiotid",
                table: "planzajec",
                column: "przedmiotid");

            migrationBuilder.CreateIndex(
                name: "IX_planzajec_salaid",
                table: "planzajec",
                column: "salaid");

            migrationBuilder.CreateIndex(
                name: "IX_pracownicy_stanowiskoid",
                table: "pracownicy",
                column: "stanowiskoid");

            migrationBuilder.CreateIndex(
                name: "IX_przedmioty_kierunekid",
                table: "przedmioty",
                column: "kierunekid");

            migrationBuilder.CreateIndex(
                name: "IX_studenci_grupaid",
                table: "studenci",
                column: "grupaid");

            migrationBuilder.CreateIndex(
                name: "IX_studenci_statusstudentaid",
                table: "studenci",
                column: "statusstudentaid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "oceny");

            migrationBuilder.DropTable(
                name: "planzajec");

            migrationBuilder.DropTable(
                name: "studenci");

            migrationBuilder.DropTable(
                name: "pracownicy");

            migrationBuilder.DropTable(
                name: "przedmioty");

            migrationBuilder.DropTable(
                name: "sale");

            migrationBuilder.DropTable(
                name: "grupydziekanskie");

            migrationBuilder.DropTable(
                name: "statusystudentow");

            migrationBuilder.DropTable(
                name: "stanowiska");

            migrationBuilder.DropTable(
                name: "kierunki");

            migrationBuilder.DropTable(
                name: "wydzialy");
        }
    }
}
