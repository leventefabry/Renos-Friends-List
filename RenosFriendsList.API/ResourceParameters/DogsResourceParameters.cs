using RenosFriendsList.API.Enums;

namespace RenosFriendsList.API.ResourceParameters
{
    public class DogsResourceParameters : BaseResourceParameters
    {
        public string Name { get; set; }
        public bool? RenoLikesIt { get; set; }
        public BodySizeEnum? BodyType { get; set; }
        public GenderEnum? Gender { get; set; }

        public string OrderBy { get; set; } = "Name";
    }
}
