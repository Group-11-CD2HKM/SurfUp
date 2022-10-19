
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace SurfUpLibary
{
    public class BoardPost
    {
        public enum Type { Shortboard, Funboard, Fish, Longboard, SUP };

        public int Id { get; set; }

        //DataAnnotation bruges til at validere user input og begrænse hvad bruger kan skrive.
        //Giver forhøjet sikkerhed, da brugeren ikke kan bruge kode som input.

        //RegularExpression = tilader kun input af (A-Z), (a-z) og ingen specialtegn for "navn".
        //Defineret i parameteren af Regularexpression som også indeholder en error message hvis andet er brugt som input.

        //Column(TypeName) = Fortæller input feltet hvilken type som forventes.

        //DisplayName = Giver View'et visning af feltet et navn når feltet ikke er blevet brugt endnu.

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
        public string? Equipment { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Pris")]
        public decimal Price { get; set; }

        [Column(TypeName = "NVarChar(255)")]
        [Display(Name = "Billede")]
        public string? BoardImage { get; set; }
        public DateTime? RentalDate { get; set; }
        DateTime? rentalDateEnd;

        //Get henter "rentalDateEnd" og retunere denne. Set giver "rentalDateEnd" en værdig og kalder på metoden "SetIsRentedStatus"
        public DateTime? RentalDateEnd 
        {
            get { return rentalDateEnd; }
            set 
            { rentalDateEnd = value; 
                SetIsRentedStatus();
            } 
        }

        [Column(TypeName = "BIT")]
        [Display(Name = "Udlejet")]
        public bool IsRented { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
        public SurfUpUser? SurfUpUser { get; set; }


        //Metoden sætter "IsRented" til at være sandt, når den bliver kaldt.
        private void SetIsRentedStatus()
        {
            IsRented = true;
        }
        public void UnRent()
        {
            RentalDateEnd = DateTime.Now;
            SurfUpUser = null;
            IsRented = false;
        }
    }
}
