using System.ComponentModel;

namespace HiPets.Domain.Helpers
{
    public enum NotificationType { AlertError, ModalError, Error, Info }

    public enum AnimalStatus { ForAdoption = 1, InAdoptionProccess = 2, Adopted = 3 };

    public enum Status { Active = 1, Inactive = 2, Deleted = 3 };

    public enum AdoptionStatus
    {
        [Description("Solicitado")]
        Requested = 1,
        [Description("Em progresso")]
        InProgress = 2,
        [Description("Aprovado")]
        Accepted = 3,
        [Description("Rejeitado")]
        Rejected = 4
    }

    public enum Behavior
    {
        [Description("Quieto")]
        Quiet = 1,
        [Description("Inquieto")]
        Fussy = 2,
        [Description("Zangado")]
        Grumpy = 3,
        [Description("Furioso")]
        Angry = 4
    }

    public enum AnimalType
    {
        [Description("Gato")]
        Cat = 1,
        [Description("Cachorro")]
        Dog = 2,
        [Description("Peixe")]
        Fish = 3,
        [Description("Cavalho")]
        Horse = 4,
        [Description("Pássaro")]
        Bird = 5,
        [Description("Coelho")]
        Bunny = 6
    }

    public enum Color
    {
        [Description("Preto")]
        Black = 1,
        [Description("Branco")]
        White = 2,
        [Description("Amarelo")]
        Yellow = 3,
        [Description("Azul")]
        Blue = 4,
        [Description("Marrom")]
        Brown = 5,
        [Description("Cinza")]
        Gray = 6
    };
}
