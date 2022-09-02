using Castle.MicroKernel.SubSystems.Conversion;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using System;

namespace SurfBoardManager.Models
{

    //[ModelBinder(BinderType = typeof(DecimalModelBinder))]

    public class BoardPost
    {
        public enum Type { Shortboard, Funboard, Fish, Longboard, SUP };

        public int Id { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z\s]*$", 
        ErrorMessage = "Må kun indeholde bogstaver(stort startbogstav) (A-Z), ingen specialtegn (*/.?) undtagen mellemrum.")]
        [Column(TypeName = "NVarChar(255)")]
        [Display(Name = "Navn")]
        public string Name { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Bredde")]
        public decimal Width { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Længde")]
        public decimal Length { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Tykkelse")]
        public decimal Thickness { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Volumen")]
        public decimal Volume { get; set; }

        [Column(TypeName = "INT")]
        [Display(Name = "Board Type")]
        public Type BoardType { get; set; }

        [Column(TypeName = "NVarChar(255)")]
        [Display(Name = "Udstyr")]
        public string Equipment { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Pris")]
        public decimal Price { get; set; }

        [Column(TypeName = "NVarChar(255)")]
        [Display(Name = "Billede")]
        public string BoardImage { get; set; }
        public DateTime? RentalDate { get; set; }
        public DateTime? RentalDateEnd { get; set; }

        [Column(TypeName = "BIT")]
        [Display(Name = "Udlejet")]
        public bool IsRented { get; set; }
    }
}
