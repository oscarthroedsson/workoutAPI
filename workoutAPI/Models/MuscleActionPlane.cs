using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace workoutAPI.Models;
public class MuscleActionPlane
{
    [Table("muscle_actions_planes")]
    public class MuscleActionPlaneTable : BaseModel
    {
        [PrimaryKey("id")]
        public string Id { get; set; } // UUID

        [System.ComponentModel.DataAnnotations.Schema.Column("muscle_id")]
        public string MuscleId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column("plane_id")]
        public int PlaneId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}