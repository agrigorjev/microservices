using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("parser_service_captchas")]
    public partial class ParserServiceCaptcha
    {
        [Column("captcha_id")]
        [Key]
        public int CaptchaId { get; set; }

        [Column("captcha_data")]
        [Required]
        [StringLength(1000)]
        public string CaptchaData { get; set; }

        [Column("captcha_url")]
        [Required]
        [StringLength(1000)]
        public string CaptchaUrl { get; set; }

        [Column("captcha_word")]
        [StringLength(50)]
        public string CaptchaWord { get; set; }
    }
}
