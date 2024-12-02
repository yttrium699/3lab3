namespace WebApplication1.Domain.Entities
{
   
    public class Emotion
        {
            public Guid? Id { get; set; }
            public DateTime StateDate { get; set; }
            public string EmotionType { get; set; }
            public Guid UserId { get; set; }
        }
    }

