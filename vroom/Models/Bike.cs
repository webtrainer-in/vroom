using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using vroom.Extensions;

namespace vroom.Models
{
    public class Bike
    {
        
        public int Id { get; set; }

        public Make Make { get; set; }

        [RegularExpression("^[1-9]*$", ErrorMessage = "Select Make")]
        public int MakeID { get; set; }

        public Model Model { get; set; }

        [RegularExpression("^[1-9]*$", ErrorMessage = "Select Model")]
        public int ModelID { get; set; }

        [Required(ErrorMessage = "Provide Year")]        
        [YearRangeTillDate(2000,ErrorMessage = "Not with in the valid Year range")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Enter Mileage")]
        [Range(1,int.MaxValue,ErrorMessage ="Not with in the valid mileage range")]
        public int Mileage { get; set; }

        
        public string Features { get; set; }

        [Required(ErrorMessage ="Provide Seller Name")]
        [StringLength(50)]
        public string SellerName { get; set; }

        [EmailAddress(ErrorMessage ="Invalid Email ID")]
        [StringLength(50)]
        public string SellerEmail { get; set; }

        [Required(ErrorMessage = "Provide Phone No.")]
        [Phone]
        [StringLength(15)]
        public string SellerPhone { get; set; }

        [Required(ErrorMessage = "Provide Price")]
         [Range(1, 999999999, ErrorMessage = "Not with in the valid price range")]
        public int Price { get; set; }

        [Required]
        [StringLength(10)]
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage ="Select Currency")]       
        public string Currency { get; set; }

        [Display(Name ="Image File")]
        [StringLength(100)]
        public string ImagePath { get; set; }

    }
}
