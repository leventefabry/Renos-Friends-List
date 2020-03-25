using RenosFriendsList.API.Enums;

namespace RenosFriendsList.API.Models.Dog
{
    public class DogDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool RenoLikesIt { get; set; }

        public BodySizeEnum BodyType { get; set; }

        public GenderEnum Gender { get; set; }

        public int? Age { get; set; }

        public int OwnerId { get; set; }
    }
}
