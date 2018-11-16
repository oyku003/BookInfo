namespace BookInfo.Entities.Messages
{
    public class ErrorMessageCode
    {
        public const int UsernameAlreadyExists = 101;
        public const int EmailAlreadyExists = 102;
        public const int UserIsNotActive = 151;
        public const int UsernameOrPassWrong = 152;
        public const int CheckYourEmail = 153;
        public const int UserAlreadyActive = 154;
        public const int ActivateIdDoesNotExist = 155;
        public const int UserNotFound = 156;
        public const int ProfileCouldNotUpdate = 157;
        public const int UserCouldNotRemove = 158;
        public const int UserCouldNotFind = 159;
        public const int UserCouldNotInserted = 160;
        public const int UserCouldNotUpdated = 161;
    }
}
