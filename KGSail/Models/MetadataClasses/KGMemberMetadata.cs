/*
* KGSail MVC Application Assignment 4
*
* KGMemberMetadata validades properties before saving to the database, using annotations and ClassLibrary Validation Class and methods
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;


namespace KGSail.Models
{

    [ModelMetadataType(typeof(KGMemberMetadata))]
    public partial class Member : IValidatableObject
    {

        /// <summary>
        /// Method that validades properties from Member Controller
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            #region global variables and database connection

            SailContext _context = SailContext_Singleton.Context();

            #endregion

            var results = new List<ValidationResult>();

            // Trim all string fields fields
            FirstName = KGClassLibrary.KGValidations.KGTrimFields(FirstName);
            LastName = KGClassLibrary.KGValidations.KGTrimFields(LastName);
            SpouseFirstName = KGClassLibrary.KGValidations.KGTrimFields(SpouseFirstName);
            SpouseLastName = KGClassLibrary.KGValidations.KGTrimFields(SpouseLastName);
            Street = KGClassLibrary.KGValidations.KGTrimFields(Street);
            City = KGClassLibrary.KGValidations.KGTrimFields(City);
            ProvinceCode = KGClassLibrary.KGValidations.KGTrimFields(ProvinceCode);
            PostalCode = KGClassLibrary.KGValidations.KGTrimFields(PostalCode);
            HomePhone = KGClassLibrary.KGValidations.KGTrimFields(HomePhone);
            Email = KGClassLibrary.KGValidations.KGTrimFields(Email);
            Comment = KGClassLibrary.KGValidations.KGTrimFields(Comment);

            // Capitalize
            FirstName = KGClassLibrary.KGValidations.KGCapitalize(FirstName);
            LastName = KGClassLibrary.KGValidations.KGCapitalize(LastName);
            SpouseFirstName = KGClassLibrary.KGValidations.KGCapitalize(SpouseFirstName);
            SpouseLastName = KGClassLibrary.KGValidations.KGCapitalize(SpouseLastName);
            Street = KGClassLibrary.KGValidations.KGCapitalize(Street);
            City = KGClassLibrary.KGValidations.KGCapitalize(City);
            ProvinceCode = ProvinceCode.ToUpper();

            // member name is provided
            if ((LastName != null || LastName != "") && (FirstName != null || FirstName != ""))
            {
                // spouse name is not provided
                if ((SpouseFirstName == null || SpouseFirstName == "") && (SpouseLastName == null || SpouseLastName == ""))
                {
                    FullName = LastName + ", " + FirstName;
                }
                // spouse name was provided
                else if ((SpouseFirstName != null || SpouseFirstName != "" && SpouseLastName != null || SpouseLastName != ""))
                {
                    FullName = LastName + ", " + FirstName + " & " + SpouseLastName + ", " + SpouseFirstName;
                }
                // spouse last name is not provided
                else if ((SpouseFirstName != null || SpouseFirstName != "") && (SpouseLastName == null || SpouseLastName == "")
                        || (SpouseLastName == LastName))
                {
                    FullName = LastName + ", " + FirstName + " & " + SpouseFirstName;
                }
                
            }

            // Validate ProvinceCode
            Province province = null;
            string provinceError = null;
            try
            {
                province = _context.Province
                    .Where(p => p.ProvinceCode == ProvinceCode)
                    .FirstOrDefault();
            }catch(Exception ex)
            {
                provinceError = ex.Message;
            }

            if (provinceError != null)
            {
                yield return new ValidationResult(provinceError);
               
            }
            
            // Member selected useCanadaPost
            if (UseCanadaPost)
            {
                if (province != null)
                {
                    
                    var country = _context.Country
                   .Where(c => c.CountryCode == province.CountryCode)
                   .FirstOrDefault();

                    // Country is CA
                    if (country.CountryCode == "CA")
                    {
                        // Validate postal code
                        if (string.IsNullOrEmpty(PostalCode))
                        {
                            yield return new ValidationResult("Postal code " + PostalCode +" not valid", new[] { nameof(PostalCode) });
                        }
                        else if (!KGClassLibrary.KGValidations.KGPostalCodeValidation(PostalCode))
                        {
                            yield return new ValidationResult("Postal code " + PostalCode + " not valid", new[] { nameof(PostalCode) });
                        }
                        // Postal Code correct
                        else
                        {
                            PostalCode = KGClassLibrary.KGValidations.KGPostalCodeFormat(PostalCode);
                        }
                    }
                    // Country is US
                    else if (country.CountryCode == "US")
                    {
                        string refPostalCode = PostalCode;
                        // Validate Zip Code
                        if (KGClassLibrary.KGValidations.KGZipCodeValidation(ref refPostalCode))
                        {
                            yield return new ValidationResult("Zip code " + PostalCode + " not valid", new[] { nameof(PostalCode) });
                        }
                        else
                        {
                            PostalCode = KGClassLibrary.KGValidations.KGPostalCodeFormat(PostalCode);

                        }
                    }
                }
                else
                {
                    yield return new ValidationResult("Province Code " + ProvinceCode + " is not on file", new[] { nameof(ProvinceCode) });
                }
            }
            //useCanadaPost is false
            else
            {
                // If postalCode is provided, Provincecode cannot be null
                if (PostalCode != null && string.IsNullOrEmpty(ProvinceCode))
                {
                    yield return new ValidationResult("PostalCode is  optional but, if provided, provinceCode becomes required", new[] { nameof(ProvinceCode) });
                }else
                // If postal Code inserted it is validated
                if (!KGClassLibrary.KGValidations.KGPostalCodeValidation(PostalCode))
                {
                    yield return new ValidationResult("Postal code " + PostalCode + " not valid", new[] { nameof(PostalCode) });
                }

                // if provinceCode exists on province table
                if (province != null)
                {
                    var country = _context.Country
                   .Where(c => c.CountryCode == province.CountryCode)
                   .FirstOrDefault();

                    // Country is Canada
                    if (country.CountryCode == "CA")
                    {
                        // Validate postalCode
                        if (string.IsNullOrEmpty(PostalCode))
                        {
                            yield return new ValidationResult("Postal code " + PostalCode + " not valid", new[] { nameof(PostalCode) });
                        }
                        else if (!KGClassLibrary.KGValidations.KGPostalCodeValidation(PostalCode))
                        {
                            yield return new ValidationResult("Postal code " + PostalCode + " not valid", new[] { nameof(PostalCode) });
                        }
                        // Format postal code if no error
                        else
                        {
                            PostalCode = KGClassLibrary.KGValidations.KGPostalCodeFormat(PostalCode);
                        }
                    }
                    // country is US
                    else if (country.CountryCode == "US")
                    {

                        string refPostalCode = PostalCode;
                        // Validate Zip Code
                        if (!KGClassLibrary.KGValidations.KGZipCodeValidation(ref refPostalCode))
                        {
                            yield return new ValidationResult("Zip code " + PostalCode + " not valid", new[] { nameof(PostalCode) });
                        }
                        // // Format zip code if no error
                        else
                        {
                            PostalCode = refPostalCode;
                            PostalCode = KGClassLibrary.KGValidations.KGPostalCodeFormat(PostalCode);
                        }
                    }
                }           
            }
            // if province Code is empty set it to null, because database does not acept empty 
            if (ProvinceCode == "")
            {
                ProvinceCode = null;
            }

            // Validate HomePhone
            HomePhone = KGClassLibrary.KGValidations.KGExtractDigits(HomePhone);
            if (HomePhone.Length != 10 )
            {
                yield return new ValidationResult("Home phone " + HomePhone + " must be 10 digits long: 519-748-5220", new[] { nameof(HomePhone) });
            }
            else
            {
                // Format to insert dashes
                HomePhone = string.Format("{0:###-###-####}", Convert.ToSingle(HomePhone));  
            }

            // Validate email, only if it is provided
            if (!string.IsNullOrEmpty(Email))
            {
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                if (!regex.IsMatch(Email))
                {
                    yield return new ValidationResult("Email " + Email + " is in a wrong format, please follow example@example.com", new[] { nameof(Email) });
                }
            }

            // If use CanadaPost is not selected, email is required
            if (UseCanadaPost == false)
            {
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                if (!regex.IsMatch(Email))
                {
                    yield return new ValidationResult("Email " + Email + " is required unless member has restricted communication to Canada Post", new[] { nameof(Email) });
                }
            }
            else
            {
                // usePostalcode is true, Street Address is required
                if (string.IsNullOrEmpty(Street))
                {
                    yield return new ValidationResult("Member wants to use Cananda Post - Street Address is required", new[] { nameof(Street) });
                }
                // usePostalcode is true, city is required
                if (string.IsNullOrEmpty(City))
                {
                    yield return new ValidationResult("Member wants to use Cananda Post - City is required", new[] { nameof(City) });
                }
            }

            // Validate Year Joined
           
                if (YearJoined != null)
                {
                    if (YearJoined > DateTime.Today.Year)
                    {
                        yield return new ValidationResult("The Year the member Joined the club cannot be in the future", new[] { nameof(YearJoined) });
                    }
                }
                else
                {
                    // When creating a new member, year joined is required
                    if (MemberId == 0)
                    { 
                        yield return new ValidationResult("The Year the member Joined the club cannot be null", new[] { nameof(YearJoined)});
                    }
                }


            // if no error
            yield return ValidationResult.Success;
        }
    }
    public partial class KGMemberMetadata
    {

        public int MemberId { get; set; }
        [Display(Name = "Member")]
        public string FullName { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Spouse First Name")]
        public string SpouseFirstName { get; set; }
        [Display(Name = "Spouse Last Name")]
        public string SpouseLastName { get; set; }
        [Display(Name = "Street Address")]
        public string Street { get; set; }
        public string City { get; set; }
        [Display(Name = "Province Code")]
        public string ProvinceCode { get; set; }
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        [Required]
        [Display(Name = "Home Phone")]
        public string HomePhone { get; set; }
        public string Email { get; set; }
        [Display(Name = "Year Joined")]
        public int? YearJoined { get; set; }
        public string Comment { get; set; }
        [Display(Name = "Task Exempt?")]
        public bool TaskExempt { get; set; }
        [Display(Name = "Use Canada Post?")]
        public bool UseCanadaPost { get; set; }
    }


}
