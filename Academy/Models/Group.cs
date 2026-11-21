using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Academy.Models
{
    public class Group
    {
        [Key]
        public int group_id { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 3)]
        public string group_name { get; set; }

        [Column ("direction", TypeName ="TINYINT")]
        [ForeignKey(nameof(Direction))]
        public int direction { get; set; }

        [Column("weekdays", TypeName = "TINYINT")]
        public int? learning_days { get; set; }

        [Column("start_time", TypeName = "TIME(0)")]
        public TimeOnly? start_time { get; set; }

        [Column("start_date", TypeName = "DATE")]
        public DateOnly? start_date { get; set; }

        //Navigation properties:
        public Direction Direction { get; set; }
        public ICollection<Academy.Models.Student> Students { get; set; }

        [NotMapped]
        [Display(Name = "learning_days")]
        public string WeekdaysString
        {
            get
            {
                if (learning_days == null || learning_days == 0)
                    return "-";

                string[] days = { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" };
                List<string> activeDays = new List<string>();

                for (int i = 0; i < days.Length; i++)
                {
                    if ((learning_days.Value & (1 << i)) != 0)
                    {
                        activeDays.Add(days[i]);
                    }
                }

                return string.Join(", ", activeDays);
            }
        }
    }
}
